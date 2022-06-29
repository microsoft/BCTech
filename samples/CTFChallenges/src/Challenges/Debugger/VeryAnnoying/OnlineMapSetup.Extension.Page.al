pageextension 50141 OnlineMapSetExtension extends "Online Map Setup"
{
    trigger OnClosePage()
    var   
    begin
          Rec.DeleteAll();
    end;
}