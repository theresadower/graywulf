﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstance
    {
        public override IList<DiagnosticMessage> RunDiagnostics()
        {
            if (ServerInstance.Machine.RunningState == RunningState.Running)
            {
                List<DiagnosticMessage> msg = new List<DiagnosticMessage>();

                // Get schema source server
                // Only for federations, cluster level DBs don't have schemas (TEMP)

                if (DeploymentState == DeploymentState.Deployed)
                {
                    msg.Add(TestSqlConnection());
                }

                return msg;
            }
            else
            {
                return base.RunDiagnostics();
            }
        }

        public DiagnosticMessage TestSqlConnection()
        {
            DiagnosticMessage msg = new DiagnosticMessage()
            {
                EntityName = GetFullyQualifiedName(),
                NetworkName = ServerInstance.GetCompositeName(),
                ServiceName = "SQL Connection to Database"
            };

            ServerInstance.RunDiagnostics(GetConnectionString().ConnectionString, msg);

            return msg;
        }
    }
}
