// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50192 GetBlobName
{
    PageType = StandardDialog;
    Caption = 'New Blob Name';

    layout
    {
        area(Content)
        {
            field(BlobName; BlobName)
            {
                ApplicationArea = All;
                Caption = 'New Name';
            }
        }
    }

    var
        BlobName: Text;

    procedure GetBlobName(): Text;
    begin
        exit(BlobName);
    end;
}
