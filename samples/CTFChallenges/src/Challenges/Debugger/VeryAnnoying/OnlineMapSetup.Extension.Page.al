pageextension 50141 Flag_f6509017 extends "Online Map Setup"
{
    trigger OnClosePage()
    var   
    begin
          Rec.DeleteAll();
    end;
}