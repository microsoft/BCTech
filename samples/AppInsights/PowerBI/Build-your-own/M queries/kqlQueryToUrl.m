let
    aad_tenant_id = #"Application Insights AAD tenant id",
    subscription_id = #"Application Insights subscription id",
    resource_group_name = #"Application Insights resource group name",
    resource_name = #"Application Insights resource name",
    baseurl = "https://ms.portal.azure.com" &
        "#@" & aad_tenant_id & 
        "/blade/Microsoft_OperationsManagementSuite_Workspace/Logs.ReactView/resourceId/" &
        "%2Fsubscriptions%2F" & subscription_id &
        "%2FresourceGroups%2F" & resource_group_name &
        "%2Fproviders%2Fmicrosoft.insights" & 
        "%2Fcomponents%2F" & resource_name & 
        "/source/LogsBlade.AnalyticsShareLinkToQuery/q/",
    queries = Table.FromRecords(
    {
        [Source = "Test", Query = "traces | take 5"]
    }
    ),
    #"Added NewQuery" = Table.AddColumn(queries, "NewQuery", each 
"// change lookback, limit, ... as needed 
" & [Query] & "
// change/remove this line
| take 10
"   
    ),
    #"QueryZippedAndBase64Encoded" = Table.AddColumn(#"Added NewQuery", "QueryZippedAndBase64Encoded", 
    each 
        Uri.EscapeDataString(
            Binary.ToText(
                Binary.Compress(Text.ToBinary([NewQuery]), Compression.GZip),
                BinaryEncoding.Base64
            )
        )
    ),
    #"Added Url" = Table.AddColumn(#"QueryZippedAndBase64Encoded", "Url", each baseurl & [QueryZippedAndBase64Encoded]),
    #"Removed Columns" = Table.RemoveColumns(#"Added Url",{"QueryZippedAndBase64Encoded"})
in
    #"Removed Columns"