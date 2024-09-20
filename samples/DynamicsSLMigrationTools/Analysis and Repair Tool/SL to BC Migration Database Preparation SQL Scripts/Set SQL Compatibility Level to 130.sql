SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

-- Set Compatibility Level to 130
DECLARE @DBName AS VARCHAR(128) = DB_Name()
IF (SELECT [compatibility_level] FROM sys.databases WHERE [name] = @DBName) < 130
    EXEC('ALTER DATABASE ' + @DBName + ' SET COMPATIBILITY_LEVEL = 130')
GO