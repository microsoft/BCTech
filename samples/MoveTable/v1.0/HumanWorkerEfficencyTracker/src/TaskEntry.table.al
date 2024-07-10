table 50102 "Task Entry"
{
    DataClassification = CustomerContent;

    fields
    {
        field(1; "EntryNo."; BigInteger)
        {
            AutoIncrement = true;
            Caption = 'Entry No.';
        }
        field(2; "Employee No."; Code[20])
        {
            TableRelation = Employee."No.";
        }
        field(3; TaskCode; Code[20])
        {
            TableRelation = Task.TaskCode;

            trigger OnValidate()
            var
                Task: Record "Task";
            begin
                Clear(Rec."Start Datetime");
                Clear(Rec."End Datetime");
                Clear(Rec."Expected Duration");
                Clear(Rec."Efficiency Score");
                if Task.Get(Rec.TaskCode) then
                    Validate("Expected Duration", Task."Expected Duration");
            end;
        }
        field(4; "Start Datetime"; DateTime)
        {
            trigger OnValidate()
            begin
                CalculateEfficiencyScore();
            end;
        }
        field(5; "End Datetime"; DateTime)
        {
            trigger OnValidate()
            begin
                CalculateEfficiencyScore();
            end;
        }
        field(6; "Expected Duration"; Duration)
        {
            Editable = false;
        }
        field(7; "Actual Duration"; Duration)
        {
            Editable = false;
        }
        field(8; "Efficiency Score"; Integer)
        {
            Editable = false;
        }
    }

    keys
    {
        key(Key1; "EntryNo.")
        {
            Clustered = true;
        }
    }

    local procedure CalculateEfficiencyScore()
    begin
        if ("Start Datetime" <> 0DT) and ("End Datetime" <> 0DT) and ("Expected Duration" <> 0) and ("Start Datetime" <> "End Datetime") then begin
            "Actual Duration" := "End Datetime" - "Start Datetime";
            "Efficiency Score" := Round("Expected Duration" / "Actual Duration" * 100, 1);
        end else begin
            Clear("Actual Duration");
            Clear("Efficiency Score");
        end;
    end;
}