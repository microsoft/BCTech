namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using Microsoft.Extensions.Logging;
using Microsoft.GP.MigrationDiagnostic.Configuration;
using Microsoft.GP.MigrationDiagnostic.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

public class GpConfigurationView
{
    private readonly IRuntimeConfigurationSource<GpConfiguration> gpConfigStore;
    private readonly ILogger logger;

    public GpConfigurationView(IRuntimeConfigurationSource<GpConfiguration> gpConfigStore, ILogger logger)
    {
        this.gpConfigStore = gpConfigStore ?? throw new ArgumentNullException(nameof(gpConfigStore));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ShowConfiguration(Form parentForm)
    {
        var config = this.gpConfigStore.GetConfiguration();
        var configForm = new DatabaseSettings(logger);
        configForm.txtServer.Text = config.Server ?? GetServerName();
        configForm.txtSystemDatabase.Text = config.SystemDatabase ?? "DYNAMICS";
        configForm.txtUsername.Text = config.UserName;
        configForm.txtPassword.Text = config.Password;
        configForm.chkIntSecurity.Checked = config.IntegratedSecurity;

        foreach (var company in config.CompanyNames)
        {
            configForm.clbCompanyList.Items.Add(string.Format(@"{0} ({1})", company.DatabaseName, company.DisplayName), company.Selected);
        }

        configForm.ShowDialog(parentForm);

        config.Server = configForm.txtServer.Text;
        config.SystemDatabase = configForm.txtSystemDatabase.Text;
        config.Password = configForm.txtPassword.Text;
        config.UserName = configForm.txtUsername.Text;
        config.IntegratedSecurity = configForm.chkIntSecurity.Checked;
        config.CompanyNames.Clear();

        string dbName;
        string displayName;
        string? value;

        foreach (var company in configForm.clbCompanyList.Items)
        {
            value = company.ToString();

            if (!string.IsNullOrWhiteSpace(value))
            {
                displayName = value.Substring(value.IndexOf('(') + 1).TrimEnd(')');
                dbName = value.Substring(0, value.IndexOf(' '));
                config.CompanyNames.Add((dbName, displayName, configForm.clbCompanyList.CheckedItems.Contains(company)));
            }
        }

        config.IsValidConfig = config.CompanyNames.Count(x => x.Selected) > 0;
        this.gpConfigStore.UpdateConfiguration(config);
    }

    private static string GetServerName()
    {
        string? gpOdbcServerName = string.Empty;
        RegistryKey? odbcDataSources = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\ODBC\ODBC.INI\ODBC Data Sources", false);

        if (odbcDataSources != null)
        {
            List<string> odbcConnections = odbcDataSources.GetValueNames().ToList();

            if (odbcConnections != null)
            {
                string? gpConnectionName = odbcConnections.Find(x => x.StartsWith("Dynamics GP", false, CultureInfo.InvariantCulture));

                if (!string.IsNullOrEmpty(gpConnectionName))
                {
                    RegistryKey? gpOdbcConnection = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\WOW6432Node\ODBC\ODBC.INI\{gpConnectionName}", false);

                    if (gpOdbcConnection != null)
                    {
                        gpOdbcServerName = gpOdbcConnection.GetValue("Server") as string;
                    }
                }
            }
        }

        return string.IsNullOrEmpty(gpOdbcServerName) ? "localhost" : gpOdbcServerName;
    }
}
