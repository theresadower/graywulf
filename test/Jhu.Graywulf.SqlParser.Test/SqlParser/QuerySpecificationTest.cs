﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class QuerySpecificationTest
    {

        private SelectStatement CreateSelect(string query)
        {
            var p = new SqlParser();
            return (SelectStatement)p.Execute(new SelectStatement(), query);
        }

        private ITableSource[] EnumerateTableSourcesTestHelper(string query, bool recursive)
        {
            return
                (from ats in CreateSelect(query)
                     .EnumerateQuerySpecifications()
                     .First()
                     .EnumerateSourceTables(recursive)
                 select ats).ToArray();
        }

        [TestMethod]
        public void EnumerateTableSourcesTest()
        {
            string sql;
            ITableSource[] tables;

            sql = "SELECT 1";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(0, tables.Length);



            sql = "SELECT * FROM table1";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(1, tables.Length);
            Assert.AreEqual("table1", tables[0].TableReference.DatabaseObjectName);

            sql = "SELECT * FROM table1, table2";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(2, tables.Length);
            Assert.AreEqual("table1", tables[0].TableReference.DatabaseObjectName);
            Assert.AreEqual("table2", tables[1].TableReference.DatabaseObjectName);

            sql = "SELECT * FROM table1, table2, table3";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(3, tables.Length);
            Assert.AreEqual("table1", tables[0].TableReference.DatabaseObjectName);
            Assert.AreEqual("table2", tables[1].TableReference.DatabaseObjectName);
            Assert.AreEqual("table3", tables[2].TableReference.DatabaseObjectName);

            sql = "SELECT * FROM table1 CROSS JOIN table2";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(2, tables.Length);
            Assert.AreEqual("table1", tables[0].TableReference.DatabaseObjectName);
            Assert.AreEqual("table2", tables[1].TableReference.DatabaseObjectName);

            sql = @"
SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(3, tables.Length);
            Assert.AreEqual("Book", tables[0].TableReference.DatabaseObjectName);
            Assert.AreEqual("BookAuthor", tables[1].TableReference.DatabaseObjectName);
            Assert.AreEqual("Author", tables[2].TableReference.DatabaseObjectName);

        }

        [TestMethod]
        public void EnumerateSourcesTables_SubqueryTest()
        {
            string sql;
            ITableSource[] tables;

            sql = "SELECT * FROM table1";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(1, tables.Length);
            Assert.AreEqual("table1", tables[0].TableReference.DatabaseObjectName);

            sql = "SELECT * FROM (SELECT * FROM table1) a";
            tables = EnumerateTableSourcesTestHelper(sql, false);
            Assert.AreEqual(1, tables.Length);
            Assert.AreEqual("a", tables[0].TableReference.Alias);

            sql = "SELECT * FROM (SELECT * FROM table1) a";
            tables = EnumerateTableSourcesTestHelper(sql, true);
            Assert.AreEqual(2, tables.Length);
            Assert.AreEqual("a", tables[0].TableReference.Alias);
            Assert.IsTrue(tables[0].TableReference.IsSubquery);
            Assert.AreEqual("table1", tables[1].TableReference.DatabaseObjectName);
            Assert.IsFalse(tables[1].TableReference.IsSubquery);
        }

        [TestMethod]
        public void EnumerateSourcesTables_SemiJoinTest()
        {
            string sql;
            ITableSource[] tables;

            sql = "SELECT * FROM table1 WHERE ID IN (SELECT * FROM table2)";
            tables = EnumerateTableSourcesTestHelper(sql, true);
            Assert.AreEqual(2, tables.Length);
            Assert.AreEqual("table1", tables[0].TableReference.DatabaseObjectName);
            Assert.IsFalse(tables[0].TableReference.IsSubquery);
            Assert.AreEqual("table2", tables[1].TableReference.DatabaseObjectName);
            Assert.IsFalse(tables[1].TableReference.IsSubquery);
        }

    }
}
