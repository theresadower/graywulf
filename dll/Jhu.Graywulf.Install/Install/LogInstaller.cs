﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Install
{
    public class LogInstaller : DBInstaller
    {
        public LogInstaller()
        {
        }

        public LogInstaller(string connectionString)
            : base(connectionString)
        {
        }

        public override void CreateSchema()
        {
            ExecuteSqlScript(Scripts.Jhu_Graywulf_Logging);
        }
    }
}
