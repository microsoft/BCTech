namespace CopilotToolkitDemo.ItemSubstitution;

using System.TestTools.TestRunner;
using System.TestTools.AITestToolkit;
using System.Utilities;

codeunit 54324 "ItemSubstitutionTest"
{
    Subtype = Test;
    TestPermissions = Disabled;

    [Test]
    procedure TestItemSubWorking()
    var
        AITestContext: Codeunit "AIT Test Context";
        GenerateItemSubProposal: Codeunit "Generate Item Sub Proposal";
        TmpItemSubstAIProposal: Record "Copilot Item Sub Proposal" temporary;
        Attempts: Integer;
    begin
        // The AITestContext provides access to test data for every test case.
        // You can access the parts of your dataset entry, such as the question and expected data.
        // The test context is automatically set up for each test case.
        // You can also set the test output for the test case using the AITestContext.SetTestOutput() method.

        // Set the user prompt for substitution using the question from the test dataset entry
        GenerateItemSubProposal.SetUserPrompt(AITestContext.GetQuestion().ValueAsText());

        TmpItemSubstAIProposal.Reset();
        TmpItemSubstAIProposal.DeleteAll();

        Attempts := 0;
        while TmpItemSubstAIProposal.IsEmpty and (Attempts < 2) do begin
            if GenerateItemSubProposal.Run() then
                GenerateItemSubProposal.GetResult(TmpItemSubstAIProposal);
            Attempts += 1;
        end;

        // It is a good practice to have an method of accessing the output for testing.
        // Set the test output for the test case using the completion result from the codeunit
        AITestContext.SetTestOutput(GenerateItemSubProposal.GetCompletionResult());

        if (Attempts < 2) then begin
            // Check if the proposal contains at least one item
            if TmpItemSubstAIProposal.IsEmpty() then
                Error('No item substitutions found.');
        end else
            Error(GetLastErrorText());
    end;

    [Test]
    procedure TestSubstitutionAvailableItems()
    var
        AITestContext: Codeunit "AIT Test Context";
        GenerateItemSubProposal: Codeunit "Generate Item Sub Proposal";
        TmpItemSubstAIProposal: Record "Copilot Item Sub Proposal" temporary;
        ExpectedItemNos: Text;
        Attempts: Integer;
    begin
        // Get the expected item numbers from the test context
        ExpectedItemNos := AITestContext.GetExpectedData().ValueAsText();

        // Set the user prompt for substitution using the question from the test dataset entry
        GenerateItemSubProposal.SetUserPrompt(AITestContext.GetQuestion().ValueAsText());

        // Set the option to suggest only available items
        GenerateItemSubProposal.SetSuggestOnlyAvailableItems();

        TmpItemSubstAIProposal.Reset();
        TmpItemSubstAIProposal.DeleteAll();

        Attempts := 0;
        while TmpItemSubstAIProposal.IsEmpty and (Attempts < 2) do begin
            if GenerateItemSubProposal.Run() then
                GenerateItemSubProposal.GetResult(TmpItemSubstAIProposal);
            Attempts += 1;
        end;

        // Set the test output for the test case using the completion result from the codeunit
        AITestContext.SetTestOutput(GenerateItemSubProposal.GetCompletionResult());

        if (Attempts < 2) then begin
            // Check if the proposal contains at least one item
            if TmpItemSubstAIProposal.IsEmpty() then
                Error('No item substitutions found.');

            // Check if the proposal contains valid item numbers
            if TmpItemSubstAIProposal.FindSet() then begin
                repeat
                    if not ExpectedItemNos.Contains(TmpItemSubstAIProposal."No.") then
                        Error('Item with negative inventory found: ' + TmpItemSubstAIProposal."No." + ', Quantity: ' + Format(TmpItemSubstAIProposal.Quantity));
                until TmpItemSubstAIProposal.Next() = 0;
            end else
                Error('Failed to retrieve item substitutions from the proposal.');
        end else
            Error(GetLastErrorText());
    end;
}