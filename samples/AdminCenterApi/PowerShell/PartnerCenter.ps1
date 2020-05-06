# This sample is relevant for partners who are registered as resellers for one or more customers.
# It allows the partner to perform Partner Center operations programmatically, e.g., to enumerate customers
# see their subscriptions, etc.


# Install-Module PartnerCenter # only needs to be done once, see also https://docs.microsoft.com/powershell/module/partnercenter

Connect-PartnerCenter


# Get all customers
Write-Host -ForegroundColor Cyan "Getting customers from Partner Center..."
$customers = Get-PartnerCustomer
Write-Host "Found $($customers.Length) customers."


# Filter down the list for demo purposes
$customers = $customers | ? -Property Name -Like "*MS Virtual Event*"
Write-Host "Will work with these $($customers.Length) customers going forward:"
$customers | Format-Table


# Get subscriptions
Write-Host -ForegroundColor Cyan "Getting the customers' subscriptions from Partner Center..."
foreach ($customer in $customers)
{
    Write-Host "$($customer.CustomerId), $($customer.Domain), $($customer.Name) has the following subscriptions:"
    $subscriptions = Get-PartnerCustomerSubscription -InputObject $customer
    $subscriptions | % { Write-Output @{Status=$_.Status; Quantity=$_.Quantity; FriendlyName=$_.FriendlyName; IsTrial=$_.IsTrial } } | % {[PSCustomObject]$_} | Format-Table
}
