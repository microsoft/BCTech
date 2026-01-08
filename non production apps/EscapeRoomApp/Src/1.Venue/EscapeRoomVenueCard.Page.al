page 73928 "Escape Room Venue Card"
{
    ApplicationArea = All;
    Caption = 'Escape Room Venue';
    PageType = Card;
    SourceTable = "Escape Room Venue";
    InsertAllowed = false;
    DeleteAllowed = false;

    layout
    {
        area(content)
        {
            group(General)
            {
                field(Id; Rec."Id")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the Id of the escape room venue.';

                    trigger OnAssistEdit()
                    begin
                        if Rec."Id" = '' then
                            Error('Id cannot be edited manually. Use the Update Venues action to populate venues.');
                    end;
                }
                field(Name; Rec.Name)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the name of the escape room venue.';
                    Editable = false;
                }
                field(Description; Rec.Description)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the description of the escape room venue.';
                    Editable = false;
                }
                field(Publisher; Rec.Publisher)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the publisher of the escape room venue.';
                    Editable = false;
                }
                field("App ID"; Rec."App ID")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the App ID of the escape room venue.';
                    Editable = false;
                }
            }
            group("Details Attendee")
            {
                field("Full Name"; Rec."Full Name")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the full name of the person completing this venue.';
                }
                field("Partner Name"; Rec."Partner Name")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the name of the partner for this venue.';
                }
                field("Start DateTime"; Rec."Start DateTime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the start date and time of the escape room venue.';
                    Editable = false;
                }
                field("Stop DateTime"; Rec."Stop DateTime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the stop date and time of the escape room venue.';
                    Editable = false;
                }
            }
        }
    }
    actions
    {
        area(Navigation)
        {
            action(EscapeRooms)
            {
                ApplicationArea = All;
                Caption = 'Escape Rooms';
                Image = List;
                RunObject = Page "Escape Room List";
                RunPageLink = "Venue Id" = field(Id);
                ToolTip = 'View the escape rooms for this venue.';
            }
        }
        area(Promoted)
        {
            actionref(EscapeRoomsRef; EscapeRooms)
            {
                Visible = true;
            }
        }
    }

    trigger OnOpenPage()
    begin
        Rec.RefreshRooms();
    end;
}
