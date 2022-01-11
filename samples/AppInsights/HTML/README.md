In this folder, you will find information that illustrate how you can construct a link that runs a KQL query and shows the result in the Azure Application Insights portal.

# How do I create a link (URL) that opens a KQL query in the Azure Application Insights portal?
It is possible to programatically create links that will run and show results of a KQL query in Azure Application Insights portal.

To do this, first you need to encode the KQL query text: Take the raw query text, zip it, and Base64 encode it.

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


Stefano Demiliani has written out this example in great details. Read more on his blog here: https://demiliani.com/2022/01/11/create-a-link-to-an-application-insights-query-programmatically/


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
