# Introduction

Business Central in the cloud continuously emits telemetry about events that happen in the service.

This telemetry can be useful for partners, e.g., when troubleshooting an issue or to determine how often a feature is used.

As a developer of an app (typically referred to as an **ISV**), which gets installed in a Business Central environment, or as the partner on record for a customer (typically referred to as a **VAR**), you can obtain some of this telemetry.

This repo contains instructions for how you can obtain the telemetry.
It also contains resources that help you get immediate value from the telemetry.


# Obtain the telemetry in your AppInsights account

Business Central can send telemetry to one or more **Azure Application Insights** (AppInsights) accounts.
The first step thus is for you to create an AppInsights account.
See HERE for instructions on how to do that.
Once you have created the AppInsights account, make a note of the *instrumentation key*.

The next step depends on whether you are an ISV or a VAR.

If you are an **ISV**, you must specify the instrumentation key in your app.json file. Once you install your app in a Business Central environment, telemetry relating to yoru app will start to flow into your AppInsights account.

If you are a **VAR**, you must enter the instrumentation key in the Business Central Admin Center of your customer(s). Once you have done that, telemetry relating to your customers will start to flow into your AppInsights account.


# Get alerts when something is not right

If something happens in your app or for one of your customers that you need to take action on, it is better that the system sends you an alert.

App Insights makes it easy to define such alerts.

Here is an example to get you started:
 1. Open the Azure portal and locate your AppInsights account
 2. Click "Alerts" in the navigation pane on the left
 3. Set the condition to a custom log search for "dependencies | where resultCode >= 500"
 4. Create a new action group, add an action to send an email to you
Now you will get an email whenever your app or your customers make HTTP calls that fail.


# Template dashboards

To reduce the time-to-value for you, we have prepared a set of dashboards that you can deploy to your Azure portal.

Clicking the Deploy To Azure button below will launch the Azure Portal with an ARM template, where you need to specify the subscription, resource group and name of 
your Application Insights Resource. All requested dashboards will be installed and you can now remove the ones you do not need.

<a href="https://freddyk.azurewebsites.net/api/AzureDeploy" target="_blank"><img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.png"/></a>

Each dashboard is a JSON file, that describes which *widgets* the dashboard should contain.


# Clone the repo

We know that the dashboards we have provided might not match your needs exactly, and if you want to customize them, we recommend that you clone this repo and make your adjustments there, before importing the dashboard in the Azure portal.

Adding dashboards is done by exporting a dashboard from the Azure Portal, running the ConvertExportedDashboardToDashboardTemplate.ps1 and then adding the new template to the resources section in azuredeploy.json.

As we improve our dashboards, you can merge the changes into your cloned repo and in this way stay up-to-date.
