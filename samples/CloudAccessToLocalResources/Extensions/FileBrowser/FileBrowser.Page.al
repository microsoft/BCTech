// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50150 FileBrowser
{
    PageType = Worksheet;
    SourceTable = DirectoryItems;
    SourceTableTemporary = true;
    Editable = false;
    InsertAllowed = false;
    LinksAllowed = false;
    ModifyAllowed = false;
    DeleteAllowed = false;
    DataCaptionExpression = CurrentDirectory;
    SourceTableView = sorting(DisplayName) order(ascending);
    ApplicationArea = All;
    UsageCategory = Tasks;
    Caption = 'File Browser';

    layout
    {
        area(Content)
        {
            field(CurrentDriveField; CurrentDrive)
            {
                ApplicationArea = All;
                Lookup = true;
                Caption = 'Drive';

                trigger OnLookup(var Text: Text): Boolean;
                var
                    Drives: record Drives temporary;
                begin
                    DriveInfo.GetDrives(Drives);

                    if Page.RunModal(Page::DriveLookupList, Drives) = Action::LookupOK then begin
                        Text := Drives.Name;
                        exit(true);
                    end;

                    exit(false);
                end;
            }
            field(CurrentDirectoryField; CurrentDirectory)
            {
                ApplicationArea = All;
                Caption = 'Directory';
            }
            repeater(DirectoryItemsList)
            {
                field(DisplayName; DisplayName)
                {
                    ApplicationArea = All;

                    DrillDown = true;

                    trigger OnDrillDown()
                    begin
                        if Rec.IsDirectory then begin
                            DirectoryInfo.SetPath(Rec.Name);
                            RefreshDirectoryItems();
                        end;
                    end;
                }
                field(Created; Created)
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    var
        DriveInfo: codeunit DriveInfo;
        DirectoryInfo: codeunit DirectoryInfo;
        CurrentDrive: Text;
        CurrentDirectory: Text;

    trigger OnOpenPage()
    begin
        Initialize();
        RefreshDirectoryItems();
    end;

    local procedure Initialize();
    var
        List: List of [Text];
    begin
        DriveInfo.GetDrives(List);
        if List.Count() > 0 then begin
            List.Get(1, CurrentDrive);
            DriveInfo.GetRootDirectory(CurrentDrive, DirectoryInfo);
        end;
    end;

    local procedure RefreshDirectoryItems();
    var
        ParentPath: Text;
    begin
        Rec.DeleteAll();
        CurrentDirectory := DirectoryInfo.GetPath();
        ParentPath := DirectoryInfo.GetParentPath();
        if ParentPath <> '' then begin
            Rec.Init();
            Rec.Name := CopyStr(ParentPath, 1, MaxStrLen(Rec.Name));
            Rec.DisplayName := '[..]';
            Rec.IsDirectory := true;
            Rec.Insert();
        end;
        DirectoryInfo.GetDirectoryItems('', Rec);
    end;
}
