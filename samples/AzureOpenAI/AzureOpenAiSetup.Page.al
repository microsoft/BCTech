page 50100 "Azure OpenAi Setup"
{
    Caption = 'Azure OpenAI Setup';
    PageType = StandardDialog;
    ApplicationArea = All;
    UsageCategory = Administration;

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                field(Endpoint; Endpoint)
                {
                    ApplicationArea = All;
                    Caption = 'Endpoint';
                    ToolTip = 'Specifies the completions endpoint of the LLM';

                    trigger OnValidate()
                    var
                        AzureOpenAi: Codeunit "Azure OpenAi";
                    begin
                        AzureOpenAi.SetEndpoint(Endpoint);
                        AzureOpenAi.SetSecret('');
                        Clear(Secret);
                    end;
                }

                field(Secret; Secret)
                {
                    ApplicationArea = All;
                    Caption = 'Secret';
                    ToolTip = 'Sepcifies the secret to connect to the LLM';
                    ExtendedDatatype = Masked;

                    trigger OnValidate()
                    var
                        AzureOpenAi: Codeunit "Azure OpenAi";
                    begin
                        AzureOpenAi.SetSecret(Secret);
                    end;
                }

                field(TestConnection; TestConnectionTxt)
                {
                    ApplicationArea = All;
                    ShowCaption = false;
                    Editable = false;

                    trigger OnDrillDown()
                    var
                        AzureOpenAi: Codeunit "Azure OpenAi";
                    begin
                        AzureOpenAi.TestPrompt();
                    end;
                }
            }
        }
    }

    trigger OnOpenPage()
    var
        AzureOpenAi: Codeunit "Azure OpenAi";
    begin
        Endpoint := AzureOpenAi.GetEndpoint();
        if AzureOpenAi.HasSecret() then
            Secret := '***';
    end;

    var
        Endpoint: Text;
        Secret: Text;
        TestConnectionTxt: Label 'Test the connection to Azure OpenAI';
}