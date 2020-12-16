let AnalyticsQuery =
let 
NullToBlank = (input) => if (input = null) then "" else input,
// Kustos result set limit is 500000 rows
Limit = (input) => if (input = null) then "500000" else input,
Source = Json.Document(Web.Contents("https://api.applicationinsights.io/v1/apps/" & #"App id" & "/query", 
// here you can see how PowerBI parameters can be passed on to the KQL query
[Query=[#"query"="traces
| where 1==1 
    and ('" & NullToBlank( #"AAD Tenant Id" ) & "'=='' or customDimensions.aadTenantId == '" & NullToBlank( #"AAD Tenant Id" ) & "')
    and ('" & NullToBlank( #"Environment Name" ) & "'=='' or customDimensions.environmentName == '" & NullToBlank( #"Environment Name" ) & "')
    and timestamp >= todatetime('" & Date.ToText( #"Start Date", "yyyy-MM-dd" ) & "')
    and timestamp <= todatetime('"& Date.ToText( #"End Date", "yyyy-MM-dd" ) &"') + totimespan(24h) - totimespan(1ms)
| where customDimensions.eventId == 'RT0005'
    and customDimensions.alObjectId != '0'
| limit " & Limit(#"Top") & "
| extend AadTenantId = customDimensions.aadTenantId
, EnvironmentName = customDimensions.environmentName
, EnvironmentType = customDimensions.environmentType
, sqlStatement = tostring(customDimensions.sqlStatement)
, PlatformVersion = tostring(customDimensions.componentVersion)
, ClientType = tostring(customDimensions.clientType)
, ObjectId = tostring(customDimensions.alObjectId)
, ObjectName = tostring( customDimensions.alObjectName )
, ObjectType = customDimensions.alObjectType
, StackTrace = customDimensions.alStackTrace
, ExtensionName = tostring( customDimensions.extensionName )
, ExecutionTimeInMS = toreal(totimespan(customDimensions.executionTime))/10000
| extend NumberOfJoins = countof(sqlStatement, 'JOIN') 
, NumberOfOuterApplys = countof(sqlStatement, 'OUTER APPLY')
| project timestamp
, AadTenantId, EnvironmentName, EnvironmentType
, PlatformVersion, ClientType
, ExtensionName
, ObjectId, ObjectName, ObjectType
, StackTrace
, SqlStatement=sqlStatement, NumberOfJoins, NumberOfOuterApplys
, ExecutionTimeInMS 
",#"x-ms-app"="AAPBI",#"prefer"="ai.response-thinning=true"],Timeout=#duration(0,0,4,0)])),
TypeMap = #table(
{ "AnalyticsTypes", "Type" }, 
{ 
{ "string",   Text.Type },
{ "int",      Int32.Type },
{ "long",     Int64.Type },
{ "real",     Double.Type },
{ "timespan", Duration.Type },
{ "datetime", DateTimeZone.Type },
{ "bool",     Logical.Type },
{ "guid",     Text.Type },
{ "dynamic",  Text.Type }
}),
DataTable = Source[tables]{0},
Columns = Table.FromRecords(DataTable[columns]),
ColumnsWithType = Table.Join(Columns, {"type"}, TypeMap , {"AnalyticsTypes"}),
Rows = Table.FromRows(DataTable[rows], Columns[name]), 
Table = Table.TransformColumnTypes(Rows, Table.ToList(ColumnsWithType, (c) => { c{0}, c{3}}))
in
Table
in 
AnalyticsQuery