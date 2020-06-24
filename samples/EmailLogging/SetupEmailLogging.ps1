<#
THIS CODE-SAMPLE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND.
#>

param
(
  [Parameter(Mandatory = $true)][string]$EmailLoggingUser
)
Write-Host "Start setting up email logging..."

Write-Host "Checking that basic authentication is allowed for WinRM..."
$winRmAuth = winrm get winrm/config/client/auth
if (!($winRmAuth | where { $_.Trim().Replace(' ', '').ToLower().StartsWith("basic=true")}))
{
  Write-Host "WinRM on your computer must allow Basic authentication"
  winrm set winrm/config/client/auth @{Basic="true"}
  Write-Host "Basic authentication is added for WinRM"
  Write-Host "Close and re-open the elevated Windows PowerShell window to get the changes from the previous step"
  Exit
}
else
{
  Write-Host "Basic authentication is allowed for WinRM"
}

Write-Host "Checking that PowershellGet is installed..."
$psGetModule = Get-InstalledModule PowershellGet | sort Version | select -last 1
if ($psGetModule)
{
  Write-Host "Updating PowershellGet..."
  Update-Module PowershellGet
  Write-Host "Getting PowershellGet module version..."
  $psGetModule = Get-InstalledModule PowershellGet | sort Version | select -last 1
}
else
{
  Write-Host "Installing PowershellGet..."
  Install-Module PowershellGet -Force
  Write-Host "Getting PowershellGet module version..."
  $psGetModule = Get-InstalledModule PowershellGet | sort Version | select -last 1
}
Write-Host "PowershellGet module version: $($psGetModule.Version)"

Write-Host "Checking execution policy..."
$executionPolicy = Get-ExecutionPolicy
Write-Host "Current execution policy is $executionPolicy"
if ($executionPolicy -ne "RemoteSigned")
{
  Write-Host "Execution policy RemoteSigned is needed to allow running scripts downloaded from the internet"
  Set-ExecutionPolicy "RemoteSigned"
  Write-Host "Close and re-open the elevated Windows PowerShell window to get the changes from the previous step"
  Exit
}

Write-Host "Importing EXO module..."
Import-Module ExchangeOnlineManagement

Write-Host "Getting EXO module version..."
$exoModule = Get-Module ExchangeOnlineManagement | sort Version | select -last 1
Write-Host "EXO module version: $($exoModule.Version)"

Write-Host "Connecting to Exchange Online..."
Connect-ExchangeOnline -ShowProgress $true -ShowBanner:$false
Write-Host "Sucessfully connected to Exchange Online"

Write-Host "Getting organization configuration..."
$config = Get-OrganizationConfig
if ($config.IsDehydrated)
{
  Write-Host "Enabling organization customization..."
  Enable-OrganizationCustomization
}
else
{
  Write-Host "Organization customization is already enabled"
}

if ($config.PublicFoldersEnabled -eq "Remote")
{
  throw "The PublicFoldersEnabled attribute set to Remote confirms that the Office 365 users are set to use the On-Premises Public Folders"
}

$roleGroupName = "Public Folders Management"
$roleName = "Public Folders"
Write-Host "Checking that role group '$roleGroupName' exists..."
$roleGroup = Get-RoleGroup -Identity $roleGroupName -ErrorAction SilentlyContinue
if (!$roleGroup)
{
  Write-Host "Adding a new role group for public folders"
  $roleGroup = New-RoleGroup -Name $roleGroupName -Roles $roleName
  if (!$roleGroup)
  {
    throw "Cannot create role group '$roleGroupName'"
  }
  Write-Host "Role group '$roleGroupName' has been created"
}
else
{
  Write-Host "Role group '$roleGroupName' already exists"
}
if (!($roleGroup.Roles | where {$_ -eq $roleName}))
{
  throw "Role group '$roleGroupName' is configured incorrectly"
}

Write-Host "Checking members of role group '$roleGroupName'..."
$groupMembers = Get-RoleGroupMember -Identity $roleGroupName -ErrorAction SilentlyContinue
if (!($groupMembers.PrimarySmtpAddress | where { $_ -eq $EmailLoggingUser }))
{
  Write-Host "Adding the email logging user to role group '$roleGroupName'"
  Add-RoleGroupMember -Identity $roleGroupName -Member $EmailLoggingUser
  if (!($groupMembers.PrimarySmtpAddress | where { $_ -eq $EmailLoggingUser }))
  {
    throw "The email logging user is not found in the list of members of role group '$roleGroupName'"
  }
  else
  {
    Write-Host "The email logging user has been added to role group '$roleGroupName'"
  }
}
else
{
  Write-Host "The email logging user is already a member of role group '$roleGroupName'"
}

