// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

table 50172 AzureBlobStorageBlob
{

    fields
    {
        field(1; Name; Text[512])
        {
        }
        field(2; "Last-Modified"; DateTime)
        {
            Caption = 'Last Modified';
        }
        field(3; Etag; Text[20])
        {
        }
        field(4; LeaseStatus; enum BlobLeaseStatus)
        {
            Caption = 'Lease State';
        }
        field(5; LeaseState; enum BlobLeaseState)
        {
            Caption = 'Lease Status';
        }
        field(6; "Content-Length"; BigInteger)
        {
            Caption = 'Size';
        }
        field(7; "Content-Type"; Text[50])
        {
            Caption = 'Content Type';
        }
        field(8; "Content-Encoding"; Text[50])
        {
            Caption = 'Encoding';
        }
        field(9; Container; Text[512])
        {
        }
    }

    procedure GetBlobPath(): Text;
    begin
        exit(Container + '/' + Name);
    end;
}