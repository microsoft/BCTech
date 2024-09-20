SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

----------------------------------BEGIN STORED PROC BCMChangeTracking---------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects where [object_id] = OBJECT_ID(N'dbo.BCMChangeTracking') AND [type] = N'P')
    DROP PROCEDURE BCMChangeTracking
GO

CREATE PROCEDURE BCMChangeTracking @Action VARCHAR(10) = 'STATE' AS
SET NOCOUNT ON
DECLARE @SQLVersion AS CHAR(2) = (SELECT LEFT(CONVERT(VARCHAR, SERVERPROPERTY('ProductVersion')), 2))
DECLARE @AppDatabaseName AS VARCHAR(128) = DB_NAME()
DECLARE @AppDatabaseID AS SMALLINT = DB_ID(@AppDatabaseName)
DECLARE @log AS VARCHAR(4000) = 'BCMChangeTracking' + CHAR(10) -- Store info about what's occurring.
DECLARE @msgText AS VARCHAR(500) -- Temp storage for message text

-- Check the SQL instance version
IF @SQLVersion >= '13'
    SET @log = @log + 'SQL Server ' + @@SERVERNAME + '; SQL Version ' + @SQLVersion + CHAR(10)
ELSE
BEGIN
    SET @log = @log + 'FAILURE: SQL Server ' + @@SERVERNAME + '; SQL Version ' + @SQLVersion + ' is not supported; Minimum SQL Version required is 13 (SQL 2016)' + CHAR(10)
	PRINT @log
	SET @msgText = 'SQL Server ' + @@SERVERNAME + '; SQL Version ' + @SQLVersion + ' is not supported; Minimum SQL Version required is 13 (SQL 2016)';
	THROW 550030, @msgText, 1
	RETURN
END

-- Check database version before doing anything
DECLARE @version AS REAL = 5.0
IF EXISTS(SELECT * FROM sys.objects WHERE [name] = 'getversion' AND [type] = 'P')
BEGIN
	CREATE TABLE #tmpVersion (currentversion CHAR(20))
	INSERT INTO #tmpVersion EXEC getversion
	SELECT @version = CONVERT(REAL, SUBSTRING(currentversion, 1, 4)),
	       @log = @log + 'Database: ' + @AppDatabaseName + '; Version: ' + RTRIM(currentversion + CHAR(10))
	FROM #tmpVersion
	DROP TABLE #tmpVersion
END
ElSE -- This should never happen with an DSL database within Database Maintenance
BEGIN
    SET @log = @log + 'Database: ' + @AppDatabaseName + '; Version: <ERROR: Unknown database version>' + CHAR(10)
	PRINT @log
	SET @msgText = 'Database ' + @AppDatabaseName + '; Unknown database version';
	THROW 550031, @msgText, 1
	RETURN
END

IF @version < 9.00 -- DSL 2015 uses a version 9 database
BEGIN
    SET @log = @log + 'FAILURE: Database version ' + @version + ' is not supported' + CHAR(10)
	PRINT @log
	SET @msgText = 'Database ' + @AppDatabaseName + '; Database version ' + @version + ' is not supported';
	THROW 550032, @msgText, 1
	RETURN
END

-- Valid Action parameters are: '
--    'ENABLE', 'ON', or 'TRUE' = Enable BC Migration Change Tracking
--    'DISABLE', 'OFF', or 'FALSE' = Disable BC Migration Change Tracking
--    'STATE' = Show the current state of BC Migration Change Tracking
--'STATE' is the default when no parameter is passed
SET @Action = UPPER(@Action)
IF @Action = 'TRUE' OR @Action = 'ON'
    SET @Action = 'ENABLE'
ELSE IF @Action = 'FALSE' OR @Action = 'OFF'
    SET @Action = 'DISABLE'
-- FYI - an invalid parm will behave as if they requested 'STATE', but also fling message about valid parameters

-- Setup a table of tables for Change Tracking
DECLARE @Tables TABLE(tableName VARCHAR(128) NOT NULL, isColumnTracked BIT NOT NULL)

