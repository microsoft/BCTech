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
    begin
        InventoryModule.CreateSetupData();
        Codeunit.Run(Codeunit::"Contoso Shoes Item Category");
        Codeunit.Run(Codeunit::"Contoso Shoes Unit of Measure");
    end;

    procedure CreateMasterData()
    begin

    end;

    procedure CreateTransactionalData()
    begin

    end;

    procedure CreateHistoricalData()
    begin

    end;
}