USE [Graywulf_IO_Test]

IF (OBJECT_ID('[dbo].[SampleData]') IS NOT NULL)
DROP TABLE [dbo].[SampleData]

GO

CREATE TABLE [dbo].[SampleData](
	[float] [real] NULL,
	[double] [float] NULL,
	[decimal] [money] NULL,
	[nvarchar(50)] [nvarchar](50) NULL,
	[bigint] [bigint] NULL,
	[int] [int] NOT NULL,
	[tinyint] [tinyint] NULL,
	[smallint] [smallint] NULL,
	[bit] [bit] NULL,
	[ntext] [nvarchar](max) NULL,
	[char] [char](1) NULL,
	[datetime] [datetime] NULL,
	[guid] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

INSERT [dbo].[SampleData]
VALUES (1.234568,
		1.23456789,
		1.2346,
		'this is text',
		123456789,
		123456,
		123,
		12345,
		1,
		N'this is unicode text ő',
		'A',
		'2012-08-17 00:00:00.000',
		'68652251-C9E4-4630-80BE-88B96D3258CE')

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[SampleData_NumericTypes]') IS NOT NULL)
DROP TABLE [dbo].[SampleData_NumericTypes]

GO

CREATE TABLE [dbo].[SampleData_NumericTypes](
	[TinyIntColumn] [tinyint] NOT NULL,
	[SmallIntColumn] [smallint] NOT NULL,
	[IntColumn] [int] NOT NULL,
	[BigIntColumn] [bigint] NOT NULL,
	[RealColumn] [real] NOT NULL,
	[FloatColumn] [float] NOT NULL
) ON [PRIMARY]

GO

INSERT [dbo].[SampleData_NumericTypes]
VALUES (1, 1, 1, 1, 1, 1),
	   (0, -1, -1, -1, -1, -1),
	   (0, 0, 0, 0, 0, 0),
	   (0, -32768, -2147483648, -9223372036854775808, 0, 0),
	   (255, 32767, 2147483647, 9223372036854775807, 1, 1)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[SampleData_NumericTypes_Null]') IS NOT NULL)
DROP TABLE [dbo].[SampleData_NumericTypes_Null]

GO

CREATE TABLE [dbo].[SampleData_NumericTypes_Null](
	[TinyIntColumn] [tinyint] NULL,
	[SmallIntColumn] [smallint] NULL,
	[IntColumn] [int] NULL,
	[BigIntColumn] [bigint] NULL,
	[RealColumn] [real] NULL,
	[FloatColumn] [float] NULL
) ON [PRIMARY]

GO

INSERT [dbo].[SampleData_NumericTypes_Null]
VALUES (NULL, NULL, NULL, NULL, NULL, NULL),
	   (1, 1, 1, 1, 1, 1),
	   (0, -1, -1, -1, -1, -1),
	   (0, 0, 0, 0, 0, 0),
	   (0, -32768, -2147483648, -9223372036854775808, 0, 0),
	   (255, 32767, 2147483647, 9223372036854775807, 1, 1)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[SampleData_AllTypes]') IS NOT NULL)
DROP TABLE [dbo].[SampleData_AllTypes]

GO

CREATE TABLE [dbo].[SampleData_AllTypes](
-- Exact Numerics
	[BigIntColumn] bigint NOT NULL,
	[NumericColumn] numeric NOT NULL,
	[BitColumn] bit  NOT NULL,
	[SmallIntColumn] smallint NOT NULL,
	[DecimalColumn] decimal NOT NULL,
	[SmallMoneyColumn] smallmoney NOT NULL,
	[IntColumn] int  NOT NULL,
	[TinyIntColumn] tinyint NOT NULL,
	[MoneyColumn] money NOT NULL,
-- Approximate Numerics
	[FloatColumn] float  NOT NULL,
	[RealColumn] real NOT NULL,
-- Date and Time
	[DateColumn] date NOT NULL,
	[DateTimeOffsetColumn] datetimeoffset NOT NULL,
	[DateTime2Column] datetime2 NOT NULL,
	[SmallDateTimeColumn] smalldatetime NOT NULL,
	[DateTimeColumn] datetime NOT NULL,
	[TimeColumn] time NOT NULL,
-- Character Strings
	[CharColumn] char(10) NOT NULL,
	[VarCharColumn] varchar(10) NOT NULL,
	[VarCharMaxColumn] varchar(max) NOT NULL,
	[TextColumn] text NOT NULL,
-- Unicode Character Strings
	[NCharColumn] nchar(10) NOT NULL,
	[NVarCharColumn] nvarchar(10) NOT NULL,
	[NVarCharMaxColumn] nvarchar(max) NOT NULL, 
	[NTextColumn] ntext NOT NULL,
-- Binary Strings
	[BinaryColumn] binary(10) NOT NULL,
	[VarBinaryColumn] varbinary(10) NOT NULL,
	[VarBinaryMaxColumn] varbinary(max) NOT NULL,
	[ImageColumn] image NOT NULL,
-- Other Data Types
	-- cursor 
	[TimeStampColumn] timestamp,
	-- hierarchyid 
	[UniqueIdentifierColumn] uniqueidentifier NOT NULL
	-- sql_variant 
	-- xml 
	-- table
)

GO
 
INSERT [dbo].[SampleData_AllTypes]
VALUES (
-- Exact Numerics
	1234567890, -- bigint 
	123.456789, -- numeric 
	1, -- bit 
	12345,      -- smallint 
	1234.56789, -- decimal 
	123456.89, -- smallmoney 
	123456789, -- int
	123, -- tinyint 
	1234567.89, -- money 
-- Approximate Numerics
	123.456789, -- float
	1.23456789, -- real 
-- Date and Time
	'2014-01-01', -- date 
	'2014-01-01 11:59:59.123 +1:00', -- datetimeoffset 
	'2014-01-01 11:59:59.123', -- datetime2 
	'2014-01-01 11:59:59.123', -- smalldatetime 
	'2014-01-01 11:59:59.123', -- datetime 
	'11:59:59.123', -- time 
-- Character Strings
	'0123456789', -- char(10)
	'012345', -- varchar(10)
	'1234567890', -- vharchar(max)
	'1234567890', -- text 
-- Unicode Character Strings
	N'áíűőüöúóé', -- nchar(10)
	N'áíűőüöúóé', -- nvarchar(10)
	N'áíűőüöúóé', -- nvharchar(max)
	N'áíűőüöúóé', -- ntext 
-- Binary Strings
	0x01020304050607080900, -- binary(10)
	0x0102030405, -- varbinary(10)
	0x01020304050607080900, -- varbinary(max)
	0x01020304050607080900, -- image 
-- Other Data Types
	-- cursor 
	NULL, -- timestamp
	-- hierarchyid
	'A4466A24-5140-4EFB-BB7D-0E4B4F34A75E' -- uniqueidentifier
	-- sql_variant
	-- xml
	-- table 
 )

 GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[SampleData_AllTypes_Nullable]') IS NOT NULL)
DROP TABLE [dbo].[SampleData_AllTypes_Nullable]

GO

 CREATE TABLE [dbo].[SampleData_AllTypes_Nullable](
-- Exact Numerics
	[BigIntColumn] bigint NULL,
	[NumericColumn] numeric NULL,
	[BitColumn] bit  NULL,
	[SmallIntColumn] smallint NULL,
	[DecimalColumn] decimal NULL,
	[SmallMoneyColumn] smallmoney NULL,
	[IntColumn] int  NULL,
	[TinyIntColumn] tinyint NULL,
	[MoneyColumn] money NULL,
-- Approximate Numerics
	[FloatColumn] float  NULL,
	[RealColumn] real NULL,
-- Date and Time
	[DateColumn] date NULL,
	[DateTimeOffsetColumn] datetimeoffset NULL,
	[DateTime2Column] datetime2 NULL,
	[SmallDateTimeColumn] smalldatetime NULL,
	[DateTimeColumn] datetime NULL,
	[TimeColumn] time NULL,
-- Character Strings
	[CharColumn] char(10) NULL,
	[VarCharColumn] varchar(10) NULL,
	[VarCharMaxColumn] varchar(max) NULL,
	[TextColumn] text NULL,
-- Unicode Character Strings
	[NCharColumn] nchar(10) NULL,
	[NVarCharColumn] nvarchar(10) NULL,
	[NVarCharMaxColumn] nvarchar(max) NULL, 
	[NTextColumn] ntext NULL,
-- Binary Strings
	[BinaryColumn] binary(10) NULL,
	[VarBinaryColumn] varbinary(10) NULL,
	[VarBinaryMaxColumn] varbinary(max) NULL,
	[ImageColumn] image NULL,
-- Other Data Types
	-- cursor 
	[TimeStampColumn] timestamp,
	-- hierarchyid 
	[UniqueIdentifierColumn] uniqueidentifier NULL
	-- sql_variant 
	-- xml 
	-- table
)

GO

INSERT [dbo].[SampleData_AllTypes_Nullable]
VALUES (
-- Exact Numerics
	1234567890, -- bigint 
	123.456789, -- numeric 
	1, -- bit 
	12345,      -- smallint 
	1234.56789, -- decimal 
	123456.89, -- smallmoney 
	123456789, -- int
	123, -- tinyint 
	1234567.89, -- money 
-- Approximate Numerics
	123.456789, -- float
	1.23456789, -- real 
-- Date and Time
	'2014-01-01', -- date 
	'2014-01-01 11:59:59.123 +1:00', -- datetimeoffset 
	'2014-01-01 11:59:59.123', -- datetime2 
	'2014-01-01 11:59:59.123', -- smalldatetime 
	'2014-01-01 11:59:59.123', -- datetime 
	'11:59:59.123', -- time 
-- Character Strings
	'0123456789', -- char(10)
	'012345', -- varchar(10)
	'1234567890', -- vharchar(max)
	'1234567890', -- text 
-- Unicode Character Strings
	N'áíűőüöúóé', -- nchar(10)
	N'áíűőüöúóé', -- nvarchar(10)
	N'áíűőüöúóé', -- nvharchar(max)
	N'áíűőüöúóé', -- ntext 
-- Binary Strings
	0x01020304050607080900, -- binary(10)
	0x0102030405, -- varbinary(10)
	0x01020304050607080900, -- varbinary(max)
	0x01020304050607080900, -- image 
-- Other Data Types
	-- cursor 
	NULL, -- timestamp
	-- hierarchyid
	'A4466A24-5140-4EFB-BB7D-0E4B4F34A75E' -- uniqueidentifier
	-- sql_variant
	-- xml
	-- table 
 ),
 (
-- Exact Numerics
	NULL, -- bigint 
	NULL, -- numeric 
	NULL, -- bit 
	NULL,      -- smallint 
	NULL, -- decimal 
	NULL, -- smallmoney 
	NULL, -- int
	NULL, -- tinyint 
	NULL, -- money 
-- Approximate Numerics
	NULL, -- float
	NULL, -- real 
-- Date and Time
	NULL, -- date 
	NULL, -- datetimeoffset 
	NULL, -- datetime2 
	NULL, -- smalldatetime 
	NULL, -- datetime 
	NULL, -- time 
-- Character Strings
	NULL, -- char(10)
	NULL, -- varchar(10)
	NULL, -- vharchar(max)
	NULL, -- text 
-- Unicode Character Strings
	NULL, -- nchar(10)
	NULL, -- nvarchar(10)
	NULL, -- nvharchar(max)
	NULL, -- ntext 
-- Binary Strings
	NULL, -- binary(10)
	NULL, -- varbinary(10)
	NULL, -- varbinary(max)
	NULL, -- image 
-- Other Data Types
	-- cursor 
	NULL, -- timestamp
	-- hierarchyid
	NULL -- uniqueidentifier
	-- sql_variant
	-- xml
	-- table 
 )

 GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[SampleData_AllPrecision]') IS NOT NULL)
DROP TABLE [dbo].[SampleData_AllPrecision]

GO

CREATE TABLE [dbo].[SampleData_AllPrecision]
(
	[Decimal_01] decimal( 1) NOT NULL,
	
	[Decimal_02] decimal(3,2) NOT NULL,

	[Decimal_38] decimal(38) NOT NULL,
	[DateTime2_0] datetime2(0) NOT NULL,
	[DateTime2_1] datetime2(1) NOT NULL,
	[DateTime2_2] datetime2(2) NOT NULL,
	[DateTime2_3] datetime2(3) NOT NULL,
	[DateTime2_4] datetime2(4) NOT NULL,
	[DateTime2_5] datetime2(5) NOT NULL,
	[DateTime2_6] datetime2(6) NOT NULL,
	[DateTime2_7] datetime2(7) NOT NULL,
	[DateTimeOffset_0] datetimeoffset(0) NOT NULL,
	[DateTimeOffset_1] datetimeoffset(1) NOT NULL,
	[DateTimeOffset_2] datetimeoffset(2) NOT NULL,
	[DateTimeOffset_3] datetimeoffset(3) NOT NULL,
	[DateTimeOffset_4] datetimeoffset(4) NOT NULL,
	[DateTimeOffset_5] datetimeoffset(5) NOT NULL,
	[DateTimeOffset_6] datetimeoffset(6) NOT NULL,
	[DateTimeOffset_7] datetimeoffset(7) NOT NULL,
	[Time_0] time(0) NOT NULL,
	[Time_1] time(1) NOT NULL,
	[Time_2] time(2) NOT NULL,
	[Time_3] time(3) NOT NULL,
	[Time_4] time(4) NOT NULL,
	[Time_5] time(5) NOT NULL,
	[Time_6] time(6) NOT NULL,
	[Time_7] time(7) NOT NULL
)

GO

INSERT [dbo].[SampleData_AllPrecision]
VALUES
	(1, 1, 1,
	 '1900-01-01 00:00:00.000', '1900-01-01 00:00:00.000', '1900-01-01 00:00:00.000', '1900-01-01 00:00:00.000', '1900-01-01 00:00:00.000', '1900-01-01 00:00:00.000', '1900-01-01 00:00:00.000', '1900-01-01 00:00:00.000',
	 '1900-01-01 00:00:00.000 +0:00', '1900-01-01 00:00:00.000 +0:00', '1900-01-01 00:00:00.000 +0:00', '1900-01-01 00:00:00.000 +0:00', '1900-01-01 00:00:00.000 +0:00', '1900-01-01 00:00:00.000 +0:00', '1900-01-01 00:00:00.000 +0:00', '1900-01-01 00:00:00.000 +0:00',
	 '00:00:00.000', '00:00:00.000', '00:00:00.000', '00:00:00.000', '00:00:00.000', '00:00:00.000', '00:00:00.000', '00:00:00.000'),

	(1, 1, 1,
	 '1900-01-01 00:00:01.000', '1900-01-01 00:00:01.000', '1900-01-01 00:00:01.000', '1900-01-01 00:00:01.000', '1900-01-01 00:00:01.000', '1900-01-01 00:00:01.000', '1900-01-01 00:00:01.000', '1900-01-01 00:00:01.000',
	 '1900-01-01 00:00:01.000 +1:00', '1900-01-01 00:00:01.000 +1:00', '1900-01-01 00:00:01.000 +1:00', '1900-01-01 00:00:01.000 +1:00', '1900-01-01 00:00:01.000 +1:00', '1900-01-01 00:00:01.000 +1:00', '1900-01-01 00:00:01.000 +1:00', '1900-01-01 00:00:01.000 +1:00',
	 '00:00:01.000', '00:00:01.000', '00:00:01.000', '00:00:01.000', '00:00:01.000', '00:00:01.000', '00:00:01.000', '00:00:01.000')

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[SampleData_PrimaryKey]') IS NOT NULL)
DROP TABLE [dbo].[SampleData_PrimaryKey]

GO

CREATE TABLE [dbo].[SampleData_PrimaryKey](
	[ID] int NOT NULL PRIMARY KEY,
	Data1 float,
	Data2 float
)

GO

INSERT [dbo].[SampleData_PrimaryKey]
VALUES
	(1, 1.0, 2.0),
	(2, 2.0, 4.0)

GO