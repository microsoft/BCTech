// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MS.DataMigration.BC14.Examples;

permissionset 50200 "BC14 Item Cat. Migr."
{
    Assignable = true;
    Caption = 'BC14 Item Category Migration';

    Permissions =
        table "BC14 Item Category" = X,
        tabledata "BC14 Item Category" = RIMD;
}
