// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Format;

using Microsoft.eServices.EDocument;

/// <summary>
/// Extends the E-Document Format enum to add SimpleJson format.
/// This enum value is used to identify which format implementation to use.
/// </summary>
enumextension 81000 "SimpleJson Format" extends "E-Document Format"
{
    value(81000; "SimpleJson")
    {
        Caption = 'Simple JSON Format';
        Implementation = "E-Document" = "SimpleJson Format Impl.";
    }
}
