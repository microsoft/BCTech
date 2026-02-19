table 73920 "Escape Room"
{
    DataClassification = CustomerContent;
    Caption = 'Escape Room';

    fields
    {
        field(1; "Venue Id"; Text[50])
        {
            Caption = 'Venue Id';
            DataClassification = CustomerContent;
            TableRelation = "Escape Room Venue";
        }
        field(2; "Name"; Text[100])
        {
            Caption = 'Code';
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
        field(6; Status; Enum "Escape Room Status")
        {
            Caption = 'Status';
            DataClassification = CustomerContent;
        }
        field(7; Room; enum "Escape Room")
        {
            Caption = 'Room';
            DataClassification = CustomerContent;
        }
        field(8; "Big Description"; blob)
        {
            Caption = 'Big Description';
            DataClassification = CustomerContent;
        }
        field(9; "Start DateTime"; DateTime)
        {
            Caption = 'Start DateTime';
            DataClassification = CustomerContent;
        }
        field(10; "Stop DateTime"; DateTime)
        {
            Caption = 'Stop DateTime';
            DataClassification = CustomerContent;
        }
        field(11; SolutionDelayInMinutes; Integer)
        {
            Caption = 'Solution Delay In Minutes';
            DataClassification = CustomerContent;
            InitValue = 10;
        }
        field(20; Solution; Text[2048])
        {
            Caption = 'Solution';
            DataClassification = CustomerContent;
        }
        field(21; "Solution DateTime"; DateTime)
        {
            Caption = 'Solution DateTime';
            DataClassification = CustomerContent;
        }
        field(30; "No. of Uncompleted Tasks"; Integer)
        {
            Caption = 'No. of Uncompleted Tasks';
            FieldClass = FlowField;
            CalcFormula = count("Escape Room Task" where("Venue Id" = field("Venue Id"), "Room Name" = field(Name), Status = const("Escape Room Task Status"::Open)));
        }
    }

    keys
    {
        key(PK; "Venue Id", "Name")
        {
            Clustered = true;
        }
        key(Sequence; Sequence)
        {
        }
    }

    procedure UpdateStatus()
    var
        Task: Record "Escape Room Task";
    begin
        if Rec.Status <> Rec.Status::InProgress then exit;

        Task.Setrange("Venue Id", Rec."Venue Id");
        Task.Setrange("Room Name", Rec.Name);
        Task.SetRange(Status, task.Status::Open);
        if Task.FindSet() then
            repeat
                Task.UpdateStatus();
            until Task.Next() = 0;

        SelectLatestVersion();

        Task.Setrange("Venue Id", Rec."Venue Id");
        Task.Setrange("Room Name", Rec.Name);
        Task.SetRange(Status, task.Status::Open);
        if not Task.IsEmpty() then exit;

        Rec.Stop();

        OpenNextRoom();
    end;

    procedure CloseRoomIfCompleted()
    var
        Task: Record "Escape Room Task";
    begin
        if Rec.Status <> Rec.Status::InProgress then exit;

        Task.Setrange("Venue Id", Rec."Venue Id");
        Task.Setrange("Room Name", Rec.Name);
        Task.SetRange(Status, task.Status::Open);
        if not task.IsEmpty then exit;

        Rec.Stop();

        OpenNextRoom();
    end;

    internal procedure OpenNextRoom()
    var
        NextRoom: Record "Escape Room";
        Venue: Record "Escape Room Venue";
    begin
        NextRoom.SetCurrentKey(Sequence);
        NextRoom.Ascending := true;
        NextRoom.SetRange("Venue Id", Rec."Venue Id");
        NextRoom.SetFilter(Sequence, '>%1', Rec.Sequence);
        NextRoom.SetRange(Status, NextRoom.Status::Locked);
        if NextRoom.FindFirst() then begin
            NextRoom.Start();
        end
        else begin
            Venue.Get(Rec."Venue Id");
            Venue.Stop();
        end;
    end;

    procedure Start()
    var
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
    begin
        if Rec.Status = Rec.Status::Locked then begin
            Rec.Status := Rec.Status::InProgress;
            Rec."Start DateTime" := CurrentDateTime();
            Rec.Modify();
            Commit();

            EscapeRoomTelemetry.LogRoomStarted(Rec);
        end;
    end;

    procedure Stop()
    var
        EscapeRoomNotifications: Codeunit EscapeRoomNotifications;
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
    begin
        if Rec.Status = Rec.Status::InProgress then begin
            Rec.Status := Rec.Status::Completed;
            Rec."Stop DateTime" := CurrentDateTime();
            Rec.Modify();
            Commit();

            EscapeRoomNotifications.RoomFinished(Rec);
            EscapeRoomTelemetry.LogRoomCompleted(Rec);
        end;
    end;

    procedure GetHint()
    var
        TaskRec: Record "Escape Room Task";
        Task: Interface iEscapeRoomTask;
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
        Hint: Text;
    begin
        UpdateStatus();

        if Rec.Status = Rec.Status::Completed then error('The room is completed.  No hints can (or should) be given.');
        if Rec.Status = Rec.Status::Locked then error('The room is locked.  No hints can be given yet.');

        TaskRec.SetRange("Venue Id", Rec."Venue Id");
        TaskRec.SetRange("Room Name", Rec.Name);
        TaskRec.SetRange(Status, TaskRec.Status::Open);
        TaskRec.SetRange("Hint", '');
        TaskRec.SetCurrentKey(Sequence);
        if not TaskRec.FindFirst() then error('No remaining hints found for this room were found');

        Task := TaskRec.Task;
        Hint := Task.GetHint();
        TaskRec.Hint := CopyStr(Hint, 1, MaxStrLen(TaskRec.Hint));
        TaskRec."Hint DateTime" := CurrentDateTime();
        TaskRec.Modify();

        Message(Hint);

        EscapeRoomTelemetry.LogHintRequested(TaskRec);
    end;

    procedure GetStatus(): Enum "Escape Room Status"
    begin
        if Rec.Find('=') then begin
            exit(Rec.Status);
        end;

        exit(Rec.Status::Locked);
    end;

    procedure Solve()
    var
        Room: Interface iEscapeRoom;
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
    begin
        UpdateStatus();

        if Rec.Status = Rec.Status::Completed then error('The room is completed.  So it is already solved..');
        if Rec.Status = Rec.Status::Locked then error('The room is locked.  Solution cannot be given yet.');

        if not SolutionIsAvailable() then
            error('Solution is not available yet.  Please wait for %1 minutes.', Rec.SolutionDelayInMinutes);

        Room := Rec.Room;
        Room.Solve();

        Rec.Find('=');
        Rec."Solution DateTime" := CurrentDateTime();
        Rec.Modify();

        Commit();

        EscapeRoomTelemetry.LogSolutionRequested(Rec);

        UpdateStatus();

    end;

    procedure SolutionIsAvailable(): Boolean
    var
        CurrentDateTime: DateTime;
        CheckdateTime: DateTime;
    begin
        if Rec.Status <> Rec.Status::InProgress then exit(false);
        if Rec."Start DateTime" = 0DT then exit(false);

        CurrentDateTime := CurrentDateTime();

        CheckdateTime := Rec."Start DateTime" + (Rec.SolutionDelayInMinutes * 60);
        if CheckdateTime > CurrentDateTime then
            exit(false);

        exit(true);
    end;

    procedure ResetRoom()
    var
        Task: Record "Escape Room Task";
        EscapeRoomMgt: Codeunit "Escape Room";
        CurrentRoom: Interface iEscapeRoom;
        ConfirmManagement: Codeunit "Confirm Management";
        ResetRoomQst: Label 'Are you sure you want to reset this room? All task progress and hints will be permanently lost and there is no way back.';
    begin
        if Rec.Status = Rec.Status::Locked then exit;

        if not ConfirmManagement.GetResponseOrDefault(ResetRoomQst, false) then
            exit;

        Task.SetRange("Venue Id", Rec."Venue Id");
        Task.SetRange("Room Name", Rec.Name);
        Task.DeleteAll();

        CurrentRoom := Rec.Room;
        EscapeRoomMgt.RefreshTasks(CurrentRoom);

        Rec.Status := Rec.Status::InProgress;
        Rec."Start DateTime" := CurrentDateTime();
        Rec."Stop DateTime" := 0DT;
        Rec."Solution DateTime" := 0DT;
        Rec.Modify();
        Commit();
    end;


}