using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationsBuilder.Models;
using TranslationsBuilder.Services;

namespace TranslationsBuilder {
  public partial class FormAddLocalizedLabels : Form {

    public FormMain frmMain;

    public FormAddLocalizedLabels() {
      InitializeComponent();
      UpdateUI();
    }

    private void UpdateUI() {
      btnAddLabel.Enabled = txtLabel.Text.Replace(" ", "").Length > 0;
      bool AdvancedMode = chkAdvancedMode.Checked;
      this.AcceptButton = AdvancedMode ? null : btnAddLabel;
      lblLabelTextboxLabel.Text = AdvancedMode ? "Labels:" : "Label:";
      btnAddLabel.Text = AdvancedMode ? "Add Labels" : "Add Label";
      btnDeleteAllLabels.Visible = AdvancedMode;
      btnUploadLabelsFromFile.Visible = AdvancedMode;
      lblDangerZone.Visible = AdvancedMode;
      lblBuilderHint.Visible = AdvancedMode;
      this.Height = AdvancedMode ? 392 : 200;
      txtLabel.Multiline = AdvancedMode;
      txtLabel.Height = 23 * (AdvancedMode ? 12 : 1);
      txtLabel.Focus();
    }

    private void AddLabel(object sender, EventArgs e) {

      if (!string.IsNullOrEmpty(txtLabel.Text.Replace(" ", ""))) {
        if (!chkAdvancedMode.Checked) {
          TranslationsManager.AddLocalizedLabel(txtLabel.Text.TrimStart());
          txtLabel.Text = "";
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
        else {
          string[] labels = txtLabel.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
          foreach (string label in labels) {
            if (label.Replace(" ", "").Length > 0) {
              TranslationsManager.AddLocalizedLabel(label.TrimStart(), false);
            }
          }
          txtLabel.Text = "";
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      else {
        UserInteraction.PromptUserWithInformation("You cannot add a blank string as a label");
      }

    }

    private void SetAddButtonEnabled(object sender, EventArgs e) {
      btnAddLabel.Enabled = txtLabel.Text.Length > 0;
    }

    private void chkAdvancedMode_CheckedChanged(object sender, EventArgs e) {
      UpdateUI();
      if (!chkAdvancedMode.Checked) {
        txtLabel.Text = txtLabel.Text.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
      }
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void btnDeleteAllLabels_Click(object sender, EventArgs e) {
      TranslationsManager.DeleteAllLocalizedLabels();
      frmMain.PopulateGridWithTranslations();

    }

    private void btnUploadLabelsFromFile_Click(object sender, EventArgs e) {

      dialogOpenFile.InitialDirectory = AppSettings.TranslationsInboxFolderPath;
      dialogOpenFile.Filter = "Text files (*.txt)|*.txt";
      dialogOpenFile.FilterIndex = 1;
      dialogOpenFile.RestoreDirectory = true;


      if (dialogOpenFile.ShowDialog() == DialogResult.OK) {
        this.Hide();
        TranslationsManager.ImportLocalizedLabels(dialogOpenFile.FileName);
        frmMain.PopulateGridWithTranslations();
        this.DialogResult= DialogResult.OK;
        this.Close();
      }

    }


    }
}
