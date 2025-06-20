namespace DefaultPublisher.DraftProject;
using System.AI;

page 50100 "Draft Project"
{
    PageType = PromptDialog;
    ApplicationArea = All;
    Caption = 'Draft with Copilot';
    PromptMode = Prompt;
    Extensible = false;

    layout
    {
        area(Prompt)
        {
            field(ProjectDescriptionField; InputProjectDescription)
            {
                ShowCaption = false;
                MultiLine = true;
                InstructionalText = 'Please describe the project or work to be done that needs to be converted into tasks';
            }
        }

        area(PromptOptions)
        {
            // In PromptDialog pages, you can define a PromptOptions area. Here you can add different settings to tweak the output that Copilot will generate.
            // These settings must be defined as page fields, and must be of type Option or Enum. You cannot define groups in this area.
        }

        area(Content)
        {
            field("Project Tasks"; OutputProjectTasks)
            {
                MultiLine = true;
                ExtendedDatatype = RichContent;
                Editable = false;
            }
        }
    }

    actions
    {
        area(SystemActions)
        {
            systemaction(Generate)
            {
                trigger OnAction()
                var
                    SystemPrompt: Text;
                begin
                    SystemPrompt := 'The user will describe a project. Your task is to prepare the project plan for this project to be used in Microsoft Dynamics 365 Business Central.'
                        + 'The output should be html formatted bulleted list. '
                        + 'Generate at least 6 tasks.'
                        + 'Order the tasks in order of execution.'
                        + 'If a task needs an item format the item name with bold.'
                        + 'Add relevant emoji to each task.'
                        + 'If a tasks needs a person format task with underline.';

                    OutputProjectTasks := CreateSuggestion(SystemPrompt, InputProjectDescription);
                end;
            }
        }
        area(PromptGuide)
        {
            action("AOrganizeEvent")
            {
                Caption = 'Organize an event';
                trigger OnAction()
                begin
                    InputProjectDescription += 'Plan for organizing a [project name] for the attendees of [event name]';
                end;
            }
        }
    }

    var
        InputProjectDescription: Text;
        OutputProjectTasks: Text;

    local procedure CreateSuggestion(SystemPrompt: Text; ProjectDescription: Text): Text
    var
        CopilotCapability: Codeunit "Copilot Capability";
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIDeployments: Codeunit "AOAI Deployments";
        AOAIChatCompletionParams: Codeunit "AOAI Chat Completion Params";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Project Task Creation") then
            CopilotCapability.RegisterCapability(Enum::"Copilot Capability"::"Project Task Creation", 'https://about:none');

        // If you are using managed resources, call this function:
        // NOTE: account name and key are only used to verify that you have a valid Azure OpenAI subscription; we don't use them to generate the result
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions",
            GetAccountName(), GetApiKey(), AOAIDeployments.GetGPT4oLatest());
        // If you are using your own Azure OpenAI subscription, call this function instead:
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", GetEndpoint(), GetDeployment(), GetApiKey());
        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Project Task Creation");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);

        AOAIChatMessages.AddSystemMessage(SystemPrompt);
        AOAIChatMessages.AddUserMessage(ProjectDescription);

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);
        if (AOAIOperationResponse.IsSuccess()) then
            exit(AOAIChatMessages.GetLastMessage());

        case AOAIOperationResponse.GetStatusCode() of
            402: // Payment Required
                Error('Your Entra Tenant ran out of AI quota. '
                    + 'Make sure your Business Central environment is linked to a Power Platform environment, and billing is set up correctly. '
                    + 'Consult the Business Central documentation for more information.');
            429: // Too many requests
                Error('You have been using Copilot very fast! '
                    + 'So fast that we suspect you might have some automation or scheduled task that calls Copilot a lot. '
                    + 'Have a look at your Job Queues, scheduled tasks, and automations, and make sure that everything looks fine. '
                    + 'And don''t worry, you''ll be able to use Copilot again in less than a minute!');
            503: // Service Unavailable (very very rare)
                Error('It seems like our services are under heavy load right now. '
                    + 'This happens very rarely, and our engineers are notified whenever this happens. '
                    + 'We are probably already working on it as soon as you are done reading this message!');
            else // Others
                exit('A generic error occurred: ' + AOAIOperationResponse.GetError());
        end;
    end;

    local procedure GetApiKey(): SecretText
    begin
        // Use your Azure Open AI secret key.
        // NOTE: Do not add the key in plain text. Instead, use Isolated Storage or other more secure ways.
        exit(Format(CreateGuid()));
    end;

    local procedure GetAccountName(): Text
    begin
        // Use your Azure OpenAI account name (the first part of your own Azure OpenAI url, for example for "MyPartner.openai.azure.com" use "MyPartner"
        exit('MyPartner');
    end;

}