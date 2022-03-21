page 50100 "Database Operation Statistics"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Database Operation Statistics";
    
    layout
    {
        area(Content)
        {
            repeater(Tables)
            {
                field(Name; Rec."Table Name")
                {
                    ApplicationArea = All;
                }
                field(Inserts; Rec.Inserts)
                {
                    ApplicationArea = All;
                }
                field(Modifies; Rec.Modifies)
                {
                    ApplicationArea = All;
                }
                field(Deletes; Rec.Deletes)
                {
                    ApplicationArea = All;
                }
            }
        }
    }
}