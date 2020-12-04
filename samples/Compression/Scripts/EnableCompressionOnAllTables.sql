USE <enter the name of your db>

/* Enable compression on all tables */
EXEC sp_MSforeachtable 'ALTER TABLE ? REBUILD PARTITION = ALL WITH (DATA_COMPRESSION = PAGE)' 

/* Shrink database file */

DECLARE @dbname sysname
SELECT @dbname = name from sys.database_files where type=0
DBCC SHRINKFILE (@dbname)
