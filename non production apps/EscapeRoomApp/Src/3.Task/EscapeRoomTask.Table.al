table 73922 "Escape Room Task"
{
    DataClassification = CustomerContent;
    Caption = 'Escape Room Task';

    fields
    {
        field(1; "Venue Id"; Text[50])
        {
            Caption = 'Venue Id';
            DataClassification = CustomerContent;
            TableRelation = "Escape Room Venue";
        }
        field(2; "Room Name"; Text[100])
        {
            Caption = 'Code';
            DataClassification = CustomerContent;
        }
        field(3; Name; Text[100])
        {
            Caption = 'Name';
            DataClassification = CustomerContent;
        }
        field(4; Description; Text[250])
        {
            Caption = 'Description';
            DataClassification = CustomerContent;
        }
        field(5; Sequence; Integer)
        {
            Caption = 'Sequence';
            DataClassification = CustomerContent;
        }
        field(6; Status; Enum "Escape Room Task Status")
        {
            Caption = 'Status';
            DataClassification = CustomerContent;
        }
        field(7; "Task"; enum "Escape Room Task")
        {
            Caption = 'Task';
            DataClassification = CustomerContent;
        }
        field(8; "TestCodeunitId"; Integer)
        {
            Caption = 'TestCodeunitId';
            DataClassification = CustomerContent;
        }
        field(9; "Stop DateTime"; DateTime)
        {
            Caption = 'Stop DateTime';
            DataClassification = CustomerContent;
        }
        field(10; "Hint"; Text[2048])
        {
            Caption = 'Hint';
            DataClassification = CustomerContent;
        }
        field(11; "Hint DateTime"; DateTime)
        {
            Caption = 'Hint DateTime';
            DataClassification = CustomerContent;
        }
    }

    keys
    {
        key(PK; "Venue Id", "Room name", "Name")
        {
            Clustered = true;
        }
        key(Sequence; Sequence)
        {
        }
        key(Hint; Hint, "Hint DateTime")
        {
        }
    }

    procedure SetStatusCompleted()
    var
        Room: Record "Escape Room";
    begin
        if Rec.Find('=') then begin
            Rec.Stop();

            Room.Get(Rec."Venue Id", Rec."Room Name");
            Room.CloseRoomIfCompleted();
        end
    end;

    procedure UpdateStatus()
    var
        Task: Interface iEscapeRoomTask;
    begin
        if Rec.Status = Rec.Status::Completed then exit;

        Task := Rec.Task;

        if Task.IsValid() then begin
            Rec.Stop();
        end
    end;

    procedure Stop()
    var
        EscapeRoomNotifications: Codeunit EscapeRoomNotifications;
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
    begin
        if Rec.Status = Rec.Status::Open then begin
            Rec.Status := Rec.Status::Completed;
            Rec."Stop DateTime" := CurrentDateTime();
            Rec.Modify();
            Commit();

            EscapeRoomNotifications.TaskFinished(Rec);
            EscapeRoomTelemetry.LogFinishedTask(Rec);
        end
    end;
}