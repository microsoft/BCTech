// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

pageextension 50132 MyFavoriteTestBed extends "Customer List"
{
    trigger OnOpenPage();
    var
        ReturnOfTheComplexType: codeunit ReturnOfTheComplexType;
    begin
        ReturnOfTheComplexType.Test();
    end;
}

