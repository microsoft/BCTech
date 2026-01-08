pageextension 73920 "Business Manager RC Ext" extends "Business Manager Role Center"
{
    actions
    {
        addlast(embedding)
        {
            action(EscapeRoomVenues)
            {
                ApplicationArea = All;
                Caption = 'Escape Room Venues';
                RunObject = Page "Escape Room Venue List";
            }
        }
    }

}