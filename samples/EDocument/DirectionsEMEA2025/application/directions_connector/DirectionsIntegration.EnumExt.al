// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

using Microsoft.eServices.EDocument.Integration.Interfaces;

/// <summary>
/// Extends the Service Integration V2 enum to add DirectionsConnector integration.
/// This enum value is used to identify which integration implementation to use.
/// </summary>
enumextension 81100 "Directions Integration" extends "Service Integration V2"
{
    value(81100; "Directions Connector")
    {
        Caption = 'Directions Connector';
        Implementation = IDocumentSender = "Directions Integration Impl.",
                        IDocumentReceiver = "Directions Integration Impl.";
    }
}
