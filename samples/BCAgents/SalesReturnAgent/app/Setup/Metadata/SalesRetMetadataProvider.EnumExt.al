// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Metadata;

using System.Agents;

enumextension 53701 "Sales Ret. Metadata Provider" extends "Agent Metadata Provider"
{
    value(53701; "Sales Return Agent")
    {
        Caption = 'Sales Return Agent';
        Implementation = IAgentFactory = SalesRetAgentFactory, IAgentMetadata = SalesRetAgentMetadata;
    }
}
