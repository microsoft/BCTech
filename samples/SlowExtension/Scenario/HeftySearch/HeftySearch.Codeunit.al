// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50102 "Hefty Search" implements "Slow Code Example"
{
    Access = Internal;

    procedure RunSlowCode()
    begin
        FindCustomerExtensions();
    end;

    procedure GetHint(): Text
    begin
        exit('Try checking long running queries in telemetry and the ''Database Missing Indexes'' page');
    end;

    procedure IsBackground(): Boolean
    begin
        exit(false);
    end;

    local procedure FindCustomerExtensions()
    var
        AllObjWithCaption: Record AllObjWithCaption;
        NAVAppInstalledApp: Record "NAV App Installed App";
        ExtensionsWithCustomerObject: Text;
    begin
        SelectLatestVersion();
        AllObjWithCaption.SetFilter("Object Caption", '*%1*', CreateGuid());
        if AllObjWithCaption.FindSet() then
            repeat
                if NAVAppInstalledApp.Get(AllObjWithCaption."App Package ID") then
                    ExtensionsWithCustomerObject += NAVAppInstalledApp.Name + ' | ';
            until AllObjWithCaption.Next() = 0;
        ExtensionsWithCustomerObject := ExtensionsWithCustomerObject.TrimEnd(' | ');
    end;
}