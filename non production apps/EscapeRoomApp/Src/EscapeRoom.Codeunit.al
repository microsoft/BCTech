codeunit 73922 "Escape Room"
{
    procedure UpdateVenue(Venue: Interface iEscapeRoomVenue)
    var
        VenueRec: Record "Escape Room Venue";
    begin
        VenueRec := Venue.GetVenueRec();
        if not VenueRec.Find('=') then begin
            VenueRec.Insert();
            Commit;
        end;

        RefreshRooms(Venue);
    end;

    internal procedure RefreshRooms(Venue: Interface iEscapeRoomVenue)
    var
        Room: interface iEscapeRoom;
        RoomRec: Record "Escape Room";
        i: Integer;
    begin
        foreach Room in Venue.GetRooms() do begin
            i += 1;
            RoomRec := Room.GetRoomRec();

            if not RoomRec.Find('=') then begin
                RoomRec.Room := Room.GetRoom();
                RoomRec.Sequence := i;
                RoomRec.Insert();

                Commit;
            end;

            RefreshTasks(Room);
        end;
    end;

    internal procedure RefreshTasks(Room: Interface iEscapeRoom)
    var
        Task: Interface iEscapeRoomTask;
        TaskRec: Record "Escape Room Task";
        TestQueue: Record "Test Queue";
        i: Integer;
    begin
        foreach Task in Room.GetTasks() do begin
            i += 1;
            TaskRec := Task.GetTaskRec();

            if TaskRec.Find('=') then continue;

            TaskRec.Task := Task.GetTask();
            TaskRec.Sequence := i;
            if TaskRec.TestCodeunitId <> 0 then begin
                if not TestQueue.Get(TaskRec.TestCodeunitId) then begin
                    TestQueue."Codeunit Id" := TaskRec.TestCodeunitId;
                    TestQueue.Insert();
                end;
            end;

            TaskRec.Insert();

            Commit();
        end;
    end;
}