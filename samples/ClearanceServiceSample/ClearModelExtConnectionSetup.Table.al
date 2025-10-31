// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace BCTech.EServices.EDocumentConnector;

using Microsoft.eServices.EDocument;

table 50104 "ClearModelExtConnectionSetup"
{
    fields
    {
        field(1; PK; Code[10])
        {
            DataClassification = CustomerContent;
        }
        field(5; "FileAPI URL"; Text[250])
        {
            Caption = 'FileAPI URL';
            DataClassification = CustomerContent;
        }
        field(9; "Company Id"; Text[100])
        {
            Caption = 'Company ID';
            DataClassification = CustomerContent;
        }
        field(13; "E-Document Service"; Code[20])
        {
            TableRelation = "E-Document Service";
            Caption = 'E-Document Service';
            DataClassification = CustomerContent;
        }
    }

    keys
    {
        key(Key1; PK)
        {
            Clustered = true;
        }
    }
}