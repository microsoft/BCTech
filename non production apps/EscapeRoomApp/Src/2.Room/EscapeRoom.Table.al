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
        Venue: Record "Escape Room Venue";
        EscapeRoomNotifications: Codeunit EscapeRoomNotifications;
    begin
        if Rec.Status = Rec.Status::Completed then begin
            // Recovery path only: if this room was completed but the next room never got started
            // (e.g. the session was interrupted), open it now. OpenNextRoom() is idempotent and does
            // nothing while a later room is already in progress, so pressing "Update Status" on a
            // completed room can never open a second room or close the venue prematurely.
            if OpenNextRoom() then begin
                Venue.Get(Rec."Venue Id");
                EscapeRoomNotifications.VenueFinished(Venue);
            end;
            exit;
        end;

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
    end;

    /// <summary>
    /// Opens the next locked room after this one, or closes the venue when every room is completed.
    /// Idempotent: does nothing when a later room is already in progress.
    /// </summary>
    /// <returns>True when the venue got completed by this call.</returns>
    internal procedure OpenNextRoom() VenueCompleted: Boolean
    var
        NextRoom: Record "Escape Room";
        Venue: Record "Escape Room Venue";
    begin
        NextRoom.ReadIsolation := IsolationLevel::UpdLock;
        NextRoom.SetCurrentKey(Sequence);
        NextRoom.Ascending := true;
        NextRoom.SetRange("Venue Id", Rec."Venue Id");
        NextRoom.SetFilter(Sequence, '>%1', Rec.Sequence);

        // A later room is already open: nothing to do. This is what prevents a second room from
        // being opened when this procedure runs twice (concurrent sessions, or "Update Status"
        // pressed on an already completed room).
        NextRoom.SetRange(Status, NextRoom.Status::InProgress);
        if NextRoom.FindFirst() then
            exit(false);

        NextRoom.SetRange(Status, NextRoom.Status::Locked);
        if NextRoom.FindFirst() then begin
            NextRoom.Start();
            exit(false);
        end;

        // No later room left to open. Close the venue, but only when every room is really completed.
        Venue.Get(Rec."Venue Id");
        exit(Venue.CloseVenueIfCompleted());
    end;

    procedure Start()
    var
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
    begin
        if not LockAndRefresh() then exit;
        if Rec.Status <> Rec.Status::Locked then exit;

        Rec.Status := Rec.Status::InProgress;
        Rec."Start DateTime" := CurrentDateTime();
        Rec.Modify();
        Commit();

        EscapeRoomTelemetry.LogRoomStarted(Rec);
    end;

    /// <summary>
    /// Completes this room, opens the next one and then shows the completion image(s).
    /// The state transitions are committed before any UI is shown, so an interrupted or
    /// UI-less (background) session can no longer leave the next room locked.
    /// </summary>
    procedure Stop()
    var
        Venue: Record "Escape Room Venue";
        EscapeRoomNotifications: Codeunit EscapeRoomNotifications;
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
        VenueCompleted: Boolean;
    begin
        if not LockAndRefresh() then exit;
        if Rec.Status <> Rec.Status::InProgress then exit;

        Rec.Status := Rec.Status::Completed;
        Rec."Stop DateTime" := CurrentDateTime();
        Rec.Modify();
        Commit();

        EscapeRoomTelemetry.LogRoomCompleted(Rec);

        VenueCompleted := OpenNextRoom();

        EscapeRoomNotifications.RoomFinished(Rec);
        if VenueCompleted then begin
            Venue.Get(Rec."Venue Id");
            EscapeRoomNotifications.VenueFinished(Venue);
        end;
    end;

    /// <summary>
    /// Re-reads this room from the database while taking an update lock on its row, so that
    /// concurrent sessions serialize on the status transition instead of both performing it.
    /// </summary>
    local procedure LockAndRefresh(): Boolean
    var
        Found: Boolean;
    begin
        Rec.ReadIsolation := IsolationLevel::UpdLock;
        Found := Rec.Find('=');
        Rec.ReadIsolation := IsolationLevel::Default;
        exit(Found);
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