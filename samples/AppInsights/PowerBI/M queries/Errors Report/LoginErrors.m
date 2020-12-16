let AnalyticsQuery =
let 
NullToBlank = (input) => if (input = null) then "" else input,
// Kustos result set limit is 500000 rows
Limit = (input) => if (input = null) then "500000" else input,
Source = Json.Document(Web.Contents("https://api.applicationinsights.io/v1/apps/" & #"App id" & "/query", 
// here you can see how PowerBI parameters can be passed on to the KQL query
[Query=[#"query"="let failed_in_first_stage = 
traces
| where 1==1 
    and ('" & NullToBlank( #"AAD Tenant Id" ) & "'=='' or customDimensions.aadTenantId == '" & NullToBlank( #"AAD Tenant Id" ) & "')
    and ('" & NullToBlank( #"Environment Name" ) & "'=='' or customDimensions.environmentName == '" & NullToBlank( #"Environment Name" ) & "')
    and timestamp >= todatetime('" & Date.ToText( #"Start Date", "yyyy-MM-dd" ) & "')
    and timestamp <= todatetime('"& Date.ToText( #"End Date", "yyyy-MM-dd" ) &"') + totimespan(24h) - totimespan(1ms)
| where customDimensions.eventId == ('RT0001') // Authorization Failed (Pre Open Company)
| limit " & Limit( #"Top" ) & "
| extend aadTenantId = tostring( customDimensions.aadTenantId)
, environmentName = tostring( customDimensions.environmentName)
, environmentType = tostring( customDimensions.environmentType)
, platformVersion = tostring(customDimensions.componentVersion)
, clientType = 'Unknown at this stage'
, failureReason = customDimensions.failureReason
, guestUser = tostring( customDimensions.guestUser )
, userType = tostring( customDimensions.userType )
| project timestamp
, AadTenantId=aadTenantId, EnvironmentName=environmentName, EnvironmentType=environmentType
, PlatformVersion=platformVersion, ClientType=clientType
, GuestUser = guestUser
, UserType = userType
, FailureReason = failureReason
, AuthorizationStep = 'Pre OpenCompany trigger'
;
let failed_in_second_stage = 
traces
| where 1==1 
    and ('" & NullToBlank( #"AAD Tenant Id" ) & "'=='' or customDimensions.aadTenantId == '" & NullToBlank( #"AAD Tenant Id" ) & "')
    and ('" & NullToBlank( #"Environment Name" ) & "'=='' or customDimensions.environmentName == '" & NullToBlank( #"Environment Name" ) & "')
    and timestamp >= todatetime('" & Date.ToText( #"Start Date", "yyyy-MM-dd" ) & "')
    and timestamp <= todatetime('"& Date.ToText( #"End Date", "yyyy-MM-dd" ) &"') + totimespan(24h) - totimespan(1ms)
| where customDimensions.eventId == ('RT0003') // Authorization Succeeded (Pre Open Company)
| limit " & Limit( #"Top" ) & "
| project aadTenantId = tostring( customDimensions.aadTenantId)
, environmentName = tostring( customDimensions.environmentName)
, environmentType = tostring( customDimensions.environmentType)
, platformVersion = tostring(customDimensions.componentVersion)
, guestUser = tostring( customDimensions.guestUser )
, userType = tostring( customDimensions.userType )
, operation_Id
| join kind=inner (
traces
| where 1==1 
    and ('" & NullToBlank( #"AAD Tenant Id" ) & "'=='' or customDimensions.aadTenantId == '" & NullToBlank( #"AAD Tenant Id" ) & "')
    and ('" & NullToBlank( #"Environment Name" ) & "'=='' or customDimensions.environmentName == '" & NullToBlank( #"Environment Name" ) & "')
    and timestamp >= todatetime('" & Date.ToText( #"Start Date", "yyyy-MM-dd" ) & "')
    and timestamp <= todatetime('"& Date.ToText( #"End Date", "yyyy-MM-dd" ) &"') + totimespan(24h) - totimespan(1ms)
| where customDimensions.eventId == ('RT0002') // Authorization Failed (Open Company)
| project timestamp
, clientType = tostring( customDimensions.clientType )
, operation_Id
, failureReason=customDimensions.failureReason
) on $left.operation_Id == $right.operation_Id
| project timestamp
, AadTenantId=aadTenantId
, EnvironmentName=environmentName
, EnvironmentType=environmentType
, PlatformVersion=platformVersion
, ClientType=clientType
, GuestUser = guestUser
, UserType = userType
, FailureReason=failureReason
, AuthorizationStep = 'OpenCompany trigger'
;
failed_in_first_stage 
| union failed_in_second_stage
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