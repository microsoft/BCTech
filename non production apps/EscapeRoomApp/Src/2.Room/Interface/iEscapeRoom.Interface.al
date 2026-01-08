interface iEscapeRoom
{
    procedure GetRoomRec() EscapeRoom: Record "Escape Room";
    procedure GetRoom(): enum "Escape Room";
    procedure GetRoomDescription() Roomdescription: Text;
    procedure GetTasks() Tasks: list of [interface iEscapeRoomTask]
    procedure Solve();
}