// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration.Tests;

using Microsoft.DataMigration;
using MSFT.DataMigration;

xmlport 57550 "RL Migration Buffer Import"
{
    Caption = 'RL Migration Buffer Import';
    Direction = Import;
    Format = VariableText;
    FieldSeparator = ',';
    TextEncoding = UTF8;
    UseRequestPage = false;

    schema
    {
        textelement(Root)
        {
            tableelement(RLMigrationBuffer; "RL Migration Buffer")
            {
                fieldelement(LinkID; RLMigrationBuffer."Link ID") { }
                textelement(RecordIDText) { }
                fieldelement(URL1; RLMigrationBuffer.URL1) { }
                fieldelement(Description; RLMigrationBuffer.Description) { }
                textelement(TypeText) { }
                textelement(NoteText) { }
                textelement(CreatedText) { }
                fieldelement(UserID; RLMigrationBuffer."User ID") { }
                fieldelement(Company; RLMigrationBuffer.Company) { }
                textelement(NotifyText) { }
                fieldelement(ToUserID; RLMigrationBuffer."To User ID") { }

                trigger OnBeforeInsertRecord()
                var
                    NoteOutStream: OutStream;
                begin
                    if TypeText = 'Note' then
                        RLMigrationBuffer.Type := RLMigrationBuffer.Type::Note
                    else
                        RLMigrationBuffer.Type := RLMigrationBuffer.Type::Link;

                    if NotifyText = 'true' then
                        RLMigrationBuffer.Notify := true
                    else
                        RLMigrationBuffer.Notify := false;

                    if CreatedText <> '' then
                        Evaluate(RLMigrationBuffer.Created, CreatedText);

                    if NoteText <> '' then begin
                        RLMigrationBuffer.Note.CreateOutStream(NoteOutStream);
                        NoteOutStream.WriteText(NoteText);
                    end;
                end;
            }
        }
    }

    requestpage
    {
        layout
        {
        }
    }

    var
        SkipHeader: Boolean;

    procedure SetSkipHeader(Skip: Boolean)
    begin
        SkipHeader := Skip;
    end;
}
