namespace CopilotToolkitDemo.DescribeJob;

using Microsoft.Projects.Project.Job;
using Microsoft.Projects.Project.Planning;
using Microsoft.Sales.Customer;

codeunit 54314 "Save Copilot Job Proposal"
{
    procedure Save(CustomerNo: Code[20]; var CopilotJobProposal: Record "Copilot Job Proposal" temporary)
    var
        JobTask: Record "Job Task";
        Job: Record Job;
    begin
        if CopilotJobProposal.FindSet() then begin
            Job := CreateJob(CustomerNo, CopilotJobProposal."Job Short Description");

            repeat
                JobTask := CreateJobTask(Job."No.", CopilotJobProposal);

                if JobTask."Job Task Type" = JobTask."Job Task Type"::Posting then
                    CreateJobPlanningLine(JobTask, CopilotJobProposal);
            until CopilotJobProposal.Next() = 0;
        end;
    end;

    local procedure CreateJob(CustomerNo: Code[20]; JobDescription: Text[100]) Job: Record Job
    var
        Customer: Record Customer;
    begin
        Customer.Get(CustomerNo);
        Job.Init();

        Job.Description := JobDescription;
        Job.Validate("Bill-to Customer No.", Customer."No.");

        Job.Insert(true);
    end;

    local procedure CreateJobTask(JobNo: Code[20]; var CopilotJobProposal: Record "Copilot Job Proposal" temporary) JobTask: Record "Job Task"
    begin
        JobTask.Init();

        JobTask.Validate("Job No.", JobNo);
        JobTask.Validate("Job Task No.", CopilotJobProposal."Job Task No.");
        JobTask.Validate(Description, CopilotJobProposal."Task Description");
        JobTask.Validate("Job Task Type", CopilotJobProposal."Job Task Type");
        JobTask.Validate(Indentation, CopilotJobProposal.Indentation);

        JobTask.Insert(true);
    end;

    local procedure CreateJobPlanningLine(JobTask: Record "Job Task"; var CopilotJobProposal: Record "Copilot Job Proposal" temporary)
    var
        JobPlanningLine: Record "Job Planning Line";
        JobPlannigLineNo: Integer;
        ActionDescription: Text;
        InStr: InStream;
    begin
        JobPlannigLineNo := 10000;

        if CopilotJobProposal.Type in [CopilotJobProposal.Type::Resource, CopilotJobProposal.Type::Both] then begin
            JobPlanningLine := CreateBaseJobPlanningLine(JobTask, CopilotJobProposal);

            JobPlanningLine.Validate("Line No.", JobPlannigLineNo);
            JobPlanningLine.Validate(Type, JobPlanningLine.Type::Resource);
            JobPlanningLine.Validate("Description 2", CopilotJobProposal."Resource Role Description");

            JobPlanningLine.Insert(true);

            JobPlannigLineNo += 1;
        end;

        if CopilotJobProposal.Type in [CopilotJobProposal.Type::Item, CopilotJobProposal.Type::Both] then begin
            Clear(JobPlanningLine);

            JobPlanningLine := CreateBaseJobPlanningLine(JobTask, CopilotJobProposal);

            JobPlanningLine.Validate("Line No.", JobPlannigLineNo);
            JobPlanningLine.Validate(Type, JobPlanningLine.Type::Item);
            JobPlanningLine.Validate("Description 2", CopilotJobProposal."Item Category");

            JobPlanningLine.Insert(true);
        end;
    end;

    local procedure CreateBaseJobPlanningLine(JobTask: Record "Job Task"; var CopilotJobProposal: Record "Copilot Job Proposal" temporary) JobPlanningLine: Record "Job Planning Line"
    var
        JobPlannigLineNo: Integer;
        ActionDescription: Text;
        InStr: InStream;
    begin
        JobPlanningLine.Init();
        JobPlanningLine.Validate("Job No.", JobTask."Job No.");
        JobPlanningLine.Validate("Job Task No.", JobTask."Job Task No.");
        JobPlanningLine.Validate("Line No.", JobPlannigLineNo);

        CopilotJobProposal.CalcFields("Action Description");
        CopilotJobProposal."Action Description".CreateInStream(InStr);
        InStr.ReadText(ActionDescription);

        JobPlanningLine.Validate(Description, CopyStr(ActionDescription, 1, MaxStrLen(JobPlanningLine.Description))); // We asked copilot to stay under 100, but sometimes it might miscount

        JobPlanningLine.Validate("Planning Date", CopilotJobProposal."Start Date");
        JobPlanningLine."Planned Delivery Date" := CopilotJobProposal."End Date";
    end;
}