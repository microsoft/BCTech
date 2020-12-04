# Compression utilities

In this folder, you will find a few utitilies related to  compression.

## Scripts
----------

The script folder contains some SQL scripts, which can be loaded in Microsoft SQL Management Studion and executed against a mounted database.

### To10BiggestTable.sql
This script displays the name of the 10 largest tables.
On the first line with the **USE** statement, insert the name of the database, for which you want to get the name of the 10 largest tables.
You can easily tweak this script to get the results for more or less tables.
Running this script does not alter the database.

### EnableCompressionOnAllTables.sql
This script enables compression on all tables and shrinks the database to reclaim the storage space freed up by the compression.  
On the first line with the **USE** statement, insert the name of the database, for which you want to enable compression.  
Beware that this script does alter the database.

### Create_sp_MSforeachtable.sql
The scripts above call the **sp_MSforeachtable** stored procedure. Although you can use these scripts on a SQL Azure database, this particular stored procedure does not exist by default on Azure. You can easily create it by running this script.

## BCCompressionCli
-------------------
This utility is a console application written in C#, which displays the size of the biggest tables and shows estimates of the size reduction when compression is enabled for those tables.

Usage:
BCCompressionCli \<databasename\> [\<nbOfTables\>] [\<serverName\>]  
\<databaseName\>: The name of the database to perform compression estimation on.  
\<nbOfTables\>: Optional parameter. By default information for the top 10 tables are shown.  
\<serverName\>: Optional parameter. The name of the SQL server on which the database is mounted.   
If not provided, the server is assumed to be running on the local machine.  

Below is an example of the output

![](media/BCCompressionCliExampleOutput.PNG "BCCompressionCli example outup")
