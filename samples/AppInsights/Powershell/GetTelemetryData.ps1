param(
    [Parameter(Mandatory=$true)]
    [string]$appid,

    [Parameter(Mandatory=$true)]
    [string]$apikey,

    [Parameter(
        Mandatory=$true,
        ValueFromPipeline = $true)
    ]
    [string]$kqlquery,

    [switch]$help = $false,
    [switch]$debugmode = $false
)

if($help){
    Write-Host "Based on input parameters, will query Azure Application Insights and output to stdout"
    Write-Host ""
    Write-Host "Mandatory parameters: appid, apikey, kqlquery"
    exit
}

function format_uri ([string] $kql_query)
    {
        $query=[uri]::EscapeUriString("?query=" + $kql_query)
        $uri = "https://api.applicationinsights.io/v1/apps/$appid/query$query"
        if($debugmode){Write-Host "get_uri: $uri"  }        
        $uri
    }

function invoke_api ([string] $uri) {
    try{
        $http_headers = @{ "X-Api-Key" = $apikey; "Content-Type" = "application/json" }
        $response = Invoke-WebRequest -uri $uri -Headers $http_headers -Method Get
        if($debugmode){ Write-Host "invoke_api: Get data: OK"}
    }
    catch{
        Write-Error "Could not retrieve data: $_.Exception.Response.StatusCode.value__"
        exit
    }
    $response   
}

function get_data([string] $kql_query){
    $uri = format_uri($kql_query)
    invoke_api( $uri )
}

function pp_data($response){
    $json = ConvertFrom-Json $response.Content
    $header_row = $null

    # Application insights return different data structure when result is only one column
    # so this will (silently) fail for those queries
    $header_row = $json.tables.columns | Select-Object name 

    $column_count = $header_row.Count

    $result = @()

    # Application insights return different data structure when result is only one row
    # so this will fail for those queries
    foreach ($row in $json.tables.rows) {
        $rowdata = new-object PSObject
        for ($i = 0; $i -lt $column_count; $i++) {
            $rowdata | add-member -membertype NoteProperty -name $header_row[$i].name -value $row[$i]    
        }
        $result += $rowdata
        $rowdata = $null
    }

   $result
}



pp_data( get_data($kqlquery) )
