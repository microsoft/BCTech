pageextension 50101 "Employee Synch Extension" extends "Employee Card"
{
    actions
    {
        addlast(navigation)
        {
            group(ActionGroupCDS)
            {
                Caption = 'Common Data Service';
                Visible = CDSIntegrationEnabled;

                action(CDSSynchronizeNow)
                {
                    Caption = 'Synchronize';
                    ApplicationArea = All;
                    Visible = true;
                    Image = Refresh;
                    Enabled = CDSIsCoupledToRecord;
                    ToolTip = 'Send or get updated data to or from Common Data Service.';

                    trigger OnAction()
                    var
                        CRMIntegrationManagement: Codeunit "CRM Integration Management";
                    begin
                        CRMIntegrationManagement.UpdateOneNow(Rec.RecordId);
                    end;
                }
                action(ShowLog)
                {
                    Caption = 'Synchronization Log';
                    ApplicationArea = All;
                    Visible = true;
                    Image = Log;
                    ToolTip = 'View integration synchronization jobs for the employee table.';

                    trigger OnAction()
                    var
                        CRMIntegrationManagement: Codeunit "CRM Integration Management";
                    begin
                        CRMIntegrationManagement.ShowLog(Rec.RecordId);
                    end;
                }
                group(Coupling)
                {
                    Caption = 'Coupling';
                    Image = LinkAccount;
                    ToolTip = 'Create, change, or delete a coupling between the Business Central record and a Common Data Service record.';

                    action(ManageCDSCoupling)
                    {
                        Caption = 'Set Up Coupling';
                        ApplicationArea = All;
                        Visible = true;
                        Image = LinkAccount;
                        ToolTip = 'Create or modify the coupling to a Common Data Service worker.';

                        trigger OnAction()
                        var
                            CRMIntegrationManagement: Codeunit "CRM Integration Management";
                        begin
                            CRMIntegrationManagement.DefineCoupling(Rec.RecordId);
                        end;
                    }
                    action(DeleteCDSCoupling)
                    {
                        Caption = 'Delete Coupling';
                        ApplicationArea = All;
                        Visible = true;
                        Image = UnLinkAccount;
                        Enabled = CDSIsCoupledToRecord;
                        ToolTip = 'Delete the coupling to a Common Data Service worker.';

                        trigger OnAction()
                        var
                            CRMCouplingManagement: Codeunit "CRM Coupling Management";
                        begin
                            CRMCouplingManagement.RemoveCoupling(Rec.RecordId);
                        end;
                    }
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        CDSIntegrationEnabled := CRMIntegrationManagement.IsCDSIntegrationEnabled();
    end;

    trigger OnAfterGetCurrRecord()
    begin
        if CDSIntegrationEnabled then
            CDSIsCoupledToRecord := CRMCouplingManagement.IsRecordCoupledToCRM(Rec.RecordId);
    end;

    var
        CRMIntegrationManagement: Codeunit "CRM Integration Management";
        CRMCouplingManagement: Codeunit "CRM Coupling Management";
        CDSIntegrationEnabled: Boolean;
        CDSIsCoupledToRecord: Boolean;
}