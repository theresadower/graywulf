﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Represents different types of database objects
    /// </summary>
    [Flags]
    public enum DatabaseObjectType
    {
        Unknown = 0,
        DataType = 1,
        Table = 2,
        View = 4,
        Function = 8,
        SqlTableValuedFunction = 16,
        SqlInlineTableValuedFunction = 32,
        ClrTableValuedFunction = 64,
        TableValuedFunction = SqlTableValuedFunction | SqlInlineTableValuedFunction | ClrTableValuedFunction,
        SqlScalarFunction = 128,
        ClrScalarFunction = 256,
        ScalarFunction = SqlScalarFunction | ClrScalarFunction,
        SqlStoredProcedure = 512,
        ClrStoredProcedure = 1024,
        StoredProcedure = SqlStoredProcedure | ClrStoredProcedure,
        Index = 2048,
    }

    /// <summary>
    /// Represents different types of function parameters
    /// </summary>
    [Flags]
    public enum ParameterDirection
    {
        Unknown = 0,
        Input = 1,
        Output = 2,
        InputOutput = Input | Output,
        ReturnValue = 4,
    }

    /// <summary>
    /// Represents different types of index column ordering
    /// </summary>
    public enum IndexColumnOrdering
    {
        Unknown,
        Ascending,
        Descending
    }

    [Flags]
    public enum TableInitializationOptions
    {
        None = 0,
        Drop = 1,
        Create = 2,
        Clear = 4,
        Append = 8,
        GenerateUniqueName = 16,
        CreatePrimaryKey = 32,
        CreateIndexes = 64,
    }
}
