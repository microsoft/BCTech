namespace Techdays.AITestToolkitDemo;

using System.TestTools.TestRunner;
using Microsoft.Inventory.Item;
using System.TestTools.AITestToolkit;

codeunit 50200 "Marketing Text With AI Tests"
{
    Subtype = Test;
    TestPermissions = Disabled;
    InherentEntitlements = X;
    InherentPermissions = X;

    var
        StyleEnum: Enum "Marketing Text Style";

    [Test]
    procedure TestTagLineLength()
    var
        AITTestContext: Codeunit "AIT Test Context";
        MarketingTextWithAI: Codeunit "Marketing Text With AI";
        TagLine: Text;
        MaxLength: Integer;
        ItemNo: Code[20];
        TaglineLengthErr: Label 'Tagline: %1; Length: %2; Exceeds maximum length of %3', Comment = '%1 = Tagline, %2 = Length, %3 = Maximum Length';
    begin
        // [GIVEN] The item and required maximum length
        CreateItem();
        ItemNo := CopyStr(AITTestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));
        MaxLength := AITTestContext.GetExpectedData().Element('tagline_max_length').ValueAsInteger();

        // [WHEN] Generating the tagline with required maximum length
        TagLine := MarketingTextWithAI.GenerateTagLine(ItemNo, MaxLength);

        // [THEN] The tagline should be within the required maximum length
        if StrLen(TagLine) > MaxLength then
            Error(TaglineLengthErr, TagLine, StrLen(TagLine), MaxLength);
    end;

    local procedure CreateItem()
    var
        Item: Record Item;
        AITTestContext: Codeunit "AIT Test Context";
    begin
        Item."No." := CopyStr(AITTestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(Item."No."));
        Item.Description := CopyStr(AITTestContext.GetTestSetup().Element('description').ValueAsText(), 1, MaxStrLen(Item.Description));
        Item."Base Unit of Measure" := CopyStr(AITTestContext.GetTestSetup().Element('uom').ValueAsText(), 1, MaxStrLen(Item."Base Unit of Measure"));

        if Item.Get(Item."No.") then
            Item.Modify()
        else
            Item.Insert();
    end;

    [Test]
    procedure TestMarketingTextContentFormal()
    begin
        TestScenario(StyleEnum::Formal);
    end;

    [Test]
    procedure TestMarketingTextContentVerbose()
    begin
        TestScenario(StyleEnum::Verbose);
    end;

    [Test]
    procedure TestMarketingTextContentCasual()
    begin
        TestScenario(StyleEnum::Casual);
    end;

    local procedure TestScenario(Style: Enum "Marketing Text Style")
    var
        Item: Record Item;
        TestContext: Codeunit "AIT Test Context";
        MarketingTextWithAI: Codeunit "Marketing Text With AI";
        TestOutputJson: Codeunit "Test Output Json";
        ContextOutputJson: Codeunit "Test Output Json";
        MarketingText: Text;
        QueryTxt: Text;
        ItemNo: Code[20];
    begin
        // [GIVEN] The setup to create an item
        CreateItem();
        ItemNo := CopyStr(TestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));

        // [WHEN] Generating the marketing text with formal tone
        MarketingText := MarketingTextWithAI.GenerateMarketingText(ItemNo, Style);

        // [THEN] Return the marketing text for external evaluation
        // Groundedness Evaluator (Azure AI Foundry): Measures how well the generated response aligns with the given context.
        // It requires: query, response, context

        // Query
        QueryTxt := 'Generate a marketing text for the given item in Business Central in ';
        case
            Style of
            StyleEnum::Formal:
                QueryTxt += '*Formal* Tone.';
            StyleEnum::Verbose:
                QueryTxt += '*Verbose* Tone.';
            StyleEnum::Casual:
                QueryTxt += '*Casual* Tone.';
        end;

        TestOutputJson.Initialize();
        TestOutputJson.Add('query', QueryTxt);

        // Context
        ContextOutputJson.Initialize();
        ContextOutputJson.Add('item_no', ItemNo);
        Item.Get(ItemNo);
        ContextOutputJson.Add('description', Item.Description);
        ContextOutputJson.Add('uom', Item."Base Unit of Measure");

        TestOutputJson.Add('context', ContextOutputJson.ToText()); // Context on the basis of which the response is generated

        // Response
        TestOutputJson.Add('response', MarketingText);

        TestContext.SetTestOutput(TestOutputJson.ToText());
    end;
}