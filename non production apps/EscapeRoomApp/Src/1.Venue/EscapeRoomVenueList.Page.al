page 73927 "Escape Room Venue List"
{
    ApplicationArea = All;
    Caption = 'Escape Room Venues';
    PageType = List;
    SourceTable = "Escape Room Venue";
    UsageCategory = Lists;
    CardPageId = "Escape Room Venue Card";
    Editable = false;

    layout
    {
        area(content)
        {
            repeater(General)
            {
                field(Id; Rec."Id")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the Id of the escape room venue.';
                }
                field(Name; Rec.Name)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the name of the escape room venue.';
                }
                field(Description; Rec.Description)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the description of the escape room venue.';
                }
                field(Publisher; Rec.Publisher)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the publisher of the escape room venue.';
                }
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
            }
        }
    }
    actions
    {
        area(Navigation)
        {
            // action(Test)
            // {
            //     ApplicationArea = All;
            //     Caption = 'Test';
            //     Image = Test;
            //     ToolTip = 'This is a test action.';
            //     trigger OnAction()
            //     var
            //         Venue: Interface iEscapeRoomVenue;
            //         InStr: InStream;
            //         Convert: Codeunit "Base64 Convert";
            //         PictureViewer: Page "Picture Page";
            //         TempBlob: Codeunit "Temp Blob";
            //         Base64Convert: Codeunit "Base64 Convert";
            //         Base64Image: Text;
            //     begin
            //         Venue := Rec.Venue;
            //         InStr := Venue.GetRoomCompletedImage();

            //         if InStr.Length > 0 then
            //             Base64Image := Base64Convert.ToBase64(Instr)
            //         else
            //             Base64Image := '';

            //         PictureViewer.SetImage(Base64Image);
            //         PictureViewer.Run();
            //     end;

            // }
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
}
