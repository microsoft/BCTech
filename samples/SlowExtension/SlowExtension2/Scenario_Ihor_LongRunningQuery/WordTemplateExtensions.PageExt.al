pageextension 50135 "Word Template Extensions" extends "Email Editor"
{
    layout
    {
        addafter(Attachments)
        {
            field(RelatedEntityExtensions; RelatedEntityExtensions)
            {
                ApplicationArea = All;
            }
        }
    }


    trigger OnOpenPage()
    begin
        PopulateRelatedEntityExtensions();
    end;

    local procedure PopulateRelatedEntityExtensions()
    var
        WordTemplate: Record "Word Template";
        AllObjWithCaption: Record AllObjWithCaption;
        NAVAppInstalledApp: Record "NAV App Installed App";
    begin
        if not WordTemplate.FindSet() then
            exit;

        repeat
            AllObjWithCaption.SetFilter("Object Caption", '*%1*', WordTemplate."Table Caption");
            if AllObjWithCaption.FindFirst() then
                if NAVAppInstalledApp.Get(AllObjWithCaption."App Package ID") then
                    RelatedEntityExtensions += NAVAppInstalledApp.Name + ' | ';
        until WordTemplate.Next() = 0;
        RelatedEntityExtensions := RelatedEntityExtensions.TrimEnd(' | ');
    end;

    var
        RelatedEntityExtensions: Text;
}