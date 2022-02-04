pageextension 50100 "Industry Synch Extension" extends "Industry Groups"
{
    actions
    {
        addlast(navigation)
        {
            group(ActionGroupDataverse)
            {
                Caption = 'Dataverse';
                Visible = CDSIntegrationEnabled;
                action(CDSSynchronizeNow)
                {
                    Caption = 'Synchronize';
                    ApplicationArea = All;
                    Visible = true;
                    Image = Refresh;
                    ToolTip = 'Send or get updated data to or from Dataverse.';

                    trigger OnAction()
                    var
                        IndustryGroup: Record "Industry Group";
                        CRMIntegrationManagement: Codeunit "CRM Integration Management";
                        IndustryGroupRecordRef: RecordRef;
                    begin
                        CurrPage.SetSelectionFilter(IndustryGroup);
                        IndustryGroupRecordRef.GetTable(IndustryGroup);
                        CRMIntegrationManagement.UpdateMultipleNow(IndustryGroupRecordRef, true);
                    end;
                }
                group(Coupling)
                {
                    Caption = 'Coupling';
                    Image = LinkAccount;
                    ToolTip = 'Create, change, or delete a coupling between the Business Central record and a Dataverse record.';

                    action(ManageCDSCoupling)
                    {
                        Caption = 'Set Up Coupling';
                        ApplicationArea = All;
                        Visible = true;
                        Image = LinkAccount;
                        ToolTip = 'Create or modify the coupling to a Dataverse Industry.';

                        trigger OnAction()
                        var
                            CRMIntegrationManagement: Codeunit "CRM Integration Management";
                        begin
                            CRMIntegrationManagement.DefineOptionMapping(Rec.RecordId);
                        end;
                    }
                    action(DeleteCDSCoupling)
                    {
                        Caption = 'Delete Coupling';
                        ApplicationArea = All;
                        Visible = true;
                        Image = UnLinkAccount;
                        Enabled = CDSIsCoupledToRecord;
                        ToolTip = 'Delete the coupling to a Dataverse Industry.';

                        trigger OnAction()
                        var
                            IndustryGroup: Record "Industry Group";
                            CRMIntegrationManagement: Codeunit "CRM Integration Management";
                            RecRef: RecordRef;
                        begin
                            CurrPage.SetSelectionFilter(IndustryGroup);
                            RecRef.GetTable(IndustryGroup);
                            CRMIntegrationManagement.RemoveOptionMapping(RecRef);
                        end;
                    }
                }
                action(ShowLog)
                {
                    Caption = 'Synchronization Log';
                    ApplicationArea = All;
                    Visible = true;
                    Image = Log;
                    ToolTip = 'View integration synchronization jobs for the industry group table.';

                    trigger OnAction()
                    var
                        CRMIntegrationManagement: Codeunit "CRM Integration Management";
                    begin
                        CRMIntegrationManagement.ShowOptionLog(Rec.RecordId);
                    end;
                }
            }
        }
    }

    trigger OnOpenPage()
    var
        CRMIntegrationManagement: Codeunit "CRM Integration Management";
    begin
        CDSIntegrationEnabled := CRMIntegrationManagement.IsCDSIntegrationEnabled() and CRMIntegrationManagement.IsOptionMappingEnabled();
    end;

    trigger OnAfterGetCurrRecord()
    var
        CRMOptionMapping: Record "CRM Option Mapping";
    begin
        if CDSIntegrationEnabled then begin
            CRMOptionMapping.SetRange("Record ID", Rec.RecordId);
            CDSIsCoupledToRecord := not CRMOptionMapping.IsEmpty();
        end;
    end;

    var
        CDSIntegrationEnabled: Boolean;
        CDSIsCoupledToRecord: Boolean;

}