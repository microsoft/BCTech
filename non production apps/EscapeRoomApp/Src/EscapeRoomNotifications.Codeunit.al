codeunit 73923 EscapeRoomNotifications
{
    procedure TaskFinished(Task: Record "Escape Room Task")
    var
        Notification: Notification;
    begin
        Notification.Id := CreateGuid();
        Notification.Message := 'CONGRATULATIONS! Task "' + Task.Name + '" has been completed from the current Escape Room.';
        Notification.SetData('VenueId', task."Venue Id");
        Notification.SetData('RoomName', Task."Room name");
        Notification.SetData('TaskName', Task.Name);
        Notification.AddAction('Open Room', codeunit::"EscapeRoomNotifications", 'OpenRoom');
        Notification.Send();
    end;

    procedure RoomFinishedNotification(Room: Record "Escape Room")
    var
        Notification: Notification;
    begin
        Notification.Id := CreateGuid();
        Notification.Message := 'CONGRATULATIONS! Escape Room "' + Room.Name + '" has been completed.';
        Notification.SetData('VenueId', Room."Venue Id");
        Notification.SetData('RoomName', Room.Name);
        Notification.AddAction('Open Room', codeunit::"EscapeRoomNotifications", 'OpenRoom');
        Notification.Send();
    end;

    Procedure RoomFinished(Room: Record "Escape Room")
    var
        Venue: Record "Escape Room Venue";
        VenueImpl: Interface iEscapeRoomVenue;
    begin
        Venue.Get(Room."Venue Id");
        VenueImpl := Venue.Venue;
        ShowCompletionImage(VenueImpl, VenueImpl.GetRoomCompletedImage());
    end;

    procedure VenueFinished(Venue: Record "Escape Room Venue")
    var
        VenueImpl: Interface iEscapeRoomVenue;
    begin
        VenueImpl := Venue.Venue;
        ShowCompletionImage(VenueImpl, VenueImpl.GetVenueCompletedImage());
    end;

    procedure ShowCompletionImage(VenueImpl: Interface iEscapeRoomVenue; InStr: InStream)
    var
        PictureViewer: Page "Picture Page";
        Base64Convert: Codeunit "Base64 Convert";
        Base64Image: Text;
    begin

        if InStr.Length > 0 then
            Base64Image := Base64Convert.ToBase64(Instr)
        else
            Base64Image := '';

        PictureViewer.SetImage(Base64Image);
        PictureViewer.Run();
    end;

    Procedure Warning(Message: Text)
    var
        Notification: Notification;
        EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
    begin
        Notification.Id := CreateGuid();
        Notification.Message := 'MESSAGE from your Escape Room: ' + Message;
        Notification.Send();

        EscapeRoomTelemetry.LogNotification(Message);
    end;

    procedure OpenRoom(var Notification: Notification)
    var
        Room: Record "Escape Room";
    begin
        Room.Get(Notification.GetData('VenueId'), Notification.GetData('RoomName'));
        Room.SetRecFilter();
        page.RunModal(page::"Escape Room", Room);
    end;
}