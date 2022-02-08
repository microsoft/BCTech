# Authenticating to the Business Central Admin Center API

A main obstacle when calling APIs is how to authenticate.

The Admin Center API is based on Azure Active Directory (AAD) authentication using the OAuth standard. OAuth can seem complicated at first, but once you understand the basics, it is easy to use and actually makes life much easier - especially when calling across AAD tenants.

On this page, we explain a few simple steps that will get you up and running quickly.



## Register a new Azure Active Directory (AAD) Application in your AAD Tenant

 1. Open https://portal.azure.com
 2. Register new application:
     - Click "App registrations"
     - Click "New registration"
     - Give a name such as "App for managing Business Central environments"
     - Select "Accounts in any organizational directory (Any Azure AD directory - Multitenant)"
     - Add a redirect URI: "Public client/native" and "http://localhost"
     - Click "Register"
 3. Create the AAD application's service principal:
     - On the AAD application's "Overview" page, locate the "Managed application in local directory" property
     - If the value is a link called "Create Service Principal", then click that link
     - Navigate back in the browser to get back to the AAD application's "Overview" page
 4. Make a note of the "Application (client) ID"
 5. Add permission to call Business Central APIs:
     - Click "API permissions"
     - Click "Add a permission"
     - Click "Microsoft APIs"
     - Click "Dynamics 365 Business Central"
     - Click "Delegated permissions"
     - Select "user_impersonation"
     - Click "Add permissions"


**At this point**, the AAD application has been registered in your (the partner's) AAD tenant, and you should know these two values:
  
    $aadAppId = "<a guid>"
    $aadAppRedirectUri = "nativeBusinessCentralClient://auth"



## Specifically for Delegated Admins

This section only applies to partners who have a reseller relationship with customers, also known as Delegated Admins.

As a delegated admin, you want to manage your customers' Business Central environments. From an authentication perspective, this is significantly different from managing your own Business Central environments.

In order to call Business Central APIs for a customer, you need to authenticate **in the customer's AAD tenant**, not in your own AAD tenant. This process normally requires an administrator of the customer's AAD tenant to give consent to your AAD application, however, this step can be avoided for delegated admins by adding your AAD application into the Admin Agents group in your own AAD tenant. This technique is described in more detail here: https://docs.microsoft.com/graph/auth-cloudsolutionprovider.

In short, you need to execute the following PowerShell commands (remember to set the $aadAppId variable):

    Install-Module AzureAD
    Connect-AzureAD
    $adminAgentsGroup = Get-AzureADGroup -Filter "displayName eq 'AdminAgents'"
    $servicePrincipal = Get-AzureADServicePrincipal -Filter "appId eq '$aadAppId'"
    Add-AzureADGroupMember -ObjectId $adminAgentsGroup.ObjectId -RefObjectId $servicePrincipal.ObjectId

Now it's all set up, and you should be able to use your AAD application (in your AAD tenant) to call
into your customers' Business Central environments (in their AAD tenants).



## Obtain an Access Token

You will use your newly registered AAD application to obtain a so-called *access token*, which is a long string that looks something like this:

    eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkN0VHVoTUptRDVNN0RMZHpEMnYyeDNRS1NSWSIsImtpZCI6IkN0VHVoTUptRDVNN0RMZHpEMnYyeDNRS1NSWSJ9.eyJhdWQiOiJodHRwIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2...yMTc3MTQ1ZTEwIl19.LZgQnXOLNNpJgBx5q7FOUgq5ka04lJkBw75kxMTUA7hFDEL-NsMVcwQ_Zt-H0aPkOevCAQ_KWtZRQA

This string encodes different information, including who you are, and which API you want to call. You don't need to know all about access tokens - the main thing to know is that in order to use it, you need to send it in the Authorization header of each HTTP request, like this:

    GET https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments
    Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkN0VHVoTUptRDVNN0RMZHpEMnYyeDNRS1NSWSIsImtpZCI6IkN0VHVoTUptRDVNN0RMZHpEMnYyeDNRS1NSWSJ9.eyJhdWQiOiJodHRwIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2...yMTc3MTQ1ZTEwIl19.LZgQnXOLNNpJgBx5q7FOUgq5ka04lJkBw75kxMTUA7hFDEL-NsMVcwQ_Zt-H0aPkOevCAQ_KWtZRQA

To obtain an access token, you can follow the steps in either [Authenticate.ps1](PowerShell/Authenticate.ps1) for PowerShell or [Authenticate.cs](CSharp/Authenticate.cs) for C#.

And to use the access token to call the Admin Center API, you can follow the steps in e.g. [Environments.ps1](PowerShell/Environments.ps1) for PowerShell or [Environments.cs](CSharp/Environments.cs) for C#.
