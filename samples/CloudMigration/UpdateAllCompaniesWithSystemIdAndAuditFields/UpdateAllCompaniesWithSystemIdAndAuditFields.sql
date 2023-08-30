-- Create store procedure to add system id and optionally audit fields to all tables belonging to a specific company.
-- This script supports upgrades from 14.x. If audit fields are added, the target must be at least 17.x, if not, 15.x and 16.x can also be targetted.
-- Parameters:
-- @Company - company name formatted as part of table name schema
-- @Debug_mode - Set to TRUE if you only want to print out DDL statements which will be executed 
-- @IncludeAuditFields - Set to true to also add audit fields (requires that the target version is at least 17.x)
CREATE OR ALTER PROCEDURE UpdateSystemIdForAllTablesInCompany @Company nvarchar(30), @Debug_mode bit, @IncludeAuditFields bit
AS
declare @Sql NVARCHAR(MAX)
,             @AddSystemIdToTables CURSOR;

SET @AddSystemIdToTables = CURSOR FOR

SELECT
'IF NOT EXISTS ( SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'''  + QUOTENAME(name) + ''') AND ' + IIF(@IncludeAuditFields <> 1, 'name = N''$systemId''','name in (N''$systemId'', N''$systemCreatedAt'')') + ') 
BEGIN 
    ALTER TABLE '+ QUOTENAME(name) + 
	' ADD 
        [$systemId] uniqueidentifier NOT NULL CONSTRAINT [MDF$'+name + '$$systemId] DEFAULT NEWSEQUENTIALID() CONSTRAINT ['+ name +'$$systemId] UNIQUE NONCLUSTERED ("$systemId")' +
        IIF(@IncludeAuditFields <> 1, '',',
        [$systemCreatedAt] [datetime] NOT NULL CONSTRAINT [MDF$' + name + '$$systemCreatedAt] DEFAULT (''1753.01.01''), 
	    [$systemCreatedBy] [uniqueidentifier] NOT NULL CONSTRAINT [MDF$' + name + '$$systemCreatedBy] DEFAULT (''00000000-0000-0000-0000-000000000000''), 
	    [$systemModifiedAt] [datetime] NOT NULL CONSTRAINT [MDF$' + name + '$$systemModifiedAt]  DEFAULT (''1753.01.01''),
		[$systemModifiedBy] [uniqueidentifier] NOT NULL CONSTRAINT [MDF$' + name + '$$systemModifiedBy]  DEFAULT (''00000000-0000-0000-0000-000000000000'')') + '
END'
FROM sys.tables WITH (NOLOCK)
WHERE name like @Company + '$%' and TRY_CONVERT(UNIQUEIDENTIFIER, RIGHT(name,36)) IS NULL
;

OPEN @AddSystemIdToTables

FETCH NEXT FROM @AddSystemIdToTables INTO @Sql 

WHILE (@@FETCH_STATUS = 0)
BEGIN

	if (@debug_mode = 'FALSE') 
	    EXEC sp_executesql @Sql 
    else
        print @Sql 

   FETCH NEXT FROM @AddSystemIdToTables INTO @Sql 
END

CLOSE @AddSystemIdToTables
DEALLOCATE @AddSystemIdToTables

GO

-- Create store procedure to add system id to all company specific tables. 
-- Parameters:
-- @Debug_mode - Set to TRUE if you only want to print out DDL statements which will be executed 
-- @IncludeAuditFields - Set to true to also add audit fields (requires that the target version is at least 17.x)
CREATE OR ALTER PROCEDURE AddSystemdIdToAllCompanySpecificTables @Debug_mode bit, @IncludeAuditFields bit
AS
declare       
@SelectCompanyName CURSOR,
@UpdateCompanyName NVARCHAR(MAX)
;

SET @SelectCompanyName= CURSOR FOR

select Name 
from Company WITH (NOLOCK)
;

OPEN @SelectCompanyName

FETCH NEXT FROM @SelectCompanyName INTO @UpdateCompanyName 

WHILE (@@FETCH_STATUS = 0)
BEGIN

-- Format company name 
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, '.', '_')
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, '"', '_')
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, '\', '_')
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, '/', '_')
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, '''', '_')
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, '%', '_')
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, ']', '_')
SET @UpdateCompanyName = REPLACE(@UpdateCompanyName, '[','_')
SET @UpdateCompanyName = 'EXEC UpdateSystemIdForAllTablesInCompany @Company=''' + @UpdateCompanyName + ''', @Debug_mode=''' + CONVERT(nvarchar,@debug_mode)+''', @IncludeAuditFields=''' + CONVERT(nvarchar,@IncludeAuditFields) + ''''

PRINT GETDATE()
PRINT  @UpdateCompanyName 
           
if (@debug_mode = 'FALSE') 
BEGIN
EXEC sp_executesql @UpdateCompanyName
END

FETCH NEXT FROM @SelectCompanyName INTO @UpdateCompanyName
END

CLOSE @SelectCompanyName
DEALLOCATE @SelectCompanyName
GO

EXEC AddSystemdIdToAllCompanySpecificTables true, true
