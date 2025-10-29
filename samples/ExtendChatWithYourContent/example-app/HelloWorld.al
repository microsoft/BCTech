namespace DefaultPublisher.exampleapp;

using Microsoft.Sales.Customer;

pageextension 50100 CustomerListExt extends "Customer List"
{
    trigger OnOpenPage();
    begin
        Message('Test application is running successfully!');
    end;
}
