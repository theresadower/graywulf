﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Argument
    {
        public Expression Expression
        {
            get { return FindDescendant<Expression>(); }
        }

        public static Argument Create(Expression expression)
        {
            var arg = new Argument();
            arg.Stack.AddLast(expression);
            return arg;
        }
    }
}
