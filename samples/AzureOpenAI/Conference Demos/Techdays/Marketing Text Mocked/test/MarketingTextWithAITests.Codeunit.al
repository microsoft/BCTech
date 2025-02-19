namespace Techdays.AITestToolkitDemo;

using System.TestTools.TestRunner;
using System.TestLibraries.AI;
using System.AI;
using Microsoft.Inventory.Item;
using System.TestTools.AITestToolkit;

codeunit 50200 "Marketing Text With AI Tests"
{
    Subtype = Test;
    TestPermissions = Disabled;

    var
        IsInitialized: Boolean;
        StyleEnum: Enum "Marketing Text Style";
        MarketingTextSimpleAppIdLbl: Label '74d86897-1c24-4889-9335-44449c0938a1';

    [Test]
    procedure TestTagLineLength()
    var
        TestContext: Codeunit "AIT Test Context";
        MarketingTextWithAI: Codeunit "Marketing Text With AI";
        TagLine: Text;
        MaxLength: Integer;
        ItemNo: Code[20];
        TaglineLengthErr: Label 'Tagline: %1; Length: %2; Exceeds maximum lenght of %3', Comment = '%1 = Tagline, %2 = Length, %3 = Maximum Length';
    begin
        // [GIVEN] The setup to create an item and required maximum length
        Initialize();
        ItemNo := CopyStr(TestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));
        MaxLength := 20;

        // [WHEN] Generating the tagline with required maximum length
        TagLine := MarketingTextWithAI.GenerateTagLine(ItemNo, MaxLength);

        // [THEN] The tagline should be within the required maximum length
        if StrLen(TagLine) > MaxLength then
            Error(TaglineLengthErr, ItemNo, StrLen(TagLine), MaxLength);
    end;

    local procedure Initialize()
    var
        CopilotTestLibrary: Codeunit "Copilot Test Library";
    begin
        if IsInitialized then
            exit;

        // Register the feature for the test. This is useful if the capability is not activated in the tenant.
        CopilotTestLibrary.RegisterCopilotCapabilityWithAppId(Enum::"Copilot Capability"::"Marketing Text Simple", MarketingTextSimpleAppIdLbl);

        // Setup the items needed for the test
        CreateItem();

        IsInitialized := true;
    end;

    local procedure CreateItem()
    var
        Item: Record Item;
        TestContext: Codeunit "AIT Test Context";
    begin
        Item."No." := CopyStr(TestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(Item."No."));
        Item.Description := CopyStr(TestContext.GetTestSetup().Element('description').ValueAsText(), 1, MaxStrLen(Item.Description));
        Item."Base Unit of Measure" := CopyStr(TestContext.GetTestSetup().Element('uom').ValueAsText(), 1, MaxStrLen(Item."Base Unit of Measure"));
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
        Initialize();
        ItemNo := CopyStr(TestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));

        // [WHEN] Generating the marketing text with formal tone
        MarketingText := MarketingTextWithAI.GenerateMarketingText(ItemNo, Style);

        // [THEN] Return the marketing text for external evaluation
        // Groundedness Evaluator: Measures how well the generated response aligns with the given context, focusing on its relevance and accuracy with respect to the context.
        // Groundedness evaluator requires: query, response, context

        TestOutputJson.Initialize();
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

        TestOutputJson.Add('query', QueryTxt);
        TestOutputJson.Add('response', MarketingText);

        ContextOutputJson.Initialize('{}');
        ContextOutputJson.Add('item_no', ItemNo);
        Item.Get(ItemNo);
        ContextOutputJson.Add('description', Item.Description);
        ContextOutputJson.Add('uom', Item."Base Unit of Measure");

        TestOutputJson.Add('context', ContextOutputJson.ToText()); // Context on the basis of which the response is generated

        TestContext.SetTestOutput(TestOutputJson.ToText());












        // Another way of setting the test output
        // TestContext.SetAnswerForQnAEvaluation(MarketingText);
        // query and context is copied from the dataset
    end;
}