# What is the Business Central Performance Toolkit (BCPT)?
In short, the Business Central Performance Toolkit lets you simulate concurrent usage of realistic scenarios with many users. 

Read more about BCPT here: [The Performance Toolkit Extension](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-performance-toolkit)

Get the Performance Toolkit AL extension from Appsource here: 
https://appsource.microsoft.com/en-GB/product/dynamics-365-business-central/PUBID.microsoftdynsmb%7CAID.75f1590f-55c5-4501-ae63-bada5534e852%7CPAPPID.75f1590f-55c5-4501-ae63-bada5534e852?tab=Overview

To get you started quickly, open source scenario code is available here: https://github.com/microsoft/ALAppExtensions/tree/main/Other/Tests/BCPT-SampleTests

# How can I use the Performance Toolkit?
Once installed and configured, how can you use the tool? We identified four different personas as described in this table:

| Persona | Description |
| ------- | ----------- |
| Account Manager | Person making sales to new and existing customers. |
| Product Owner | Person responsible for an extension/app. |
| Project manager | Person responsible for the implementation of a Business Central environment for a customer. |
| Consultant | Person working with implementations of Business Central by setting up functionality in the product (not by writing AL code)|
| Supporter | Person performing triage, investigation, and mitigation of customer issues. |
| Developer | Person who writes AL code. |
| Operations manager | Person responsible for running Business Central environments for customers. |

<br>
In the following table, you'll find examples of scenarios for each persona where the app might be of help.

| Persona | Scenario | How the Performance Toolkit can help |
| ------- | ---------| ------------------------------------ |
| Account Manager | In pre-sales, be able to answer questions such as "can Business Central handle our scenarios?" | 0. Maybe get some help from your organization to complete steps 1-5 below. Or work on automating parts of this process. <br>
1. Spin up an online Business Central environment <br> 2. Setup telemetry on the environment (to get clean data, setup a new Application Insights ressource for this test only) <br> 3. Install and configure the BCPT AL extension with the scenarios you need and in the expected use pattern. <br> 4. Run the scenarios <br> 5. Install and configure the Power BI app on telemetry.  Look at the results in Performance report. | 
| Product Owner | Compare performance between builds of your app(s). | 1. Setup telemetry on the app (to get clean data, setup a new Application Insights ressource for devops) <br> 2. Have your developers write scenario codeunits for your main scenarios. <br> 3. Run scenarios as part of your devops pipelines. <br> 4. Install and configure the Power BI app on app telemetry. Look at the results in Performance report. <br> 5. Consider setting up alerts on regressions. | 
| Developer | Test concurrency of your code | 1. Setup telemetry on your development environment (to get clean data, setup a new Application Insights ressource for your setup) <br> 2. Write a scenario codeunit for your scenario. <br> 3. Run the scenarios in the BCPT VS code extension. <br> 4a. Install and configure the Power BI app on app telemetry. Look at the results in Performance report. <br> 4b. If you prefer to analyze with KQL, use that tool instead. | 
| Operations manager | Test Business Central environments on next version before upgrading customers | 1. Copy the production environment to a sandbox. <br> 2. Setup telemetry on the environment (to get clean data, setup a new Application Insights ressource for this test only) <br> 3a. Install and configure the BCPT AL extension with the scenarios you need and in the expected use pattern. <br> 3b. Setup scenarios as part of a devops pipeline. <br>  4. Run the scenarios <br> 5. Install and configure the Power BI app on telemetry.  Look at the results in Performance report. <br> 5. Consider setting up alerts on regressions. | 
| Project manager | Before go-live, ensure that the system scales with the expected number of users and what they are supposed to do in the system. | 1. Together with the project sponsor on the customer side, identify the key scenarios that must be tested <br> 2. Copy the production environment to a sandbox. <br> 3. Setup telemetry on the environment (to get clean data, setup a new Application Insights ressource for this test only) <br> 4a. Install and configure the BCPT AL extension with the scenarios you need and in the expected use pattern. <br> 4b. Setup scenarios as part of a devops pipeline (maybe work on automating parts of this process for easier repeatability). <br> 5. Run the scenarios <br> 6. Install and configure the Power BI app on telemetry.  Look at the results in Performance report. <br> 7. Consider setting up alerts on regressions. |
| Project manager | Test how multiple apps work together under pressure | Similar to the go-live scenario, but here you need to make sure that apps are installed in the environment. | 
| Supporter | Analyze performance issues due to locking or deadlocks | Similar to the developer scenario for concurrency testing. If you are lucky, scenario codeunits are already available on GitHub. |




# What Power Point presentations are available?
To make it easier to adopt BCPT, we added a number of Power Point presentations that you can use for various situations.

| Your role is... | You want to... | Use this deck (click and then download) |
| --------------- | ---------------| ------------- |
| as a trainer | Help partners/customers/colleagues get started with Performance Analysis (maybe do a lunch session or present at a conference) | [Troubleshooting performance in Business Central Online.pptx](<./presentations/decks/2023-directions-NA-Troubleshooting performance in Business Central Online.pptx>) |


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.