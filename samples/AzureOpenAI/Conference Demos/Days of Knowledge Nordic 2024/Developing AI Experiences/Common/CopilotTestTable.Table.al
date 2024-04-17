// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 50100 "Copilot Test Table"
{

    TableType = Temporary;
    Extensible = false;

    DataClassification = ToBeClassified;

    fields
    {
        field(1; Id; Integer)
        {
            DataClassification = ToBeClassified;
        }

        field(2; Input; Text[2048])
        {
            DataClassification = ToBeClassified;
        }

        field(3; Output; Text[2048])
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(PK; Id)
        {
        }
    }

    procedure AddEntry(InputText: Text[2048]; OutputText: Text)
    var
        myInt: Integer;
    begin
        if Rec.Output = '' then begin
            Rec.Output := OutputText;
            if Rec.Modify() then
                exit;
        end;

        Rec.Init();
        Rec.Id := Rec.Count + 1;
        Rec.Input := InputText;
        Rec.Output := CopyStr(OutputText, 1, MaxStrLen(Rec.Output));
        Rec.Insert();
    end;
}