namespace Microsoft.GP.MigrationDiagnostic.Analysis.TaskGroups;

using Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;
using System;
using System.Collections.Generic;

/// <summary>
/// A group of Financial related tasks.
/// </summary>
public class GpCompanyFinanceTaskGroup : IMultiCompanyDiagnosticTaskGroup
{
    /// <inheritdoc/>
    public string Name => "Finance";

    /// <inheritdoc />
    public string CompanyName { get; private set; }

    /// <inheritdoc />
    public string DisplayName { get; private set; }

    /// <inheritdoc/>
    public IList<IDiagnosticTask> Tasks { get; } = [];

    public GpCompanyFinanceTaskGroup(GpCompanyDatabase database, string companyName, string companyDisplayName)
    {
        this.CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        this.DisplayName = companyDisplayName ?? throw new ArgumentNullException(nameof(companyDisplayName));
        this.Tasks.Add(new GpTooManyAccountSegmentsTask(database));
        this.Tasks.Add(new GpAccountCategoryMismatchTask(database));
        this.Tasks.Add(new GpDuplicateGlSummaryTask(database));
        this.Tasks.Add(new GpGlSummaryValuesBeyondTwoDecimalsTask(database));
        this.Tasks.Add(new GpInvalidPaymentTermsTask(database));
        this.Tasks.Add(new GpUnbalancedSummaryBalanceTask(database, "GL10110")); // Open
        this.Tasks.Add(new GpUnbalancedSummaryBalanceTask(database, "GL10111")); // History
        this.Tasks.Add(new GpDuplicateApTransactionDocumentNumberTask(database));
        this.Tasks.Add(new GpDirectTransactionsPostingToRetainedEarningsAccountTask(database));
        this.Tasks.Add(new GpBlankAccountSegmentsTask(database));
        this.Tasks.Add(new GpInvalidPeriodTask(database));
        this.Tasks.Add(new GpUnbalancedJournalEntriesTask(database, "GL10001")); // Work
        this.Tasks.Add(new GpUnbalancedJournalEntriesTask(database, "GL20000")); // Open
        this.Tasks.Add(new GpUnbalancedJournalEntriesTask(database, "GL30000")); // History
    }
}
