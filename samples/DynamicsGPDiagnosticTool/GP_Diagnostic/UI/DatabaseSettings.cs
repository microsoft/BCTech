namespace Microsoft.GP.MigrationDiagnostic.UI;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

/// <summary>
/// Database settings form for GP configuration.
/// </summary>
public partial class DatabaseSettings : Form
{
    private enum AccessMode
    {
        Unknown = 0,
        SINGLE_USER,
        MULTI_USER,
        RESTRICTED_USER
    }

    private class CompanyDatabase
    {
        public string? DatabaseName { get; set; }
        public string? DisplayName { get; set; }
        public int? CompanyId { get; set; }
        public AccessMode? AccessMode { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} ({1})", DatabaseName, DisplayName);
        }
    }

    private const string MultiUserMode = "MULTI_USER";
    private const string SingleUserMode = "SINGLE_USER";
    private const string RestrictedUserMode = "RESTRICTED_USER";

    public ILogger logger;

    public DatabaseSettings(ILogger logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeComponent();
    }

    private void chkIntSecurity_CheckedChanged(object sender, EventArgs e)
    {
        this.txtUsername.Enabled = !this.chkIntSecurity.Checked;
        this.txtPassword.Enabled = !this.chkIntSecurity.Checked;
    }

    private void btnConnect_Click(object sender, EventArgs e)
    {
        // Disable the company list until SQL connection is established
        this.clbCompanyList.Enabled = false;
        this.btnSave.Enabled = false;
        this.clbCompanyList.Items.Clear();

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        SqlDataReader sqlDataReader;
        string companiesQuery = $"SELECT INTERID, CMPNYNAM, CMPANYID FROM {this.txtSystemDatabase.Text}..SY01500";
        string singleUserModeQuery = "SELECT user_access_desc FROM sys.databases WHERE name = '{0}'";

        builder.DataSource = this.txtServer.Text;
        builder.UserID = this.txtUsername.Text;
        builder.Password = this.txtPassword.Text;
        builder.IntegratedSecurity = this.chkIntSecurity.Checked;
        builder.MultipleActiveResultSets = true;
        builder.TrustServerCertificate = true;

        // The reader controls the connection closing (CommandBehavior.CloseConnection)
        SqlConnection connection = new SqlConnection(builder.ConnectionString);

        try
        {
            connection.Open();
        }
        catch (SqlException ex)
        {
            this.logger.LogError(ex, ex.Message);

            if (ex.Message.Contains("The server was not found or was not accessible"))
            {
                MessageBox.Show("Please check the Sql Server Name, its availability, your permissions, and try again.", "Unable to connect to the Sql Server");
                return;
            }
            
            if (ex.Message.StartsWith("Cannot open database"))
            {
                MessageBox.Show("Please check the GP system database name, its availability, your permissions, and try again.", "Unable to connect to the GP system database");
            }
            else
            {
                MessageBox.Show("Please check credentials and try again.", "Unable to connect to SQL.");
            }
            return;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            MessageBox.Show("Unexpected error when attempting to connect to the selected database, please review the Windows Event Log for more information.");
        }

        using (SqlCommand command = new SqlCommand(string.Format(singleUserModeQuery, this.txtSystemDatabase.Text), connection))
        {
            SqlDataReader userModeReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            string? accessMode = string.Empty;

            try
            {
                userModeReader.Read();
                accessMode = userModeReader.GetValue(0).ToString();

                if (accessMode != MultiUserMode)
                {
                    var errorMessage = $"The system database '{this.txtSystemDatabase.Text}' is currently in '{accessMode}' access mode.\r\n\r\nYou cannot continue until it is in '{MultiUserMode}' mode.";
                    MessageBox.Show(errorMessage,
                        "Access Mode",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    this.logger.LogWarning(errorMessage);

                    return;
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is SqlException)
            {
                this.logger.LogError(ex, "Exception when attempting to determine GP DB access mode");
                MessageBox.Show("Unexpected error when attempting to determine database access mode, please verify your system database name or review the Window Event Log for more details.");

                return;
            }
        }

        List<CompanyDatabase> companyDatabases = new();

        using (SqlCommand command = new SqlCommand(companiesQuery, connection))
        {
            try
            {
                sqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                this.logger.LogError(ex, ex.Message);

                if (ex.Message.StartsWith("Invalid object name") && ex.Message.Contains("SY01500"))
                {
                    MessageBox.Show("Invalid GP system database? Please check the database name and try again.", ex.Message);
                }
                else
                {
                    MessageBox.Show("Check GP system database name and availablility and try again.", "Unable to read from GP system database");
                }
                return;
            }

            while (sqlDataReader.Read())
            {
                var dbName = sqlDataReader.GetString(0);
                var displayName = sqlDataReader.GetString(1);
                var companyId = sqlDataReader.GetInt16(2);

                companyDatabases.Add(new CompanyDatabase()
                {
                    DatabaseName = dbName.TrimEnd(),
                    DisplayName = displayName.TrimEnd(),
                    CompanyId = companyId
                });
            }
        }

        var failedCompanyDatabases = new List<string>();

        foreach (CompanyDatabase companyDatabase in companyDatabases)
        {
            using (SqlCommand command = new SqlCommand(string.Format(singleUserModeQuery, companyDatabase.DatabaseName), connection))
            {
                try
                {
                    sqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    sqlDataReader.Read();
                    string? accessMode = sqlDataReader.GetValue(0).ToString();
                    companyDatabase.AccessMode = accessMode switch
                    {
                        MultiUserMode => AccessMode.MULTI_USER,
                        SingleUserMode => AccessMode.SINGLE_USER,
                        RestrictedUserMode => AccessMode.RESTRICTED_USER,
                        _ => AccessMode.Unknown,
                    };

                    this.clbCompanyList.Items.Add(companyDatabase, companyDatabase.CompanyId >= 0 && companyDatabase.AccessMode == AccessMode.MULTI_USER);
                }
                catch (InvalidOperationException)
                {
                    if (!string.IsNullOrEmpty(companyDatabase.DatabaseName))
                    {
                        failedCompanyDatabases.Add(companyDatabase.DatabaseName);
                    }
                }
            }
        }

        // If some databases aren't in the system database, they'll get skipped, but inform the user.
        if (failedCompanyDatabases.Count > 0)
        {
            MessageBox.Show($"Some company databases were not found in the query {singleUserModeQuery}. These include {string.Join(',', failedCompanyDatabases)}");
        }

        // Enable controls for things that depend on database connectivity
        this.clbCompanyList.Enabled = true;
        this.btnSave.Enabled = true;
    }

    private void txtServer_Leave(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.txtServer.Text))
        {
            MessageBox.Show("Sql Server Name cannot be empty.", "Invalid Sql Server Name");
            this.txtServer.Focus();
            return;
        }
    }

    private void txtSystemDatabase_Leave(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.txtSystemDatabase.Text))
        {
            MessageBox.Show("GP system database name cannot be empty.", "Invalid GP System Database Name");
            this.txtSystemDatabase.Focus();
            return;
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void clbCompanyList_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (sender is CheckedListBox checkedListBox
            && e.NewValue == CheckState.Checked
            && checkedListBox.Items[e.Index] is CompanyDatabase checkedListItem
            && checkedListItem.AccessMode != AccessMode.MULTI_USER)
        {
            MessageBox.Show($"The company database is in '{checkedListItem.AccessMode}' mode and cannot be selected until it is in '{AccessMode.MULTI_USER}' mode.",
                "Access Mode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            e.NewValue = CheckState.Unchecked;
        }
    }
}
