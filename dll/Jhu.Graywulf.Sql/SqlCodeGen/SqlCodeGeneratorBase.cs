﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlCodeGen
{
    public abstract class SqlCodeGeneratorBase : CodeGenerator
    {
        #region Private members

        private bool resolveNames;
        private bool quoteIdentifiers;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets whether to use resolved names in the generated code
        /// </summary>
        public bool ResolveNames
        {
            get { return resolveNames; }
            set { resolveNames = value; }
        }

        #endregion
        #region Constructors and initializers

        protected SqlCodeGeneratorBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.resolveNames = false;
            this.quoteIdentifiers = false;
        }

        #endregion

        public abstract SqlColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType); 

        #region Identifier formatting functions

        /// <summary>
        /// Puts quotes around an identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <remarks>
        /// The quoting characters used depends on the flavor of SQL
        /// </remarks>
        protected abstract string GetQuotedIdentifier(string identifier);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <remarks>
        /// Unquoting depends on the actual input and not on the
        /// code generator, so this function is implemented as non-
        /// virtual
        /// </remarks>
        public string UnquoteIdentifier(string identifier)
        {
            if (identifier[0] == '[' && identifier[identifier.Length - 1] == ']')
            {
                return identifier.Substring(1, identifier.Length - 2);
            }
            else
            {
                return identifier;
            }
        }

        public string EscapeIdentifier(string identifier)
        {
            return identifier.Replace(".", "_");
        }

        public string GenerateEscapedUniqueName(TableReference table)
        {
            if (table.IsSubquery || table.IsComputed)
            {
                // We consider a table alias unique within a query, although this is
                // not a requirement by SQL Server which support using the same alias
                // in subqueries.

                return EscapeIdentifier(table.Alias);
            }
            else
            {
                string res = String.Empty;

                if (!String.IsNullOrWhiteSpace(table.DatasetName))
                {
                    res += String.Format("{0}_", EscapeIdentifier(table.DatasetName));
                }

                if (!String.IsNullOrWhiteSpace(table.DatabaseName))
                {
                    res += String.Format("{0}_", EscapeIdentifier(table.DatabaseName));
                }

                if (!String.IsNullOrWhiteSpace(table.SchemaName))
                {
                    res += String.Format("{0}_", EscapeIdentifier(table.SchemaName));
                }

                if (!String.IsNullOrWhiteSpace(table.DatabaseObjectName))
                {
                    res += String.Format("{0}", EscapeIdentifier(table.DatabaseObjectName));
                }

                // If a table is referenced more than once we need an alias to distinguish them
                if (!String.IsNullOrWhiteSpace(table.Alias))
                {
                    res += String.Format("_{0}", EscapeIdentifier(table.Alias));
                }

                return res;
            }
        }


        public string GetResolvedTableName(TableReference table)
        {
            if (table.IsSubquery || table.IsComputed)
            {
                return GetQuotedIdentifier(table.Alias);
            }
            else
            {
                if (table.DatabaseObject != null)
                {
                    // If it is linked up to the schema already
                    return GetResolvedTableName(table.DatabaseObject);
                }
                else
                {
                    // If it's not resolved yet against the schema
                    return GetResolvedTableName(table.DatabaseName, table.SchemaName, table.DatabaseObjectName);
                }
            }
        }

        public string GetResolvedTableNameWithAlias(TableReference table)
        {
            if (table.IsSubquery || table.IsComputed)
            {
                return GetQuotedIdentifier(table.Alias);
            }
            else
            {
                string name;

                if (table.DatabaseObject != null)
                {
                    // If it is linked up to the schema already
                    name = GetResolvedTableName(table.DatabaseObject);
                }
                else
                {
                    // If it's not resolved yet against the schema
                    name = GetResolvedTableName(table.DatabaseName, table.SchemaName, table.DatabaseObjectName);
                }

                if (String.IsNullOrEmpty(table.Alias))
                {
                    return name;
                }
                else
                {
                    return name + " AS " + GetQuotedIdentifier(table.Alias);
                }
            }
        }

        public string GetUniqueName(TableReference table)
        {
            if (table.IsSubquery || table.IsComputed || !String.IsNullOrWhiteSpace(table.Alias))
            {
                return GetQuotedIdentifier(table.Alias);
            }
            else
            {
                return GetResolvedTableName(table);
            }
        }

        public string GeneratePrimaryKeyName(DatabaseObject table)
        {
            return GeneratePrimaryKeyName(table.SchemaName, table.ObjectName);
        }

        public string GeneratePrimaryKeyName(string schemaName, string tableName)
        {
            return GetQuotedIdentifier(String.Format("PK_{0}_{1}", schemaName, tableName));
        }

        public string GetResolvedTableName(DatabaseObject table)
        {
            return GetResolvedTableName(table.DatabaseName, table.SchemaName, table.ObjectName);
        }

        public string GetResolvedTableNameWithAlias(DatabaseObject table, string alias)
        {
            return GetResolvedTableNameWithAlias(table.DatabaseName, table.SchemaName, table.ObjectName, alias);
        }

        protected abstract string GetResolvedTableName(string databaseName, string schemaName, string tableName);

        protected virtual string GetResolvedTableNameWithAlias(string databaseName, string schemaName, string tableName, string alias)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                return GetResolvedTableName(databaseName, schemaName, tableName);
            }
            else
            {
                return GetResolvedTableName(databaseName, schemaName, tableName) + " AS " + GetQuotedIdentifier(alias);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks>
        /// In this function call, tables are always referenced by
        /// their aliases, if they have one.
        /// </remarks>
        public string GetResolvedColumnName(ColumnReference column)
        {
            string tablename;

            if (column.TableReference == null)
            {
                tablename = null;
            }
            else if (!String.IsNullOrEmpty(column.TableReference.Alias))
            {
                tablename = GetQuotedIdentifier(column.TableReference.Alias);
            }
            else
            {
                tablename = GetResolvedTableName(column.TableReference);
            }

            string columnname;

            if (column.IsStar)
            {
                columnname = "*";
            }
            else
            {
                columnname = GetQuotedIdentifier(column.ColumnName);
            }


            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(tablename))
            {
                res = tablename + ".";
            }

            res += columnname;

            return res;
        }

        private string GetResolvedFunctionName(FunctionReference function)
        {
            if (function.IsSystem)
            {
                // This is a built-in function
                return function.SystemFunctionName.ToUpperInvariant();
            }
            else if (function.DatabaseObject != null)
            {
                // If it is linked up to the schema already
                return GetResolvedFunctionName(function.DatabaseObject.DatabaseName, function.DatabaseObject.SchemaName, function.DatabaseObject.ObjectName);
            }
            else
            {
                // If it's not resolved yet against the schema
                return GetResolvedFunctionName(function.DatabaseName, function.SchemaName, function.DatabaseObjectName);
            }
        }

        protected abstract string GetResolvedFunctionName(string databaseName, string schemaName, string functionName);

        #endregion
        #region Specialized node writer functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <remarks>
        /// Do dispatching here based on node type
        /// </remarks>
        protected override void WriteNode(Token token)
        {
            if (token is TableAlias)
            {
                WriteTableAlias((TableAlias)token);
            }
            else if (token is TableOrViewName)
            {
                WriteTableOrViewName((TableOrViewName)token);
            }
            else if (token is ColumnIdentifier)
            {
                WriteColumnIdentifier((ColumnIdentifier)token);
            }
            else if (token is ColumnExpression)
            {
                WriteColumnExpression((ColumnExpression)token);
            }
            else if (token is FunctionIdentifier)
            {
                WriteFunctionIdentifier((FunctionIdentifier)token);
            }
            else
            {
                base.WriteNode(token);
            }
        }

        public void WriteTableAlias(TableAlias node)
        {
            Writer.Write(GetQuotedIdentifier(node.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>
        /// Table names are only written by this function when the
        /// table appears in the FROM clause. In all other cases it's
        /// WriteColumnIdentifier that generates the output
        /// </remarks>
        public void WriteTableOrViewName(TableOrViewName node)
        {
            if (resolveNames)
            {
                Writer.Write(GetResolvedTableName(node.TableReference));
            }
            else
            {
                // Fall back to original behavior
                base.WriteNode(node);
            }
        }

        /// <summary>
        /// Writes a column identifier, optionally with resolved
        /// names and quoted
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public void WriteColumnIdentifier(ColumnIdentifier node)
        {
            if (ResolveNames)
            {
                Writer.Write(GetResolvedColumnName(node.ColumnReference));
            }
            else
            {
                // Fall back to original behavior
                base.WriteNode(node);
            }
        }

        /// <summary>
        /// Writes a column expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual void WriteColumnExpression(ColumnExpression node)
        {
            // A column expression is in the form of an expression,
            // optionally followed by a column alias in the form of
            // 'AS alias'

            if (resolveNames)
            {
                // Write the expression first as it is
                var exp = node.FindDescendant<Expression>();
                WriteNode(exp);

                // If it's not a * column and there's an alias, write it
                if (!node.ColumnReference.IsStar && !String.IsNullOrEmpty(node.ColumnReference.ColumnAlias))
                {
                    Writer.Write(
                        " AS {0}",
                        GetQuotedIdentifier(node.ColumnReference.ColumnAlias));
                }
            }
            else
            {
                // Fall back to original behavior
                base.WriteNode(node);
            }
        }

        public virtual void WriteFunctionIdentifier(FunctionIdentifier node)
        {
            if (resolveNames)
            {
                Writer.Write(GetResolvedFunctionName(node.FunctionReference));
            }
            else
            {
                // Fall back to original behavior
                base.WriteNode(node);
            }
        }

        #endregion
        #region Complete query generators
        // These functions don't use the parsing tree, they generate certain
        // types of queries.

        public abstract string GenerateSelectStarQuery(TableReference tableReference, int top);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkedServerName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        /// <remarks>This is used by the web interface's 'peek' function</remarks>
        public abstract string GenerateSelectStarQuery(TableOrView tableOrView, int top);

        protected abstract string GenerateTopExpression(int top);

        public abstract string GenerateMostRestrictiveTableQuery(QuerySpecification querySpecification, TableReference table, ColumnContext columnContext, int top);

        public virtual string GenerateCountStarQuery(string subquery)
        {
            // TODO: modify to parse query first into separate SELECTS
            // remove ending ; etc

            // Use count_big maybe?

            return String.Format("SELECT CAST(COUNT(*) AS bigint) FROM ({0}) __countstar", subquery);
        }

        #endregion
    }
}
