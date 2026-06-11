// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

using Microsoft.DataMigration;

enumextension 57500 "RL Migration Provider" extends "Custom Migration Provider"
{
    value(57500; "Record Link Migration")
    {
        Caption = 'Record Link Migration';
        Implementation = "Custom Migration Provider" = "RL Migration Provider";
    }
}
