namespace Microsoft.GP.MigrationDiagnostic.UI.TaskValueAdapters;

using Microsoft.GP.MigrationDiagnostic.Analysis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

public class TableValueTaskAdapter : ITaskValueAdapter
{
    private static readonly HashSet<string> TaskIds =
    [
        "BLANKACCOUNTS",
        "CustomerAddressIdLength",
        "CustomerInvalidEmail",
        "CustomerMissingMasterRecord",
        "CustomerNameLength",
        "CustomerPhoneNumberAlpha",
        "DirectPostingRetainedEarnings",
        "DuplicateApTransactionDocumentNumber",
        "DuplicateCustomerAddressIds",
        "DuplicateGlSummary",
        "DuplicateItemIds",
        "DuplicateVendorAddressIds",
        "GlSummaryValuesBeyondTwoDecimals",
        "IncorrectMainSegment",
        "InvalidPaymentTerms",
        "ItemMissingMasterRecord",
        "ItemNoUomSchedule",
        "ItemNumberLength",
        "MSMAC",
        "OpenPosInactiveItems",
        "PostingAccountsNotSetup",
        "PurchaseReceiptsWithoutValuationMethod",
        "RCWCSY40100F9",
        "UJECGL10001",
        "UJECGL20000",
        "UJECGL30000",
        "USBCGL10110",
        "USBCGL10111",
        "VendorAddressIdLength",
        "VendorInvalidEmail",
        "VendorMissingMasterRecord",
        "VendorNameLength",
        "VendorPhoneNumberAlpha",
        "VerifyPostingTypes",
    ];

    public bool CanHandleTask(IDiagnosticTask task) => TaskIds.Contains(task.UniqueIdentifier);

    public Task<Control[]> GetPanelControlsAsync(IDiagnosticTask task)
    {
        if (!this.CanHandleTask(task))
        {
            return Task.FromResult<Control[]>([]);
        }

        return Task.FromResult<Control[]>([
            new DataGridView
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = true,
                DataSource = task.EvaluatedValue,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            },
        ]);
    }
}
