﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Tracking;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Activities
{
    public class GraywulfTrackingParticipant : TrackingParticipant
    {
        TrackingProfile trackingProfile;

        public override TrackingProfile TrackingProfile
        {
            get { return this.trackingProfile; }
            set { this.trackingProfile = value; }
        }

        public GraywulfTrackingParticipant()
        {
            trackingProfile = new TrackingProfile();

            ActivityStateQuery aq = new ActivityStateQuery();
            aq.ActivityName = "*";
            aq.States.Add("*");
            aq.Arguments.Add("JobGuid");
            aq.Arguments.Add("UserGuid");
            aq.Arguments.Add("EntityGuid");
            aq.Arguments.Add("EntityGuidFrom");
            aq.Arguments.Add("EntityGuidTo");
            trackingProfile.Queries.Add(aq);

            CustomTrackingQuery cq = new CustomTrackingQuery();
            cq.ActivityName = "*";
            trackingProfile.Queries.Add(cq);

            FaultPropagationQuery fq = new FaultPropagationQuery();
            fq.FaultHandlerActivityName = "*";
            fq.FaultSourceActivityName = "*";
            trackingProfile.Queries.Add(fq);
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            Event e = null;

            if (record is ActivityStateRecord)
            {
                e = ProcessTrackingRecord((ActivityStateRecord)record);
            }
            else if (record is CustomTrackingRecord)
            {
                e = ProcessTrackingRecord((CustomTrackingRecord)record);
            }
            else if (record is FaultPropagationRecord)
            {
                e = ProcessTrackingRecord((FaultPropagationRecord)record);
            }
            else
            {
                Console.WriteLine(record);
            }

            if (e != null)
            {
                Jhu.Graywulf.Logging.Logger.Instance.LogEvent(e);
            }
        }

        private Event ProcessTrackingRecord(ActivityStateRecord record)
        {
            // Only record events of IGraywulfActivity activities

            if (record.Arguments.ContainsKey("JobGuid"))
            {

                Event e = new Event();

                e.ContextGuid = record.InstanceId;
                e.EventDateTime = record.EventTime;
                e.EventOrder = record.RecordNumber;
                e.Operation = record.Activity.Name;

                e.EventSeverity = MapEventSeverity(record.Level);

                e.EventSource = EventSource.Workflow;

                // *** TODO
                ExecutionStatus exst;
                if (Enum.TryParse<ExecutionStatus>(record.State, true, out exst))
                {
                    e.ExecutionStatus = exst;
                }
                else
                {
                    e.ExecutionStatus = ExecutionStatus.Unknown;    //
                }

                SetEventProperties(e, record.Arguments);

                return e;
            }
            else
            {
                return null;
            }
        }

        private Event ProcessTrackingRecord(CustomTrackingRecord record)
        {
            if (record.Data != null)
            {
                var e = new Event();

                e.ContextGuid = record.InstanceId;
                e.EventDateTime = record.EventTime;
                e.EventOrder = record.RecordNumber;
                e.Operation = record.Activity.Name;

                e.EventSeverity = MapEventSeverity(record.Level);

                e.EventSource = EventSource.Workflow;

                // *** TODO

                if (record.Data.ContainsKey("ExecutionStatus"))
                {
                    e.ExecutionStatus = (ExecutionStatus)record.Data["ExecutionStatus"];
                }
                else
                {
                    e.ExecutionStatus = ExecutionStatus.Unknown;    //
                }

                SetEventProperties(e, record.Data);

                return e;
            }
            else
            {
                return null;
            }
        }

        private Event ProcessTrackingRecord(FaultPropagationRecord record)
        {
            if (record.IsFaultSource)
            {
                Event e = new Event();

                e.ContextGuid = record.InstanceId;
                e.EventDateTime = record.EventTime;
                e.EventOrder = record.RecordNumber;
                e.Operation = record.FaultSource.Name;

                e.EventSeverity = MapEventSeverity(record.Level);
                e.EventSource = EventSource.Workflow;
                e.ExecutionStatus = ExecutionStatus.Faulted;

                e.Exception = record.Fault;

                return e;
            }
            else
            {
                return null;
            }
        }

        private Logging.EventSeverity MapEventSeverity(System.Diagnostics.TraceLevel level)
        {
            switch (level)
            {
                case System.Diagnostics.TraceLevel.Error:
                    return Logging.EventSeverity.Error;
                case System.Diagnostics.TraceLevel.Verbose:
                case System.Diagnostics.TraceLevel.Info:
                    return Logging.EventSeverity.Status;
                case System.Diagnostics.TraceLevel.Off:
                    return Logging.EventSeverity.None;
                case System.Diagnostics.TraceLevel.Warning:
                    return Logging.EventSeverity.Warning;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SetEventProperties(Event e, IDictionary<string,object> data)
        {
            if (data.ContainsKey("JobGuid"))
            {
                e.JobGuid = (Guid)data["JobGuid"];
            }

            if (data.ContainsKey("UserGuid"))
            {
                e.UserGuid = (Guid)data["UserGuid"];
            }

            if (data.ContainsKey("EntityGuid"))
            {
                e.EntityGuid = (Guid)data["EntityGuid"];
            }

            if (data.ContainsKey("EntityGuidFrom"))
            {
                e.EntityGuidFrom = (Guid)data["EntityGuidFrom"];
            }

            if (data.ContainsKey("EntityGuidTo"))
            {
                e.EntityGuidTo = (Guid)data["EntityGuidTo"];
            }

            if (data.ContainsKey("Exception"))
            {
                e.Exception = (Exception)data["Exception"];
                e.ExceptionType = e.Exception.GetType().FullName;
            }
        }
    }
}
