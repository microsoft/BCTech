# Get alerts when something happens
If something happens in your environment or for one of your customers that you need to take action on, it is better that the system sends you an alert. Azure Application Insights makes it easy to define such alerts.

## How do I create alerts in Azure Application Insights?
Here is an example to get you started:
 1. Open the Azure portal and locate your Application Insights resource
 2. Click "Alerts" in the navigation pane on the left
 3. Use one of the KQL samples from this section in the condition for a custom log search 

## How do I get alerts via email?
If you want alerts via email, you can just create a new action group in your Application Insights resource, and in your alerts add an action to send an email.

## How do I get alerts using Microsoft Dynamics Logic apps?
Here in this repository, you can find templates for three different types of alerts using Microsoft Dynamics Logic apps:
* Grouped notification for available updates
* Notification for deleted environment
* Action each failed environment update

Read more here in docs:
https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tenant-admin-center-notifications#application-insights


## How do I get alerts via Microsoft Teams?
You can also send alerts to a channel in Microsoft Teams. See an example of how to set that up here: https://dailydotnettips.com/sending-your-azure-application-insights-alerts-to-team-sites-using-azure-logic-app/

## How do I get alerts on my phone?
This video shows how you can get alerts as push notifications on your phone: https://www.youtube.com/watch?v=nqqVEISjSGE

## Want to learn more about Azure Monitor alerts?
Please read more in the documentation for Azure Application Insights: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/alerts-unified-log



# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
