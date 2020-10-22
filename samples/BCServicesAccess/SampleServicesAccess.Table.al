table 50100 "Sample Services Access"
{
    DataClassification = ToBeClassified;

    fields
    {
        field(1; ID; Integer)
        {
            DataClassification = ToBeClassified;

        }
        field(2; "Scope"; Text[2048])
        {
            DataClassification = ToBeClassified;

        }
        field(3; "Status"; Text[50])
        {
            DataClassification = ToBeClassified;

        }
        field(4; "Access Token"; Blob)
        {
            DataClassification = ToBeClassified;

        }
        field(5; "Error"; Text[2048])
        {
            DataClassification = ToBeClassified;

        }
    }

    keys
    {
        key(PK; ID)
        {
            Clustered = true;
        }
    }

    var
        myInt: Integer;

    trigger OnInsert()
    begin

    end;

    trigger OnModify()
    begin

    end;

    trigger OnDelete()
    begin

    end;

    trigger OnRename()
    begin

    end;

    procedure SetResult(AccessToken: Text; ErrorMessage: Text)
    var
        OutStream: OutStream;
    begin
        Clear("Access Token");

        "Access Token".CreateOutStream(OutStream, TEXTENCODING::UTF8);
        OutStream.WriteText(AccessToken);
        Error := ErrorMessage;

        SetStatus();
    end;

    procedure GetResult() Result: Text
    var
        InStream: InStream;
    begin
        if "Access Token".HasValue() then begin
            CalcFields("Access Token");
            "Access Token".CreateInStream(InStream, TEXTENCODING::UTF8);
            InStream.ReadText(Result);
            exit;
        end;

        exit(StrSubstNo('%1 %2', Status, Error));
    end;

    local procedure SetStatus()
    begin
        if Error <> '' then
            Status := 'Error'
        else
            if "Access Token".HasValue() then
                Status := 'Token acquired'
            else
                Status := 'Token not acquired';
    end;

}