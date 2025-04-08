namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

/// <summary>
/// Builds SQL queries.
/// </summary>
public static class SqlQueryBuilder
{
    /// <summary>
    /// Check if a table exists in a database
    /// </summary>
    /// <param name="database">Database name.</param>
    /// <param name="schema">Schema name.</param>
    /// <param name="table">Table name.</param>
    /// <param name="innerSql">Query to run if table exists.</param>
    /// <returns>SQL query.</returns>
    public static DbCommandBuilder CheckIfTableExists(string database, string schema, string table, string innerSql) => CheckIfTableExists(database, schema, table, "check", command => innerSql);

    /// <summary>
    /// Check if a table exists in a database
    /// </summary>
    /// <param name="database">Database name.</param>
    /// <param name="schema">Schema name.</param>
    /// <param name="table">Table name.</param>
    /// <param name="innerCommandBuilder">Query to run if table exists.</param>
    /// <returns>SQL query.</returns>
    public static DbCommandBuilder CheckIfTableExists(string database, string schema, string table, string prefix, DbCommandBuilder innerCommandBuilder) => command =>
    {
        var tableSchemaParameter = command.CreateParameter();
        tableSchemaParameter.ParameterName = $"{prefix}_tableSchema";
        tableSchemaParameter.Value = schema;
        command.Parameters.Add(tableSchemaParameter);
        var tableNameParameter = command.CreateParameter();
        tableNameParameter.ParameterName = $"{prefix}_tableName";
        tableNameParameter.Value = table;
        command.Parameters.Add(tableNameParameter);

        var innerSql = innerCommandBuilder(command);

        return $@"
                IF EXISTS (SELECT * FROM {database.AsSqlBracketedValue()}.INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @{tableSchemaParameter.ParameterName} AND TABLE_NAME = @{tableNameParameter.ParameterName})
                BEGIN
                    {innerSql}
                END ELSE BEGIN
                    SELECT 0
                END
            ";
    };

