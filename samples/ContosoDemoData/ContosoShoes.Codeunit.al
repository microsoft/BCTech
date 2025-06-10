codeunit 50100 "Contoso Shoes" implements "Contoso Demo Data Module"
{
    procedure RunConfigurationPage()
    begin

    end;

    procedure GetDependencies() Dependencies: List of [Enum "Contoso Demo Data Module"]
    begin
        Dependencies.Add(Enum::"Contoso Demo Data Module"::Foundation);
        Dependencies.Add(Enum::"Contoso Demo Data Module"::Finance);
    end;

    procedure CreateSetupData()
    var
        InventoryModule: Codeunit "Inventory Module";
        SalesModule: Codeunit "Sales Module";
    begin
        InventoryModule.CreateSetupData();
        SalesModule.CreateSetupData();
        Codeunit.Run(Codeunit::"Contoso Shoes Item Category");
        Codeunit.Run(Codeunit::"Contoso Shoes Unit of Measure");
    end;

    procedure CreateMasterData()
    begin
        Codeunit.Run(Codeunit::"Contoso Shoes Item");
        Codeunit.Run(Codeunit::"Contoso Shoes Size");
        Codeunit.Run(Codeunit::"Contoso Shoes Customer");
    end;

    procedure CreateTransactionalData()
    begin
        Codeunit.Run(Codeunit::"Contoso Shoes Sales Order");
    end;

    procedure CreateHistoricalData()
    begin
        Codeunit.Run(Codeunit::"Contoso Shoes Posted Sales");
    end;
}