// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50191 AzureBlobList
{
    PageType = List;
    SourceTable = AzureBlobStorageBlob;
    SourceTableTemporary = true;
    Caption = 'Azure Blob Storage Blob List';
    Editable = false;
    LinksAllowed = false;
    InsertAllowed = false;
    ModifyAllowed = false;
    DeleteAllowed = true;
    DataCaptionFields = Name;
    PromotedActionCategories = 'Manage';

    layout
    {
        area(content)
        {
            repeater(Control1)
            {
                ShowCaption = false;
                field(Name; Name)
                {
                    ApplicationArea = All;
                    Drilldown = true;

                    trigger OnDrillDown()
                    begin

                    end;
                }
                field(LastModified; "Last-Modified")
                {
                    ApplicationArea = All;
                }
                field("Content-Length"; "Content-Length")
                {
                    ApplicationArea = All;
                }
                field("Content-Type"; "Content-Type")
                {
                    ApplicationArea = All;
                }
                field(LeaseStatus; LeaseStatus)
                {
                    ApplicationArea = All;
                }
                field(LeaseState; LeaseState)
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(Upload)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                Image = Import;

                trigger OnAction()
                var
                    AzureBlobStorage: codeunit AzureBlobStorage;
                    ins: InStream;
                    name: Text;
                begin
                    UploadIntoStream('', '', '', name, ins);
                    AzureBlobStorage.PutBlob(Rec.GetFilter(Container) + '/' + name, ins, AzureBlobStorage.GetContentTypeFromFileName(name));
                    Refresh();
                end;
            }
            action(Download)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                Scope = "Repeater";
                Image = ExportFile;

                trigger OnAction()
                var
                    AzureBlobStorage: codeunit AzureBlobStorage;
                    TempBlob: codeunit "Temp Blob";
                    ins: InStream;
                    name: Text;
                begin
                    TempBlob.CreateInStream(ins);
                    AzureBlobStorage.GetBlob(Rec.GetBlobPath(), ins);
                    name := Rec.Name;
                    DownloadFromStream(ins, '', '', '', name);
                end;
            }
            action(Copy)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                Image = Delete;

                trigger OnAction()
                var
                    AzureBlobStorage: codeunit AzureBlobStorage;
                    GetBlobName: page GetBlobName;
                begin
                    if GetBlobName.RunModal() <> Action::OK then
                        exit;

                    AzureBlobStorage.CopyBlob(Rec.GetBlobPath(), GetBlobName.GetBlobName());
                    Refresh();
                end;
            }
            action(Delete)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                Image = Delete;

                trigger OnAction()
                var
                    AzureBlobStorage: codeunit AzureBlobStorage;
                begin
                    AzureBlobStorage.DeleteBlob(Rec.GetBlobPath());
                    Refresh();
                end;
            }
        }
    }

    local procedure Refresh();
    var
        AzureBlobStorage: codeunit AzureBlobStorage;
    begin
        AzureBlobStorage.ListBlobs(Rec.GetFilter(Container), '', Rec);
    end;
}