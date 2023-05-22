# This sample is relevant for partners who are registered as resellers for one or more customers.
# It allows the partner to perform Partner Center operations programmatically, e.g., to enumerate customers,
# see their subscriptions, etc.


Install-Module PartnerCenter  # only needs to be done once, see documentation at https://learn.microsoft.com/powershell/module/partnercenter


# Authenticate to Partner Center
Connect-PartnerCenter


# Get all customers
$customers = Get-PartnerCustomer
Write-Host "Found $($customers.Length) customers."


# Filter down the list for demo purposes
$customers = $customers | ? -Property Name -Like "*MS Virtual Event*"
Write-Host "Will work with these $($customers.Length) customers going forward:"
$customers | Format-Table


# Get subscriptions
foreach ($customer in $customers)
{
    Write-Host "$($customer.CustomerId), $($customer.Domain), $($customer.Name) has the following subscriptions:"
    $subscriptions = Get-PartnerCustomerSubscription -InputObject $customer
    $subscriptions | Select-Object -Property Status,Quantity,FriendlyName,IsTrial,CommitmentEndDate,AutoRenewEnabled | Format-Table
}
