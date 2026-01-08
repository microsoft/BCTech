page 73922 "Escape Room"
{
    ApplicationArea = All;
    Caption = 'Escape Room';
    PageType = Document;
    SourceTable = "Escape Room";
    InsertAllowed = false;
    DeleteAllowed = false;
    Editable = false;

    layout
    {
        area(content)
        {
            group(General)
            {
                field(Name; Rec.Name)
                {
                    ToolTip = 'Specifies the name of the escape room.';
                }
                field("Venue Id"; Rec."Venue Id")
                {
                    ToolTip = 'Specifies the code of the venue this room belongs to.';
                }
                field(Description; Rec.Description)
                {
                    ToolTip = 'Specifies the description of the escape room.';
                    MultiLine = true;
                    Visible = false;
                }
                field(Sequence; Rec.Sequence)
                {
                    ToolTip = 'Specifies the sequence number of the escape room.';
                }
                field(Status; Rec.Status)
                {
                    ToolTip = 'Specifies the status of the escape room.';
                }
                field("No. of Uncompleted Tasks"; Rec."No. of Uncompleted Tasks")
                {
                    ToolTip = 'Specifies the number of uncompleted tasks in this escape room.';
                }
                field("Start DateTime"; Rec."Start DateTime")
                {
                    ToolTip = 'Specifies the start date and time of the escape room.';
                    Editable = false;
                }
                field("Stop DateTime"; Rec."Stop DateTime")
                {
                    ToolTip = 'Specifies the stop date and time of the escape room.';
                    Editable = false;
                }
            }
            Group(RoomDescription)
            {
                Caption = 'Room Description';
                field(RoomDescriptionCtrl; CurrentRoom.GetRoomDescription())
                {
                    ToolTip = 'Specifies the description of the escape room.';
                    ShowCaption = false;
                    MultiLine = true;
                    Editable = false;
                    ExtendedDatatype = RichContent;
                }
            }
            group(CompletedTasks)
            {
                Caption = 'Completed Tasks';

                part(TasksPage; "Escape Room Task List")
                {
                    SubPageLink = "Venue Id" = FIELD("Venue Id"), "Room Name" = FIELD(Name);
                    SubPageView = where(Status = const(Completed)); //Hide Tasks that are not completed
                }
            }
        }
        area(factboxes)
        {
            part(HintsList; "Hints ListPart")
            {
                ApplicationArea = All;
                Caption = 'Hints';
                SubPageLink = "Venue Id" = FIELD("Venue Id"), "Room Name" = FIELD(Name), Hint = filter(<> '');
            }
        }
    }

    actions
    {
        area(Navigation)
        {
            action(EscapeRooms)
            {
                Caption = 'Escape Rooms';
                Image = List;
                RunObject = Page "Escape Room List";
                RunPageLink = "Venue Id" = field("Venue Id");
            }
        }
        area(Processing)
        {
            action(UpdateStatus)
            {
                Caption = 'Update Status';
                image = Refresh;
                trigger OnAction()
                begin
                    Rec.UpdateStatus();
                    CurrPage.Update(false);
                end;
            }
            action(GetHint)
            {
                Caption = 'Get Hint';
                Image = Help;
                trigger OnAction()
                begin
                    Rec.GetHint();
                    CurrPage.Update(false);
                end;
            }
            action(Solve)
            {
                Caption = 'Solve';
                Image = Sparkle;
                Enabled = SolutionIsAvailable;
                trigger OnAction()
                var
                    ConfirmManagement: Codeunit "Confirm Management";
                begin
                    if not ConfirmManagement.GetResponseOrDefault('Are you sure you want to solve this room?', false) then
                        exit;

                    Rec.Solve();

                    CurrPage.Update(false);
                end;
            }
        }
        area(Promoted)
        {
            actionref(UpdateStatusRef; UpdateStatus)
            {
                Visible = true;
            }
            actionref(GetHintRef; GetHint)
            {
                Visible = true;
            }
            actionref(SolveRef; Solve)
            {
                Visible = true;
            }
        }
    }

    var
        CurrentRoom: Interface iEscapeRoom;
        SolutionIsAvailable: Boolean;

    trigger OnAfterGetRecord()
    begin
        if Rec.Status = Rec.Status::Locked then error('This escape room is still locked.');

        CurrentRoom := Rec.Room;

        SolutionIsAvailable := Rec.SolutionIsAvailable();
    end;
}
