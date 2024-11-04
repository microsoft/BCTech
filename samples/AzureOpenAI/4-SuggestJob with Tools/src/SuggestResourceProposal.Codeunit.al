codeunit 54394 "Suggest Resource Proposal"
{
    trigger OnRun()
    begin
        GenerateResourceProposal();
    end;

    procedure SetUserPrompt(InputUserPrompt: Text)
    begin
        UserPrompt := InputUserPrompt;
    end;

    procedure SetTask(InputJobDescription: Text; InputJobRoleDescription: Text)
    begin
        JobDescription := InputJobDescription;
        JobRoleDescription := InputJobRoleDescription;
    end;

    procedure GetResult(var TempResourceSuggestion2: Record "Resource Proposal" temporary)
    begin
        TempResourceSuggestion2.Copy(TempResourceSuggestion, true);
    end;

    procedure GenerateResourceProposal()
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIDeployments: Codeunit "AOAI Deployments";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAIChatCompletionParams: Codeunit "AOAI Chat Completion Params";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        SuggestResourceTool: Codeunit "Suggest Resource";
        SuggesrJobCreateJobTask: Codeunit "SuggestJob - Create Job Task";
    begin
        // If you are using managed resources, call this function:
        // NOTE: endpoint, deployment, and key are only used to verify that you have a valid Azure OpenAI subscription; we don't use them to generate the result
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions",
            GetEndpoint(), GetDeployment(), GetApiKey(), AOAIDeployments.GetGPT4oLatest());
        // If you are using your own Azure OpenAI subscription, call this function instead:
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", GetEndpoint(), GetDeployment(), GetApiKey());

        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Suggest Project");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);

        AOAIChatMessages.AddSystemMessage(GetSystemPrompt());
        AOAIChatMessages.AddUserMessage(GetUserPrompt());

        AOAIChatMessages.AddTool(SuggestResourceTool);
        AOAIChatMessages.SetToolInvokePreference("AOAI Tool Invoke Preference"::"Automatic");
        AOAIChatMessages.SetToolChoice('auto');

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);

        if not AOAIOperationResponse.IsSuccess() then
            Error(AOAIOperationResponse.GetError());

        SuggestResourceTool.GetResourceProposal(TempResourceSuggestion);
    end;

    local procedure GetSystemPrompt() SystemPrompt: Text
    begin
        SystemPrompt := 'The user will provide a task description, a list of resources, and optionally some additional notes. ' +
        'Your must call the "suggest_resource" function to suggest 3 resources.';
    end;

    local procedure GetUserPrompt() OutputUserPrompt: Text
    var
        Resource: Record Resource;
        Newline: Char;
        LeftoverCapacity: Decimal;
    begin
        Newline := 10;
        OutputUserPrompt := 'Here are all the resources: ' + Newline;

        if Resource.FindSet() then
            repeat
                OutputUserPrompt += 'Number: ' + Resource."No." + '; ';
                OutputUserPrompt += 'Name:' + Resource.Name + ' ' + Resource."Name 2" + ';';
                OutputUserPrompt += 'Job:' + Resource."Job Title" + '; ';
                OutputUserPrompt += 'Skills:' + MakeResourceSkillList(Resource) + '; ';
                LeftoverCapacity := CalculateResourceCapacity(Resource);
                if LeftoverCapacity > 0.0 then
                    OutputUserPrompt += 'Resource "' + Resource."No." + '" has capacity.'
                else
                    OutputUserPrompt += 'Resource "' + Resource."No." + '" has limited capacity!';

                OutputUserPrompt += Newline
            until Resource.Next() = 0;

        OutputUserPrompt += Newline;
        OutputUserPrompt += StrSubstNo('This is the description of the task: %1.', JobDescription);
        OutputUserPrompt += StrSubstNo('This is the description of the role needed: %1.', JobRoleDescription);
        OutputUserPrompt += StrSubstNo('This is the additional notes: %1.', UserPrompt);
    end;

    local procedure MakeResourceSkillList(Resource: Record Resource) SkillText: Text
    var
        ResourceSkill: Record "Resource Skill";
        SkillCode: Record "Skill Code";
    begin
        ResourceSkill.SetRange("No.", Resource."No.");
        if ResourceSkill.FindSet() then
            repeat
                if SkillCode.Get(ResourceSkill."Skill Code") then
                    SkillText += SkillCode.Description.Replace('&', '') + ',';
            until ResourceSkill.Next() = 0;

        SkillText := DelChr(SkillText, '>', ',')
    end;

    local procedure CalculateResourceCapacity(Resource: Record Resource) UnusedCapacity: Decimal
    var
        DateFilterCalc: Codeunit "DateFilter-Calc";
        ResDateFilter: Text[30];
        ResDateName: Text[30];
        CurrentDate: Date;
        TotalUsageUnits: Decimal;
        Chargeable: Boolean;
        ResCapacity: Decimal;
        j: Integer;
    begin
        if CurrentDate <> WorkDate() then begin
            CurrentDate := WorkDate();
            DateFilterCalc.CreateFiscalYearFilter(ResDateFilter, ResDateName, CurrentDate, 0);
        end;

        Clear(TotalUsageUnits);

        Resource.SetFilter("Date Filter", ResDateFilter);
        Resource.SetRange("Chargeable Filter");
        Resource.CalcFields(Capacity, "Usage (Cost)", "Sales (Price)");

        ResCapacity := Resource.Capacity;

        for j := 1 to 2 do begin
            if j = 1 then
                Chargeable := false
            else
                Chargeable := true;
            Resource.SetRange("Chargeable Filter", Chargeable);
            Resource.CalcFields("Usage (Qty.)", "Usage (Price)");
            TotalUsageUnits := TotalUsageUnits + Resource."Usage (Qty.)";
        end;

        UnusedCapacity := ResCapacity - TotalUsageUnits;
    end;

    local procedure GetApiKey(): SecretText
    begin
        // Use your Azure Open AI secret key.
        // NOTE: Do not add the key in plain text. Instead, use Isolated Storage or other more secure ways.
        exit(Format(CreateGuid()));
    end;

    local procedure GetDeployment(): Text
    begin
        // Use your deployment name from Azure Open AI here
        exit('gpt-' + CreateGuid());
    end;

    local procedure GetEndpoint(): Text
    begin
        // Use your endpoint name from Azure Open AI here
        exit('https://my-deployment.azure.com/');
    end;


    var
        TempResourceSuggestion: Record "Resource Proposal" temporary;
        UserPrompt: Text;
        JobDescription: Text;
        JobRoleDescription: Text;
}