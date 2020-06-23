// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

pageextension 51101 IoTWorkflowRCExt extends "Azure IoT Central RC"
{
    layout
    {
        // Add changes to page layout here
    }

    actions
    {
        // Add changes to page actions here
        addafter(ConnectionSetup)
        {
            action(IoTDeviceWFSetup)
            {
                ApplicationArea = All;
                Caption = 'Device Workflow Setup';
                RunObject = Page "IoT Device Workflow Setup";
                RunPageMode = View;
            }
            action(Workflows)
            {
                ApplicationArea = All;
                Caption = 'Workflows';
                RunObject = Page Workflows;
                RunPageMode = View;
            }
            action(ApprovalUsers)
            {
                ApplicationArea = All;
                Caption = 'Approval User Setup';
                RunObject = Page "Approval User Setup";
                RunPageMode = View;
            }
        }
    }

    var
        myInt: Integer;
}