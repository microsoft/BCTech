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
        SubstitutedItemNo: Text;
    begin
        // Since this test dataset entry has its input properties in the 'data' element,
        // We will access it using the AITestContext.GetInput() method, pointing to the corresponding elements.
        GenerateItemSubProposal.SetUserPrompt(AITestContext.GetInput().Element('data').Element('description').ValueAsText());
        SubstitutedItemNo := AITestContext.GetInput().Element('data').Element('number').ValueAsText();

        TmpItemSubstAIProposal.Reset();
        TmpItemSubstAIProposal.DeleteAll();

        Attempts := 0;
        while TmpItemSubstAIProposal.IsEmpty and (Attempts < 3) do begin
            if GenerateItemSubProposal.Run() then
                GenerateItemSubProposal.GetResult(TmpItemSubstAIProposal);
            Attempts += 1;
        end;

        // Set the test output for the test case using the completion result from the codeunit
        AITestContext.SetTestOutput(GenerateItemSubProposal.GetCompletionResult());

        if (Attempts < 2) then begin
            if TmpItemSubstAIProposal.FindSet() then begin
                repeat
                    if TmpItemSubstAIProposal."No." = SubstitutedItemNo then
                        Error('Suggested replacement item is the same as the original item.');
                until TmpItemSubstAIProposal.Next() = 0;
            end
        end else
            Error(GetLastErrorText());
    end;
}