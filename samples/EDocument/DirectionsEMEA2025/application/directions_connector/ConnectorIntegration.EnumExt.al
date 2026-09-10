// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

using Microsoft.eServices.EDocument.Integration.Interfaces;

/// <summary>
/// Extends the Service Integration V2 enum to add Connector integration.
/// This enum value is used to identify which integration implementation to use.
/// </summary>
enumextension 50124 "Connector Integration" extends "Service Integration"
{
    value(50124; "Connector")
    {
        Caption = 'Connector';
        Implementation = IDocumentSender = "Connector Integration",
                        IDocumentReceiver = "Connector Integration";
    }

}
