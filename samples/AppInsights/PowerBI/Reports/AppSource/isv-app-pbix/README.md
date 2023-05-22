In this folder, you will find the source code for the PowerBI app report on Dynamics 365 Business Central *ISV* telemetry in Azure Application Insights. 

# Power BI prerequisites
The app is not on appsource yet, so you need to allow it to be installed. 

Go to the PBI admin portal.
Under Tenant settings, Go to Template App settings. Here you can enable template apps that are not listed on app source.
Remember to set the setting back after installing the beta version of the app.

![Prereq](../../../../images/power-bi-prereq.png)

# Getting the report
Use this link to install/update the template app from Appsource: https://aka.ms/bctelemetry-isv-app-beta

The app comes with sample data.

# Connecting to Azure Application Insights
To connect the app to an Azure Application Insights resource, you need one thing: the Application Insights app id (get it from the API Access menu in the Azure Application Insights portal). 

![Workspace](../../../../images/pbi_app_app_id.png)


NB! If you get this error "The OAuth authentication method isn't supported for this data source", then please check if the application id is correct. This usually is the root cause for that error.

# Configuring the (AAD Tenant id, customer) mapping
You define the (AAD tenant id, domain name) mapping in the app parameter _AAD tenant mapping_, that you set when you configure the app to read data from your Azure Application Insights resource (you can also change parameter values after configuring the app).

![Mapping](../../../../images/mapping.png)

The mapping must be uploaded as a (minified) json file with the following format:
```
{
    "map":
    [
        { "AAD tenant id":"005bbe22-5949-4acb-9d24-3fb396c64a52" , "Domain":"Contoso 1" },
        { "AAD tenant id":"0140d8e7-ef60-4cc3-9a6b-b89042b3ea1f" , "Domain":"Contoso 2" }        
    ]
}
```

(see examples of the json format and the corresponding minified file here: [Tenant mapping examples](./tenant-mapping))

A minified json file is just a json file where all newlines have been removed. This is needed for Power BI to be able to read the data.

You can also use Powershell to produce the json input. This repository has a script [Get-AADMapping.ps1](./tenant-mapping/Get-AADMapping.ps1) that makes this easy.

# Changing parameters after initial configuration
Once you completed the setup of the app, how can you change parameters such as _Application Insights application id_ or _Lookback period_?

You can change configuration settings by going to the Power BI portal, open the workspace for the installed app, go to settings, and then Parameters.

![Workspace](../../../../images/pbi_app_workspace.png)

![Parameters](../../../../images/pbi_app_parameters.png)

# Sharing the app with coworkers and others
Once installed, it is possible to share the app with coworkers and others (e.g. customers). 

Do this:
1. Share the Power BI workspace with the person. This will make the app appear under _Workspaces_ in their Power BI portal.
2. Provide the url to the person and ask them to open it in a browser. This will make the app appear under _Apps_ in their Power BI portal.

Read more here [Share Power BI reports and dashboards with coworkers and others](https://learn.microsoft.com/en-us/power-bi/collaborate-share/service-share-dashboards)



# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
