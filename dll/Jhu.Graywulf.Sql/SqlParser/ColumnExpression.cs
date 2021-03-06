﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class ColumnExpression : ITableReference, IColumnReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

        /// <summary>
        /// Gets or sets the table reference associated with this column expression
        /// </summary>
        /// <remarks></remarks>
        public TableReference TableReference
        {
            get { return columnReference.TableReference; }
            set { columnReference.TableReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (ColumnExpression)other;

            this.columnReference = old.columnReference;
        }

        public static ColumnExpression CreateStar()
        {
            var ci = ColumnIdentifier.CreateStar();
            var exp = Expression.Create(ci);
            var ce = Create(exp);

            ce.columnReference = ci.ColumnReference;

            return ce;
        }

        public static ColumnExpression CreateStar(TableReference tableReference)
        {
            var ci = ColumnIdentifier.CreateStar(tableReference);
            var exp = Expression.Create(ci);
            var ce = Create(exp);

            ce.columnReference = ci.ColumnReference;

            return ce;
        }

        public static ColumnExpression Create(Expression exp)
        {
            var ce = new ColumnExpression();

            ce.Stack.AddLast(exp);

            return ce;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
