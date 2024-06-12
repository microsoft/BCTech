using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationsBuilder.Properties;

namespace TranslationsBuilder {
  public partial class FormConfig : Form {

    private static Settings settings = new Settings();

    public FormConfig() {
      InitializeComponent();
    }

    private void FormConfig_Load(object sender, EventArgs e) {
      txtTranslationsOutboxFolderPath.Text = AppSettings.TranslationsOutboxFolderPath;
      txtTranslationsInboxFolderPath.Text = AppSettings.TranslationsInboxFolderPath;
      txtAzureTranslatorServiceKey.Text = AppSettings.AzureTranslatorServiceKey;
      txtAzureTranslatorServiceLocation.Text = AppSettings.AzureTranslatorServiceLocation;
    }

    private void btnSaveConfigurationChanges_Click(object sender, EventArgs e) {
      AppSettings.TranslationsOutboxFolderPath = txtTranslationsOutboxFolderPath.Text;
      AppSettings.TranslationsInboxFolderPath = txtTranslationsInboxFolderPath.Text;
      AppSettings.AzureTranslatorServiceKey = txtAzureTranslatorServiceKey.Text;
      AppSettings.AzureTranslatorServiceLocation = txtAzureTranslatorServiceLocation.Text;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void SetOutboxPath(object sender, EventArgs e) {

      dialogSelectFolder.SelectedPath = AppSettings.TranslationsOutboxFolderPath;

      if (dialogSelectFolder.ShowDialog() == DialogResult.OK) {
        AppSettings.TranslationsOutboxFolderPath = dialogSelectFolder.SelectedPath;
        txtTranslationsOutboxFolderPath.Text = dialogSelectFolder.SelectedPath;
      }

    }

    private void SetInboxPath(object sender, EventArgs e) {

      dialogSelectFolder.SelectedPath = AppSettings.TranslationsInboxFolderPath;

      if (dialogSelectFolder.ShowDialog() == DialogResult.OK) {
        AppSettings.TranslationsInboxFolderPath = dialogSelectFolder.SelectedPath;
        txtTranslationsInboxFolderPath.Text = dialogSelectFolder.SelectedPath;
      }

    }
  }
}
