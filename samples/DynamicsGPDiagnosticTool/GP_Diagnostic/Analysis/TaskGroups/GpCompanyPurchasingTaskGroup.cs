namespace Microsoft.GP.MigrationDiagnostic.Analysis.TaskGroups;

using Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;
using System;
using System.Collections.Generic;

/// <summary>
/// A group of Purchasing related tasks.
/// </summary>
public class GpCompanyPurchasingTaskGroup : IMultiCompanyDiagnosticTaskGroup
{
    /// <inheritdoc/>
    public string Name => "Purchasing";

    /// <inheritdoc />
    public string CompanyName { get; private set; }

    /// <inheritdoc />
    public string DisplayName { get; private set; }

    /// <inheritdoc/>
    public IList<IDiagnosticTask> Tasks { get; } = new List<IDiagnosticTask>();

    public GpCompanyPurchasingTaskGroup(GpCompanyDatabase database, string companyName, string companyDisplayName)
    {
        this.CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        this.DisplayName = companyDisplayName ?? throw new ArgumentNullException(nameof(companyDisplayName));

        this.Tasks.Add(new GpVendorNameLengthTask(database));
        this.Tasks.Add(new GpVendorPhoneNumberAlphaTask(database));
        this.Tasks.Add(new GpVendorAddressIdLengthTask(database));
        this.Tasks.Add(new GpDuplicateVendorAddressIdsTask(database));
        this.Tasks.Add(new GpVendorInvalidEmailTask(database));
        this.Tasks.Add(new GpVendorMissingMasterRecordTask(database));
        this.Tasks.Add(new GpOpenPosInactiveItemsTask(database));
    }
}
