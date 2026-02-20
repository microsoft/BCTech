namespace SalesValidationAgent.Setup.Metadata;

using System.Agents;

enumextension 50101 "Sales Val. Metadata Provider" extends "Agent Metadata Provider"
{
    value(50101; "Sales Validation Agent")
    {
        Caption = 'Sales Validation Agent';
        Implementation = IAgentFactory = SalesValAgentFactory, IAgentMetadata = SalesValAgentMetadata;
    }
}