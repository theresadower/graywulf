﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Activities;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public class SqlQueryFactory : QueryFactory
    {
        public enum Settings
        {
            HotDatabaseVersionName,
            StatDatabaseVersionName,
            DefaultSchemaName,
            DefaultDatasetName,
            DefaultTableName,
            TemporarySchemaName,
            LongQueryTimeout,
        }

        public SqlQueryFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQueryFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        protected override Type[] LoadQueryTypes()
        {
            return new Type[] { typeof(SqlQuery) };
        }

        protected override QueryBase CreateQueryBase(Node root)
        {
            QueryBase res;
            if (root is SelectStatement)
            {
                res = new SqlQuery(Context);
            }
            else
            {
                throw new NotImplementedException();
            }

            return res;
        }

        public override ParserLib.Parser CreateParser()
        {
            return new SqlParser.SqlParser();
        }

        public override SqlValidator CreateValidator()
        {
            return new SqlParser.SqlValidator();
        }

        public override SqlNameResolver CreateNameResolver()
        {
            return new SqlParser.SqlNameResolver();
        }

        protected override void GetInitializedQuery_Graywulf(QueryBase query, string queryString, string outputTable)
        {
            var federationname = Federation.AppSettings.FederationName;

            var ef = new EntityFactory(Context);

            var federation = ef.LoadEntity<Federation>(federationname);

            var jd = ef.LoadEntity<JobDefinition>(federationname, typeof(SqlQueryJob).Name);

            var user = new User(Context);
            user.Guid = Context.UserGuid;
            user.Load();

            // Load settings
            // **** TODO: this always takes settings from SqlQueryJob!!
            var settings = Jhu.Graywulf.Registry.Util.LoadSettings<Settings>(jd.Settings);

            query.ExecutionMode = ExecutionMode.Graywulf;
            query.FederationReference.Name = federationname;
            query.QueryString = queryString;

            query.SourceDatabaseVersionName = settings[Settings.HotDatabaseVersionName];
            query.StatDatabaseVersionName = settings[Settings.StatDatabaseVersionName];
            query.DefaultSchemaName = settings[Settings.DefaultSchemaName];
            query.DefaultDatasetName = settings[Settings.DefaultDatasetName];
            query.QueryTimeout = int.Parse(settings[Settings.LongQueryTimeout]);
            query.ResultsetTarget = ResultsetTarget.DestinationTable;
            query.TemporaryDestinationTableName = "output"; // ****** TODO add to settings
            query.KeepTemporaryDestinationTable = true;

            // Add MyDB as custom source
            var mydbds = new GraywulfDataset();
            mydbds.Name = settings[Settings.DefaultDatasetName];
            mydbds.DefaultSchemaName = settings[Settings.DefaultSchemaName];
            mydbds.DatabaseInstance.Value = user.GetUserDatabaseInstance(federation.MyDBDatabaseVersion);
            query.CustomDatasets.Add(mydbds);

            // Set up MYDB for destination
            // ****** TODO add output table name to settings */
            query.Destination.Table = new Table()
            {
                Dataset = mydbds,
                SchemaName = settings[Settings.DefaultSchemaName],
                TableName = String.IsNullOrWhiteSpace(outputTable) ? "outputtable" : outputTable
            };
            query.Destination.Operation = DestinationTableOperation.Drop | DestinationTableOperation.Create;

            // Set up temporary database
            var tempds = new GraywulfDataset();
            tempds.IsOnLinkedServer = false;
            tempds.DatabaseVersion.Value = federation.TempDatabaseVersion;
            query.TemporaryDataset = tempds;
            query.TemporarySchemaName = settings[Settings.TemporarySchemaName];
        }

        protected override void GetInitializedQuery_SingleServer(QueryBase query, string queryString, string outputTable)
        {
            query.ExecutionMode = ExecutionMode.SingleServer;
            query.QueryString = queryString;

            query.DefaultSchemaName = "dbo";
            query.DefaultDatasetName = "MYDB";
            query.QueryTimeout = 7200;
            query.ResultsetTarget = ResultsetTarget.DestinationTable;
            query.TemporaryDestinationTableName = "output"; // ****** TODO add to settings
            query.KeepTemporaryDestinationTable = true;

            // Add MyDB as custom source
            /*
            GraywulfDataset mydbds = new GraywulfDataset();
            mydbds.Name = settings["DefaultDatasetName"];
            mydbds.DefaultSchemaName = settings["DefaultSchema"];
            mydbds.DatabaseInstanceName = user.MyDbDatabaseInstance.GetFullyQualifiedName();
            query.CustomDatasets.Add(mydbds);

            // Set up MYDB for destination
            query.DestinationDataset = mydbds;
            query.DestinationSchemaName = "dbo";
            if (String.IsNullOrWhiteSpace(outputTable))
            {
                query.DestinationTableName = "outputtable"; // ****** TODO add to settings
            }
            else
            {
                query.DestinationTableName = outputTable;
            }
            query.DestinationTableOperation = DestinationTableOperation.Drop | DestinationTableOperation.Create;
             * */

            // Set up temporary database
            /*
            GraywulfDataset tempds = new GraywulfDataset();
            tempds.IsOnLinkedServer = false;
            tempds.DatabaseDefinitionName = federation.TempDatabaseDefinition.GetFullyQualifiedName();
            query.TemporaryDataset = tempds;
             * */
            query.TemporarySchemaName = "dbo";
        }

        public override JobInstance ScheduleAsJob(QueryBase query, string queueName, string comments)
        {
            var job = CreateJobInstance(
                String.Format("{0}.{1}", Federation.AppSettings.FederationName, typeof(SqlQueryJob).Name),
                queueName,
                comments);

            job.Parameters["Query"].SetValue(query);

            return job;
        }

        public virtual Activity GetAsWorkflow(QueryBase query)
        {
            var wf = new SqlQueryJob();
            SetWorkflowParameters(wf, query);
            return wf;
        }

        protected void SetWorkflowParameters(IQueryJob wf, QueryBase query)
        {
            wf.Query = query;
            wf.UserGuid = Guid.Empty;
            wf.JobGuid = Guid.Empty;
        }
    }
}
