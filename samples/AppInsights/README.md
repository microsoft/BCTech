# Introduction

Business Central in the cloud continuously emits telemetry about events that happen in the service.

This telemetry can be useful for partners, e.g., when troubleshooting an issue or to determine how often a feature is used.

As a developer of an app (typically referred to as an **ISV**), which gets installed in a Business Central environment, or as the partner on record for a customer (typically referred to as a **VAR**), you can obtain some of this telemetry.

This repo contains instructions for how you can obtain the telemetry.
It also contains resources that help you get immediate value from the telemetry.


# How do I start?
Business Central can send telemetry to one or more **Azure Application Insights** (AppInsights) accounts.
The first step thus is for you to create an AppInsights account.
See [HERE](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tenant-admin-center-telemetry) for instructions on how to do that.

Once you have created the AppInsights account, make a note of the *instrumentation key*.

The next step depends on whether you are an ISV or a VAR.

**Still in private preview:** If you are an **ISV**, you must specify the instrumentation key in your app.json file. Once you install your app in a Business Central environment, telemetry relating to your app will start to flow into your AppInsights account.

If you are a **VAR**, you must enter the instrumentation key in the Business Central Admin Center of your customer(s). Once you have done that, telemetry relating to your customers will start to flow into your AppInsights account. You can also set the instrumentation key using the Business Central Administration Center API.


# Resources (use CTRL + click to open in a new browser tab/page)
* [Business Central Developer and IT-pro documentation - Monitoring and Analyzing Telemetry](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview)
* [Business Central Administration Center API - How to set the telemetry key](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/administration-center-api#put-appinsights-key)

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
