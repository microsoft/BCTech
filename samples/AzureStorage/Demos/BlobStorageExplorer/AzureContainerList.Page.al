// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50190 AzureContainerList
{
    PageType = List;
    SourceTable = AzureBlobStorageContainer;
    SourceTableTemporary = true;
    Caption = 'Azure Blob Storage Container List';
    Editable = false;
    LinksAllowed = false;
    InsertAllowed = false;
    ModifyAllowed = false;
    DeleteAllowed = true;
    DataCaptionFields = Name;
    UsageCategory = Lists;
    ApplicationArea = All;

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
                    DrillDown = true;

                    trigger OnDrillDown()
                    var
                        Blobs: record AzureBlobStorageBlob temporary;
                        AzureBlobStorage: codeunit AzureBlobStorage;
                    begin
                        AzureBlobStorage.ListBlobs('/' + Rec.Name, '', Blobs);
                        Blobs.SetFilter(Container, '/' + Rec.Name);
                        Page.Run(Page::AzureBlobList, Blobs);
                    end;
                }
                field(LastModified; "Last-Modified")
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

    trigger OnOpenPage()
    var
        AzureBlobStorage: codeunit AzureBlobStorage;
    begin
        AzureBlobStorage.ListContainers(Rec);
    end;


    trigger OnDeleteRecord(): Boolean
    var
        AzureBlobStorage: codeunit AzureBlobStorage;
    begin
        if not Confirm('Are you sure you want to delete ''%1''', false, Rec.Name) then
            exit(false);

        AzureBlobStorage.DeleteContainer('/' + Rec.Name);
    end;

}