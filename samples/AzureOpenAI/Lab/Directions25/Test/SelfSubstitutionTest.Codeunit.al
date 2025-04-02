namespace CopilotToolkitDemo.ItemSubstitution;

using System.TestTools.TestRunner;
using System.TestTools.AITestToolkit;
using System.Utilities;

codeunit 54325 "SelfSubstitutionTest"
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
        ItemNo: Text;
    begin
        // Set up the test context
        GenerateItemSubProposal.SetUserPrompt(AITestContext.GetInput().Element('data').Element('description').ValueAsText());
        ItemNo := AITestContext.GetInput().Element('data').Element('number').ValueAsText();

        TmpItemSubstAIProposal.Reset();
        TmpItemSubstAIProposal.DeleteAll();

        Attempts := 0;
        while TmpItemSubstAIProposal.IsEmpty and (Attempts < 3) do begin
            if GenerateItemSubProposal.Run() then
                GenerateItemSubProposal.GetResult(TmpItemSubstAIProposal);
            Attempts += 1;
        end;

        AITestContext.SetTestOutput(GenerateItemSubProposal.GetCompletionResult());

        // Check if the proposal is not empty
        if (Attempts < 2) then begin
            if TmpItemSubstAIProposal.FindSet() then begin
                repeat
                    if TmpItemSubstAIProposal."No." = ItemNo then
                        Error('Suggested replacement item is the same as the original item.');
                until TmpItemSubstAIProposal.Next() = 0;
            end
        end else
            Error(GetLastErrorText());
    end;
}