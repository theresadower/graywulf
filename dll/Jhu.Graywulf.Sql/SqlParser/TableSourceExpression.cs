﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class TableSourceExpression
    {
        public static TableSourceExpression Create(TableSource tableSource)
        {
            return Create(tableSource, null);
        }

        public static TableSourceExpression Create(TableSource tableSource, JoinedTable joinedTable)
        {
            var tse = new TableSourceExpression();

            tse.Stack.AddLast(tableSource);

            if (joinedTable != null)
            {
                tse.Stack.AddLast(Whitespace.CreateNewLine());
                tse.Stack.AddLast(joinedTable);
            }

            return tse;
        }
    }
}
