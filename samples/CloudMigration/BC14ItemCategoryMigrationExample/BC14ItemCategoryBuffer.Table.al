// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// STEP 1: Create a Buffer Table
///
/// Buffer tables temporarily hold the source data replicated from BC14 on-premises.
/// The cloud migration pipeline copies rows from the BC14 source table into this buffer,
/// and then the Migrator codeunit (Step 3) reads from here and writes into the
/// standard BC online table.
///
/// Guidelines:
///   - Mirror the field structure of the BC14 source table exactly (same field numbers,
///     types, and lengths) so that the replication pipeline can copy data without errors.
///   - Set ReplicateData = false — this table is only used as a staging area; it should
///     not be replicated between environments.
///   - Set Extensible = false — partners should not extend buffer tables.
///   - Use DataClassification = CustomerContent for fields containing business data.
///   - The table ID must fall within your extension's allocated ID range (see app.json).
///
/// In this example we replicate the BC14 "Item Category" table (fields: Code, Parent
/// Category, Description) into this buffer table "BC14 Item Category".
/// </summary>
namespace MS.DataMigration.BC14.Examples;

table 50200 "BC14 Item Category"
{
    DataClassification = CustomerContent;
    ReplicateData = false;
    Extensible = false;

    fields
    {
        // These fields must match the BC14 source table "Item Category".
        // Use the same field numbers, data types, and lengths as the source.
        field(1; "Code"; Code[20])
        {
            DataClassification = CustomerContent;
        }
        field(2; "Parent Category"; Code[20])
        {
            DataClassification = CustomerContent;
        }
        field(3; Description; Text[100])
        {
            DataClassification = CustomerContent;
        }
    }

    keys
    {
        key(PK; "Code")
        {
            Clustered = true;
        }
    }
}
