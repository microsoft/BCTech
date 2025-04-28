codeunit 54300 "Copilot Telemetry Logger" implements "Telemetry Logger"
{
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure LogMessage(EventId: Text; Message: Text; Verbosity: Verbosity; DataClassification: DataClassification; TelemetryScope: TelemetryScope; CustomDimensions: Dictionary of [Text, Text])
    begin
        Session.LogMessage(EventId, Message, Verbosity, DataClassification, TelemetryScope, CustomDimensions);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Telemetry Loggers", OnRegisterTelemetryLogger, '', true, true)]
    local procedure OnRegisterTelemetryLogger(var Sender: Codeunit "Telemetry Loggers")
    var
        CopilotTelemetryLogger: Codeunit "Copilot Telemetry Logger";
    begin
        Sender.Register(CopilotTelemetryLogger);
    end;
}