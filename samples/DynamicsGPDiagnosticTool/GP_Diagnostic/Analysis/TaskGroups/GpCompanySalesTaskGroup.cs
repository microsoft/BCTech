namespace Microsoft.GP.MigrationDiagnostic.Analysis.TaskGroups;

using Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;
using System;
using System.Collections.Generic;

/// <summary>
/// A group of Sales related tasks.
/// </summary>
public class GpCompanySalesTaskGroup : IMultiCompanyDiagnosticTaskGroup
{
    /// <inheritdoc/>
    public string Name => "Sales";

    /// <inheritdoc />
    public string CompanyName { get; private set; }

    /// <inheritdoc />
    public string DisplayName { get; private set; }

    /// <inheritdoc/>
    public IList<IDiagnosticTask> Tasks { get; } = new List<IDiagnosticTask>();

    public GpCompanySalesTaskGroup(GpDatabase systemDatabase, GpCompanyDatabase companyDatabase, string companyName, string companyDisplayName)
    {
        this.CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        this.DisplayName = companyDisplayName ?? throw new ArgumentNullException(nameof(companyDisplayName));

        this.Tasks.Add(new GpCustomerNameLengthTask(companyDatabase));
        this.Tasks.Add(new GpCustomerPhoneNumberAlphaTask(companyDatabase));
        this.Tasks.Add(new GpCustomerAddressIdLengthTask(companyDatabase));
        this.Tasks.Add(new GpDuplicateCustomerAddressIdsTask(companyDatabase));
        this.Tasks.Add(new GpCustomerInvalidEmailTask(companyDatabase));
        this.Tasks.Add(new GpCustomerMissingMasterRecordTask(companyDatabase));
    }
}
