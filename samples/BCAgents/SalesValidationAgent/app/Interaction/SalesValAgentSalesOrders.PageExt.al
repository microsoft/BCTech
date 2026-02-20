namespace ThirdPartyPublisher.SalesValidationAgent.Interaction;

using Microsoft.Sales.Document;
using System.Agents;
using ThirdPartyPublisher.SalesValidationAgent.Setup;

/// <summary>
/// Extends the Sales Order List page to enable Sales Validation Agent task assignment.
/// Adds an action that allows users to trigger the agent to validate and process open sales orders.
/// </summary>
pageextension 50101 "Sales Val. Agent Sales Orders" extends "Sales Order List"
{
    actions
    {
        addlast(processing)
        {
            action(ValidateWithAgent)
            {
                Caption = 'Validate with Agent';
                ToolTip = 'Assign a validation task to the Sales Validation Agent to process open orders for a specific date.';
                Image = Task;
                ApplicationArea = All;

                trigger OnAction()
                var
                    AgentTask: Record "Agent Task";
                    Agent: Codeunit Agent;
                    AgentTaskBuilder: Codeunit "Agent Task Builder";
                    SalesValAgentSetup: Codeunit "Sales Val. Agent Setup";
                    DatePicker: Page "Sales Val. Agent Date Picker";
                    AgentUserSecurityId: Guid;
                    TaskTitle: Text[150];
                    From: Text[250];
                    Message: Text;
                    ShipmentDate: Date;
                    ExternalId: Text;
                begin
                    IF not SalesValAgentSetup.TryGetAgent(AgentUserSecurityId) then
                        Error(SVAgentDoesNotExistErr);

                    if not Agent.IsActive(AgentUserSecurityId) then
                        Error(SVAgentNotActiveErr);

                    DatePicker.SetShipmentDate(WorkDate());
                    if DatePicker.RunModal() <> Action::OK then
                        exit;

                    ShipmentDate := DatePicker.GetShipmentDate();
                    if ShipmentDate = 0D then
                        Error(ShipmentDateRequiredErr);

                    Message := StrSubstNo(TaskMessageLbl, ShipmentDate);
                    TaskTitle := CopyStr(StrSubstNo(TaskTitleLbl, ShipmentDate), 1, MaxStrLen(TaskTitle));
                    From := CopyStr(UserId(), 1, MaxStrLen(From));

                    AgentTaskBuilder.Initialize(AgentUserSecurityId, TaskTitle);
                    ExternalId := Format(CreateGuid());
                    AgentTaskBuilder.SetExternalId(ExternalId);
                    AgentTaskBuilder.AddTaskMessage(From, Message);
                    AgentTaskBuilder.Create();

                    AgentTask.ReadIsolation(IsolationLevel::ReadCommitted);
                    AgentTask.SetRange("External ID", ExternalId);
                    if not AgentTask.FindFirst() then
                        Error(TaskCreateFailedErr);

                    Message(TaskAssignedMsg, AgentTask.ID, ShipmentDate);
                end;
            }
        }
    }

    var
        SVAgentDoesNotExistErr: Label 'The Sales Validation Agent has not been created.';
        SVAgentNotActiveErr: Label 'The Sales Validation Agent is not active.';
        ShipmentDateRequiredErr: Label 'A shipment date must be specified.';
        TaskCreateFailedErr: Label 'The agent task could not be created.';
        TaskMessageLbl: Label 'Run and process shipment date %1.', Locked = true, Comment = '%1 = Shipment Date';
        TaskTitleLbl: Label 'Validate Sales Orders for %1', Comment = '%1 = Shipment Date';
        TaskAssignedMsg: Label 'Task %1 assigned successfully to validate sales orders for date: %2.', Comment = '%1 = Task ID, %2 = Shipment Date';
}
