# Business Central Local Agent
or "How to connect to local resources from the cloud."

Business Central Local Agent is a sample/prototype framework for accessing local resources from the cloud. The framework uses a Hybrid Connection from 
[Azure Service Bus Relay](https://learn.microsoft.com/en-us/azure/service-bus-relay/) to allow Business Central online to connect to a local resource. The local resource can be attached hardware, drives, network, or anything accessible from .NET code. 


## Framework
The concept consists of two framework parts; a local executable (command line or service) that listens to incoming requests and dispatches them to plugins, and a Business Central AL extension that encapsulates the configuration and implementation details of the Azure Service Bus Relay communication. 
The framework handles multiple different plugin/extension pairs on the same endpoint.

![Business Central Local Agent Diagram](images/BCAgentDiagram.png)

The framework is designed to be pluggable and can serve many different plugins at the same time.

## Plugins 
A **Plugin** is a .NET assembly containing a class attributed with metadata for the framework to load. The code sample below illustrates how to create a plugin for calculation:

```C#
    namespace CalculatorPlugin
    {
        using Microsoft.Dynamics.BusinessCentral.Agent.Common;

        [AgentPlugin("calculator/V1.0")]
        public class Calculator : IAgentPlugin
        {
            [PluginMethod("GET")]
            public decimal Add(decimal a, decimal b)
            {
                return a + b;
            }

            [PluginMethod("GET")]
            public decimal Subtract(decimal a, decimal b)
            {
                return a - b;
            }
        }
    }
```

The plugin class must implement the `IAgentPlugin` interface and be attributed with the `AgentPlugin` attribute. The path in the attribute is the routing information used to route requests to this plugin. It is appended to endpoint of the Hybrid Connection (see how to create the Hybrid Connection below).

## Extensions
A plugin will normally be accompanied by a corresponding AL extension. The extension will contain one or more wrapper codeunits that make the plugin method easy to consume from AL. It doesn't have to be implemented in a separate extension, but could also be a part of a larger consuming extension. 
The wrapper codeunit the corresponds to the CalculatorPlugin looks like:

```AL
codeunit 50130 Calculator
{
    var
        ServiceBusRelay: codeunit ServiceBusRelay;
        CalculatorPluginName: Label '/calculator/V1.0', Locked = true;
        AddFuncDef: Label '/Add?a=%1&b=%2', Locked = true;
        SubtractFuncDef: Label '/Subtract?a=%1&b=%2', Locked = true;
    
    procedure Add(a: Decimal; b: Decimal) Result: Decimal;
    var
        ResultText: Text;
    begin
        ServiceBusRelay.Get(
            StrSubstNo(AddFuncDef, Format(a, 0, 9), Format(b, 0, 9)),    
            ResultText);
        Evaluate(Result, ResultText, 9);
    end;
    
    procedure Subtract(a: Decimal; b: Decimal) Result: Decimal;
    var
        ResultText: Text;
    begin
        ServiceBusRelay.Get(
            StrSubstNo(SubtractFuncDef, Format(a, 0, 9), Format(b, 0, 9)), 
            ResultText);
        Evaluate(Result, ResultText, 9);
    end;
}

```

## Running the samples
The are a few prerequisites to run the samples:

1. Create an Azure Service Bus Relay Hybrid Connection. 
2. Compile the BCAgent project. 
3. Run the BCAgent command line.
4. Deploy the AzureServiceBusLibrary extension.
5. Setup the AzureServiceBusLibrary extension.
6. Deploy one or more of the BCAgent samples. 


### 1. Create a Hybrid Connection in Azure Portal
Follow the steps in the documentation below to create a Relay namespace and a Hybrid Connection, see [Get started with Relay Hybrid Connections HTTP requests in .NET](https://learn.microsoft.com/en-us/azure/service-bus-relay/relay-hybrid-connections-http-requests-dotnet-get-started).

I prefer to create a separate Resource Group for samples or maybe even for individual samples. It makes it easier to keep track of related resources that should be deleted/cleaned up together.

The getting started document/samples for Hybrid Connections uses only the RootManageSharedKey. You should create a separate "Sender" and "Listener" for the Hybrid Connection under "Shared Access Policies".

### 2. Compile the BCAgent project
Open the BCAgent source, which can be found here [https://github.com/microsoft/BCTech/tree/master/samples/BCAgent](https://github.com/microsoft/BCTech/tree/master/samples/BCAgent). 

Open and build the BCAgent.sln solution using Visual Studio 2019. 
The command line version of BCAgent is build with .NET Core, but can easily be changed. You may want to compile it with .NET Framework if you are integrating with older 3rd party libraries.

### 3. Run the BCAgent command line
The .NET core version can be started with the following command line.

```
dotnet BCAgent.dll
     -namespace:<yournamespace>.servicebus.windows.net 
     -connectionname:<hybrid connection name> 
     -keyname:<key - with listen claim> 
     -key:<key>
```
The BCAgent with open up a terminal with a log. It logs load diagnostics and request activity.

### 4. Deploy the AzureServiceBusLibrary extension
Open [Azure Service Bus Library extension](https://https://github.com/microsoft/BCTech/tree/master/samples/AzureServiceBus/AzureServiceBusLibrary) with Visual Studio Code and the AL Language extension. Publish the extension to Business Central online or to a local server.

### 5. Set up the AzureServiceBusLibrary extension
The AzureServiceBusLibrary integrates into the Business Central Service Connections setup. 

Type **Alt+Q** to go to the "Tell Me" dialog. Type "Service Connections" and select the Service Connections List. 

The list new contains an entry for "Azure Service Bus Relay Setup". Select it to open the configuration page for Azure Service Bus Relay.

![Business Central Local Agent Diagram](images/AzureServiceBusRelaySetup.png)

Enter the **Azure Relay Namespace**, **Hybrid Connection Name**, **Shared Access Policy Name**, and **SharedAccessKey** into the dialog.  

### 6. Deploy one or more of the BCAgent samples
Open the AgentExtensions.codeworkspace in the extensions folder. The workspace contains the sample extensions matching the plugins. Compile, publish, and enjoy!

## Creating your own plugins and extensions
The BCAgent is designed to be extensible. It should be easy to create new plugins and extensions to access on-premise resources in a secure way.

We hope you find it useful. It is free to use, but remember this is sample code. Please consider contributing if you fix errors or improve the code base.  