INSERT INTO @Tables VALUES ('APAdjust', 'FALSE')
INSERT INTO @Tables VALUES ('APDoc', 'FALSE')
INSERT INTO @Tables VALUES ('APTran', 'FALSE')
INSERT INTO @Tables VALUES ('ARAdjust', 'FALSE')
INSERT INTO @Tables VALUES ('ARDoc', 'FALSE')
INSERT INTO @Tables VALUES ('ARTran', 'FALSE')
INSERT INTO @Tables VALUES ('Batch', 'FALSE')
INSERT INTO @Tables VALUES ('INTran', 'FALSE')
INSERT INTO @Tables VALUES ('InventoryADG', 'FALSE')
INSERT INTO @Tables VALUES ('ItemCost', 'FALSE')
INSERT INTO @Tables VALUES ('Item2Hist', 'FALSE')
INSERT INTO @Tables VALUES ('ItemHist', 'FALSE')
INSERT INTO @Tables VALUES ('ItemXRef', 'FALSE')
INSERT INTO @Tables VALUES ('LotSerMst', 'FALSE')
INSERT INTO @Tables VALUES ('LotSerT', 'FALSE')
INSERT INTO @Tables VALUES ('POAddress', 'FALSE')
INSERT INTO @Tables VALUES ('POReceipt', 'FALSE')
INSERT INTO @Tables VALUES ('POSetup', 'FALSE')
INSERT INTO @Tables VALUES ('POTran', 'FALSE')
INSERT INTO @Tables VALUES ('ProductClass', 'FALSE')
INSERT INTO @Tables VALUES ('PurchOrd', 'FALSE')
INSERT INTO @Tables VALUES ('PurOrdDet', 'FALSE')
INSERT INTO @Tables VALUES ('Site', 'FALSE')
INSERT INTO @Tables VALUES ('SOAddress', 'FALSE')
INSERT INTO @Tables VALUES ('SOHeader', 'FALSE')
INSERT INTO @Tables VALUES ('SOLine', 'FALSE')
INSERT INTO @Tables VALUES ('SOSetup', 'FALSE')
INSERT INTO @Tables VALUES ('SOShipHeader', 'FALSE')
INSERT INTO @Tables VALUES ('SOShipLine', 'FALSE')
INSERT INTO @Tables VALUES ('SOShipLot', 'FALSE')
INSERT INTO @Tables VALUES ('SOType', 'FALSE')

