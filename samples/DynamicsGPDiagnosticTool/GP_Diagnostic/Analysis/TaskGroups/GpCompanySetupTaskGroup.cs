namespace Microsoft.GP.MigrationDiagnostic.Analysis.TaskGroups;

using Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;
using System;
using System.Collections.Generic;

/// <summary>
/// A group of Company Setup related tasks.
/// </summary>
public class GpCompanySetupTaskGroup : IMultiCompanyDiagnosticTaskGroup
{
    /// <inheritdoc/>
    public string Name => "Setup";

    /// <inheritdoc />
    public string CompanyName { get; private set; }

    /// <inheritdoc />
    public string DisplayName { get; private set; }

    /// <inheritdoc/>
    public IList<IDiagnosticTask> Tasks { get; } = new List<IDiagnosticTask>();

    public GpCompanySetupTaskGroup(GpDatabase systemDatabase, GpCompanyDatabase companyDatabase, string companyName, string companyDisplayName)
    {
        this.CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        this.DisplayName = companyDisplayName ?? throw new ArgumentNullException(nameof(companyDisplayName));

        this.Tasks.Add(new SqlChangeTrackingDisabledTask(companyDatabase));
        this.Tasks.Add(new SqlCompatibilityLevelTask(companyDatabase));
        this.Tasks.Add(new GpCompanyVersionTask(systemDatabase, companyDatabase));
        //this.Tasks.Add(new GpAdditionalProductsTask(systemDatabase, companyDatabase));
        this.Tasks.Add(new GpIncorrectMainSegmentTask(companyDatabase));
        this.Tasks.Add(new GpVerifyPostingTypesTask(companyDatabase));
        this.Tasks.Add(new GpPostingAccountsNotSetupTask(companyDatabase));
        this.Tasks.Add(new GpMainSegmentNotIdentifiedTask(companyDatabase));
    }
}