Write-Host "Checking that public mailbox exists..."
$mailboxName = "Public MailBox"
$mailbox = Get-Mailbox -PublicFolder -Filter "Name -eq '$mailboxName'"
if (!$mailbox)
{
  Write-Host "Creating public mailbox '$mailboxName'..."
  $mailbox = New-Mailbox -PublicFolder -Name $mailboxName
  if (!$mailbox)
  {
    throw "Cannot create public mailbox '$mailboxName'."
  }
  Write-Host "Public mailbox '$mailboxName' has been created"
}
else
{
  Write-Host "Public mailbox '$mailboxName' already exists"
}

Write-Host "Creating public folders..."
$rootFolderName = "Email Logging"
$rootFolderPath = "\$rootFolderName"
Write-Host "Checking that root public folder '$rootFolderPath' exists in mailbox '$mailboxName'..."
$rootFolder = Get-PublicFolder -Identity $rootFolderPath -ErrorAction SilentlyContinue
if (!$rootFolder)
{
  Write-Host "Creating root public folder $rootFolderPath in mailbox '$mailboxName'..."
  $rootFolder = New-PublicFolder -Name $rootFolderName -Mailbox $mailboxName
  if (!$rootFolder)
  {
    throw "Cannot create root public folder '$rootFolderPath' in mailbox '$mailboxName'"
  }
  Write-Host "Root public folder '$rootFolderPath' has been created in mailbox '$mailboxName'"
}
else
{
  Write-Host "Root public folder '$rootFolderPath' already exists in mailbox '$mailboxName'"
}

$queueFolderName = "Queue"
$queueFolderPath = "$rootFolderPath\$queueFolderName"
Write-Host "Checking that public folder '$queueFolderName' exists under '$rootFolderPath' in mailbox '$mailboxName'..."
$queueFolder = Get-PublicFolder -Identity $queueFolderPath -ErrorAction SilentlyContinue
if (!$queueFolder)
{
  Write-Host "Creating public folder '$queueFolderName' under '$rootFolderPath' in mailbox '$mailboxName'..."
  $queueFolder = New-PublicFolder -Name $queueFolderName -Path $rootFolderPath -Mailbox $mailboxName
  if (!$queueFolder)
  {
    throw "Cannot create public folder '$queueFolderName' under '$rootFolderPath' in mailbox '$mailboxName'"
  }
  Write-Host "Public folder '$queueFolderName' has been created under '$rootFolderPath' in mailbox '$mailboxName'"
}
else
{
  Write-Host "Public folder '$queueFolderName' already exists under '$rootFolderPath' in mailbox '$mailboxName'"
}

$storageFolderName = "Storage"
$storageFolderPath = "$rootFolderPath\$storageFolderName"
Write-Host "Checking that public folder '$storageFolderName' exists under '$rootFolderPath' in mailbox '$mailboxName'..."
$storageFolder = Get-PublicFolder -Identity $storageFolderPath -ErrorAction SilentlyContinue
if (!$storageFolder)
{
  Write-Host "Creating public folder '$storageFolderName' under '$rootFolderPath' in mailbox '$mailboxName'..."
  $storageFolder = New-PublicFolder -Name $storageFolderName -Path $rootFolderPath -Mailbox $mailboxName
  if (!$storageFolder)
  {
    throw "Cannot create public folder '$storageFolderName' under '$rootFolderPath' in mailbox '$mailboxName'"
  }
  Write-Host "Public folder '$storageFolderName' has been created under '$rootFolderPath' in mailbox '$mailboxName'"
}
else
{
  Write-Host "Public folder '$storageFolderName' already exists under '$rootFolderPath' in mailbox '$mailboxName'"
}

Write-Host "Checking that public folder '$queueFolderPath' is enabled..."
$queueFolder = Get-PublicFolder -Identity $queueFolderPath -ErrorAction SilentlyContinue
if (!$queueFolder.MailEnabled)
{
  Write-Host "Enabling public folder '$queueFolderPath'..."
  Enable-MailPublicFolder -Identity $queueFolderPath
  $queueFolder = Get-PublicFolder -Identity $queueFolderPath -ErrorAction SilentlyContinue
  if (!$queueFolder.MailEnabled)
  {
    throw "Cannot enable public folder '$queueFolderName'"
  }
  Write-Host "Public folder '$queueFolderPath' has been enabled"
}
else
{
  Write-Host "Public folder '$queueFolderPath' has already been enabled"
}

