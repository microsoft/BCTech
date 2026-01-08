permissionset 73920 EscapeRoomAdmin
{
    Caption = 'Escape Room Admin';
    Assignable = true;
    Permissions = tabledata "Escape Room" = RIMD,
        tabledata "Escape Room Task" = RIMD,
        tabledata "Escape Room Venue" = RIMD,
        tabledata "Test Queue" = RIMD;
}