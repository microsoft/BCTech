# Authenticating to the Business Central Admin Center API


TODO: link to Docs





## Register a new Azure Active Directory (AAD) Application in your AAD Tenant

 1. Open https://portal.azure.com
 2. *Register new application*
     - Click "App registrations"
     - Click "New registration"
     - Give a name such as "App for managing customer Business Central environments"
     - Select "Accounts in any organizational directory (Any Azure AD directory - Multitenant)"
     - Add a redirect URI: "Public client/native" and "nativeBusinessCentralClient://auth"
     - Click "Register"
 3. Create the AAD application's service principal
     - On the AAD application's "Overview" page, locate the "Managed application in local directory" property
     - If the value is a link that is called "Create Service Principal", then click that link
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



## The Delegated Admin case

If you are a VAR who has a Reseller relationship with your customers, then you can also manage your customers' Business Central environments.

***TODO: add link to docs***

In order to call Business Central APIs fir a customer, you need to authenticate in the customer's AAD tenant, not in your own AAD tenant. This process normally requires an administrator of the customer's AAD tenant to give consent to your AAD application, however, this step can be avoided for delegated admins by adding your AAD application into the Admin Agents group in your own AAD tenant. This is described in more detail here: https://docs.microsoft.com/graph/auth-cloudsolutionprovider.

    Install-Module AzureAD
    Connect-AzureAD
    $adminAgentsGroup = Get-AzureADGroup -Filter "displayName eq 'AdminAgents'"
    $servicePrincipal = Get-AzureADServicePrincipal -Filter "appId eq '$aadAppId'"
    Add-AzureADGroupMember -ObjectId $adminAgentsGroup.ObjectId -RefObjectId $servicePrincipal.ObjectId

Now it's all set up, and you should be able to use your AAD application (in your AAD tenant) to call
into your customers' Business Central environments (in their AAD tenants)
