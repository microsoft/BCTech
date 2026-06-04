// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Test;

using SalesReturnAgent.Setup;
using SalesReturnAgent.Test.Libraries;
using SalesReturnAgent.Test.Setup;
using System.Agents;
using System.TestLibraries.Agents;
using System.TestLibraries.Utilities;
using System.TestTools.AITestToolkit;
using System.TestTools.TestRunner;

codeunit 53745 "SR Accuracy Test"
{
    Subtype = Test;
    TestType = AITest; // Defines the test as an AI Eval.
    TestPermissions = Disabled;
    RequiredTestIsolation = Disabled;
    InherentEntitlements = X;
    InherentPermissions = X;

    local procedure Initialize()
    var
        SalesRetAgentSetup: Codeunit "Sales Ret. Agent Setup";
        AgentTestContext: Codeunit "Agent Test Context";
    begin
        if not Initialized then begin
            // Gets whether an agent has already been selected in the AI Eval Suite page.
            // If not, get or create a default agent.
            AgentTestContext.GetAgentUserSecurityID(AgentUserSecurityId);
            if IsNullGuid(AgentUserSecurityId) then
                AgentUserSecurityId := SalesRetAgentSetup.GetOrCreateDefaultAgent();

            // Run the per-suite test data setup only once.
            SetupPerSuiteTestData();
            Initialized := true;
            Commit();
        end;

        // Ensure the agent is active.
        LibraryAgent.EnsureAgentIsActive(AgentUserSecurityId);

        // Clear any existing data that might interfere with the test.
        SRTestLibrary.DeleteAllSalesCreditMemos();
        SRTestLibrary.DeleteAllUnpostedSalesInvoices();
        Commit();
    end;

    [Test]
    procedure TestAccuracy()
    var
        AgentTask: Record "Agent Task";
        SREventHandler: Codeunit "SR Event Handler";
        SRResourceProvider: Codeunit "SR Resource Provider";
        TurnSuccessful: Boolean;
        ErrorReason: Text;
        ContinueWithNextTurn: Boolean;
        AgentStatusErr: Label 'The agent task did not complete successfully. Task status: %1.', Comment = '%1 = task status';
    begin
        BindSubscription(SREventHandler);
        Initialize();

        repeat
            Clear(ErrorReason);
            SetupTurnData();

            TurnSuccessful := LibraryAgent.RunTurnAndWait(AgentUserSecurityId, AgentTask, SRResourceProvider);

            if TurnSuccessful then
                TurnSuccessful := ValidateTurn(ErrorReason)
            else
                ErrorReason := StrSubstNo(AgentStatusErr, AgentTask.Status);

            ContinueWithNextTurn := LibraryAgent.FinalizeTurn(AgentTask, TurnSuccessful, ErrorReason);
        until not ContinueWithNextTurn;

        Assert.IsTrue(TurnSuccessful, ErrorReason);
    end;

    local procedure ValidateTurn(var ErrorReason: Text): Boolean
    begin
        if not SRTestLibrary.ValidateCreditMemoCreated(ErrorReason) then
            exit(false);

        if not SRTestLibrary.ValidateWorkDescription(ErrorReason) then
            exit(false);

        exit(true);
    end;

    local procedure SetupTurnData()
    var
        TurnSetup: Codeunit "Test Input Json";
        TurnSetupExists: Boolean;
    begin
        TurnSetupExists := AITTestContext.GetTurnSetup(TurnSetup);
        if not TurnSetupExists then
            exit;

        SRTestLibrary.CreateCreditMemoTestData(TurnSetup);
    end;

    local procedure SetupPerSuiteTestData()
    var
        SuiteTestSetup: Codeunit "Test Input Json";
    begin
        if AITTestContext.IsSuiteSetupDone() then
            exit;

        SuiteTestSetup := AITTestContext.GetEvalSuiteSetupDataInput();

        SRTestLibrary.CreateCustomers(SuiteTestSetup);
        SRTestLibrary.CreateItems(SuiteTestSetup);
        SRTestLibrary.CreatePostedSalesInvoices(SuiteTestSetup);

        AITTestContext.SetEvalSuiteSetupCompleted();
    end;

    var
        Assert: Codeunit "Library Assert";
        LibraryAgent: Codeunit "Library - Agent";
        SRTestLibrary: Codeunit "SR Test Library";
        AITTestContext: Codeunit "AIT Test Context";
        AgentUserSecurityId: Guid;
        Initialized: Boolean;
}
