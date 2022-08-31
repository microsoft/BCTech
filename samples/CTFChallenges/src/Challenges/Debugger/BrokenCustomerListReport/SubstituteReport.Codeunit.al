codeunit 50147 SubstitutingAReport
{  
    SingleInstance = true;

    var
        HitCount : integer;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::ReportManagement, 'OnAfterSubstituteReport', '', false, false)]
    local procedure Flag_e7509018(ReportId: Integer; var NewReportId: Integer)
    begin
        if ReportId = Report::"Customer - Sales List" then begin
            NewReportId := Report::"My New Customer - List";
            HitCount := HitCount + 1;

            if (HitCount mod 2) = 0 then 
                Error ('Snapshot debugging should break here. Time to stop your snapshot session.');
        end;        
    end;
}