codeunit 50108 "CTF Challenges Install"
{
    SubType = Install;

    trigger OnInstallAppPerCompany()
    begin
        InsertDefaultCTFChallengeSetup();
    end;

    local procedure InsertDefaultCTFChallengeSetup()
    var
        CTFChallengesSetup: Record "CTF Challenges Setup";
    begin
        if not CTFChallengesSetup.FindFirst() then begin
            CTFChallengesSetup."External Mode" := false;
            CTFChallengesSetup.Insert();
        end;
    end;
}