    public static DbCommandBuilder EmailFormatValidation(string database, string schema, string masterType, string prefix) => command =>
    {
        string activeQuery = string.Empty;

        switch (masterType)
        {
            case "CUS":
                activeQuery = $"SELECT [CUSTNMBR] AS [Master_ID] FROM {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.[RM00101] WHERE [INACTIVE] = 0";
                break;
            case "VEN":
                activeQuery = $"SELECT [VENDORID] AS [Master_ID] FROM {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.[PM00200] WHERE [VENDSTTS] = 1";
                break;
        }

        var masterTypeParameter = command.CreateParameter();
        masterTypeParameter.ParameterName = $"{prefix}_masterType";
        masterTypeParameter.Value = masterType;
        command.Parameters.Add(masterTypeParameter);

        return $@"WITH ACTIVE AS ( 
    {activeQuery}
),INET1QUERY AS (
    SELECT
        Master_ID,
        ADRSCODE,
        INET1.value AS INET1,
        '' AS EmailToAddress,
        '' AS EmailCcAddress,
        '' AS EmailBccAddress
    FROM
        {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.[SY01200] CROSS APPLY string_split(INET1, ';') INET1
    WHERE
        Master_Type = @{masterTypeParameter.ParameterName}
        AND Master_ID IN (SELECT Master_ID FROM ACTIVE) --Remove to include inactive records
        AND INET1.value != ''
        AND (
            INET1.value NOT LIKE '%_@_%_.__%'
            OR INET1.value LIKE '%:%'
            OR LEN(INET1.value) - LEN(REPLACE(INET1.value, '@', '')) > 1
        )
),
EMAILTOQUERY AS (
    SELECT
        Master_ID,
        ADRSCODE,
        '' AS INET1,
        EmailToAddress.value AS EmailToAddress,
        '' AS EmailCcAddress,
        '' AS EmailBccAddress
    FROM
        {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.[SY01200] CROSS APPLY string_split(
            CONVERT(
            VARCHAR(MAX),
            EmailToAddress
            ),
            ';'
        ) EmailToAddress
    WHERE
        Master_Type = @{masterTypeParameter.ParameterName}
        AND Master_ID IN (SELECT Master_ID FROM ACTIVE) --Remove to include inactive records
        AND EmailToAddress.value != ''
        AND (
            EmailToAddress.value NOT LIKE '%_@_%_.__%'
            OR EmailToAddress.value LIKE '%:%'
            OR LEN(EmailToAddress.value) - LEN(REPLACE(EmailToAddress.value, '@', '')) > 1
        )
),
EMAILCCQUERY AS (
    SELECT
        Master_ID,
        ADRSCODE,
        '' AS INET1,
        '' AS EmailToAddress,
        EmailCcAddress.value AS EmailCcAddress,
        '' AS EmailBccAddress
    FROM
        {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.[SY01200] CROSS APPLY string_split(
            CONVERT(
            VARCHAR(MAX),
            EmailCcAddress
            ),
            ';'
        ) EmailCcAddress
    WHERE
        Master_Type = @{masterTypeParameter.ParameterName}
        AND Master_ID IN (SELECT Master_ID FROM ACTIVE) --Remove to include inactive records
        AND EmailCcAddress.value != ''
        AND (
            EmailCcAddress.value NOT LIKE '%_@_%_.__%'
            OR EmailCcAddress.value LIKE '%:%'
            OR LEN(EmailCcAddress.value) - LEN(REPLACE(EmailCcAddress.value, '@', '')) > 1
        )
),
EMAILBCCQUERY AS (
    SELECT
        Master_ID,
        ADRSCODE,
        '' AS INET1,
        '' AS EmailToAddress,
        '' AS EmailCcAddress,
        EmailBccAddress.value AS EmailBccAddress
    FROM
        {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.[SY01200] CROSS APPLY string_split(
            CONVERT(
            VARCHAR(MAX),
            EmailBccAddress
            ),
            ';'
        ) EmailBccAddress
    WHERE
    Master_Type = @{masterTypeParameter.ParameterName}
        AND Master_ID IN (SELECT Master_ID FROM ACTIVE) --Remove to include inactive records
        AND EmailBccAddress.value != ''
        AND (
            EmailBccAddress.value NOT LIKE '%_@_%_.__%'
            OR EmailBccAddress.value LIKE '%:%'
            OR LEN(EmailBccAddress.value) - LEN(REPLACE(EmailBccAddress.value, '@', '')) > 1
        )
)
SELECT
    Master_ID,
    ADRSCODE,
    INET1,
    EmailToAddress,
    EmailCcAddress,
    EmailBccAddress
FROM
    INET1QUERY
UNION
SELECT
    Master_ID,
    ADRSCODE,
    INET1,
    EmailToAddress,
    EmailCcAddress,
    EmailBccAddress
FROM
    EMAILTOQUERY
UNION
SELECT
    Master_ID,
    ADRSCODE,
    INET1,
    EmailToAddress,
    EmailCcAddress,
    EmailBccAddress
FROM
    EMAILCCQUERY
UNION
SELECT
    Master_ID,
    ADRSCODE,
    INET1,
    EmailToAddress,
    EmailCcAddress,
    EmailBccAddress
FROM
    EMAILBCCQUERY";
    };

    public static DbCommandBuilder GetGpDatabaseVersion(string systemDatabase, string database, string prefix) => command =>
    {
        var dbNameParam = command.CreateParameter();
        dbNameParam.ParameterName = $"{prefix}_dbName";
        dbNameParam.Value = database;
        command.Parameters.Add(dbNameParam);
        return $@"SELECT
    CAST(db_verMajor AS varchar(2)) + '.' + CAST(db_verMinor AS varchar(2)) + '.' + CAST(db_verBuild AS varchar(4)) AS [build]
FROM
    {systemDatabase.AsSqlBracketedValue()}.[dbo].[DB_Upgrade]
WHERE
    db_name = @{dbNameParam.ParameterName}
    AND PRODID = 0
";
    };

    /// <summary>
    /// Get record count of table.
    /// </summary>
    /// <param name="database">Database name.</param>
    /// <param name="schema">Schema name.</param>
    /// <param name="table">Table name.</param>
    /// <returns>Number of records in table, returns zero if table not found.</returns>
    public static DbCommandBuilder RecordCount(string database, string schema, string table) => CheckIfTableExists(database, schema, table, $"SELECT COUNT(1) FROM {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.{table.AsSqlBracketedValue()}");

    /// <summary>
    /// Get record count of table where records match a condition.
    /// </summary>
    /// <param name="database">Database name.</param>
    /// <param name="schema">Schema name.</param>
    /// <param name="table">Table name.</param>
    /// <param name="whereClause">Body of where clause.</param>
    /// <returns></returns>
    public static DbCommandBuilder RecordCountWhere(string database, string schema, string table, string whereClause) => RecordCountWhere(database, schema, table, command => whereClause);

    /// <summary>
    /// Get record count of table where records match a condition.
    /// </summary>
    /// <param name="database">Database name.</param>
    /// <param name="schema">Schema name.</param>
    /// <param name="table">Table name.</param>
    /// <param name="whereClause">Body of where clause.</param>
    /// <returns></returns>
    public static DbCommandBuilder RecordCountWhere(string database, string schema, string table, DbCommandBuilder whereClauseBuilder) => command =>
    {
        string whereClause = whereClauseBuilder(command);
        return CheckIfTableExists(database, schema, table, $"SELECT COUNT(1) FROM {database.AsSqlBracketedValue()}.{schema.AsSqlBracketedValue()}.{table.AsSqlBracketedValue()} WHERE {whereClause}")(command);
    };
}
