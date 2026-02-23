// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

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