Write-Host "Checking that public folder '$queueFolderPath' has email address..."
$queueMailPublicFolder = Get-MailPublicFolder -Identity $queueFolderPath -ErrorAction SilentlyContinue
if (!$queueMailPublicFolder)
{
  throw "Cannot get email address of public folder '$queueFolderPath'"
}
$queueEmailAddress = $queueMailPublicFolder.PrimarySmtpAddress
if (!$queueEmailAddress)
{
  throw "Empty email address of public folder '$queueFolderPath'"
}
Write-Host "Public folder '$queueFolderPath' has email address $queueEmailAddress"

Write-Host "Checking anonymous user permissions for public folder '$queueFolderPath'..."
$anonymousUserPermission = Get-PublicFolderClientPermission -Identity $queueFolderPath -User Anonymous -Mailbox $mailboxName -ErrorAction SilentlyContinue
if (!$anonymousUserPermission)
{
  Write-Host "Adding anonymous user permissions for public folder '$queueFolderPath'..."
  $anonymousUserPermission = Add-PublicFolderClientPermission -Identity $queueFolderPath -AccessRights CreateItems -User Anonymous
  if (!$anonymousUserPermission -or !($anonymousUserPermission.AccessRights | where {$_ -eq "CreateItems"}))
  {
    throw "Cannot add the anonymous user permission to public folder '$queueFolderPath'"
  }
  Write-Host "Anonymous user permissions have been added for public folder '$queueFolderPath'..."
}
else
{
  if (!($anonymousUserPermission.AccessRights | where {$_ -eq "CreateItems"}))
  {
    Write-Host "Anonymous user permissions need to be updated for public folder '$queueFolderPath'..."
    Write-Host "Removing anonymous user permissions for public folder '$queueFolderPath'..."
    Remove-PublicFolderClientPermission -Identity $queueFolderPath -User Anonymous -Confirm:$false
    Write-Host "Adding anonymous user permissions for public folder '$queueFolderPath'..."
    $anonymousUserPermission = Add-PublicFolderClientPermission -Identity $queueFolderPath -AccessRights CreateItems -User Anonymous
    if (!$anonymousUserPermission -or !($anonymousUserPermission.AccessRights | where {$_ -eq "CreateItems"}))
    {
      throw "Cannot update the anonymous user permission for public folder '$queueFolderPath'"
    }
    else
    {
      Write-Host "Anonymous user permissions have been updated for public folder '$queueFolderPath'..."
    }
  }
  else
  {
    Write-Host "Anonymous user already has the required permissions for public folder '$queueFolderPath'..."
  }
}

Write-Host "Checking the permissions of the email logging user for public folder '$queueFolderPath'..."
$emailLoggingUserPermission = Get-PublicFolderClientPermission -Identity $queueFolderPath -User $EmailLoggingUser -Mailbox $mailboxName -ErrorAction SilentlyContinue
if (!$emailLoggingUserPermission)
{
  Write-Host "Adding permissions for the email logging user to public folder '$queueFolderPath'..."
  $emailLoggingUserPermission = Add-PublicFolderClientPermission -Identity $queueFolderPath -AccessRights Owner -User $EmailLoggingUser
  if (!$emailLoggingUserPermission -or !($emailLoggingUserPermission.AccessRights | where {$_ -eq "Owner"}))
  {
    throw "Cannot add permissions for the email logging user to public folder '$queueFolderPath'"
  }
  Write-Host "Permissions for the email logging user have been added to public folder '$queueFolderPath'..."
}
else
{
  if (!($emailLoggingUserPermission.AccessRights | where {$_ -eq "Owner"}))
  {
    Write-Host "Permissions for the email logging user must be updated for public folder '$queueFolderPath'..."
    Write-Host "Removing the permissions for the email logging user from public folder '$queueFolderPath'..."
    Remove-PublicFolderClientPermission -Identity $queueFolderPath -User $EmailLoggingUser -Confirm:$false
    Write-Host "Adding permissions for the email logging user to public folder '$queueFolderPath'..."
    $emailLoggingUserPermission = Add-PublicFolderClientPermission -Identity $queueFolderPath -AccessRights Owner -User $EmailLoggingUser
    if (!($emailLoggingUserPermission.AccessRights | where {$_ -eq "Owner"}))
    {
      throw "Cannot update the permissions for the email logging user to public folder '$queueFolderPath'"
    }
    else
    {
      Write-Host "Permissions for the email logging user have been updated for public folder '$queueFolderPath'..."
    }
  }
  else
  {
    Write-Host "The email logging user already has the required permissions to public folder '$queueFolderPath'..."
  }
}

