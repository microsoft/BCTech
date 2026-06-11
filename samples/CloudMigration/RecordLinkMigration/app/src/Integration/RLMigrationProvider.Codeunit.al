// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

using Microsoft.DataMigration;
using Microsoft.Utilities;
using System.Environment.Configuration;
using System.Reflection;

codeunit 57500 "RL Migration Provider" implements "Custom Migration Provider"
{
    InherentEntitlements = X;
    InherentPermissions = X;

    var
        RecordLinkMigrationTelemetryCategoryTok: Label 'RecordLinkMigration', Locked = true;
        MappingsRegisteredLbl: Label 'Replication table mappings registered for Record Link migration.', Locked = true;
        ReplicationMappingExistsQst: Label 'Replication table mappings already exist for Record Link migration. Do you want to delete and recreate them?';
        BCTableSeparatorTok: Label '$', Locked = true;
        OpenBraceTok: Label '{', Locked = true;
        CloseBraceTok: Label '}', Locked = true;
        DisplayNameLbl: Label 'Record Link Migration', MaxLength = 250;
        DescriptionLbl: Label 'Migrates record links and notes from an on-premises SQL database to Business Central Online.';

    procedure GetDisplayName(): Text[250]
    begin
        exit(DisplayNameLbl);
    end;

    procedure GetDescription(): Text
    begin
        exit(DescriptionLbl);
    end;

    procedure GetAppId(): Guid
    var
        CurrentAppModuleInfo: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(CurrentAppModuleInfo);
        exit(CurrentAppModuleInfo.Id);
    end;

    procedure SetupReplicationTableMappings()
    var
        ReplicationTableMapping: Record "Replication Table Mapping";
        RecordLink: Record "Record Link";
        DestinationTableName: Text;
        Exists: Boolean;
    begin
        ReplicationTableMapping.SetFilter("Destination Sql Table Name", '<>%1', GetSqlTableName(Database::"RL Migration Buffer"));
        if not ReplicationTableMapping.IsEmpty() then begin
            if GuiAllowed() then
                if not Confirm(ReplicationMappingExistsQst) then
                    exit;

            ReplicationTableMapping.DeleteAll();
        end;

        Clear(ReplicationTableMapping);
        DestinationTableName := GetSqlTableName(Database::"RL Migration Buffer");
        Exists := ReplicationTableMapping.Get(RecordLink.TableName(), DestinationTableName);

        ReplicationTableMapping."Source Sql Table Name" := CopyStr(RecordLink.TableName(), 1, MaxStrLen(ReplicationTableMapping."Source Sql Table Name"));
        ReplicationTableMapping."Destination Sql Table Name" := CopyStr(DestinationTableName, 1, MaxStrLen(ReplicationTableMapping."Destination Sql Table Name"));
        ReplicationTableMapping."Company Name" := '';
        ReplicationTableMapping."Table Name" := CopyStr(RecordLink.TableCaption(), 1, MaxStrLen(ReplicationTableMapping."Table Name"));
        ReplicationTableMapping."Preserve Cloud Data" := true;
        if Exists then
            ReplicationTableMapping.Modify(true)
        else
            ReplicationTableMapping.Insert(true);

        Session.LogMessage('0000RL1', MappingsRegisteredLbl, Verbosity::Normal, DataClassification::SystemMetadata, TelemetryScope::ExtensionPublisher, 'Category', RecordLinkMigrationTelemetryCategoryTok);
    end;

    procedure SetupMigrationSetupTableMappings()
    begin
        // No setup-phase tables needed for record link migration
    end;

    procedure GetDemoDataType() DemoDataType: Enum "Company Demo Data Type"
    begin
        exit(DemoDataType::"Production - Setup Data Only");
    end;

    local procedure GetSqlTableName(TableID: Integer): Text
    var
        TableMetadata: Record "Table Metadata";
    begin
        TableMetadata.Get(TableID);
        exit(TableMetadata.Name + BCTableSeparatorTok + LowerCase(Format(GetAppId()).TrimStart(OpenBraceTok).TrimEnd(CloseBraceTok)));
    end;
}
