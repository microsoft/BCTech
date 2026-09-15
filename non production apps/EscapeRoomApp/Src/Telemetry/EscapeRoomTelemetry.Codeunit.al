codeunit 73925 "Escape Room Telemetry"
{
    procedure LogFinishedTask(var Task: Record "Escape Room Task")
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForTask(Task, CustomDimensions);
        AddScorePoints(CustomDimensions, 3); // +3 points for completing a task

        this.LogMessage(
                'EscapeRoomTaskFinished',
                'Task has been completed from the current Escape Room.',
                CustomDimensions
            );
    end;

    procedure LogHintRequested(var Task: Record "Escape Room Task")
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForTask(Task, CustomDimensions);
        AddScorePoints(CustomDimensions, -1); // -1 point for requesting a hint

        this.LogMessage(
                'EscapeRoomHintRequested',
                'Hint has been requested from the current Escape Room.',
                CustomDimensions
            );
    end;

    procedure LogSolutionRequested(var Room: record "Escape Room")
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForRoom(Room, CustomDimensions);
        AddScorePoints(CustomDimensions, -3); // -3 points for requesting solution

        this.LogMessage(
                'EscapeRoomSolutionRequested',
                'Solution has been requested for the current Escape Room.',
                CustomDimensions
            );
    end;

    procedure LogRoomStarted(var Room: Record "Escape Room")
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForRoom(Room, CustomDimensions);
        AddScorePoints(CustomDimensions, 0); // No score change for starting

        this.LogMessage(
                'EscapeRoomStarted',
                'Escape Room has been started.',
                CustomDimensions
            );
    end;

    procedure LogRoomCompleted(var Room: Record "Escape Room")
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForRoom(Room, CustomDimensions);
        AddScorePoints(CustomDimensions, 5); // +5 bonus points for completing entire room

        this.LogMessage(
                'EscapeRoomCompleted',
                'Escape Room has been completed.',
                CustomDimensions
            );
    end;

    procedure LogNotification(NotificationText: Text)
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        CustomDimensions.Add('NotificationText', NotificationText);

        this.LogMessage(
                'EscapeRoomNotification',
                'Notification has been sent.',
                CustomDimensions
            );
    end;

    procedure LogVenueCompleted(var Venue: Record "Escape Room Venue")
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForVenue(Venue, CustomDimensions);
        AddScorePoints(CustomDimensions, 10); // +10 bonus points for completing entire venue

        this.LogMessage(
                'EscapeRoomVenueCompleted',
                'Escape Room Venue has been completed.',
                CustomDimensions
            );
    end;

    /// <summary>Log a custom telemetry event scoped to a task. Standard task/room/venue dimensions are added automatically.</summary>
    procedure LogCustomEvent(var EscapeRoomTask: Record "Escape Room Task"; EventId: Text; EventMessage: Text; ScorePoints: Integer)
    var
        ExtraDimensions: Dictionary of [Text, Text];
    begin
        LogCustomEvent(EscapeRoomTask, EventId, EventMessage, ScorePoints, ExtraDimensions);
    end;

    /// <summary>Log a custom telemetry event scoped to a room. Standard room/venue dimensions are added automatically.</summary>
    procedure LogCustomEvent(var EscapeRoom: Record "Escape Room"; EventId: Text; EventMessage: Text; ScorePoints: Integer)
    var
        ExtraDimensions: Dictionary of [Text, Text];
    begin
        LogCustomEvent(EscapeRoom, EventId, EventMessage, ScorePoints, ExtraDimensions);
    end;

    /// <summary>Task-scoped custom event with additional caller-provided dimensions.</summary>
    procedure LogCustomEvent(var EscapeRoomTask: Record "Escape Room Task"; EventId: Text; EventMessage: Text; ScorePoints: Integer; ExtraDimensions: Dictionary of [Text, Text])
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForTask(EscapeRoomTask, CustomDimensions);
        AddScorePoints(CustomDimensions, ClampScorePoints(ScorePoints));
        AddCustomEventDimensions(CustomDimensions, EventId);
        MergeExtraDimensions(CustomDimensions, ExtraDimensions);

        this.LogMessage(
                'EscapeRoomCustomEvent',
                EventMessage,
                CustomDimensions
            );
    end;

    /// <summary>Room-scoped custom event with additional caller-provided dimensions.</summary>
    procedure LogCustomEvent(var EscapeRoom: Record "Escape Room"; EventId: Text; EventMessage: Text; ScorePoints: Integer; ExtraDimensions: Dictionary of [Text, Text])
    var
        CustomDimensions: Dictionary of [Text, Text];
    begin
        GetCustomDimensionsForRoom(EscapeRoom, CustomDimensions);
        AddScorePoints(CustomDimensions, ClampScorePoints(ScorePoints));
        AddCustomEventDimensions(CustomDimensions, EventId);
        MergeExtraDimensions(CustomDimensions, ExtraDimensions);

        this.LogMessage(
                'EscapeRoomCustomEvent',
                EventMessage,
                CustomDimensions
            );
    end;

    local procedure ClampScorePoints(ScorePoints: Integer): Integer
    begin
        if ScorePoints < -5 then
            exit(-5);
        if ScorePoints > 5 then
            exit(5);
        exit(ScorePoints);
    end;

    local procedure AddCustomEventDimensions(var CustomDimensions: Dictionary of [Text, Text]; EventId: Text)
    begin
        EventId := EventId.Trim();
        if EventId = '' then
            EventId := 'Unspecified';
        if StrLen(EventId) > 80 then
            EventId := CopyStr(EventId, 1, 80);

        CustomDimensions.Add('EventId', EventId);
        CustomDimensions.Add('EventSource', 'Custom');
    end;

    local procedure MergeExtraDimensions(var CustomDimensions: Dictionary of [Text, Text]; ExtraDimensions: Dictionary of [Text, Text])
    var
        DimensionKey: Text;
    begin
        foreach DimensionKey in ExtraDimensions.Keys() do
            if not CustomDimensions.ContainsKey(DimensionKey) then
                CustomDimensions.Add(DimensionKey, ExtraDimensions.Get(DimensionKey));
    end;

    local procedure GetCustomDimensionsForRoom(var Room: Record "Escape Room"; var CustomDimensions: Dictionary of [Text, Text])
    var
        Venue: Record "Escape Room Venue";
        User: Record User;
    begin
        Venue.Get(Room."Venue Id");

        CustomDimensions.Add('VenueId', Room."Venue Id");
        CustomDimensions.Add('UserId', UserId);
        CustomDimensions.Add('VenueUserName', Venue."Full Name");
        CustomDimensions.Add('VenuePartner', Venue."Partner Name");
        CustomDimensions.Add('VenueName', Venue.Name);
        CustomDimensions.Add('RoomName', Room.Name);
        CustomDimensions.Add('Description', Room.Description);
        if room."Start DateTime" <> 0DT then
            CustomDimensions.Add('RoomStartDateTime', Format(Room."Start DateTime", 0, 9));
        if room."Stop DateTime" <> 0DT then
            CustomDimensions.Add('RoomStopDateTime', Format(Room."Stop DateTime", 0, 9));
    end;

    local procedure GetCustomDimensionsForVenue(var Venue: Record "Escape Room Venue"; var CustomDimensions: Dictionary of [Text, Text])
    var
        User: Record User;
    begin
        CustomDimensions.Add('VenueId', Venue.Id);
        CustomDimensions.Add('UserId', UserId);
        CustomDimensions.Add('VenueUserName', Venue."Full Name");
        CustomDimensions.Add('VenuePartner', Venue."Partner Name");
        CustomDimensions.Add('VenueName', Venue.Name);
        CustomDimensions.Add('VenueDescription', Venue.Description);
        if Venue."Start DateTime" <> 0DT then
            CustomDimensions.Add('VenueStartDateTime', Format(Venue."Start DateTime", 0, 9));
        if Venue."Stop DateTime" <> 0DT then
            CustomDimensions.Add('VenueStopDateTime', Format(Venue."Stop DateTime", 0, 9));

        // Calculate total venue completion time if both start and stop are available
        if (Venue."Start DateTime" <> 0DT) and (Venue."Stop DateTime" <> 0DT) then
            CustomDimensions.Add('VenueCompletionTimeMinutes', Format((Venue."Stop DateTime" - Venue."Start DateTime") / (1000 * 60)));
    end;

    local procedure GetCustomDimensionsForTask(var Task: Record "Escape Room Task"; var CustomDimensions: Dictionary of [Text, Text])
    var
        Venue: Record "Escape Room Venue";
        Room: record "Escape Room";
        User: Record User;
    begin
        Venue.Get(Task."Venue Id");
        Room.Get(Task."Venue Id", Task."Room name");

        GetCustomDimensionsForRoom(Room, CustomDimensions);

        CustomDimensions.Add('TaskName', Task.Name);
        CustomDimensions.Add('TaskStopDateTime', Format(Task."Stop DateTime", 0, 9));
        CustomDimensions.Add('Hint', Task.Hint);
        CustomDimensions.Add('HintDateTime', Format(Task."Hint DateTime", 0, 9));
    end;

    local procedure LogMessage(EventId: Text; Message: Text; CustomDimensions: Dictionary of [Text, Text])
    var
        Telemetry: Codeunit Telemetry;
    begin
        Telemetry.LogMessage(
                    EventId,
                    Message,
                    Verbosity::Normal,
                    DataClassification::SystemMetadata,
                    TelemetryScope::ExtensionPublisher,
                    CustomDimensions
                );
    end;

    local procedure AddScorePoints(var CustomDimensions: Dictionary of [Text, Text]; ScorePoints: Integer)
    var
        CompanyInfo: Record "Company Information";
    begin
        // Add scoring information
        CustomDimensions.Add('ScorePoints', Format(ScorePoints));
    end;
}