Write-Host "Checking the permissions for the email logging user on public folder '$storageFolderPath'..."
$emailLoggingUserPermission = Get-PublicFolderClientPermission -Identity $storageFolderPath -User $EmailLoggingUser -Mailbox $mailboxName -ErrorAction SilentlyContinue
if (!$emailLoggingUserPermission)
{
  Write-Host "Adding the permissions for the email logging user to public folder '$storageFolderPath'..."
  $emailLoggingUserPermission = Add-PublicFolderClientPermission -Identity $storageFolderPath -AccessRights Owner -User $EmailLoggingUser
  if (!$emailLoggingUserPermission -or !($emailLoggingUserPermission.AccessRights | where {$_ -eq "Owner"}))
  {
    throw "Cannot add permissions for the email logging user to public folder '$storageFolderPath'"
  }
  Write-Host "Permissions for the email logging user have been added to public folder '$storageFolderPath'..."
}
else
{
  if (!($emailLoggingUserPermission.AccessRights | where {$_ -eq "Owner"}))
  {
    Write-Host "Permissions for the email logging user must be updated for public folder '$storageFolderPath'..."
    Write-Host "Removing the permissions for the email logging user from public folder '$storageFolderPath'..."
    Remove-PublicFolderClientPermission -Identity $storageFolderPath -User $EmailLoggingUser -Confirm:$false
    Write-Host "Adding permissions for the email logging user to public folder '$storageFolderPath'..."
    $emailLoggingUserPermission = Add-PublicFolderClientPermission -Identity $storageFolderPath -AccessRights Owner -User $EmailLoggingUser
    if (!($emailLoggingUserPermission.AccessRights | where {$_ -eq "Owner"}))
    {
      throw "Cannot update the permissions of the email logging user on public folder '$storageFolderPath'"
    }
    else
    {
      Write-Host "Permissions for the email logging user have been updated on public folder '$storageFolderPath'..."
    }
  }
  else
  {
    Write-Host "The email logging user already has the required permissions to public folder '$storageFolderPath'..."
  }
}

$incomingRuleName = "Log Email Sent to This Organization"
Write-Host "Checking that transport rule '$incomingRuleName' exits..."
$incomingRule = Get-TransportRule -Identity $incomingRuleName -ErrorAction SilentlyContinue
if (!$incomingRule)
{
  Write-Host "Creating transport rule '$incomingRuleName'..."
  $incomingRule = New-TransportRule -Name $incomingRuleName -FromScope "NotInOrganization" -SentToScope "InOrganization" -BlindCopyTo $queueEmailAddress
  Write-Host "Transport rule '$incomingRuleName' has been created"
}
else
{
  Write-Host "Transport rule '$incomingRuleName' already exits"
}
if (($incomingRule.FromScope -ne "NotInOrganization") -or ($incomingRule.SentToScope -ne "InOrganization") -or ($incomingRule.BlindCopyTo -ne $queueEmailAddress))
{
  throw "The transport rule '$incomingRuleName' is configured incorrectly"
}

$outgoingRuleName = "Log Email Sent from This Organization"
Write-Host "Checking that transport rule '$outgoingRuleName' exits..."
$outgoingRule = Get-TransportRule -Identity $outgoingRuleName -ErrorAction SilentlyContinue
if (!$outgoingRule)
{
  Write-Host "Creating transport rule '$outgoingRuleName'..."
  $outgoingRule = New-TransportRule -Name $outgoingRuleName -FromScope "InOrganization" -SentToScope "NotInOrganization" -BlindCopyTo $queueEmailAddress
  Write-Host "Transport rule '$outgoingRuleName' has been created"
}
else
{
  Write-Host "Transport rule '$outgoingRuleName' already exits"
}
if (($outgoingRule.FromScope -ne "InOrganization") -or ($outgoingRule.SentToScope -ne "NotInOrganization") -or ($outgoingRule.BlindCopyTo -ne $queueEmailAddress))
{
  throw "The transport rule '$outgoingRuleName' is configured incorrectly"
}

Write-Host "Disconnecting from Exchange Online..."
Disconnect-ExchangeOnline -Confirm:$false -ErrorAction SilentlyContinue
Write-Host "Email logging has been set up"
