namespace DefaultPublisher.EDocDemo;

using Microsoft.eServices.EDocument;
using Microsoft.eServices.EDocument.Integration.Interfaces;

enumextension 50101 "EDocument Integration" extends "E-Document Integration"
{
    value(50100; "Demo Integration")
    {
        Implementation = "E-Document Integration" = "Demo Integration";
    }
    value(50101; "Demo Integration V2")
    {
        Implementation = Sender = "Demo Integration V2", Receiver = "Demo Integration V2", "Default Int. Actions" = "Demo Integration V2";
    }
}