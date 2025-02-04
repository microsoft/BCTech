namespace CopilotToolkitDemo.DescribeJob;

using CopilotToolkitDemo.Common;
using System.IO;
using System.Utilities;

codeunit 54320 "Generate Job Proposal"
{
    trigger OnRun()
    begin
        GenerateJobProposal();
    end;

    procedure SetUserPrompt(InputUserPrompt: Text)
    begin
        UserPrompt := InputUserPrompt;
    end;

    procedure GetResult(var InputTempCopilotJobProposal: Record "Copilot Job Proposal" temporary)
    begin
        InputTempCopilotJobProposal.Copy(TempCopilotJobProposal, true);
    end;

    local procedure GenerateJobProposal()
    var
        TempXmlBuffer: Record "XML Buffer" temporary;
        SimplifiedCopilotChat: Codeunit "Simplified Copilot Chat";
        TempBlob: Codeunit "Temp Blob";
        InStr: InStream;
        OutStr: OutStream;
        CurrInd, LineNo : Integer;
        DateVar: Date;
    begin
        TempBlob.CreateOutStream(OutStr);
        OutStr.WriteText(SimplifiedCopilotChat.Chat(GetSystemPrompt(), UserPrompt));
        TempBlob.CreateInStream(InStr);

        TempXmlBuffer.DeleteAll();
        TempXmlBuffer.LoadFromStream(InStr);

        Clear(OutStr);
        LineNo := 10000;
        if TempXmlBuffer.FindSet() then
            repeat
                case TempXmlBuffer.Path of
                    '/project/shortDescription':
                        begin
                            TempCopilotJobProposal.Init();
                            TempCopilotJobProposal."Job Short Description" := CopyStr(TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Job Short Description"));
                            TempCopilotJobProposal."Job Task No." := Format(LineNo);
                            TempCopilotJobProposal."Task Description" := CopyStr(TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Task Description"));
                            TempCopilotJobProposal."Job Task Type" := TempCopilotJobProposal."Job Task Type"::"Begin-Total";
                            TempCopilotJobProposal.Insert();

                            TempCopilotJobProposal."Job Task No." := Format(LineNo * 10 - 10);
                            TempCopilotJobProposal."Task Description" := CopyStr('Total ' + TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Task Description"));
                            TempCopilotJobProposal."Job Task Type" := TempCopilotJobProposal."Job Task Type"::"End-Total";
                            TempCopilotJobProposal.Insert();
                        end;
                    '/project/detailedDescription':
                        begin
                            TempCopilotJobProposal.FindFirst();
                            TempCopilotJobProposal."Job Full Description".CreateOutStream(OutStr);
                            OutStr.WriteText(TempXmlBuffer.GetValue());
                            TempCopilotJobProposal.Modify();
                        end;
                    '/project/customerName':
                        begin
                            TempCopilotJobProposal."Job Customer Name" := CopyStr(TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Job Customer Name"));
                            TempCopilotJobProposal.Modify();
                        end;
                    '/project/tasks':
                        CurrInd := 1;
                    '/project/tasks/task':
                        TempCopilotJobProposal.Init();
                    '/project/tasks/task/shortDescription':
                        begin
                            LineNo += 100;
                            TempCopilotJobProposal."Job Task No." := Format(LineNo);
                            TempCopilotJobProposal."Task Description" := CopyStr(TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Task Description"));
                            TempCopilotJobProposal."Job Task Type" := TempCopilotJobProposal."Job Task Type"::Posting;
                            TempCopilotJobProposal.Indentation := CurrInd;
                            TempCopilotJobProposal.Insert();
                        end;
                    '/project/tasks/task/detailedDescription':
                        begin
                            TempCopilotJobProposal."Action Description Preview" := CopyStr(TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Action Description Preview"));
                            TempCopilotJobProposal."Action Description".CreateOutStream(OutStr);
                            OutStr.WriteText(TempXmlBuffer.GetValue());
                            TempCopilotJobProposal.Modify();
                        end;
                    '/project/tasks/task/startDate':
                        begin
                            Evaluate(DateVar, TempXmlBuffer.GetValue(), 9);
                            TempCopilotJobProposal."Start Date" := DateVar;
                            TempCopilotJobProposal.Modify();
                        end;
                    '/project/tasks/task/endDate':
                        begin
                            Evaluate(DateVar, TempXmlBuffer.GetValue(), 9);
                            TempCopilotJobProposal."End Date" := DateVar;
                            TempCopilotJobProposal.Modify();
                        end;
                    '/project/tasks/task/type':
                        begin
                            case LowerCase(TempXmlBuffer.GetValue()) of
                                'resource':
                                    TempCopilotJobProposal.Type := TempCopilotJobProposal.Type::Resource;
                                'item':
                                    TempCopilotJobProposal.Type := TempCopilotJobProposal.Type::Item;
                                'both':
                                    TempCopilotJobProposal.Type := TempCopilotJobProposal.Type::Both;
                            end;
                            TempCopilotJobProposal.Modify();
                        end;
                    '/project/tasks/task/itemCategory':
                        begin
                            TempCopilotJobProposal."Item Category" := CopyStr(TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Item Category"));
                            TempCopilotJobProposal.Modify();
                        end;
                    '/project/tasks/task/resourceRole':
                        begin
                            TempCopilotJobProposal."Resource Role Description" := CopyStr(TempXmlBuffer.GetValue(), 1, MaxStrLen(TempCopilotJobProposal."Resource Role Description"));
                            TempCopilotJobProposal.Modify();
                        end;
                end;
            until TempXmlBuffer.Next() = 0;
    end;

    local procedure GetSystemPrompt() SystemPrompt: Text
    begin
        SystemPrompt := StrSubstNo(
            'The user will describe a project. Your task is to prepare the project plan for this project to be used in Microsoft Dynamics 365 Business Central.' +
            'The output should be in XML format.' +
            'Use project as the root level tag, use tasks as list tag, use task as task tag.' +
            'The project must contain a shortDescription tag, a detailedDescription tag, and a customerName tag (if the user specified any customer in their description).' +
            'Generate at least 6 tasks for the project. Use %1 as a start date for the project, and suggest a timeline. Use the yyyy-mm-dd date format for dates.' +
            'Each task should contain a shortDescription tag, a detailedDescription tag (make sure it''s less than 100 characters), a startDate tag, an endDate tag.' +
            'Each task should also contain a type tag, which can be "resource" if the task needs a resource, "item" if the task needs a physical item, or "both" if it needs both.' +
            'If the type is "item" or "both", include a tag itemCategory, if the type is "resource" or "both" include a resourceRole tag.',
            Format(CurrentDateTime(), 0, 9));
    end;

    var
        TempCopilotJobProposal: Record "Copilot Job Proposal" temporary;
        UserPrompt: Text;
}