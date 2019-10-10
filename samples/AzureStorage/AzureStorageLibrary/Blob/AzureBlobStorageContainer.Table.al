// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

table 50171 AzureBlobStorageContainer
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
    }
}