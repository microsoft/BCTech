tableextension 50100 "Contact Extension" extends Contact
{
    fields
    {
        field(50100; "Industry"; Text[100])
        {
            Caption = 'Industry';
        }
    }
}

pageextension 50100 "Contact Card Extension" extends "Contact Card"
{
    layout
    {
        addlast(General)
        {
            field("Industry"; Rec."Industry")
            {
                ApplicationArea = All;
                Caption = 'Industry';
            }
        }
    }
}

codeunit 50101 "Contact Extension"
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CDS Setup Defaults", 'OnAfterResetContactContactMapping', '', true, true)]
    local procedure HandleOnAfterResetContactContactMapping(IntegrationTableMappingName: Code[20])
    var
        CRMContact: Record "CRM Contact";
        Contact: Record Contact;
        IntegrationFieldMapping: Record "Integration Field Mapping";
    begin
        InsertIntegrationFieldMapping(
            IntegrationTableMappingName,
            Contact.FieldNo("Industry"),
            CRMContact.FieldNo(cre59_industry),
            IntegrationFieldMapping.Direction::Bidirectional,
            '', true, false);
    end;

    procedure InsertIntegrationFieldMapping(IntegrationTableMappingName: Code[20]; TableFieldNo: Integer; IntegrationTableFieldNo: Integer; SynchDirection: Option; ConstValue: Text; ValidateField: Boolean; ValidateIntegrationTableField: Boolean)
    var
        IntegrationFieldMapping: Record "Integration Field Mapping";
    begin
        IntegrationFieldMapping.CreateRecord(IntegrationTableMappingName, TableFieldNo, IntegrationTableFieldNo, SynchDirection,
            ConstValue, ValidateField, ValidateIntegrationTableField);
    end;
}

tableextension 50101 ContactExt extends "CRM Contact"
{
    Description = 'Person with whom a business unit has a relationship, such as customer, supplier, and colleague.';

    fields
    {
        field(50101; cre59_Industry; Text[100])
        {
            ExternalName = 'cre59_industry';
            ExternalType = 'String';
            Description = '';
            Caption = 'Industry';
        }
    }
}