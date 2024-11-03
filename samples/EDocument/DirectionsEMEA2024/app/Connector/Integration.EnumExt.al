namespace Workshop.Integration;

using Microsoft.eServices.EDocument;

enumextension 50100 Integration extends "E-Document Integration"
{
    value(50100; "My Connector")
    {
        Caption = 'My Connector';
        Implementation = "E-Document Integration" = ConnectorImpl;
    }
}