// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides functions for managing sample tables (e. g. Milk and Cereal)
/// </summary>
codeunit 50106 "Food Management"
{
    Access = Internal;

    procedure SetupFood()
    var
        Milk: Record Milk;
        Cereal: Record Cereal;
    begin
        Milk.DeleteAll();
        Cereal.DeleteAll();

        Milk.Insert();
        Cereal.Insert();

        Commit();
    end;
}