﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlCodeGen.PostgreSql
{
    public class PostgreSqlColumnListGenerator : SqlColumnListGeneratorBase
    {
        public PostgreSqlColumnListGenerator(IEnumerable<ColumnReference> columns)
            : base(columns)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override string QuoteIdentifier(string identifier)
        {
            return PostgreSqlCodeGenerator.QuoteIdentifier(identifier);
        }
    }
}
