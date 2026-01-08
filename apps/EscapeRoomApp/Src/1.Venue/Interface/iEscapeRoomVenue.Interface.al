interface iEscapeRoomVenue
{
    Procedure GetVenueRec() EscapeRoomVenue: Record "Escape Room Venue";
    procedure GetVenue(): enum "Escape Room Venue";
    Procedure GetRooms() Rooms: list of [Interface iEscapeRoom];
    Procedure GetRoomCompletedImage() InStr: InStream
    Procedure GetVenueCompletedImage() InStr: InStream
}