-- If there are tables to process
IF (SELECT COUNT(*) FROM @Tables) <> 0
BEGIN
    -- If turn on Change Tracking requested
    IF @Action = 'ENABLE'
    BEGIN
        SET @log = @log + 'Enabling Change Tracking' + CHAR(10)

        -- If Database Change Tracking is on
        IF EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = @AppDatabaseID)
            SET @log = @log + CHAR(9) + 'Database Change Tracking already enabled' + CHAR(10)
        ELSE -- Database Change Tracking is off
        BEGIN 
            -- If Snapshot Isolation is on
            IF EXISTS (SELECT * FROM sys.databases WHERE database_id = @AppDatabaseID AND snapshot_isolation_state IN (1, 3))
                SET @log = @log + CHAR(9) + 'Snapshot Isolation already on' + CHAR(10)
            ELSE -- Snapshot Isolation is off
            BEGIN
	            -- MS SQL highly recommends turning on Snapshot Isolation when Change Tracking is on
				-- Only need to turn this on if sync queries will be run within an isolated snapshot transaction (SET TRANSACTION ISOLATION LEVEL SNAPSHOT)
				-- https://docs.microsoft.com/en-us/sql/t-sql/statements/alter-database-transact-sql-set-options?view=sql-server-2017
				-- https://docs.microsoft.com/en-us/sql/t-sql/statements/set-transaction-isolation-level-transact-sql?view=sql-server-2017
                -- Turn on Snapshot Isolation
                EXECUTE ('ALTER DATABASE ' + @AppDatabaseName + ' SET ALLOW_SNAPSHOT_ISOLATION ON')
                SET @log = @log + CHAR(9) + 'Snapshot Isolation turned On' + CHAR(10)
            END

            -- Turn on Database Change Tracking
            EXECUTE ('ALTER DATABASE ' + @AppDatabaseName + ' SET CHANGE_TRACKING = ON --(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)')
            SET @log = @log + CHAR(9) + 'Database Change Tracking enabled' + CHAR(10)
        END
    END
    ELSE IF @Action = 'DISABLE' -- Turn off Change Tracking requested
        SET @log = @log + 'Disabling Change Tracking' + CHAR(10)
    ELSE -- Just report the state
	BEGIN
        SET @log = @log + 'Change Tracking State' + CHAR(10)

        SET @log = @log + CHAR(9) + 'Database Change Tracking is '
        IF EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = @AppDatabaseID)
            SET @log = @log + 'enabled' + CHAR(10)
		ELSE
		    SET @log = @log + 'disabled' + CHAR(10)

        SET @log = @log + CHAR(9) + 'Snapshot Isolation is '
        IF EXISTS (SELECT * FROM sys.databases WHERE database_id = @AppDatabaseID AND snapshot_isolation_state IN (1, 3))
            SET @log = @log + 'on' + CHAR(10)
		ELSE
		    SET @log = @log + 'off' + CHAR(10)
	END

    -- If Database Change Tracking is on
    IF EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = @AppDatabaseID)
    BEGIN
        DECLARE Table_Cursor CURSOR FOR SELECT tableName, isColumnTracked FROM @Tables
        DECLARE @tableName AS VARCHAR(30)
        DECLARE @isColumnTracked AS BIT

        -- Loop through the tables
        OPEN Table_Cursor
        FETCH NEXT FROM Table_Cursor INTO @tableName, @isColumnTracked
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @tableName = RTRIM(@tableName)
            SET @isColumnTracked = UPPER(RTRIM(@isColumnTracked))
            -- If the table exists
            IF EXISTS (SELECT * FROM sys.tables WHERE [object_id] = OBJECT_ID(@tableName, N'U'))
            BEGIN
                -- If Table Change Tracking is enabled
                IF EXISTS (SELECT * FROM sys.change_tracking_tables where [object_id] = OBJECT_ID(@tableName, N'U'))
                    -- If turn on Change Tracking requested
                    IF @Action = 'ENABLE'
                        -- If Track Columns Updated on is required
                        IF @isColumnTracked = 'TRUE'
                            -- If Track Columns Updated is off
                            IF EXISTS (SELECT * FROM sys.change_tracking_tables where [object_id] = OBJECT_ID(@tableName, N'U') AND is_track_columns_updated_on = 0)
                            BEGIN
                                -- FYI - In the meeting, we decided that an error should be thrown rather than turning off table change tracking and then turning it on the way we want it
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking already enabled but Track Columns Updated is not on' + CHAR(10)
                                -- Disable Table Change Tracking
                                EXECUTE ('ALTER TABLE ' + @tableName + ' DISABLE CHANGE_TRACKING')
                                SET @log = @log + CHAR(9) + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking Disabled' + CHAR(10)

                                -- Enable Table Change Tracking with
                                EXECUTE ('ALTER TABLE ' + @tableName + ' ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON)')
                                SET @log = @log + CHAR(9) + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking Enabled with Track Columns Updated turned On' + CHAR(10)
                            END
                            ELSE -- Track Columns Updated is on
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking already enabled with Track Columns Updated on' + CHAR(10)
                        ELSE -- Track Columns Updated off is required
                            -- If Track Columns Updated is off
                            IF EXISTS (SELECT * FROM sys.change_tracking_tables where [object_id] = OBJECT_ID(@tableName, N'U') AND is_track_columns_updated_on = 0)
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking already enabled with Track Columns Updated off' + CHAR(10)
                            ELSE -- Track Columns Updated is on
                                -- FYI - In the meeting, we decided that an error should be thrown rather than turning off table change tracking and then turning it on the way we want it
                                -- I don't think it matters for our purposes if we want column tracking off but it's on; we just won't be using it
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking already enabled but Track Columns Updated is not off' + CHAR(10)
                    ELSE IF @Action = 'DISABLE' -- Turn off Change Tracking requested
                    BEGIN
                        -- Disable Table Change Tracking
                        EXECUTE ('ALTER TABLE ' + @tableName + ' DISABLE CHANGE_TRACKING')
                        SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking Disabled' + CHAR(10)
                    END
					ELSE -- Just report the state
					BEGIN
                        -- If Track Columns Updated on is required
                        IF @isColumnTracked = 'TRUE'
                            -- If Track Columns Updated is off
                            IF EXISTS (SELECT * FROM sys.change_tracking_tables where [object_id] = OBJECT_ID(@tableName, N'U') AND is_track_columns_updated_on = 0)
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking is enabled but Track Columns Updated is not on' + CHAR(10)
                            ELSE -- Track Columns Updated is on
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking is enabled with Track Columns Updated on' + CHAR(10)
                        ELSE -- Track Columns Updated off is required
                            -- If Track Columns Updated is off
                            IF EXISTS (SELECT * FROM sys.change_tracking_tables where [object_id] = OBJECT_ID(@tableName, N'U') AND is_track_columns_updated_on = 0)
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking is enabled with Track Columns Updated off' + CHAR(10)
                            ELSE -- Track Columns Updated is on
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking is enabled but Track Columns Updated is not off' + CHAR(10)
					END
                ELSE -- Table Change Tracking is disabled
                    -- If turn on Change Tracking requested
                    IF @Action = 'ENABLE'
                    BEGIN
                        -- If Primary Key exists
                        IF EXISTS (SELECT * FROM sys.indexes WHERE [object_id] = OBJECT_ID(@tableName, N'U') AND is_primary_key = 1)
                            -- Enable Table Change Tracking
                            -- If Track Columns Updated off is required
                            IF @isColumnTracked <> 'TRUE'
                            BEGIN
                                EXECUTE ('ALTER TABLE ' + @tableName + ' ENABLE CHANGE_TRACKING')
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking Enabled with Track Columns Updated turned off' + CHAR(10)
                            END
                            ELSE -- Track Columns Updated on is required
                            BEGIN
                                EXECUTE ('ALTER TABLE ' + @tableName + ' ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON)')
                                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking Enabled with Track Columns Updated turned on' + CHAR(10)
                            END
                        ELSE -- Primary Key does not exist
                            -- If Unique Clustered Index exists
                            IF EXISTS (SELECT * FROM sys.indexes WHERE [object_id] = OBJECT_ID(@tableName, N'U') AND [type] = 1 AND is_unique = 1)
                            BEGIN
							    -- We are going to drop the Unique Clustered Index and then generate and identicle Primary Key
								-- Need a cursor to loop through the index fields and build a string that defines fields of the unique clustered index
                                DECLARE Field_Cursor CURSOR FOR SELECT i.[name], c.[name], ic.is_descending_key
                                                                    FROM sys.indexes i
                                                                    JOIN sys.index_columns ic ON ic.[object_id] = i.[object_id] AND ic.index_id = i.index_id
                                                                    JOIN sys.columns c ON c.[object_id] = ic.[object_id] AND c.column_id = ic.column_id
                                                                    WHERE i.[object_id] = OBJECT_ID(@tableName, N'U') AND i.[type] = 1 AND i.is_unique = 1 and ic.is_included_column = 0
                                                                    ORDER BY ic.key_ordinal
                                DECLARE @indexName AS VARCHAR(128)
                                DECLARE @fieldName AS VARCHAR(128)
                                DECLARE @is_descending_key AS BIT

                                DECLARE @indexFields AS VARCHAR(4000) = SPACE(0) -- Comma separated list of fields
                                DECLARE @primaryKey AS VARCHAR(128)

                                OPEN Field_Cursor
                                FETCH NEXT FROM Field_Cursor INTO @indexName, @fieldName, @is_descending_key
                                WHILE @@FETCH_STATUS = 0
                                BEGIN
                                    --- If there is already one or more fields listed
                                    IF LEN(@indexFields) > 0
                                        SET @indexFields = @indexFields + ', '
                                    ELSE
                                        SET @primaryKey = @indexName

                                    SET @indexFields = @indexFields + RTRIM(@fieldName) + CASE @is_descending_key WHEN 0 THEN ' ASC' ELSE ' DESC' END

                                    FETCH NEXT FROM Field_Cursor INTO @indexName, @fieldName, @is_descending_key
                                END
                                CLOSE Field_Cursor
                                DEALLOCATE Field_Cursor

                                -- Drop the Unique Clustered index
                                EXECUTE ('DROP INDEX ' + @primaryKey + ' ON ' + @tableName)

                                -- Create a Primary Key
                                EXECUTE ('ALTER TABLE ' + @tableName + ' ADD  CONSTRAINT ' + @primaryKey + ' PRIMARY KEY CLUSTERED (' + @indexFields + ')')
                              
				-- Update the entry in the SLIndex table to ensure this modification still applies if the index is updated later.
				Update slindex set IndexID = 1, IsClustered = 1, IsUnique = 1, IsPrimaryKey = 1 where TableName = 
 					 @tableName and IndexName =  @primaryKey 

			        SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Index ' + @primaryKey + ' dropped; Primary Key ' + @primaryKey + ' created' + CHAR(10)
                                SET @log = @log + CHAR(9) + CHAR(9) + SPACE(42) + 'Keys Fields (' + @indexFields + ')' + CHAR(10)

                                -- If Track Columns Updated off is required
                                IF @isColumnTracked <> 'TRUE'
                                BEGIN
                                    EXECUTE ('ALTER TABLE ' + @tableName + ' ENABLE CHANGE_TRACKING')
                                    SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking Enabled with Track Columns Updated turned Off' + CHAR(10)
                                END
                                ELSE -- Track Columns Updated on is required
                                BEGIN
                                    EXECUTE ('ALTER TABLE ' + @tableName + ' ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON)')
                                    SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking Enabled with Track Columns Updated turned On' + CHAR(10)
                                END
                            END
                            ELSE -- Unique Clustered Index does not exists
							BEGIN
                                SET @log = @log + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'FAILURE: Primary Key not found; Unique Clustered index not found' + CHAR(10)
								PRINT @log
								SET @msgText = 'Database ' + @AppDatabaseName + '; Table ' + @tableName + '; Primary Key not found; Unique Clustered index not found';
								THROW 550033, @msgText, 1
								RETURN
							END
                    END
                    ELSE IF @Action = 'DISABLE' -- Turn off Change Tracking requested
                        SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking already disabled' + CHAR(10)
					ELSE -- Just report the state
                        SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'Table Change Tracking is disabled' + CHAR(10)

                -- If Table Change Tracking is enabled
                IF EXISTS (SELECT * FROM sys.change_tracking_tables where [object_id] = OBJECT_ID(@tableName, N'U'))
                BEGIN
                    -- If turn off Change Tracking requested
                    IF @Action = 'DISABLE'
					BEGIN
                        SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'FAILURE: Disable Table Change Tracking' + CHAR(10)
						PRINT @log
						SET @msgText = 'Database ' + @AppDatabaseName + '; Table ' + @tableName + '; Failed to disable Table Change Tracking';
						THROW 55034, @msgText, 1
						RETURN
					END
                END
                ELSE -- Table Change Tracking is disabled
                    -- If turn on Change Tracking requested
                    IF @Action = 'ENABLE'
					BEGIN
                        SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'FAILURE: Enable Table Change Tracking' + CHAR(10)
						PRINT @log
						SET @msgText = 'Database ' + @AppDatabaseName + '; Table ' + @tableName + '; Failed to enable Table Change Tracking';
						THROW 55035, @msgText, 1
						RETURN
					END
            END
            ELSE -- The table does not exist
			BEGIN
                SET @log = @log + CHAR(9) + CHAR(9) + 'Table ' + @tableName + REPLICATE('.', 33 - LEN(@tableName)) + 'FAILURE: Table not found' + CHAR(10)
				PRINT @log
				SET @msgText = 'Database ' + @AppDatabaseName + '; Table ' + @tableName + '; Table not found';
				THROW 55036, @msgText, 1
				RETURN
			END

            FETCH NEXT FROM Table_Cursor INTO @tableName, @isColumnTracked
        END
        CLOSE Table_Cursor
        DEALLOCATE Table_Cursor

        -- If turn off Change Tracking requested
        IF @Action = 'DISABLE'
        BEGIN
            SET @log = @log + CHAR(10)
            -- If there are one or more tables with Table Change Tracking enabled
            IF EXISTS (SELECT * FROM sys.change_tracking_tables)
			BEGIN
                -- Executing the ALTER DATABASE sql command to turn off Database Change Tracking would result ing SQL Error    Msg 22115
                    --Change tracking is enabled for one or more tables in database 'SLDemoApp60'. Disable change tracking on each table before disabling it for the database. Use the sys.change_tracking_tables catalog view to obtain a list of tables for which change tracking is enabled.
                SET @log = @log + CHAR(9) + 'Database Change Tracking will remain enabled because one or more tables have Change Tracking enabled' + CHAR(10)
				PRINT @log
				SET @msgText = 'Database ' + @AppDatabaseName + '; Failed to disable Database Change Tracking - one or more tables have Change Tracking enabled';
				THROW 55037, @msgText, 1
				RETURN
			END
            ELSE -- There are no tables with Table Change Tracking enabled
            BEGIN
                -- Turn off Database Change Tracking
                EXECUTE ('ALTER DATABASE ' + @AppDatabaseName + ' SET CHANGE_TRACKING = OFF')
                SET @log = @log + CHAR(9) + 'Database Change Tracking disabled' + CHAR(10)
            END

            -- If Database Change Tracking is on
            -- This is just a safe guard to make sure what we want to happen did happen
            IF EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = @AppDatabaseID)
			BEGIN
                SET @log = @log + CHAR(9) + 'FAILURE: Disabling Database Change Tracking' + CHAR(10)
				PRINT @log
				SET @msgText = 'Database ' + @AppDatabaseName + '; Failed to disable Database Change Tracking';
				THROW 55038, @msgText, 1
				RETURN
			END
            ELSE -- Database Change Tracking is off
                -- If Snapshot Isolation is off
                IF EXISTS (SELECT * FROM sys.databases WHERE database_id = @AppDatabaseID AND snapshot_isolation_state IN (0, 2))
                    SET @log = @log + CHAR(9) + 'Snapshot Isolation already off' + CHAR(10)
                ELSE
                BEGIN
                    -- Turn off Snapshot Isolation
                    -- Careful with this because if any one is using Snapshot Isolation, this could break their SQL code
                    EXECUTE ('ALTER DATABASE ' + @AppDatabaseName + ' SET ALLOW_SNAPSHOT_ISOLATION OFF')
                    SET @log = @log + CHAR(9) + 'Snapshot Isolation turned Off' + CHAR(10)
                END
        END
    END
    ELSE -- Database Change Tracking is not on
        -- If turn on Change Tracking requested
        IF @Action = 'ENABLE'
		BEGIN
            -- This is just a safe guard to make sure what should have happened did happen
            SET @log = @log + CHAR(9) + 'FAILURE: Enabling Database Change Tracking' + CHAR(10)
			PRINT @log
			SET @msgText = 'Database ' + @AppDatabaseName + '; Failed to enable Database Change Tracking';
			THROW 55039, @msgText, 1
			RETURN
		END
        ELSE IF @Action = 'DISABLE' -- Turn off Change Tracking requested
            SET @log = @log + CHAR(9) + 'Database Change Tracking is already disabled'
