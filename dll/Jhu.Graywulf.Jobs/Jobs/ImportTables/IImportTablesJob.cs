﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Jobs.ImportTables
{
    public interface IImportTablesJob
    {
        InArgument<ImportTablesParameters> Parameters { get; set; }
    }
}
