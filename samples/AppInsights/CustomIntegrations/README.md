In this folder, you will find information that illustrate how you can integrate Application Insights into your own applications or how to control where data is ingested.

# How do I create a link (URL) that opens a KQL query in the Azure Application Insights portal?
It is possible to programatically create links that will run and show results of a KQL query in Azure Application Insights portal.

To do this, first you need to encode the KQL query text: Take the raw query text, zip it, Base64 encode it, and then url encode it.

Next, go to the Azure Application Insights portal where data resides and note the following GUIDs and names:
- AAD tenant id
- Azure subscription id
- Azure resource group name
- Azure Application Insights instance name

Now, you can construct the URL:
```
https://portal.azure.com/#@<AAD tenant id>/blade/Microsoft_Azure_Monitoring_Logs/LogsBlade/resourceId/%2Fsubscriptions%2F<Azure subscription id>%2FresourceGroups%2F<Azure resource group name>%2Fproviders%2Fmicrosoft.insights%2Fcomponents%2F<Azure Application Insights instance name>/source/LogsBlade.AnalyticsShareLinkToQuery/q/<Encoded KQL query>
```
(substitute the strings "<id>" with the actual values)


Microsoft MVP Stefano Demiliani has written out this example in great details. Read more on his blog here: https://demiliani.com/2022/01/11/create-a-link-to-an-application-insights-query-programmatically/

# How do I read telemetry data from Azure Application Insights with C#?
The Azure SDK for .NET has an API for interacting with Azure Application Insights, see https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.applicationinsights?view=azure-dotnet

Specifically, after having authenticated to Azure Application Insights, the method ApplicationInsightsDataClientExtensions.Query can be used to execute KQL queries from C#, see https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.applicationinsights.applicationinsightsdataclientextensions.query?view=azure-dotnet

Use these NuGet packages in your project:
- Microsoft.Rest.ClientRuntime.Azure.Authentication (for authenticating your app to read data from Azure Application Insights)
- Microsoft.Azure.ApplicationInsights (for reading data using the Azure Application Insights REST API)


Microsoft MVP Tobias Zimmergren has written out an example in great details. Read more on his blog here: 
https://zimmergren.net/retrieve-logs-from-application-insights-programmatically-with-net-core-c/


# How do I send telemetry data to a different endpoint than Azure Application Insights?
See the code sample and guidance in [CustomEndpoint](CustomEndpoint/README.md)


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
