# This sample iterates all customers in Partner Center, which have Business Central, and check which apps they have
# installed in which versions, and if there are available updates. In the end, it creates a report on this.

# APIs used
#  - Partner Center API:  get all customers
#  - Partner Center API:  get a customer's subscriptions
#  - BC Admin Center API: get a customer's environments
#  - BC Admin Center API; get an environment's installed apps
#  - BC Admin Center API: get an environment's available app updates

Install-Module PartnerCenter  # only needs to be done once, see documentation at https://learn.microsoft.com/powershell/module/partnercenter

# PREPARATION
Connect-PartnerCenter

Add-Type -Path "C:\Program Files\WindowsPowerShell\Modules\AzureAD\2.0.2.76\Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
$aadAppId = "a19cb26a-2e4c-408b-82e1-6311742ecc50"
$aadAppRedirectUri = "http://localhost"

$outputFilePath = "c:\temp\AppReport.csv"



# HELPER FUNCTIONS

function GetCustomersWithBusinessCentralSubscriptions()
{
    Write-Host -ForegroundColor Cyan "Getting all customers with Business Central subscriptions..."
    $customers = Get-PartnerCustomer
    $i = $customers.Length
    foreach ($customer in $customers)
    {
        Write-Host -NoNewline (" {0,3} {1} {2}..." -f $i,$customer.Domain,$customer.Name)
        $subscriptions = Get-PartnerCustomerSubscription -InputObject $customer -ErrorAction Ignore
        $bcSubscriptions = $subscriptions | ? -Property FriendlyName -Like "*business central*" | ? -Property Status -eq "active"
        if ($bcSubscriptions)
        {
            Write-Output @{
                aadTenantId = $customer.CustomerId
                domain = $customer.Domain
                name = $customer.Name
            }
            Write-Host " has Business Central license"
        }
        else
        {
            Write-Host
        }

        $i--
    }
}

function GetBusinessCentralAccessTokenForCustomer($CustomerAadTenantId)
{
    $ctx = [Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext]::new("https://login.microsoftonline.com/$CustomerAadTenantId")
    $redirectUri = New-Object -TypeName System.Uri -ArgumentList $aadAppRedirectUri
    $platformParameters = New-Object -TypeName Microsoft.IdentityModel.Clients.ActiveDirectory.PlatformParameters -ArgumentList ([Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior]::Auto)
    $accessToken = $ctx.AcquireTokenAsync("https://api.businesscentral.dynamics.com", $aadAppId, $redirectUri, $platformParameters).GetAwaiter().GetResult().AccessToken
    return $accessToken
}

function GetCustomerEnvironments($CustomerAadTenantId)
{
    $accessToken = GetBusinessCentralAccessTokenForCustomer -CustomerAadTenantId $CustomerAadTenantId

    $response = Invoke-WebRequest `
        -Method Get `
        -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments" `
        -Headers @{Authorization=("Bearer $accessToken")}

    $environments = (ConvertFrom-Json $response.Content).Value
    $environments | Select-Object -Property aadTenantId,name,type,applicationVersion,countryCode | Write-Output
}

function GetEnvironmentInstalledApps($CustomerAadTenantId, $EnvironmentName)
{
    $accessToken = GetBusinessCentralAccessTokenForCustomer -CustomerAadTenantId $CustomerAadTenantId

    $response = Invoke-WebRequest `
        -Method Get `
        -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$EnvironmentName/apps" `
        -Headers @{Authorization=("Bearer $accessToken")}

    $installedApps = (ConvertFrom-Json $response.Content).Value
    $installedApps | ? -Property state -eq installed | Select-Object -Property id,name,publisher,version | Write-Output
}

function GetEnvironmentAppAvailableUpdates($CustomerAadTenantId, $EnvironmentName)
{
    $accessToken = GetBusinessCentralAccessTokenForCustomer -CustomerAadTenantId $CustomerAadTenantId

    $response= Invoke-WebRequest `
        -Method Get `
        -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$EnvironmentName/apps/availableUpdates" `
        -Headers @{Authorization=("Bearer $accessToken")}

    $availableUpdates = (ConvertFrom-Json $response.Content).Value
    $availableUpdates | Select-Object -Property appId,version | Write-Output
}



# MAIN SECTION

# Get all customers from Partner Center
$businessCentralCustomers = GetCustomersWithBusinessCentralSubscriptions

# Query the Admin Center for these customers and their environments
$result = @()
foreach ($businessCentralCustomer in $businessCentralCustomers)
{
    Write-Host -ForegroundColor Cyan "Processing customer $($businessCentralCustomer.aadTenantId)..."
    $customerEnvironments = GetCustomerEnvironments -CustomerAadTenantId $($businessCentralCustomer.aadTenantId)

    foreach ($customerEnvironment in $customerEnvironments)
    {
        Write-Host -NoNewline "  Processing environment $($customerEnvironment.name)..."
        $environmentInstalledApps = GetEnvironmentInstalledApps -CustomerAadTenantId $($businessCentralCustomer.aadTenantId) -EnvironmentName $customerEnvironment.name
        $environmentAvailableAppUpdates = GetEnvironmentAppAvailableUpdates -CustomerAadTenantId $($businessCentralCustomer.aadTenantId) -EnvironmentName $customerEnvironment.name

        foreach ($environmentInstalledApp in $environmentInstalledApps)
        {
            $result += @{
                CustomerAadTenantId = $($businessCentralCustomer.aadTenantId)
                CustomerName = $($businessCentralCustomer.name)
                CustomerDomain = $($businessCentralCustomer.domain)
                EnvironmentName = $customerEnvironment.name
                EnvironmentType = $customerEnvironment.type
                Version = $customerEnvironment.applicationVersion
                Country = $customerEnvironment.countryCode
                AppId = $environmentInstalledApp.id
                AppName = $environmentInstalledApp.name
                AppPublisher = $environmentInstalledApp.publisher
                AppInstalledVersion = $environmentInstalledApp.version
                AppAvailableUpdateVersion = ($environmentAvailableAppUpdates | ? -Property appId -EQ $environmentInstalledApp.id | Select-Object -First 1).version
                QualifiedEnvironmentName = "$($businessCentralCustomer.aadTenantId) $($customerEnvironment.name)"
                QualifiedAppName = "$($environmentInstalledApp.id) $($environmentInstalledApp.name)"
            }
        }
        Write-Host " done"
    }
}

# Save to file
$result | 
   % { New-Object PSObject -Property $_ } | 
   Select-Object -Property CustomerAadTenantId,CustomerName,CustomerDomain,EnvironmentName,EnvironmentType,Version,Country,AppId,AppName,AppPublisher,AppInstalledVersion,AppAvailableUpdateVersion,QualifiedEnvironmentName,QualifiedAppName | 
   Export-Csv -Path $outputFilePath -NoTypeInformation

Write-Host "File saved to $outputFilePath. You can e.g. open it in Excel and make a pivot table analysis on it."
