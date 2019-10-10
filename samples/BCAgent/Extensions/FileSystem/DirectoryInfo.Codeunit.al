// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50132 DirectoryInfo
{
    var
        ServiceBusRelay: codeunit AzureServiceBusRelay;
        Path: Text;
        FileSystemDirectoryInfoPluginNameTok: Label '/filesystem/V1.0/directoryinfo', Locked = true;
        GetFilesFuncDefTok: Label '/GetFiles?path=%1&searchPattern=%2', Locked = true;
        GetDirectoriesFuncDefTok: Label '/GetDirectories?path=%1&searchPattern=%2', Locked = true;
        GetDirectoryItemsFuncDefTok: Label '/GetDirectoryItems?path=%1&searchPattern=%2', Locked = true;

    procedure Initialize(ServiceBusRelayValue: codeunit AzureServiceBusRelay; PathValue: Text);
    begin
        ServiceBusRelay := ServiceBusRelayValue;
        Path := PathValue;
    end;

    procedure SetPath(PathValue: Text);
    begin
        Path := PathValue;
    end;

    procedure GetPath(): Text;
    begin
        exit(Path);
    end;

    procedure GetParentPath(): Text;
    var
        idx: Integer;
    begin
        idx := Path.LastIndexOf('\', StrLen(Path) - 1);
        if idx < 1 then
            exit('');

        exit(Path.Substring(1, idx));
    end;

    procedure GetFiles(SearchPattern: Text; var Files: List of [Text]);
    var
        ResultText: Text;
    begin
        ServiceBusRelay.Get(FileSystemDirectoryInfoPluginNameTok + StrSubstNo(GetFilesFuncDefTok, Path, SearchPattern), ResultText);
        JsonArrayToList(ResultText, Files);
    end;

    procedure GetDirectories(SearchPattern: Text; var Items: List of [Text]);
    var
        ResultText: Text;
    begin
        Clear(Items);

        ServiceBusRelay.Get(FileSystemDirectoryInfoPluginNameTok + StrSubstNo(GetDirectoriesFuncDefTok, Path, SearchPattern), ResultText);
        JsonArrayToList(ResultText, Items);
    end;

    procedure GetDirectories(SearchPattern: Text; var Items: record DirectoryItems temporary);
    var
        ResultText: Text;
    begin
        ServiceBusRelay.Get(FileSystemDirectoryInfoPluginNameTok + StrSubstNo(GetDirectoriesFuncDefTok, Path, SearchPattern), ResultText);
        JsonArrayToDirectoryItems(ResultText, Items);
    end;

    procedure GetDirectoryItems(SearchPattern: Text; var Items: record DirectoryItems temporary);
    var
        ResultText: Text;
        JArray: JsonArray;
        JToken: JsonToken;
        JObject: JsonObject;
        i: Integer;
    begin
        ServiceBusRelay.Get(FileSystemDirectoryInfoPluginNameTok + StrSubstNo(GetDirectoryItemsFuncDefTok, Path, SearchPattern), ResultText);

        JArray.ReadFrom(ResultText);
        for i := 0 to JArray.Count() - 1 do begin
            JArray.Get(i, JToken);
            JObject := JToken.AsObject();

            Items.Init();

            JObject.Get('FullName', JToken);
            Items.Name := CopyStr(JToken.AsValue().AsText(), 1, MaxStrLen(Items.Name));

            JObject.Get('Name', JToken);
            Items.DisplayName := CopyStr(JToken.AsValue().AsText(), 1, MaxStrLen(Items.DisplayName));

            JObject.Get('IsDirectory', JToken);
            Items.IsDirectory := JToken.AsValue().AsBoolean();
            if Items.IsDirectory then
                Items.DisplayName := CopyStr('[' + Items.DisplayName + ']', 1, MaxStrLen(Items.DisplayName));

            JObject.Get('Created', JToken);
            Items.Created := JToken.AsValue().AsDateTime();

            Items.Insert();
        end;
    end;

    local procedure JsonArrayToList(Value: Text; var List: List of [Text]);
    var
        JArray: JsonArray;
        JToken: JsonToken;
        i: Integer;
    begin
        JArray.ReadFrom(Value);
        for i := 0 to JArray.Count() - 1 do begin
            JArray.Get(i, JToken);
            List.Add(JToken.AsValue().AsText());
        end;
    end;

    local procedure JsonArrayToDirectoryItems(Value: Text; var Items: record DirectoryItems temporary);
    var
        JArray: JsonArray;
        JToken: JsonToken;
        i: Integer;
    begin
        JArray.ReadFrom(Value);
        for i := 0 to JArray.Count() - 1 do begin
            JArray.Get(i, JToken);

            Items.Init();
            Items.Name := CopyStr(JToken.AsValue().AsText(), 1, MaxStrLen(Items.Name));
            Items.DisplayName := Items.Name;
            Items.Insert();
        end;
    end;
}