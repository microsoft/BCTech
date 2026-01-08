table 73926 "Escape Room Venue"
{
    DataClassification = CustomerContent;
    Caption = 'Escape Venue';

    fields
    {
        field(1; "Id"; Text[50])
        {
            Caption = 'Code';
            DataClassification = CustomerContent;
        }
        field(2; Name; Text[100])
        {
            Caption = 'Name';
            DataClassification = CustomerContent;
        }
        field(3; Description; Text[250])
        {
            Caption = 'Description';
            DataClassification = CustomerContent;
        }
        field(4; "App ID"; Guid)
        {
            Caption = 'App ID';
            DataClassification = CustomerContent;
        }
        field(5; "Publisher"; Text[100])
        {
            Caption = 'Publisher';
            DataClassification = CustomerContent;
        }
        field(6; "Full Name"; Text[100])
        {
            Caption = 'Full Name';
            DataClassification = CustomerContent;

            trigger OnValidate()
            begin
                if Rec."Start DateTime" = 0DT then begin
                    Start();
                end
            end;
        }
        field(7; "Partner Name"; Text[100])
        {
            Caption = 'Partner Name';
            DataClassification = CustomerContent;

            trigger OnValidate()
            begin
                if Rec."Start DateTime" = 0DT then begin
                    Start();
                end
            end;
        }
        field(8; Venue; enum "Escape Room Venue")
        {
            Caption = 'Venue';
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
    }

    keys
    {
        key(PK; "Id")
        {
            Clustered = true;
        }
    }

    procedure Start()
    var
        Venue: Interface iEscapeRoomVenue;
    begin
        if Rec."Full Name" = '' then exit;
        if Rec."Partner Name" = '' then exit;

        Rec."Start DateTime" := CurrentDateTime();
        Rec.Modify();

        Rec.StartFirstRoom();
    end;

    procedure Stop()
    var
        Venue: Interface iEscapeRoomVenue;
        EscapeRoomNotifications: Codeunit EscapeRoomNotifications;
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
    begin
        Rec."Stop DateTime" := CurrentDateTime();
        Rec.Modify();

        Commit;

        EscapeRoomNotifications.venueFinished(Rec);
        EscapeRoomTelemetry.LogVenueCompleted(Rec);
    end;

    procedure StartFirstRoom()
    var
        Room: Record "Escape Room";
    begin
        Room.SetRange("Venue Id", Rec."Id");
        Room.SetCurrentKey(Sequence);
        Room.ReadIsolation := IsolationLevel::UpdLock;
        Room.FindFirst();

        Room.Start();
    end;

    procedure CloseVenueIfCompleted()
    var
        Room: Record "Escape Room";
    begin
        Room.Setrange("Venue Id", Rec.Id);
        Room.SetRange(Status, Room.Status::Completed);
        if not Room.IsEmpty then exit;

        Rec.Stop();
    end;

    procedure RefreshRooms()
    var
        EscapeRoom: Codeunit "Escape Room";
    begin
        EscapeRoom.UpdateVenue(Rec.Venue);
    end;
}