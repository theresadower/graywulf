﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    public class QueryJob : Job
    {
        #region Private member variables

        private string query;

        #endregion
        #region Properties

        [DataMember(Name="query")]
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        #endregion
        #region Constructors and initializers

        public QueryJob()
        {
            InitializeMembers();
        }

        public QueryJob(string query, JobQueue queue)
            : this()
        {
            InitializeMembers();

            this.query = query;
            this.Queue = queue;
        }

        public QueryJob(JobInstance jobInstance)
            : base(jobInstance)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Query;

            this.query = null;
        }

        #endregion

        protected override void CopyFromJobInstance(JobInstance jobInstance)
        {
            base.CopyFromJobInstance(jobInstance);

            // Because job parameter type might come from an unknown 
            // assembly, instead of deserializing, read xml directly here

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Jobs.Constants.JobParameterQuery))
            {
                var xml = new XmlDocument();
                xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Jobs.Constants.JobParameterQuery].XmlValue);

                this.query = GetXmlInnerText(xml, "Query/QueryString");
            }
            else
            {
                // TODO
                // This is probably a wrong job in the database
            }
        }

        /// <summary>
        /// Creates a new QueryBase object based on the job settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public QueryBase CreateQuery(FederationContext context)
        {
            var qf = QueryFactory.Create(context.Federation);
            var q = qf.CreateQuery(query, ExecutionMode.Graywulf);

            switch (Queue)
            {
                case JobQueue.Quick:
                    q.Destination = new IO.Tasks.DestinationTable(
                        context.MyDBDataset,
                        context.MyDBDataset.DatabaseName,
                        context.MyDBDataset.DefaultSchemaName,
                        "quickResults",
                        TableInitializationOptions.Drop | TableInitializationOptions.Create);
                    break;
                case JobQueue.Long:
                    throw new NotImplementedException();
                default:
                    break;
            }

            return q;
        }

        /// <summary>
        /// Schedules a new query job based on the job settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override void Schedule(FederationContext context)
        {
            var q = CreateQuery(context);
            q.Verify();

            var qf = QueryFactory.Create(context.Federation);
            var job = qf.ScheduleAsJob(q, GetQueueName(context), Comments);

            job.Save();

            CopyFromJobInstance(job);
        }
    }
}
