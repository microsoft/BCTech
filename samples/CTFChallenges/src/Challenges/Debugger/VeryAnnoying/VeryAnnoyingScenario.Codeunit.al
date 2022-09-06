codeunit 50142 VeryAnnoyingScenario implements "CTF Challenge"
{
    trigger OnRun()
    var
        OnlineMapSetup: Record "Online Map Setup";
    begin
        OnlineMApSetup.DeleteAll();
    end;

    procedure RunChallenge();
    var
        Contact: Record Contact;
        ScenarioLabel1: Label 'Challenge: Try showing on the map a contacts''s address. \Click on the ''Show Map'' action. Setup what is required, by opening the ''Online Map Setup'' page. \Then click again on the ''Show Map'' action. What is blocking it?';
        ScenarioLabel2: Label 'You can rerun this action on the Contact card page by clicking on the ''CTF Challenge'' action found on the Process group';
    begin
        Contact.FindFirst();
        PAGE.Run(Page::"Contact Card", Contact);
        Message(ScenarioLabel1 + ScenarioLabel2);
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Create a page extension on the "Online Map Setup" page.';
        HintLine2: Label 'Set this page as a startup page in the launch.json.';
        HintLine3: Label 'Rely on breakonReadWrite and breakOnError settings.';
        HintLine4: Label 'Start debugging your app and just be patient.';

        Hints: List of [Text];
    begin
        Hints.Add(HintLine1 + HintLine2 + HintLine3 + HintLine4);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Debugging);
    end;
}