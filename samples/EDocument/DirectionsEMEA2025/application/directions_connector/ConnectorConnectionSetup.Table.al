// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

/// <summary>
/// Stores connection settings for Connector API.
/// This is a singleton table that holds the API URL and authentication key.
/// </summary>
table 50122 "Connector Connection Setup"
{
    Caption = 'Connector Connection Setup';
    DataClassification = CustomerContent;

    fields
    {
        field(1; "Primary Key"; Code[10])
        {
            Caption = 'Primary Key';
            DataClassification = SystemMetadata;
        }
        field(10; "API Base URL"; Text[250])
        {
            Caption = 'API Base URL';
            DataClassification = CustomerContent;

            trigger OnValidate()
            begin
                if "API Base URL" <> '' then
                    if not "API Base URL".EndsWith('/') then
                        "API Base URL" := "API Base URL" + '/';
            end;
        }
        field(11; "API Key"; Guid)
        {
            Caption = 'API Key';
            DataClassification = EndUserIdentifiableInformation;
        }
        field(20; "User Name"; Text[100])
        {
            Caption = 'User Name';
            DataClassification = EndUserIdentifiableInformation;
        }
        field(21; Registered; Boolean)
        {
            Caption = 'Registered';
            DataClassification = CustomerContent;
            Editable = false;
        }
    }

    keys
    {
        key(PK; "Primary Key")
        {
            Clustered = true;
        }
    }

    /// <summary>
    /// Gets or creates the singleton setup record.
    /// </summary>
    procedure GetOrCreate(): Boolean
    begin
        if not Get() then begin
            Init();
            "Primary Key" := '';
            Insert();
        end;
        exit(true);
    end;

    /// <summary>
    /// Sets the API key from text value.
    /// </summary>
    procedure SetAPIKey(NewAPIKey: Text)
    var
        APIKeyGuid: Guid;
    begin
        if Evaluate(APIKeyGuid, NewAPIKey) then
            "API Key" := APIKeyGuid
        else
            Error('Invalid API Key format. Must be a valid GUID.');
    end;

    /// <summary>
    /// Gets the API key as text.
    /// </summary>
    procedure GetAPIKeyText(): Text
    begin
        exit(Format("API Key"));
    end;
}
