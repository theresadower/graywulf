﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public class PrepareDestinationTable : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            SqlQueryPartition queryPartition = QueryPartition.Get(activityContext);
            SqlQuery query = queryPartition.Query;

            using (Context context = queryPartition.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                query.InitializeQueryObject(context, null, true);
                queryPartition.PrepareDestinationTable(context, activityContext.GetExtension<IScheduler>());
            }
        }
    }
}
