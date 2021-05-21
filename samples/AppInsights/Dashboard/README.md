# How do I get started with Azure dashboards?

To reduce the time-to-value for you, we have prepared a set of dashboards that you can deploy to your Azure portal:
1. Open Company Performance
2. Session overview
3. Long Running SQL Queries
4. Failed Authorization
5. Web Service Calls

Clicking the *Deploy To Azure* button below will launch the Azure Portal with an ARM template, where you need to specify the subscription, resource group and name of 
your Application Insights Resource. All requested dashboards will be installed and you can now remove the ones you do not need.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3a%2f%2fraw.githubusercontent.com%2fmicrosoft%2fBCTech%2fmaster%2fsamples%2fAppInsights%2fDashboard%2fazuredeploy.json" target="_blank"><img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.png"/></a>

Each dashboard is a JSON file, that describes which *widgets* the dashboard should contain.


# How can I make my own Azure dashboards?

We know that the dashboards we have provided might not match your needs exactly, and if you want to customize them, we recommend that you clone this repo and make your adjustments there, before importing the dashboard in the Azure portal.

**Note:** The "Deploy To Azure" button above navigates to `https://portal.azure.com/#create/Microsoft.Template/uri/` followed by an escaped version of the URI of the azuredeploy.json file (In this repo: `https%3a%2f%2fraw.githubusercontent.com%2fmicrosoft%2fBCTech%2fmaster%2fsamples%2fAppInsights%2fDashboard%2fazuredeploy.json`). You will need to modify this URI for the button to deploy from your repository. 

Adding dashboards is done by exporting a dashboard from the Azure Portal, running the ConvertExportedDashboardToDashboardTemplate.ps1 and then adding the new template to the resources section in azuredeploy.json.

As we improve our dashboards, you can merge the changes into your cloned repo and in this way stay up-to-date.



# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
