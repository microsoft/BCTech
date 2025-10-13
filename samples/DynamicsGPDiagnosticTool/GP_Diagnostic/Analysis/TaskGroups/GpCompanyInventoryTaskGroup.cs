namespace Microsoft.GP.MigrationDiagnostic.Analysis.TaskGroups;

using Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;
using System;
using System.Collections.Generic;

/// <summary>
/// A group of Inventory related tasks.
/// </summary>
public class GpCompanyInventoryTaskGroup : IMultiCompanyDiagnosticTaskGroup
{
    /// <inheritdoc/>
    public string Name => "Inventory";

    /// <inheritdoc />
    public string CompanyName { get; private set; }

    /// <inheritdoc />
    public string DisplayName { get; private set; }

    /// <inheritdoc/>
    public IList<IDiagnosticTask> Tasks { get; } = new List<IDiagnosticTask>();

    public GpCompanyInventoryTaskGroup(GpCompanyDatabase database, string companyName, string companyDisplayName)
    {
        this.CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        this.DisplayName = companyDisplayName ?? throw new ArgumentNullException(nameof(companyDisplayName));

        this.Tasks.Add(new GpPurchaseReceiptsWithoutValuationMethodTask(database));
        this.Tasks.Add(new GpItemNoUomScheduleTask(database));
        this.Tasks.Add(new GpItemMissingMasterRecordTask(database));
        this.Tasks.Add(new GpItemNumberLengthTask(database));
        this.Tasks.Add(new GpDuplicateItemIdsTask(database));
        this.Tasks.Add(new GpItemNumberSpacePrefixTask(database));
    }
}
