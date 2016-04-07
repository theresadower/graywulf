USE [GraywulfEntitiesTest]
GO

IF (OBJECT_ID('EntityWithIdentityKey') IS NOT NULL)
DROP TABLE EntityWithIdentityKey
GO

CREATE TABLE EntityWithIdentityKey
(
	[ID] int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    [Name] nvarchar(50) NOT NULL,
	[SomethingElse] nvarchar(50) NOT NULL,
    [Four] nvarchar(50) NOT NULL,
    [AnsiText] varchar(50) NOT NULL,
	[VarCharText] varchar(10) NOT NULL,
    [SByte] tinyint NOT NULL,
	[Int16] smallint NOT NULL,
	[Int32] int NOT NULL,
	[Int64] bigint NOT NULL,
	[Byte] tinyint NOT NULL,
	[UInt16] smallint NOT NULL,
	[UInt32] int NOT NULL,
	[UInt64] bigint NOT NULL,
	[Single] real NOT NULL,
	[Double] float NOT NULL,
	[Decimal] money NOT NULL,
	[String] nvarchar(50) NOT NULL,
	[ByteArray] varbinary(50) NOT NULL,
	[DateTime] datetime NOT NULL,
	[Guid] uniqueidentifier NOT NULL,
	[XmlElement] xml NOT NULL
)

GO