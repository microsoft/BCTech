pageextension 50100 "BCTech_Company Information" extends "Company Information"
{
    actions
    {
        addfirst(Processing)
        {
            action(BCTech_RemoveChangeLogEntries)
            {
                ApplicationArea = All;
                Caption = 'Remove Change Log Entries';
                Image = Delete;
                ToolTip = 'Removes all change log entries.';
                trigger OnAction()
                var
                    RemoveChangeLogEntries: Codeunit "BCTech_RemoveChangeLogEntries";
                begin
                    RemoveChangeLogEntries.RemoveChangeLogEntries();
                end;
            }
        }
    }
}