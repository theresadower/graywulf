﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Runtime.DurableInstancing;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Implements the main functions of the job queue and polling.
    /// </summary>
    /// <remarks>
    /// This class is instantiated as a singleton and is accessed via the
    /// QueueManager.Instance member
    /// </remarks>
    public class QueueManager
    {
        #region Static declarations

        /// <summary>
        /// QueueManager singleton
        /// </summary>
        public static QueueManager Instance = new QueueManager();

        #endregion
        #region Property storage

        /// <summary>
        /// If true, process is running in interactive mode, otherwise
        /// it's a Windows service
        /// </summary>
        private bool interactive;

        #endregion
        #region Private member varibles

        /// <summary>
        /// Used for synchronization when app domains are managed
        /// </summary>
        private object syncRoot = new object();

        /// <summary>
        /// AppDomains stored by their IDs.
        /// </summary>
        /// <remarks>
        /// AppDomain.Id, AppDomainHost
        /// </remarks>
        private ConcurrentDictionary<int, AppDomainHost> appDomains;

        /// <summary>
        /// All jobs listed by their workflow instance IDs
        /// </summary>
        /// <remarks>
        /// WorkflowInstanceId, Job
        /// </remarks>
        private ConcurrentDictionary<Guid, Job> runningJobs;

        /// <summary>
        /// Thread to run the poller on
        /// </summary>
        private Thread pollerThread;

        /// <summary>
        /// If true, requests the poller to stop
        /// </summary>
        private bool pollerStopRequested;

        /// <summary>
        /// Used by the poller thread to signal to end of polling
        /// </summary>
        private AutoResetEvent pollerStopEvent;

        /// <summary>
        /// Supports logging, counts events
        /// </summary>
        private int eventOrder;

        /// <summary>
        /// Supports logging
        /// </summary>
        private Guid contextGuid;

        /// <summary>
        /// Cached cluster information
        /// </summary>
        private Cluster cluster;

        /// <summary>
        /// Task scheduler
        /// </summary>
        private Scheduler scheduler;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the value determining if the scheduler is running in interactive (command-line) mode.
        /// </summary>
        public bool Interactive
        {
            get { return interactive; }
        }

        /// <summary>
        /// Gets a cached version of the cluster configuration
        /// </summary>
        public Cluster Cluster
        {
            get { return cluster; }
        }

        /// <summary>
        /// Gets a reference to the task scheduler.
        /// </summary>
        public Scheduler Scheduler
        {
            get { return scheduler; }
        }

        #endregion
        #region Constructors and initializers

        private QueueManager()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.interactive = true;

            this.appDomains = null;
            this.runningJobs = null;

            this.pollerStopRequested = false;
            this.pollerThread = null;

            this.eventOrder = 0;
            this.contextGuid = Guid.Empty;

            this.cluster = null;
            this.scheduler = null;
        }

        private void InitializeScheduler()
        {
            this.scheduler = new Scheduler(this);
        }

        #endregion
        #region Cluster configuration caching functions

        /// <summary>
        /// Initializes cluster settings
        /// </summary>
        /// <param name="clusterName"></param>
        private void InitializeCluster(string clusterName)
        {
            // TODO: wrap this into retry logic once recurring cluster loading is implemented
            this.cluster = LoadCluster(clusterName);
        }

        /// <summary>
        /// Loads the configuration of the entire cluster from the database.
        /// </summary>
        /// <returns></returns>
        private Cluster LoadCluster(string clusterName)
        {
            var cluster = new Cluster();
            cluster.Load(clusterName);

            LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.LoadCluster", Guid.Empty));

            return cluster;
        }

        #endregion
        #region QueueManager control functions

        /// <summary>
        /// Starts the scheduler
        /// </summary>
        /// <param name="interactive"></param>
        public void Start(string clusterName, bool interactive)
        {
            // Initialize Queue Manager
            this.interactive = interactive;

            this.appDomains = new ConcurrentDictionary<int, AppDomainHost>();
            this.runningJobs = new ConcurrentDictionary<Guid, Job>();
            this.eventOrder = 0;
            this.contextGuid = Guid.NewGuid();

            // Initialize logger
            Logger.Instance.Writers.Add(new SqlLogWriter());

            if (interactive)
            {
                Logger.Instance.Writers.Add(new StreamLogWriter(Console.Out));
            }

            // Log starting event
            Event e = new Event("Jhu.Graywulf.Scheduler.QueueManager.Start", Guid.Empty);
            e.UserData.Add("MachineName", Environment.MachineName);
            e.UserData.Add("UserAccount", String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName));
            LogEvent(e);

            // *** TODO: error handling, and repeat a couple of times, then shut down with exception

            InitializeCluster(clusterName);
            InitializeScheduler();

            new DelayedRetryLoop(5).Execute(
                ProcessInterruptedJobs,
                ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.Start[ProcessInterruptedJobs]", ex)));

            StartPoller();
        }
        
        /// <summary>
        /// Persists all jobs and stops the scheduler.
        /// </summary>
        /// <remarks>
        /// Waits until running workflow are idle and persists them.
        /// </remarks>
        public void Stop(TimeSpan timeout)
        {
            Stop(timeout, true);
        }

        /// <summary>
        /// Waits for all jobs to complete and stops the scheduler.
        /// </summary>
        /// <param name="timeout"></param>
        public void DrainStop(TimeSpan timeout)
        {
            Stop(timeout, false);
        }

        private void Stop(TimeSpan timeout, bool requestPersist)
        {
            // Stop the poller thread
            StopPoller();

            if (requestPersist)
            {
                var jobs = runningJobs.Values.ToArray();

                foreach (var job in jobs)
                {
                    new DelayedRetryLoop(5).Execute(
                        () => { PersistJob(job); },
                        ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.Stop[PersistJob]", ex)));
                }
            }

            StopAppDomains(timeout);

            // Log stop event
            var e = new Event("Jhu.Graywulf.Scheduler.QueueManager.Stop", Guid.Empty);
            e.UserData.Add("MachineName", Environment.MachineName);
            e.UserData.Add("UserAccount", String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName));
            LogEvent(e);
        }

        /// <summary>
        /// Cancels all running workflows and shuts down the scheduler.
        /// </summary>
        public void Kill(TimeSpan timeout)
        {
            StopPoller();

            // Cancel all jobs
            var jobs = runningJobs.Values.ToArray();

            foreach (var job in jobs)
            {
                new DelayedRetryLoop(5).Execute(
                    () => { CancelOrTimeOutJob(job, false); },
                    ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.Kill[CancelOrTimeOutJob]", ex)));
            }

            // Wait for all jobs to complete
            StopAppDomains(timeout);

            // Log stop event
            var e = new Event("Jhu.Graywulf.Scheduler.QueueManager.Kill", Guid.Empty);
            e.UserData.Add("MachineName", Environment.MachineName);
            e.UserData.Add("UserAccount", String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName));
            LogEvent(e);
        }

        private void StopAppDomains(TimeSpan timeout)
        {
            // Shut down all the AppDomains
            // Do it in parallel to save time: jobs will slowly become idle
            lock (syncRoot)
            {
                Parallel.ForEach(appDomains.Keys, id =>
                {
                    var ad = appDomains[id];

                    ad.Stop(timeout, interactive);
                    Components.AppDomainManager.Instance.UnloadAppDomain(id);
                });
            }
        }

        #endregion
        #region Poller functions

        /// <summary>
        /// Starts the poller thread asynchronously
        /// </summary>
        public void StartPoller()
        {
            if (pollerThread == null)
            {
                pollerStopEvent = new AutoResetEvent(false);
                pollerStopRequested = false;

                pollerThread = new Thread(new ThreadStart(Poller));
                pollerThread.Name = "Graywulf Scheduler Poller";
                pollerThread.Start();
            }
            else
            {
                throw new InvalidOperationException(ExceptionMessages.PollerHasAlreadyStarted);
            }

            LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.StartPoller", Guid.Empty));
        }

        /// <summary>
        /// Stops the poller thread synchronously
        /// </summary>
        /// <remarks>
        /// Waits until the last polling completes
        /// </remarks>
        public void StopPoller()
        {
            if (pollerThread != null)
            {
                pollerStopRequested = true;
                pollerStopEvent.WaitOne();
                pollerThread = null;
            }
            else
            {
                throw new InvalidOperationException(ExceptionMessages.PollerHasNotStarted);
            }

            LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.StopPoller", Guid.Empty));
        }

        /// <summary>
        /// Poller loop
        /// </summary>
        private void Poller()
        {
            while (!pollerStopRequested)
            {
                // Polling might fail, but wrap it into retry logic so it keeps trying
                // on errors. This is to prevent the scheduler from stopping when the
                // connection to the database is lost intermittently.
                var pollRetry = new DelayedRetryLoop()
                {
                    InitialDelay = 1000,
                    MaxDelay = 30000,
                    DelayMultiplier = 1.5,
                    MaxRetries = -1
                };

                pollRetry.Execute(Poll);

                Thread.Sleep(Scheduler.Configuration.PollingInterval);
            }

            // Signal the completion event
            pollerStopEvent.Set();
        }

        /// <summary>
        /// Polling proecdure
        /// </summary>
        private void Poll()
        {
            UnloadOldAppdomains();

            new DelayedRetryLoop(1).Execute(
                ProcessTimedOutJobs,
                ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.Poll[ProcessTimedOutJobs]", ex)));

            new DelayedRetryLoop(1).Execute(
                PollNewJobs,
                ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.Poll[PollNewJobs]", ex)));

            new DelayedRetryLoop(1).Execute(
                PollCancellingJobs,
                ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.Poll[PollCancellingJobs]", ex)));
        }

        /// <summary>
        /// Checks each job if it timed out then cancels them.
        /// </summary>
        private void ProcessTimedOutJobs()
        {
            var jobs = runningJobs.Values.ToArray();

            foreach (Job job in jobs)
            {
                if (job.IsTimedOut(Cluster.Queues[job.QueueGuid].Timeout))
                {
                    new DelayedRetryLoop(5).Execute(
                    () => { CancelOrTimeOutJob(job, true); },
                    ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.ProcessTimedOutJobs[CancelOrTimeOutJob]", ex)));
                }
            }
        }

        /// <summary>
        /// Process jobs that are found in an intermediate stage, possibly
        /// due to an unexpected shutdown of the scheduler.
        /// </summary>
        private void ProcessInterruptedJobs()
        {
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                int failed = 0;
                int started = 0;
                Event e;
                var jf = new JobInstanceFactory(context);

                jf.UserGuid = Guid.Empty;
                jf.QueueInstanceGuids.UnionWith(Cluster.Queues.Keys);

                jf.JobExecutionStatus =
                    JobExecutionState.Executing |
                    JobExecutionState.Persisting |
                    JobExecutionState.Cancelling |
                    JobExecutionState.Starting;

                foreach (var j in jf.FindJobInstances())
                {
                    // Locking must be handled
                    context.ContextGuid = j.WorkflowInstanceId;

                    j.ReleaseLock(true);

                    if ((jf.JobExecutionStatus & JobExecutionState.Starting) != 0)
                    {
                        // Jobs marked as waiting probably can be restarted without a side effect
                        j.JobExecutionStatus = JobExecutionState.Scheduled;

                        started++;
                    }
                    else
                    {
                        // Process previously interrupted jobs (that are marked as running)
                        // Process jobs that are marked as executing - these remained in this state
                        // because of a failure in the scheduler
                        j.JobExecutionStatus = JobExecutionState.Failed;
                        j.ExceptionMessage = Jhu.Graywulf.Registry.ExceptionMessages.SchedulerUnexpectedShutdown;

                        failed++;
                    }

                    j.Save();

                    if (j.JobExecutionStatus == JobExecutionState.Failed)
                    {
                        j.RescheduleIfRecurring();
                    }
                }

                if (failed > 0)
                {
                    e = new Event("Jhu.Graywulf.Scheduler.QueueManager.ProcessInterruptedJobs[Executing]", Guid.Empty);
                    e.Message = String.Format("Marked {0} jobs as failed.", failed);
                    LogEvent(e);
                }

                if (started > 0)
                {
                    e = new Event("Jhu.Graywulf.Scheduler.QueueManager.ProcessInterruptedJobs[Starting]", Guid.Empty);
                    e.Message = String.Format("Marked {0} jobs as scheduled.", started);
                    LogEvent(e);
                }
            }
        }

        /// <summary>
        /// Queries the registry for new jobs to schedule
        /// </summary>
        private void PollNewJobs()
        {
            List<Job> temp = new List<Job>();

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.ContextGuid = contextGuid;

                foreach (var queue in Cluster.Queues.Values)
                {
                    // The number of jobs to be requested from the queue is the
                    // number of maximum outstanding jobs minus the number of
                    // already running jobs

                    var maxjobs = queue.MaxOutstandingJobs - queue.Jobs.Count;

                    var jf = new JobInstanceFactory(context);
                    var jis = jf.FindNextJobInstances(
                        queue.Guid,
                        queue.LastUserGuid,
                        maxjobs);

                    foreach (var ji in jis)
                    {
                        var user = new User(context);
                        user.Guid = ji.UserGuidOwner;
                        user.Load();

                        var job = new Job()
                        {
                            Guid = ji.Guid,
                            JobID = ji.JobID,
                            UserGuid = user.Guid,
                            UserName = user.Name,
                            QueueGuid = ji.ParentReference.Guid,
                            WorkflowTypeName = ji.WorkflowTypeName,
                            Timeout = ji.JobTimeout,
                        };

                        if ((ji.JobExecutionStatus & JobExecutionState.Scheduled) != 0)
                        {
                            job.Status = JobStatus.Starting;
                            ji.JobExecutionStatus = JobExecutionState.Starting;
                        }
                        else if ((ji.JobExecutionStatus & JobExecutionState.Persisted) != 0)
                        {
                            // Save cancel requested flag here
                            ji.JobExecutionStatus ^= JobExecutionState.Persisted;
                            ji.JobExecutionStatus |= JobExecutionState.Starting;

                            job.Status = JobStatus.Resuming;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        ji.Save();

                        if (queue.Jobs.TryAdd(job.Guid, job))
                        {
                            temp.Add(job);
                        }
                    }
                }
            }

            foreach (var job in temp)
            {
                new DelayedRetryLoop(5).Execute(
                    () => { StartOrResumeJob(job); },
                    ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.PollNewJobs[StartOrResumeJob]", ex)));
            }
        }

        private void PollCancellingJobs()
        {
            List<Job> temp = new List<Job>();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var jf = new JobInstanceFactory(context);

                foreach (var queue in Cluster.Queues.Values)
                {
                    jf.UserGuid = Guid.Empty;
                    jf.QueueInstanceGuids.Clear();
                    jf.QueueInstanceGuids.Add(queue.Guid);
                    jf.JobExecutionStatus = JobExecutionState.CancelRequested;

                    foreach (var ji in jf.FindJobInstances())
                    {
                        Job job;
                        if (queue.Jobs.TryGetValue(ji.Guid, out job))
                        {
                            temp.Add(job);
                        }
                    }
                }
            }

            // This is to be done outside the registry context
            foreach (var job in temp)
            {
                new DelayedRetryLoop(5).Execute(
                    () => { CancelOrTimeOutJob(job, false); },
                    ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.PollCancellingJobs[CancelOrTimeOutJob]", ex)));
            }
        }

        #endregion
        #region Direct job control functions

        /// <summary>
        /// Starts a single job
        /// </summary>
        /// <param name="job"></param>
        private void StartOrResumeJob(Job job)
        {
            AppDomainHost adh = null;

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.JobGuid = job.Guid;
                context.ContextGuid = contextGuid;

                JobInstance ji = new JobInstance(context);
                ji.Guid = job.Guid;
                ji.Load();

                // Lock the job, so noone else can pick it up
                ji.DateStarted = DateTime.Now;
                ji.ObtainLock();

                ji.Save();
            }

            // Schedule job in the appropriate app domain
            adh = GetOrCreateAppDomainHost(job);

            // Check if job is a new instance or previously persisted and
            // has to be resumed
            switch (job.Status)
            {
                case JobStatus.Starting:
                    if (interactive)
                    {
                        Console.WriteLine("Starting job: {0}", job.Guid);
                    }

                    job.WorkflowInstanceId = adh.PrepareStartJob(job);
                    break;
                case JobStatus.Resuming:
                    if (interactive)
                    {
                        Console.WriteLine("Resuming job: {0}", job.Guid);
                    }

                    job.WorkflowInstanceId = adh.PrepareResumeJob(job);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Update job status
            job.TimeStarted = DateTime.Now;
            job.Status = JobStatus.Executing;
            job.AppDomainID = adh.ID;

            if (runningJobs.TryAdd(job.WorkflowInstanceId, job))
            {
                // Update job queue last user, so round-robin scheduling will work
                var queue = cluster.Queues[job.QueueGuid];
                queue.LastUserGuid = job.UserGuid;
            }

            // Send job for execution in the designated AppDomain
            adh.RunJob(job);
        }

        private void CancelOrTimeOutJob(Job job, bool timeout)
        {
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.JobGuid = job.Guid;
                context.ContextGuid = contextGuid;

                var ji = new JobInstance(context);
                ji.Guid = job.Guid;
                ji.Load();

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Cancelling;

                ji.Save();
            }

            // Update job status
            if (timeout)
            {
                job.Status = JobStatus.TimedOut;
                appDomains[job.AppDomainID].TimeOutJob(job);
            }
            else
            {
                job.Status = JobStatus.Cancelled;
                appDomains[job.AppDomainID].CancelJob(job);
            }
        }

        private void PersistJob(Job job)
        {
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.JobGuid = job.Guid;
                context.ContextGuid = contextGuid;

                var ji = new JobInstance(context);
                ji.Guid = job.Guid;
                ji.Load();

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Persisting;

                ji.Save();
            }

            // Update job status
            job.Status = JobStatus.Persisted;

            appDomains[job.AppDomainID].PersistJob(job);
        }

        /// <summary>
        /// Finished the execution of a job and records the results in the registry.
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <param name="eventType"></param>
        private void FinishJob(Guid instanceID, WorkflowApplicationHostEventArgs e)
        {
            // TODO: certain workflows (probably failing ones) raise more than
            // one event that mark the job as finished. For now, we do the bookeeping
            // only once and remove the job from running jobs at the very first event

            Job job;
            if (runningJobs.TryRemove(instanceID, out job))
            {
                using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                {
                    context.JobGuid = job.Guid;
                    context.ContextGuid = contextGuid;

                    JobInstance ji = new JobInstance(context);
                    ji.Guid = job.Guid;
                    ji.Load();

                    // Update execution status, error message and finish time
                    switch (e.EventType)
                    {
                        case WorkflowEventType.Completed:
                            ji.JobExecutionStatus = JobExecutionState.Completed;
                            break;
                        case WorkflowEventType.Cancelled:
                            ji.JobExecutionStatus = JobExecutionState.Cancelled;
                            break;
                        case WorkflowEventType.TimedOut:
                            ji.JobExecutionStatus = JobExecutionState.TimedOut;
                            break;
                        case WorkflowEventType.Persisted:
                            ji.JobExecutionStatus = JobExecutionState.Persisted;
                            break;
                        case WorkflowEventType.Failed:
                            ji.JobExecutionStatus = JobExecutionState.Failed;
                            ji.ExceptionMessage = e.ExceptionMessage;
                            break;
                    }

                    // Update registry
                    ji.DateFinished = DateTime.Now;
                    ji.Save();

                    ji.ReleaseLock(false);
                    ji.RescheduleIfRecurring();

                    // Do local bookkeeping
                    cluster.Queues[job.QueueGuid].Jobs.TryRemove(job.Guid, out job);

                    if (interactive)
                    {
                        Console.WriteLine("Finishing job: {0}", ji.Guid);
                    }
                }
            }
        }

        /// <summary>
        /// Looks up information about a job necessary to configure a registry context
        /// </summary>
        /// <remarks>
        /// This function is called by the workflow framework via IScheduler when creating
        /// a new context to access the registry.
        /// </remarks>
        /// <param name="workflowInstanceId"></param>
        /// <param name="userGuid"></param>
        /// <param name="userName"></param>
        /// <param name="jobGuid"></param>
        /// <param name="jobID"></param>
        internal void GetContextInfo(Guid workflowInstanceId, out Guid userGuid, out string userName, out Guid jobGuid, out string jobID)
        {
            var job = runningJobs[workflowInstanceId];

            userGuid = job.UserGuid;
            userName = job.UserName;
            jobGuid = job.Guid;
            jobID = job.JobID;
        }

        #endregion
        #region AppDomainHost control functions

        /// <summary>
        /// Returns an AppDomainHost to run the job.
        /// </summary>
        /// <remarks>
        /// Either an existing AppDomainHost is returned that has the
        /// required assembly, or a new AppDomainHost is created.
        /// </remarks>
        /// <param name="job"></param>
        /// <returns></returns>
        private AppDomainHost GetOrCreateAppDomainHost(Job job)
        {
            AppDomain ad;
            Components.AppDomainManager.Instance.GetAppDomainForType(job.WorkflowTypeName, true, out ad);

            if (!appDomains.ContainsKey(ad.Id))
            {
                // New app domain, create host
                var adh = new AppDomainHost(ad, contextGuid);
                adh.WorkflowEvent += new EventHandler<WorkflowApplicationHostEventArgs>(adh_WorkflowEvent);
                adh.UnhandledException += adh_UnhandledException;

                appDomains.TryAdd(ad.Id, adh);

                adh.Start(Scheduler, interactive);

                return adh;
            }
            else
            {
                // Old app domain, return existing host
                return appDomains[ad.Id];
            }
        }

        /// <summary>
        /// Checks AppDomain usage and unload those that have been idle
        /// for a given period of time.
        /// </summary>
        private void UnloadOldAppdomains()
        {
            // TODO: Here we need to synchronize but make
            // sure it's on a syncRoot object, not runningjobs
            lock (syncRoot)
            {
                // Find app domains with no running jobs
                var adids = new HashSet<int>(appDomains.Keys);

                foreach (var job in runningJobs.Values)
                {
                    if (adids.Contains(job.AppDomainID))
                    {
                        adids.Remove(job.AppDomainID);
                    }
                }

                // adhguids not contains only empty appdomains
                foreach (var id in adids)
                {
                    var adh = appDomains[id];

                    // Unload if idle for more than a minute
                    if ((DateTime.Now - adh.LastTimeActive) > Scheduler.Configuration.AppDomainIdle)
                    {
                        // This shutdown should happen quickly
                        adh.Stop(Scheduler.Configuration.AppDomainShutdownTimeout, interactive);
                        Components.AppDomainManager.Instance.UnloadAppDomain(id);

                        AppDomainHost a;
                        appDomains.TryRemove(id, out a);
                    }
                }
            }
        }

        /// <summary>
        /// Handles events raised by any AppDomainHost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void adh_WorkflowEvent(object sender, WorkflowApplicationHostEventArgs e)
        {
            new DelayedRetryLoop(5).Execute(
                () => FinishJob(e.InstanceId, e),
                ex => LogEvent(new Event("Jhu.Graywulf.Scheduler.QueueManager.adh_WorkflowEvent[FinishJob]", ex)));
        }

        private void adh_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO: terminate failing app domain

            var ex = e.ExceptionObject as Exception;
            LogEvent(new Event("Jhu.Graywulf.Scheduler.WorkflowApplicationHost.UnhandledException", ex));
        }

        #endregion
        #region Logging functions

        /// <summary>
        /// Logs scheduler events
        /// </summary>
        /// <param name="e"></param>
        private void LogEvent(Event e)
        {
            // TODO: add guids, etc.

            e.UserGuid = Guid.Empty;
            e.EventSource = EventSource.Scheduler;
            e.ExecutionStatus = ExecutionStatus.Closed;

            e.JobGuid = Guid.Empty;
            e.ContextGuid = contextGuid;
            e.EventOrder = ++eventOrder;

            Logger.Instance.LogEvent(e);
        }

        #endregion
    }
}
