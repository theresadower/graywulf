﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.Query
{
    public class AssignServerInstance : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryObject> QueryObject { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            var queryObject = QueryObject.Get(activityContext);

            switch (queryObject.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    queryObject.InitializeQueryObject(null, null, true);
                    break;
                case ExecutionMode.Graywulf:
                    using (var context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();

                        queryObject.InitializeQueryObject(context, scheduler, false);
                        queryObject.AssignServerInstance();
                        EntityGuid.Set(activityContext, queryObject.AssignedServerInstance.Guid);
                    }
                    break;
            }
        }
    }
}
