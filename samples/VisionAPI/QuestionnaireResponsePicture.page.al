page 50005 "Questionnaire Response Picture"
{
    Caption = 'Questionnaire Response Picture';
    DeleteAllowed = false;
    InsertAllowed = false;
    PageType = CardPart;
    SourceTable = "Questionnaire Response Header";

    layout
    {
        area(content)
        {
            field(Picture; Picture)
            {
                ApplicationArea = All;
                ShowCaption = false;
                ToolTip = 'Specifies the picture that has been captured for the response.';
            }
        }
    }

    actions
    {
        area(processing)
        {
            action(ImportPicture)
            {
                ApplicationArea = All;
                Caption = 'Import';
                Image = Import;
                ToolTip = 'Import a picture file.';

                trigger OnAction()
                begin
                    ImportFromDevice;
                end;
            }
            action(ExportFile)
            {
                ApplicationArea = All;
                Caption = 'Export';
                Enabled = DeleteExportEnabled;
                Image = Export;
                ToolTip = 'Export the picture to a file.';

                trigger OnAction()
                var
                begin
                    TestField("Entry No.");
                    TestField(Picture);
                    message('not implemented yet');
                    // Picture.ExportFile('');
                end;
            }
            action(DeletePicture)
            {
                ApplicationArea = All;
                Caption = 'Delete';
                Enabled = DeleteExportEnabled;
                Image = Delete;
                ToolTip = 'Delete the record.';

                trigger OnAction()
                begin
                    TestField("Entry No.");
                    TestField(Picture);
                    DeleteItemPicture;
                end;
            }
        }
    }

    trigger OnAfterGetCurrRecord()
    begin
        SetEditableOnPictureActions;
    end;

    trigger OnOpenPage()
    begin
    end;

    var
        OverrideImageQst: Label 'The existing picture will be replaced. Do you want to continue?';
        DeleteImageQst: Label 'Are you sure you want to delete the picture?';
        SelectPictureTxt: Label 'Select a picture to upload';
        DeleteExportEnabled: Boolean;

    [Scope('OnPrem')]
    procedure ImportFromDevice()
    var
        ClientFileName: Text;
        InStr: InStream;
        OutStr: OutStream;
    begin
        Find;
        TestField("Entry No.");

        if Picture.HasValue then
            if not Confirm(OverrideImageQst) then
                Error('');
        if not UploadIntoStream('Select a file...', '', 'All Files (*.*)|*.*', Clientfilename, InStr) then
            exit;

        Clear(Picture);
        Picture.ImportStream(InStr, '');
        Modify(true);
    end;

    local procedure SetEditableOnPictureActions()
    begin
        DeleteExportEnabled := Picture.HasValue;
    end;

    procedure DeleteItemPicture()
    begin
        TestField("Entry No.");

        if not Confirm(DeleteImageQst) then
            exit;

        Clear(Picture);
        Modify(true);
    end;

}

