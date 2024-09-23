namespace CopilotToolkitDemo.SuggestResource;

using CopilotToolkitDemo.Common;
using System.IO;
using System.Utilities;
using Microsoft.Service.Setup;
using Microsoft.Service.Resources;
using Microsoft.Foundation.Period;
using Microsoft.Projects.Resources.Resource;

codeunit 54321 "Generate Resource Proposal"
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

    procedure GetResult(var TempCopilotResourceProposal2: Record "Copilot Resource Proposal" temporary)
    begin
        TempCopilotResourceProposal2.Copy(TempCopilotResourceProposal, true);
    end;

    local procedure GenerateResourceProposal()
    var
        TmpXmlBuffer: Record "XML Buffer" temporary;
        SimplifiedCopilotChat: Codeunit "Simplified Copilot Chat";
        TempBlob: Codeunit "Temp Blob";
        InStr: InStream;
        OutStr: OutStream;
        CurrInd, LineNo : Integer;
        DateVar: Date;
        TmpText: Text;
    begin
        TempBlob.CreateOutStream(OutStr);
        OutStr.WriteText(SimplifiedCopilotChat.Chat(GetSystemPrompt(), GetUserPrompt()));
        OutStr.WriteText(TmpText);
        TempBlob.CreateInStream(InStr);

        TmpXmlBuffer.DeleteAll();
        TmpXmlBuffer.LoadFromStream(InStr);

        Clear(OutStr);
        LineNo := 10000;
        if TmpXmlBuffer.FindSet() then
            repeat
                case TmpXmlBuffer.Path of
                    '/resources/resource':
                        TempCopilotResourceProposal.Init();
                    '/resources/resource/resourceNumber':
                        begin
                            TempCopilotResourceProposal."No." := UpperCase(CopyStr(TmpXmlBuffer.GetValue(), 1, 20));
                            TempCopilotResourceProposal.Insert();
                        end;
                    '/resources/resource/resourceName':
                        begin
                            TempCopilotResourceProposal.Name := CopyStr(TmpXmlBuffer.GetValue(), 1, 100);
                            TempCopilotResourceProposal.Modify();
                        end;
                    '/resources/resource/resourceJobTitle':
                        begin
                            TempCopilotResourceProposal."Job Title" := CopyStr(TmpXmlBuffer.GetValue(), 1, 30);
                            TempCopilotResourceProposal.Modify();
                        end;
                    '/resources/resource/explanation':
                        begin
                            TempCopilotResourceProposal.Explanation := CopyStr(TmpXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotResourceProposal.Explanation));
                            TempCopilotResourceProposal."Full Explanation".CreateOutStream(OutStr);
                            OutStr.WriteText(RemoveNewLineCharacters(TmpXmlBuffer.GetValue()));
                            TempCopilotResourceProposal.Modify();
                        end;
                end;
            until TmpXmlBuffer.Next() = 0;
    end;

    procedure RemoveNewLineCharacters(InputString: Text): Text
    var
        Regex: Codeunit Regex;
        OutputString: Text;
    begin
        OutputString := Regex.Replace(InputString, '/(\r\n|\n|\r)/gm', '');
        exit(OutputString);
    end;

    local procedure GetSystemPrompt() SystemPrompt: Text
    begin
        SystemPrompt += 'The user will provide a task description, a list of resources, and optionally some additional notes.' +
                   'Your goal is to suggest resources from the list that could do the described task and that have capacity.' +
                   'The output should be in xml. Use resources as a root level tag, use resourse as resource tag.' +
                   'Each resource should contain a resourceNumber tag, a resourceName tag, a resourceJobTitle tag, and an explanation tag explaining why this resource was suggested.' +
                   'Do not use line breaks or other special characters in explanation.' +
                   'Only include resources with capacity; if you include a resource with no capacity then add a warning about it in the explanation.';
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
                    OutputUserPrompt += 'Resource "' + Resource."No." + '" does not have capacity!';

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

    var
        TempCopilotResourceProposal: Record "Copilot Resource Proposal" temporary;
        UserPrompt: Text;
        JobDescription: Text;
        JobRoleDescription: Text;

}