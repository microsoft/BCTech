// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50102 "Hefty Search"
{
    Access = Internal;

    procedure FindCustomerExtensions()
    var
        AllObjWithCaption: Record AllObjWithCaption;
        NAVAppInstalledApp: Record "NAV App Installed App";
        ExtensionsWithCustomerObject: Text;
    begin
        AllObjWithCaption.SetFilter("Object Caption", '*%1*', 'Customer');
        if AllObjWithCaption.FindSet() then
            repeat
                if NAVAppInstalledApp.Get(AllObjWithCaption."App Package ID") then
                    ExtensionsWithCustomerObject += NAVAppInstalledApp.Name + ' | ';
            until AllObjWithCaption.Next() = 0;
        ExtensionsWithCustomerObject := ExtensionsWithCustomerObject.TrimEnd(' | ');
    end;
}