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
        // Set up the test context
        GenerateItemSubProposal.SetUserPrompt(AITestContext.GetQuestion().ValueAsText());

        TmpItemSubstAIProposal.Reset();
        TmpItemSubstAIProposal.DeleteAll();

        Attempts := 0;
        while TmpItemSubstAIProposal.IsEmpty and (Attempts < 2) do begin
            if GenerateItemSubProposal.Run() then
                GenerateItemSubProposal.GetResult(TmpItemSubstAIProposal);
            Attempts += 1;
        end;

        // Check if the proposal is not empty
        if (Attempts < 2) then begin
            // Check if the proposal contains at least one item
            if TmpItemSubstAIProposal.IsEmpty() then
                Error('No item substitutions found.');
        end else
            Error(GetLastErrorText());
    end;

    [Test]
    procedure TestSubstitutionAccuracy()
    var
        AITestContext: Codeunit "AIT Test Context";
        GenerateItemSubProposal: Codeunit "Generate Item Sub Proposal";
        TmpItemSubstAIProposal: Record "Copilot Item Sub Proposal" temporary;
        ExpectedItemNos: Text;
        Attempts: Integer;
    begin
        // Get the expected item numbers from the test context
        ExpectedItemNos := AITestContext.GetExpectedData().ValueAsText();

        // Set up the test context
        GenerateItemSubProposal.SetUserPrompt(AITestContext.GetQuestion().ValueAsText());

        TmpItemSubstAIProposal.Reset();
        TmpItemSubstAIProposal.DeleteAll();

        Attempts := 0;
        while TmpItemSubstAIProposal.IsEmpty and (Attempts < 2) do begin
            if GenerateItemSubProposal.Run() then
                GenerateItemSubProposal.GetResult(TmpItemSubstAIProposal);
            Attempts += 1;
        end;

        // Check if the proposal is not empty
        if (Attempts < 2) then begin
            // Check if the proposal contains at least one item
            if TmpItemSubstAIProposal.IsEmpty() then
                Error('No item substitutions found.');

            // Check if the proposal contains valid item numbers
            if TmpItemSubstAIProposal.FindSet() then begin
                repeat
                    if not ExpectedItemNos.Contains(TmpItemSubstAIProposal."No.") then
                        Error('Invalid item number found in the proposal.');
                until TmpItemSubstAIProposal.Next() = 0;
            end else
                Error('Failed to retrieve item substitutions from the proposal.');
        end else
            Error(GetLastErrorText());
    end;
}