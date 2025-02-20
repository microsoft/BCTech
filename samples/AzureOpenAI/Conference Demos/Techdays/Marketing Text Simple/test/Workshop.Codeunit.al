codeunit 50201 Workshop
{
    Subtype = Test;
    TestPermissions = Disabled;
    InherentEntitlements = X;
    InherentPermissions = X;

    [Test]
    [HandlerFunctions('HandleCopilotMarketingTextPage')]
    procedure TestMarketingText()
    var
        TestContext: Codeunit "AIT Test Context";
        ItemNo: Code[20];
        ItemTestPage: TestPage "Item Card";
    begin
        ItemTestPage.OpenEdit();
        ItemNo := TestContext.GetTestSetup().Element('item_no').ValueAsText();
        ItemTestPage.GoToKey(ItemNo);
        ItemTestPage.EntityTextFactBox.Suggest.Invoke();
    end;

    [ModalPageHandler]
    procedure HandleCopilotMarketingTextPage(var CopilotMarketingTextTestPage: TestPage "Copilot Marketing Text")
    var
        AITestContext: Codeunit "AIT Test Context";
        TestJsonOutput: Codeunit "Test Output Json";
    begin
        CopilotMarketingTextTestPage.Generate.Invoke();
        TestJsonOutput.Initialize();
        TestJsonOutput.Add('marketing_text', CopilotMarketingTextTestPage.ItemText.Value);
        AITestContext.SetTestOutput(TestJsonOutput.ToText());
    end;
}