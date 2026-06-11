// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

using System.Environment.Configuration;

codeunit 57502 "RL Migration Duplicate Finder"
{
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure IsDuplicate(var RLMigrationBuffer: Record "RL Migration Buffer"): Boolean
    var
        RecordLink: Record "Record Link";
    begin
        RecordLink.SetRange(Type, RLMigrationBuffer.Type);
        RecordLink.SetRange(Description, RLMigrationBuffer.Description);
        RecordLink.SetRange(Company, RLMigrationBuffer.Company);

        if Format(RLMigrationBuffer."Record ID") <> '' then
            RecordLink.SetRange("Record ID", RLMigrationBuffer."Record ID");

        if not RecordLink.FindSet() then
            exit(false);

        if RLMigrationBuffer.Type = RLMigrationBuffer.Type::Link then begin
            RecordLink.SetRange(URL1, RLMigrationBuffer.URL1);
            exit(not RecordLink.IsEmpty());
        end;

        repeat
            if CompareNoteContent(RecordLink, RLMigrationBuffer) then
                exit(true);
        until RecordLink.Next() = 0;

        exit(false);
    end;

    procedure IdentifyDuplicates()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationMapping: Record "RL Migration Mapping";
    begin
        if not RLMigrationBuffer.FindSet() then
            exit;

        repeat
            // Skip records already in the mapping table
            RLMigrationMapping.SetRange("Source Link ID", RLMigrationBuffer."Link ID");
            RLMigrationMapping.SetRange(Company, RLMigrationBuffer.Company);
            if RLMigrationMapping.IsEmpty() then
                if IsDuplicate(RLMigrationBuffer) then begin
                    // Mark as duplicate directly in the mapping table
                    RLMigrationMapping.Reset();
                    RLMigrationMapping."Source Link ID" := RLMigrationBuffer."Link ID";
                    RLMigrationMapping."Target Link ID" := FindMatchingLinkId(RLMigrationBuffer);
                    RLMigrationMapping.Company := RLMigrationBuffer.Company;
                    RLMigrationMapping."Record ID" := RLMigrationBuffer."Record ID";
                    RLMigrationMapping.Type := RLMigrationBuffer.Type;
                    RLMigrationMapping.Description := RLMigrationBuffer.Description;
                    RLMigrationMapping."Note Prefix" := GetBufferNotePrefix(RLMigrationBuffer);
                    RLMigrationMapping."Is Duplicate" := true;
                    RLMigrationMapping."Duplicate Action" := RLMigrationMapping."Duplicate Action"::Pending;
                    if not RLMigrationMapping.Insert() then
                        RLMigrationMapping.Modify();
                end;
        until RLMigrationBuffer.Next() = 0;
    end;

    local procedure FindMatchingLinkId(var RLMigrationBuffer: Record "RL Migration Buffer"): Integer
    var
        RecordLink: Record "Record Link";
    begin
        RecordLink.SetRange(Type, RLMigrationBuffer.Type);
        RecordLink.SetRange(Description, RLMigrationBuffer.Description);
        RecordLink.SetRange(Company, RLMigrationBuffer.Company);

        if Format(RLMigrationBuffer."Record ID") <> '' then
            RecordLink.SetRange("Record ID", RLMigrationBuffer."Record ID");

        if RLMigrationBuffer.Type = RLMigrationBuffer.Type::Link then begin
            RecordLink.SetRange(URL1, RLMigrationBuffer.URL1);
            if RecordLink.FindFirst() then
                exit(RecordLink."Link ID");
            exit(0);
        end;

        if not RecordLink.FindSet() then
            exit(0);

        repeat
            if CompareNoteContent(RecordLink, RLMigrationBuffer) then
                exit(RecordLink."Link ID");
        until RecordLink.Next() = 0;

        exit(0);
    end;

    local procedure CompareNoteContent(var RecordLink: Record "Record Link"; var RLMigrationBuffer: Record "RL Migration Buffer"): Boolean
    var
        ExistingNotePrefix: Text[2048];
        BufferNotePrefix: Text[2048];
    begin
        ExistingNotePrefix := GetRecordLinkNotePrefix(RecordLink);
        BufferNotePrefix := GetBufferNotePrefix(RLMigrationBuffer);
        exit(ExistingNotePrefix = BufferNotePrefix);
    end;

    local procedure GetRecordLinkNotePrefix(var RecordLink: Record "Record Link"): Text[2048]
    var
        NoteInStream: InStream;
        NoteText: Text;
    begin
        RecordLink.CalcFields(Note);
        if not RecordLink.Note.HasValue() then
            exit('');

        RecordLink.Note.CreateInStream(NoteInStream);
        NoteInStream.ReadText(NoteText);
        exit(CopyStr(NoteText, 1, 2048));
    end;

    local procedure GetBufferNotePrefix(var RLMigrationBuffer: Record "RL Migration Buffer"): Text[2048]
    var
        NoteInStream: InStream;
        NoteText: Text;
    begin
        RLMigrationBuffer.CalcFields(Note);
        if not RLMigrationBuffer.Note.HasValue() then
            exit('');

        RLMigrationBuffer.Note.CreateInStream(NoteInStream);
        NoteInStream.ReadText(NoteText);
        exit(CopyStr(NoteText, 1, 2048));
    end;
}
