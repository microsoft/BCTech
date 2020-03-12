# Introduction

Business Central in the cloud continuously emits telemetry about events that happen in the service.

This telemetry can be useful for partners, e.g., when troubleshooting an issue or to determine how often a feature is used.

As a developer of an app (typically referred to as an **ISV**), which gets installed in a Business Central environment, or as the partner on record for a customer (typically referred to as a **VAR**), you can obtain some of this telemetry.

This repo contains instructions for how you can obtain the telemetry.
It also contains resources that help you get immediate value from the telemetry.


# Obtain the telemetry in your AppInsights account

Business Central can send telemetry to one or more **Azure Application Insights** (AppInsights) accounts.
The first step thus is for you to create an AppInsights account.
See [HERE](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tenant-admin-center-telemetry) for instructions on how to do that.
Once you have created the AppInsights account, make a note of the *instrumentation key*.

The next step depends on whether you are an ISV or a VAR.

**Still in private preview:** If you are an **ISV**, you must specify the instrumentation key in your app.json file. Once you install your app in a Business Central environment, telemetry relating to your app will start to flow into your AppInsights account.

If you are a **VAR**, you must enter the instrumentation key in the Business Central Admin Center of your customer(s). Once you have done that, telemetry relating to your customers will start to flow into your AppInsights account.

# Frequently Asked Questions (FAQ)
## What is the data retention policy in AppInsights?
The default retention for Application Insights resources is 90 days. Different retention periods can be selected for each Application Insights resource. The full set of available retention periods is 30, 60, 90, 120, 180, 270, 365, 550 or 730 days.

See <https://docs.microsoft.com/en-us/azure/azure-monitor/app/pricing#change-the-data-retention-period> 

## How do I delete data from AppInsights?
Purge data in an Application Insights component by a set of user-defined filters.

See <https://docs.microsoft.com/en-us/rest/api/application-insights/components/purge#examples> 

## Can I grant read-only access to AppInsights?
To grant a person read-only access to AppInsights, go to the Access control (IAM) page in the AppInsights portal, and then add the role assignment "Reader" to the person. 

You might also need to add the role assignment "Reader" to the person on the Resource Group for the AppInsights subscription.


# Resources
* [Business Central Developer and IT-pro documentation - Monitoring and Analyzing Telemetry](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview)
