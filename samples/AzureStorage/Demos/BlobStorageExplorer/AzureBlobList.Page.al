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

            action("Download Monochrome")
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                Image = Picture;
                Enabled = "Content-Type" = 'image/jpeg';

                trigger OnAction()
                var
                    AzureBlobStorage: codeunit AzureBlobStorage;
                    TempBlob: codeunit "Temp Blob";
                    ins: InStream;
                    outs: OutStream;
                    name: Text;
                begin
                    TempBlob.CreateInStream(ins);
                    AzureBlobStorage.GetBlob(Rec.GetBlobPath(), ins);
                    TempBlob.CreateOutStream(outs);
                    CopyStream(Outs, Ins);
                    MakeMonochrome(TempBlob);
                    TempBlob.CreateInStream(ins);

                    name := Rec.Name;
                    DownloadFromStream(ins, '', '', '', name);
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

    // 
    // Send the Jpg Image in the Temp to a AzureFunction as a Base64 encoded
    // string. The AzureFunction returns a monochrome version of the image
    // as a Base64 encoded string that is put back into the TempBlob.
    // 
    //                 MakeMonochrome(TempBlob);
    //
    local procedure MakeMonochrome(TempBlob: codeunit "Temp Blob");
    var
        Base64Convert: codeunit "Base64 Convert";
        ImageAsString: Text;
        Content: HttpContent;
        Client: HttpClient;
        Response: HttpResponseMessage;
        Ins: InStream;
        Outs: OutStream;
        len: Integer;
    begin
        len := TempBlob.Length();
        TempBlob.CreateInStream(Ins);
        ImageAsString := Base64Convert.ToBase64(Ins);
        Content.WriteFrom(ImageAsString);
        Client.Post(
            'https://imagehelperv62019.azurewebsites.net/api/MakeMonochrome?code=aMmhBXEgbYsTFI0Br9vqzpZlR/iZlBDneaG/BL8V81JbY2qikoBhng==',
            Content,
            Response);
        if not Response.IsSuccessStatusCode() then
            Error('MakeMonochrome: ' + Response.ReasonPhrase());

        Response.Content().ReadAs(ImageAsString);
        TempBlob.CreateOutStream(Outs);
        Base64Convert.FromBase64(ImageAsString, Outs);
    end;

}