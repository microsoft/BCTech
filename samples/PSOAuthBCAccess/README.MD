# Powershell samples for Business Central access token acquisition and usage

This folder contains 4 samples for different OAuth flows to acquire token to access Business Central:

* Acquire token as Application
* Acquire token with Resource Owner Password Credentials
* Acquire token by authorization Code
* Acquire token interactively

Prerequisites:

Installed `MSAL.PS` Powershell module.

In order to access **Business Central Automation API**, it is required to **[Setup Azure AD application in Business Central](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/automation-apis-using-s2s-authentication#task-2-set-up-the-azure-ad-application-in-)** .

**Azure AD Application** registration:

1. Sign into azure portal under tenant admin user, go to **'Azure Active Directory'** - **'App registrations'**

2. Click **'+ New registration'**. Provide a **'Name'** for application, set **'Supported account types'** to **"Accounts in this organizational directory only"**. Click **'Register'** button.

3. Setup API permissions for newly created application, go to **'API permissions'**:
    * In order to acquire tokens as application (used for automation APIs), add: **'Dynamics 365 Business Central'** - **'Application permissions'** - **'API.ReadWrite.All'** or **'Automation.ReadWrite.All'**
    * In order to acquire tokens as user, i.e. impersonate somebody (used to call BC web services), add: **'Dynamics 365 Business Central'** - **'Delegated permissions'** - **'Financials.ReadWrite.All'**

4. After permissions are added, click **'Grant admin consent for ...'**. Status in the table of permissions shoud change to **'Granted'**

5. Setup client secret (used in authorization code flow scenario and application scenario):
    * Go to **'Certificates & secrets'** - **'Client secrets'**, click **'+ New client secret'**
    * Add some description, choose **'Expires'** setting and click **'Add'**
    * After secret is created, copy secret **'Value'** and save for later.

6. Setup Redirect Uris:
    * For authorization code flow it is required to setup redirect Uri for web platform. Go to **'Authentication'** - **'Platform configurations'**, click **'+ Add a platform'**. Choose **'Web'**, enter **`'http://localhost:8080/login'`** as redirect Uri. Click **'Configure'**.
    * For interactive and ROPC flows it is required to setup redirect Uri for mobile and desktop platform. Go to **'Authentication'** - **'Platform configurations'**, click **'+ Add a platform'**. Choose **'Mobile and desktop applications'**, click to switch on **'... (MSAL only)'** redirect Uri. Click **'Configure'**.

7. In order to use Resource Owner Password Credential flow, allow public client flows. Go to **'Authentication'** - **'Advanced settings'**, and set **'Allow public client flows'** to **'Yes'**. Click **'Save'** button at the top.

Change values in **CommonData.psm1** module:

  * Set **`$ClientId`** in the module to registered application Client Id value 
  * Set **`$RedirectUri`** in the module to registered application Web redirect Uri
  * Set **`$AadTenantId`** in the module to Azure AD Tenant Id where Business Central is located

## Disclaimer

Microsoft Corporation ("Microsoft") grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys' fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise.

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
