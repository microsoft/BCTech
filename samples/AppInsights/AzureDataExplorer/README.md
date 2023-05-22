In this folder, you will find samples that illustrate how you can query Azure Application Insights data using **Azure Data Explorer** with **KQL Queries**.

**Which Azure Data Explorer queries are available?**  This repository contains an Azure Data Explorer sample dashboard for Business Central.  The dashboard is made up of the following pages.  Each page contains tiles that are powered by KQL queries.
* Overview
* SQL Query Usage
* AL Method Usage
* SOAP Usage
* OData Usage
* API Usage
* Job Queue
* Upgrades
* Database Locks
* Lookup

**How do I use Azure Data Explorer queries to query telemetry?**  
To use the dashboard, do as follows
1.	Ensure you have your AppInsights Resources setup.
2.	Download the sample Azure Data Explorer dashboard.
3.	Go to https://DataExplorer.azure.com/dashboards 
4.	In the upper left corner, click on the dropdown next to “New Dashboard” and select “Import dashboard from file”.
5.	Select the downloaded Azure Data Explorer dashboard sample to import.
6.	After the dashboard has been imported, click on the “Edit” icon in the upper left corner of the dashboard.
7.	Next, click on the “Data Sources” icon and then click on the edit icon on the BCAppInsights Data Source.
8.	Edit the Cluster URI to match your AppInsights information:  
https://ade.applicationinsights.io/subscriptions/ **Subscription ID** /resourcegroups/ **Resource Group**
* **Subscription ID** - can be found at [Subscriptions - Microsoft Azure](https://portal.azure.com/#blade/Microsoft_Azure_Billing/SubscriptionsBlade)
* **Resource Group** - can be found at [Application Insights - Microsoft Azure](https://portal.azure.com/#blade/HubsExtension/BrowseResource/resourceType/microsoft.insights%2Fcomponents)

9.	Lastly, click on the “Connect” button.  The Database name should be that same as your AppInsights Resource.  Leave the Data Source Name” as BCAppInsights.  This will be used by all the queries and if it is changed, then all the queries will need to be updated.
Limitations: The current version of the queries fails if the KQL queries do not return any rows. This could be due to data not being available within a selected time frame.  Try extending the query time frame to increase the chances of finding data to report on.  Another issue might be related to when the AppInsights key was updated on the tenants.  It does take a little time to have the data collected for your AppInsights key.
For more information about Azure Data Explorer you can go here [What is Azure Data Explorer?](https://learn.microsoft.com/en-us/azure/data-explorer/data-explorer-overview)

  
Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise.
THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
