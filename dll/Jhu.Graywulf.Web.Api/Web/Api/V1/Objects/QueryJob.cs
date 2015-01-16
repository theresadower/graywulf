﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a query job.")]
    public class QueryJob : Job
    {
        #region Private member variables

        private string query;

        #endregion
        #region Properties

        [DataMember(Name = "query")]
        [Description("Query text in SQL.")]
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

        public static QueryJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new QueryJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Query;

            this.query = null;
        }

        #endregion

        protected override void LoadFromRegistryObject(JobInstance jobInstance)
        {
            base.LoadFromRegistryObject(jobInstance);

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
            var q = qf.CreateQuery(query);

            var udf = UserDatabaseFactory.Create(context.Federation);
            var udb = udf.GetUserDatabase(context.RegistryUser);
            var usi = udf.GetUserDatabaseServerInstance(context.RegistryUser);

            qf.AppendUserDatabase(q, udb, usi);

            // TODO: Target table settings will need to be modified
            // once multi-select queries are implemented
            q.BatchName = null;
            q.QueryName = this.Name;

            q.Destination = new IO.Tasks.DestinationTable()
            {
                Dataset = context.MyDBDataset,
                DatabaseName = context.MyDBDataset.DatabaseName,
                SchemaName = context.MyDBDataset.DefaultSchemaName
            };

            switch (Queue)
            {
                case JobQueue.Quick:
                    q.Destination.Options = TableInitializationOptions.Drop | TableInitializationOptions.Create;
                    q.Destination.TableNamePattern = Jhu.Graywulf.Jobs.Constants.DefaultQuickResultsTableNamePattern;
                    break;
                case JobQueue.Long:
                    q.Destination.Options = TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName;
                    q.Destination.TableNamePattern = Jhu.Graywulf.Jobs.Constants.DefaultLongResultsTableNamePattern;
                    break;
                default:
                    // This option is used when only parsing queries
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
            // Create the query object from the query test
            var query = CreateQuery(context);

            // Run syntax and other sanity tests
            query.Verify();

            // Schedule as a job
            var qf = QueryFactory.Create(context.Federation);

            this.JobInstance = qf.ScheduleAsJob(this.Name, query, GetQueueName(context), Comments);
            this.JobInstance.Save();

            // Save dependencies
            SaveDependencies();

            // Reload
            LoadFromRegistryObject(this.JobInstance);
        }
    }
}
