﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Jobs.CmdLineUtil
{
    [Verb(Name = "Schedule", Description = "Executes a query in single server mode.")]
    class Query : Parameters
    {
        protected string inputFile;
        protected string queueName;
        protected string outputTable;
        protected string taskName;

        [Parameter(Name = "InputFile", Description = "File containing query.", Required = true)]
        public string InputFile
        {
            get { return inputFile; }
            set { inputFile = value; }
        }

        [Parameter(Name = "Queue", Description = "Queue to schedule in.")]
        public string Queue
        {
            get { return queueName; }
            set { queueName = value; }
        }

        [Parameter(Name = "OutputTable", Description = "Name of output table. Overwrites SELECT INTO.")]
        public string OutputTable
        {
            get { return outputTable; }
            set { outputTable = value; }
        }

        [Parameter(Name = "TaskName", Description = "Task name")]
        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        public Query()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.inputFile = null;
            this.outputTable = null;
            this.taskName = null;
        }

        public override void Run()
        {
            throw new NotImplementedException();

            // TODO: implement query submission logic
            // might require user authentication!
        }
    }
}
