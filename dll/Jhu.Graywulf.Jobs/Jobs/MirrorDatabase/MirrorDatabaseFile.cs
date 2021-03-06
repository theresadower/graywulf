﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class MirrorDatabaseFile : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }
        public OutArgument<Guid> EntityGuidFrom { get; set; }
        public OutArgument<Guid> EntityGuidTo { get; set; }

        [RequiredArgument]
        public InArgument<Guid> SourceFileGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> DestinationDatabaseInstanceGuid { get; set; }
        [RequiredArgument]
        public InArgument<FileCopyDirection> FileCopyDirection { get; set; }
        [RequiredArgument]
        public InArgument<bool> SkipExistingFile { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            Guid sourcefileguid = SourceFileGuid.Get(activityContext);
            Guid destinationdatabaseinstanceguid = DestinationDatabaseInstanceGuid.Get(activityContext);
            FileCopyDirection filecopydirection = FileCopyDirection.Get(activityContext);
            bool skipExistingFile = SkipExistingFile.Get(activityContext);

            string sourcefilename, destinationfilename;
            string hostname;

            // Load files
            using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                // Load destination database instance
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = destinationdatabaseinstanceguid;
                di.Load();
                di.LoadFileGroups(false);

                EntityGuid.Set(activityContext, di.Guid);

                // Math source file with destination file
                DatabaseInstanceFile df;

                DatabaseInstanceFile sf = new DatabaseInstanceFile(context);
                sf.Guid = sourcefileguid;
                sf.Load();

                EntityGuidFrom.Set(activityContext, sourcefileguid);

                sourcefilename = sf.GetFullUncFilename();

                DatabaseInstanceFileGroup fg = di.FileGroups[sf.DatabaseInstanceFileGroup.Name];
                fg.LoadFiles(false);
                df = fg.Files[sf.Name];

                EntityGuidTo.Set(activityContext, df.Guid);

                destinationfilename = df.GetFullUncFilename();

                DatabaseInstanceFile ssf = filecopydirection == Jhu.Graywulf.Registry.FileCopyDirection.Push ? sf : df;
                hostname = ssf.DatabaseInstanceFileGroup.DatabaseInstance.ServerInstance.Machine.HostName.ResolvedValue;
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, hostname, sourcefilename, destinationfilename, skipExistingFile), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, string hostName, string sourceFilename, string destinationFilename, bool skipExistingFile)
        {
            if (!skipExistingFile ||
                !File.Exists(destinationFilename) ||
                new FileInfo(sourceFilename).Length != new FileInfo(destinationFilename).Length)
            {

                var fc = RemoteServiceHelper.CreateObject<ICopyFile>(hostName, true);

                fc.Source = sourceFilename;
                fc.Destination = destinationFilename;
                fc.Overwrite = true;
                fc.Method = FileCopyMethod.AsyncFileCopy;

                RegisterCancelable(workflowInstanceGuid, activityInstanceId, fc);
                fc.Execute();
                UnregisterCancelable(workflowInstanceGuid, activityInstanceId, fc);

            }
        }
    }
}