END
ELSE -- There are no tables to process
BEGIN
    IF @Action = 'ENABLE' OR @Action = 'DISABLE'
	BEGIN
        SET @log = @log + 'FAILURE: No Change Tracking Table(s) defined' + CHAR(10)
		PRINT @log
		SET @msgText = 'Database ' + @AppDatabaseName + '; No Change Tracking Table(s) defined';
		THROW 55040, @msgText, 1
		RETURN
	END
	ELSE -- Just report the state
	BEGIN
	    SET @log = @log + 'Change Tracking State' + CHAR(10)

        SET @log = @log + CHAR(9) + 'Database Change Tracking is '
        IF EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = @AppDatabaseID)
            SET @log = @log + 'enabled' + CHAR(10)
		ELSE
		    SET @log = @log + 'disabled' + CHAR(10)

        SET @log = @log + CHAR(9) + 'Snapshot Isolation is '
        IF EXISTS (SELECT * FROM sys.databases WHERE database_id = @AppDatabaseID AND snapshot_isolation_state IN (1, 3))
            SET @log = @log + 'on' + CHAR(10)
		ELSE
		    SET @log = @log + 'off' + CHAR(10)

        IF EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = @AppDatabaseID)
            SET @log = @log + CHAR(9) + 'No Change Tracking Table(s) defined.' + CHAR(10)
	END
END

IF @Action <> 'ENABLE' AND @Action <> 'DISABLE' AND @Action <> 'STATE'
BEGIN
    SET @log = @log + CHAR(10) + 'INFORMATION: Action parameter ''' + @Action + ''' is invalid' + CHAR(10)
    SET @log = @log + CHAR(9) + 'Valid Action parameters are:' + CHAR(10)
    SET @log = @log + CHAR(9) + CHAR(9) + '''ENABLE'', ''ON'', or ''TRUE'' = Enable BC Migration Change Tracking' + CHAR(10)
    SET @log = @log + CHAR(9) + CHAR(9) + '''DISABLE'', ''OFF'', or ''FALSE'' = Disable BC Migration Change Tracking' + CHAR(10)
    SET @log = @log + CHAR(9) + CHAR(9) + '''STATE'' = Show the current state of BC Migration Change Tracking' + CHAR(10)
    SET @log = @log + CHAR(9) + '''STATE'' is the default when no parameter is passed' + CHAR(10)
END

PRINT @log
GO
----------------------------------FINISH STORED PROC BCMChangeTracking--------------------------------------------------------------
