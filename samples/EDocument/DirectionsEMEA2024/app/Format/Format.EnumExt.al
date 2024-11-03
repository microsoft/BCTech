namespace Workshop.Format;

using Microsoft.eServices.EDocument;

enumextension 50101 Format extends "E-Document Format"
{
    value(50100; "My Format")
    {
        Caption = 'My Format';
        Implementation = "E-Document" = FormatImpl;
    }
}