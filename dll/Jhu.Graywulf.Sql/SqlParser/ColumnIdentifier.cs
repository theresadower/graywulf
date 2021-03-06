﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class ColumnIdentifier : ITableReference, IColumnReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

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

            var old = (ColumnIdentifier)other;

            this.columnReference = old.columnReference;
        }

        public static ColumnIdentifier CreateStar()
        {
            var ci = new ColumnIdentifier();

            ci.columnReference = ColumnReference.CreateStar();
            ci.Stack.AddLast(Mul.Create());

            return ci;
        }

        public static ColumnIdentifier CreateStar(TableReference tableReference)
        {
            var ci = new ColumnIdentifier();

            ci.columnReference = ColumnReference.CreateStar(tableReference);
            ci.Stack.AddLast(Mul.Create());

            return ci;
        }

        public static ColumnIdentifier Create(ColumnReference cr)
        {
            var nci = new ColumnIdentifier();
            nci.ColumnReference = cr;

            if (cr.TableReference != null && !cr.TableReference.IsUndefined)
            {
                if (String.IsNullOrEmpty(cr.TableReference.Alias))
                {
                    nci.Stack.AddLast(TableName.Create(cr.TableReference.DatabaseObjectName));
                    nci.Stack.AddLast(Dot.Create());
                }
                else
                {
                    nci.Stack.AddLast(TableName.Create(cr.TableReference.Alias));
                    nci.Stack.AddLast(Dot.Create());
                }
            }

            nci.Stack.AddLast(ColumnName.Create(cr.ColumnName));

            return nci;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
