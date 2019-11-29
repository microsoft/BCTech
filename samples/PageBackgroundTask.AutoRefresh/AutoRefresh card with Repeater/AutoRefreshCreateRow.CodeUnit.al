// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50110 "AutoRefresh CreateRow"
{
    trigger OnRun()
    var
        AutoRefresh: Record AutoRefresh;
    begin
        AutoRefresh.Init();
        AutoRefresh.Date := CurrentDateTime();
        AutoRefresh.CreatedBySessionId := SessionId();
        AutoRefresh.Insert();
    end;
}