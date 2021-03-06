﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Name = "Query", Namespace = "")]
    public class SqlQuery : QueryObject, ICloneable
    {
        #region Property storage member variables

        /// <summary>
        /// Destination table including target table naming pattern.
        /// Output table names are either automatically generater or
        /// taken from the INTO clause.
        /// </summary>
        private DestinationTable destination;

        /// <summary>
        /// If true, query destination table has already been initialized.
        /// TODO: this will need to be changed for multi-select queries
        /// </summary>
        private bool isDestinationTableInitialized;

        /// <summary>
        /// Points to the output table of the query.
        /// </summary>
        private Table output;

        /// <summary>
        /// Holds table statistics gathered for all the tables in the query
        /// </summary>
        private List<ITableSource> tableSourceStatistics;

        /// <summary>
        /// Holds the individual partitions. Usually many, but for simple queries
        /// only one.
        /// </summary>
        private List<SqlQueryPartition> partitions;

        /// <summary>
        /// Name of the table used for partitioning
        /// TODO: use Table class or TableReference here!
        /// </summary>
        [NonSerialized]
        private string partitioningTable;

        /// <summary>
        /// Name of the column to partition on
        /// TODO: use ColumnReference here
        /// </summary>
        [NonSerialized]
        private string partitioningKey;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the destination table of the query
        /// </summary>
        [DataMember]
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// Gets whether the destination table is initialized.
        /// </summary>
        [IgnoreDataMember]
        public bool IsDestinationTableInitialized
        {
            get { return isDestinationTableInitialized; }
            internal set { isDestinationTableInitialized = value; }
        }

        [DataMember]
        public Table Output
        {
            get { return output; }
            set { output = value; }
        }

        [IgnoreDataMember]
        public List<ITableSource> TableSourceStatistics
        {
            get { return tableSourceStatistics; }
        }

        [IgnoreDataMember]
        public List<SqlQueryPartition> Partitions
        {
            get { return partitions; }
        }

        /// <summary>
        /// Gets whether the query is partitioned
        /// </summary>
        [IgnoreDataMember]
        public virtual bool IsPartitioned
        {
            get { return SelectStatement.IsPartitioned; }
        }

        [IgnoreDataMember]
        private SqlQueryCodeGenerator CodeGenerator
        {
            get { return CreateCodeGenerator(); }
        }

        #endregion
        #region Constructors and initializer

        protected SqlQuery()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQuery(SqlQuery old)
            : base(old)
        {
            CopyMembers(old);
        }

        public SqlQuery(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());

            this.Context = context;
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.destination = null;
            this.isDestinationTableInitialized = false;
            this.output = null;

            this.tableSourceStatistics = new List<ITableSource>();
            this.partitions = new List<SqlQueryPartition>();

            this.partitioningTable = null;
            this.partitioningKey = null;
        }

        private void CopyMembers(SqlQuery old)
        {
            this.destination = old.destination;
            this.isDestinationTableInitialized = old.isDestinationTableInitialized;
            this.output = old.output;

            this.tableSourceStatistics = new List<ITableSource>();
            this.partitions = new List<SqlQueryPartition>(old.partitions.Select(p => (SqlQueryPartition)p.Clone()));

            this.partitioningTable = old.partitioningTable;
            this.partitioningKey = old.partitioningKey;
        }

        public override object Clone()
        {
            return new SqlQuery(this);
        }

        #endregion
        #region Query parsing and interpretation

        public virtual void Verify()
        {
            // TODO: this is used to validate query before scheduling
            // this needs to be merged with with InitializeQuery.
            // Also: add mechanism to collect validation messages
            InitializeQueryObject(null, null, true);
        }

        protected override void FinishInterpret(bool forceReinitialize)
        {
            // Retrieve target table information
            IntoClause into = SelectStatement.FindDescendantRecursive<IntoClause>();
            if (into != null)
            {
                var sm = GetSchemaManager();

                if (into.TableReference.DatabaseObjectName != null)
                {
                    if (into.TableReference.DatasetName != null)
                    {
                        var ds = (SqlServerDataset)sm.Datasets[into.TableReference.DatasetName];
                        destination.Dataset = ds;
                        destination.DatabaseName = ds.DatabaseName;
                    }

                    destination.SchemaName = into.TableReference.SchemaName ?? destination.Dataset.DefaultSchemaName;
                    destination.TableNamePattern = into.TableReference.DatabaseObjectName;
                }
                
                // Turn off unique name generation in case an into clause is used
                destination.Options &= ~TableInitializationOptions.GenerateUniqueName;
            }
        }

        public override void Validate()
        {
            base.Validate();

            // Perform validation on the query string
            var validator = QueryFactory.CreateValidator();
            validator.Execute(SelectStatement);

            // TODO: add additional validation here
            Destination.CheckTableExistence();
        }


        #endregion
        #region Table statistics

        /// <summary>
        /// Collects tables for which statistics should be computed before
        /// executing the query
        /// </summary>
        /// <returns></returns>
        public virtual void CollectTablesForStatistics()
        {
            TableSourceStatistics.Clear();

            if (IsPartitioned)
            {
                // Partitioning is always done on the table specified right after the FROM keyword
                // TODO: what if more than one QS?
                var qs = SelectStatement.EnumerateQuerySpecifications().FirstOrDefault();
                var ts = (SimpleTableSource)qs.EnumerateSourceTables(false).First();
                var tr = ts.TableReference;

                tr.Statistics = new SqlParser.TableStatistics();

                // TODO: modify this when expression output type functions are implemented
                // and figure out data type directly from expression
                tr.Statistics.KeyColumn = ts.PartitioningKeyExpression;
                tr.Statistics.KeyColumnDataType = ts.PartitioningKeyDataType;

                TableSourceStatistics.Add(ts);
            }
        }

        public Jhu.Graywulf.Schema.DatasetBase GetStatisticsDataset(ITableSource tableSource)
        {
            if (!String.IsNullOrEmpty(StatDatabaseVersionName))
            {
                var nts = (ITableSource)tableSource.Clone();
                var sm = GetSchemaManager();
                var ds = sm.Datasets[tableSource.TableReference.DatasetName];

                if (ds is GraywulfDataset)
                {
                    var dd = ((GraywulfDataset)ds).DatabaseDefinitionReference.Value;
                    var sis = GetAvailableDatabaseInstances(AssignedServerInstance, dd, StatDatabaseVersionName, SourceDatabaseVersionName);

                    if (sis.Length > 0)
                    {
                        var nds = sis[0].GetDataset();
                        return nds;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gather statistics for the table with the specified bin size
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="binSize"></param>
        public void ComputeTableStatistics(ITableSource tableSource, DatasetBase statisticsDataset)
        {
            var stat = tableSource.TableReference.Statistics;

            using (var cmd = CodeGenerator.GetTableStatisticsCommand(tableSource, statisticsDataset))
            {
                ExecuteSqlOnAssignedServerReader(cmd, CommandTarget.Code, dr =>
                {
                    long rc = 0;
                    while (dr.Read())
                    {
                        stat.KeyCount.Add(dr.GetInt64(0));
                        stat.KeyValue.Add((IComparable)dr.GetValue(1));

                        rc = dr.GetInt64(0);    // the very last value will give row count
                    }
                    stat.RowCount = rc;
                });
            }
        }

        #endregion
        #region Query partitioning

        protected virtual SqlQueryCodeGenerator CreateCodeGenerator()
        {
            return new SqlQueryCodeGenerator(this);
        }

        protected virtual SqlQueryPartition CreatePartition()
        {
            return new SqlQueryPartition(this);
        }

        private int DeterminePartitionCount()
        {
            int partitionCount = 1;

            switch (ExecutionMode)
            {
                case Query.ExecutionMode.SingleServer:
                    break;
                case Query.ExecutionMode.Graywulf:
                    // Single server mode will run on one partition by definition,
                    // Graywulf mode has to look at the registry for available machines
                    // If query is partitioned, statistics must be gathered
                    if (IsPartitioned)
                    {
                        var mirroredDatasets = FindMirroredGraywulfDatasets().Values.Select(i => i.DatabaseDefinitionReference.Value).ToArray();
                        var specificDatasets = FindServerSpecificGraywulfDatasets().Values.Select(i => i.DatabaseInstanceReference.Value).ToArray();

                        if (mirroredDatasets.Length == 0)
                        {
                            partitionCount = 1;
                        }
                        else
                        {
                            // *** TODO: find optimal number of partitions
                            // TODO: replace "4" with a value from settings
                            var sis = GetAvailableServerInstances(mirroredDatasets, SourceDatabaseVersionName, null, specificDatasets);
                            partitionCount = 4 * sis.Length;

                            if (MaxPartitions > 0)
                            {
                                partitionCount = Math.Max(partitionCount, MaxPartitions);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return partitionCount;
        }

        public void GeneratePartitions()
        {
            // Partitioning is only supperted using Graywulf mode, single server mode always
            // falls back to a single partition

            int partitionCount = DeterminePartitionCount();

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        OnGeneratePartitions(1, null);
                    }
                    break;
                case ExecutionMode.Graywulf:
                    if (!SelectStatement.IsPartitioned)
                    {
                        OnGeneratePartitions(1, null);
                    }
                    else
                    {
                        // See if maxmimum number of partitions is limited
                        if (MaxPartitions != 0)
                        {
                            partitionCount = Math.Min(partitionCount, MaxPartitions);
                        }

                        // Determine partition limits based on the first table's statistics
                        // In certaint cases all tables of the query are remote tables which
                        // means no statistics are generated at all. In this case a single
                        // partition will be used.
                        if (TableSourceStatistics == null || TableSourceStatistics.Count == 0)
                        {
                            OnGeneratePartitions(1, null);
                        }
                        else
                        {
                            var stat = TableSourceStatistics[0].TableReference.Statistics;
                            OnGeneratePartitions(partitionCount, stat);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Generate partitions based on table statistics.
        /// </summary>
        /// <param name="partitionCount"></param>
        /// <param name="stat"></param>
        protected virtual void OnGeneratePartitions(int partitionCount, Jhu.Graywulf.SqlParser.TableStatistics stat)
        {
            // TODO: fix issue with repeating keys!
            // Maybe just throw those partitions away?

            SqlQueryPartition qp = null;


            if (stat == null || stat.KeyValue.Count / partitionCount == 0)
            {
                qp = CreatePartition();
                AppendPartition(qp);
            }
            else
            {
                int s = stat.KeyValue.Count / partitionCount;

                for (int i = 0; i < partitionCount; i++)
                {
                    qp = CreatePartition();
                    qp.PartitioningKeyMax = stat.KeyValue[Math.Min((i + 1) * s, stat.KeyValue.Count - 1)];

                    if (i == 0)
                    {
                        qp.PartitioningKeyMin = null;
                    }
                    else
                    {
                        qp.PartitioningKeyMin = Partitions[i - 1].PartitioningKeyMax;
                    }

                    AppendPartition(qp);
                }

                Partitions[Partitions.Count - 1].PartitioningKeyMax = null;
            }
        }

        protected void AppendPartition(SqlQueryPartition partition)
        {
            partition.ID = partitions.Count;
            partitions.Add(partition);
        }

        #endregion
        #region Temporary table logic

        public override Table GetTemporaryTable(string tableName)
        {
            string tempname;

            switch (ExecutionMode)
            {
                case Jobs.Query.ExecutionMode.SingleServer:
                    tempname = String.Format("skyquerytemp_{0}", tableName);
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    tempname = String.Format("{0}_{1}_{2}", Context.UserName, Context.JobID, tableName);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return GetTemporaryTableInternal(tempname);
        }

        #endregion
    }
}
