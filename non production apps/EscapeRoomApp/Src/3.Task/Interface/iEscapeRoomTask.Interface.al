interface iEscapeRoomTask
{
    procedure GetTaskRec() EscapeRoomTask: Record "Escape Room Task";
    procedure GetTask(): enum "Escape Room Task";
    procedure IsValid(): Boolean;
    procedure GetHint(): Text;
}