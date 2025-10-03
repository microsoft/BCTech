// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace BCTech.EServices.EDocumentConnector;

using Microsoft.eServices.EDocument.Integration;
using Microsoft.eServices.EDocument.Integration.Interfaces;

enumextension 50102 "Clearance Service Integration" extends "Service Integration"
{
    value(50101; "Clearance Service")
    {
        Implementation = IDocumentSender = "Clearance Service Impl.", IDocumentReceiver = "Clearance Service Impl.";
    }
}