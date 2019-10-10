// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50131 DriveInfo
{
    var
        ServiceBusRelay: codeunit AzureServiceBusRelay;
        FileSystemDriveInfoPluginNameTok: Label '/filesystem/V1.0/driveinfo', Locked = true;
        GetDrivesFuncDefTok: Label '/GetDrives', Locked = true;
        GetAvailableFreeSpaceFuncTok: Label '/GetAvailableFreeSpace?driveName=%1', Locked = true;

    procedure GetDrives(var Drives: List of [Text]);
    var
        ResultText: Text;
        JArray: JsonArray;
        JToken: JsonToken;
        i: Integer;
    begin
        Clear(Drives);

        ServiceBusRelay.Get(FileSystemDriveInfoPluginNameTok + GetDrivesFuncDefTok, ResultText);
        JArray.ReadFrom(ResultText);
        for i := 0 to JArray.Count() - 1 do begin
            JArray.Get(i, JToken);
            Drives.Add(JToken.AsValue().AsText());
        end;
    end;

    procedure GetDrives(var Drives: record Drives temporary)
    var
        ResultText: Text;
        JArray: JsonArray;
        JToken: JsonToken;
        i: Integer;
    begin
        Drives.DeleteAll();

        ServiceBusRelay.Get(FileSystemDriveInfoPluginNameTok + GetDrivesFuncDefTok, ResultText);
        JArray.ReadFrom(ResultText);
        for i := 0 to JArray.Count() - 1 do begin
            JArray.Get(i, JToken);
            Drives.Name := CopyStr(JToken.AsValue().AsText(), 1, MaxStrLen(Drives.Name));
            Drives.Insert();
        end;
    end;

    procedure GetAvailableFreeSpace(DriveName: Text) Result: BigInteger;
    var
        ResultText: Text;
    begin
        ServiceBusRelay.Get(FileSystemDriveInfoPluginNameTok + StrSubstNo(GetAvailableFreeSpaceFuncTok, DriveName), ResultText);
        Evaluate(Result, ResultText, 9);
    end;

    procedure GetRootDirectory(drive: Text; var rootDirectory: codeunit DirectoryInfo)
    begin
        rootDirectory.Initialize(ServiceBusRelay, drive);
    end;

    [EventSubscriber(ObjectType::Table, Database::Drives, 'MyCrazeEvent', '', false, false)]
    procedure MySub(var sender: Record Drives)
    begin
    end;
}

