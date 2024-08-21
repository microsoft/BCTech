using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TranslationsBuilder.Models;
using TranslationsBuilder.Services;

using Microsoft.AnalysisServices.Tabular;
using System.Security.AccessControl;
using TranslationsBuilder.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;
using static System.Windows.Forms.DataGridView;
using System.Configuration;
using System.Diagnostics;
using System.Security.Policy;

namespace TranslationsBuilder {

  public partial class FormMain : Form {

    public FormMain() {
      InitializeComponent();
      this.Text = GlobalConstants.ApplicationTitle;
    }

    private void onLoad(object sender, EventArgs e) {
      if (TranslationsManager.IsConnected) {
        LoadModel();
      }
      else {
        SetGenenrateMachineTranslationsButton();
        btnAddSecondaryLanguage.Enabled = false;
        labelStatusBar.Text = "Not connected";
      }
    }

    public void LoadModel() {

      labelStatusBar.Text = "Loading Data Model...";

      var model = TranslationsManager.model;
      var connection = TranslationsManager.ActiveConnection;

      txtServerConnection.Text = connection.ConnectString;
      tooltipServiceConnection.SetToolTip(txtServerConnection, connection.ConnectString);
      txtDatasetName.Text = connection.DatasetName + (connection.ConnectionType == ConnectionType.PowerBiDesktop ? ".pbix" : "");
      tooltipDatasetName.SetToolTip(txtDatasetName, connection.DatasetName);

      txtdefaultLanguage.Text = SupportedLanguages.AllLanguages[model.Culture].FullName;
      txtCompatibilityLevel.Text = model.Database.CompatibilityLevel.ToString();

      listSecondaryLanguages.Items.Clear();
      listSecondaryLanguages.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());

      listLanguageForTransation.Items.Clear();
      listLanguageForTransation.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
      if (listLanguageForTransation.Items.Count > 0) {
        listLanguageForTransation.SelectedIndex = 0;
      }

      listCultureToPopulate.Items.Clear();
      listCultureToPopulate.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
      if (listCultureToPopulate.Items.Count > 0) {
        listCultureToPopulate.SelectedIndex = 0;
      }

      gridTranslations.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Black;
      gridTranslations.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
      gridTranslations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      gridTranslations.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
      gridTranslations.AllowUserToAddRows = false;

      SetGenenrateMachineTranslationsButton();

      SetMenuCommands();

      PopulateGridWithTranslations();

      labelStatusBar.Text = "Data Model loaded successfully";

    }

    private void menuConnect_Click(object sender, EventArgs e) {

      bool sessionsExist = (PowerBiDesktopUtilities.GetActiveDatasetConnections().Count() > 0);

      if (sessionsExist) {
        using (FormConnect dialog = new FormConnect()) {
          dialog.StartPosition = FormStartPosition.CenterScreen;
          dialog.ShowDialog(this);
          if (dialog.DialogResult == DialogResult.OK && !string.IsNullOrEmpty(dialog.ConnectString)) {
            if (TranslationsManager.IsConnected) {
              TranslationsManager.Disconnect();
              InitializeAfterDisconected();
            }
            TranslationsManager.Connect(dialog.ConnectString);
            LoadModel();
          }
        }
      }
      else {
        UserInteraction.PromptUserWithInformation("You cannot create a local dataset connection because there are no active sessions of Power BI Desktop.\r\n\r\nStart a session of Power BI Desktop so you can to connect.");
      }

    }

    private void menuDisconnect_Click(object sender, EventArgs e) {
      TranslationsManager.Disconnect();
      InitializeAfterDisconected();
    }

    public void InitializeAfterDisconected() {

      txtServerConnection.Text = "";
      txtDatasetName.Text = "";

      txtdefaultLanguage.Text = "";
      txtCompatibilityLevel.Text = "";

      listSecondaryLanguages.Items.Clear();

      listLanguageForTransation.Items.Clear();

      listCultureToPopulate.Items.Clear();

      gridTranslations.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Black;
      gridTranslations.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
      gridTranslations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      gridTranslations.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
      gridTranslations.AllowUserToAddRows = false;

      SetMenuCommands();

      gridTranslations.Rows.Clear();
      gridTranslations.Columns.Clear();

      labelStatusBar.Text = "Not connected";

    }

    public void PopulateGridWithTranslations() {

      gridTranslations.Rows.Clear();
      gridTranslations.Columns.Clear();

      TranslationsManager.RefreshDataFromServer();
      var translationsTable = TranslationsManager.GetTranslationsTable();

      // populate colum headers
      gridTranslations.ColumnCount = translationsTable.Headers.Length;
      for (int index = 0; index <= translationsTable.Headers.Length - 1; index++) {
        string language = translationsTable.Headers[index];
        gridTranslations.Columns[index].Name = language;

        // add context menu to delete secondary language
        if (index > 3) {
          gridTranslations.Columns[index].HeaderCell.ContextMenuStrip = contextMenuSecondaryLanguageHeader;
        }
        // only enable translated column cells for update
        gridTranslations.Columns[index].ReadOnly = index <= 3;
      }

      // populate rows
      foreach (var row in translationsTable.Rows) {
        gridTranslations.Rows.Add(row);
      }

      gridTranslations.AutoResizeColumns();
      gridTranslations.ClearSelection();
      this.Refresh();
    }

    private void AddSecondaryLanguage(object sender, EventArgs e) {

      using (FormAddLanguage dialog = new FormAddLanguage()) {

        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.ShowDialog(this);

        if (dialog.DialogResult == DialogResult.OK) {

          string[] languages = dialog.GetSelectedLanguages();
          string lastLanguage = "";
          foreach (string language in languages) {
            TranslationsManager.model.Cultures.Add(new Culture { Name = language });
            lastLanguage = language;
          }
          TranslationsManager.model.SaveChanges();
          listSecondaryLanguages.Items.Clear();
          listSecondaryLanguages.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
          PopulateGridWithTranslations();

          string lastLanguageFullName = SupportedLanguages.AllLanguages[lastLanguage].FullName;

          listCultureToPopulate.Items.Clear();
          listCultureToPopulate.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
          listCultureToPopulate.SelectedIndex = listCultureToPopulate.Items.IndexOf(lastLanguageFullName);

          listLanguageForTransation.Items.Clear();
          listLanguageForTransation.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
          listLanguageForTransation.SelectedIndex = listLanguageForTransation.Items.IndexOf(lastLanguageFullName);
          labelStatusBar.Text = "New Culture[" + lastLanguageFullName + "] successfully added";
        }
      }

    }

    private void ExportTranslationsSheet(object sender, EventArgs e) {
      labelStatusBar.Text = "Exporting Translations";
      Language targetLanguage = SupportedLanguages.GetLanguageFromFullName(listLanguageForTransation.SelectedItem.ToString());
      TranslationsManager.ExportTranslations(targetLanguage.LanguageId, chkOpenExportInExcel.Checked);
      labelStatusBar.Text = "Translations export Complete";
    }

    private void ExportAllTranslations(object sender, EventArgs e) {
      labelStatusBar.Text = "Exporting Translations";
      TranslationsManager.ExportTranslations(null, chkOpenExportInExcel.Checked);
      labelStatusBar.Text = "Translations export Complete";
    }

    private void ExportAllTranslationSheets(object sender, EventArgs e) {
      labelStatusBar.Text = "Exporting Translations";
      TranslationsManager.ExportAllTranslationSheets(chkOpenExportInExcel.Checked);
      labelStatusBar.Text = "Translations export Complete";
    }

    private void ImportTranslations(object sender, EventArgs e) {

      dialogOpenFile.InitialDirectory = AppSettings.TranslationsInboxFolderPath;
      dialogOpenFile.Filter = "CSV files (*.csv)|*.csv";
      dialogOpenFile.FilterIndex = 1;
      dialogOpenFile.RestoreDirectory = true;

      if (dialogOpenFile.ShowDialog() == DialogResult.OK) {
        TranslationsManager.ImportTranslations(dialogOpenFile.FileName);
        LoadModel();
        PopulateGridWithTranslations();
      }

    }

    private void ConfigureSettings(object sender, EventArgs e) {
      using (FormConfig dialog = new FormConfig()) {
        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.ShowDialog(this);
        if (dialog.DialogResult == DialogResult.OK) {
          SetGenenrateMachineTranslationsButton();
        }
      }
    }

    private void SetGenenrateMachineTranslationsButton() {
      if (TranslatorService.IsAvailable) {
        grpMachineTranslationsSingleLanguage.Visible = true;
        grpMachineTranslationsAllLanguages.Visible = true;
      }
      else {
        grpMachineTranslationsSingleLanguage.Visible = false;
        grpMachineTranslationsAllLanguages.Visible = false;
      }
    }

    private void SetMenuCommands() {

      menuDisconnect.Enabled = TranslationsManager.IsConnected;

      btnAddSecondaryLanguage.Enabled = TranslationsManager.IsConnected;

      bool localizedLabelsTableExists = TranslationsManager.DoesTableExistInModel(TranslationsManager.LocalizedLabelsTableName);
      menuCreateLocalizedLabelsTable.Enabled = !localizedLabelsTableExists;
      menuAddLabelsToLocalizedLabelsTable.Enabled = localizedLabelsTableExists;
      menuGenerateTranslatedLocalizedLabelsTable.Enabled = localizedLabelsTableExists;
      menuGenerateTranslatedDatasetObjectNamesTable.Enabled = TranslationsManager.TranslationsExist();
    }

    private void GenenrateMachineTranslations(object sender, EventArgs e) {

      Language targetLanguage = SupportedLanguages.GetLanguageFromFullName(listCultureToPopulate.SelectedItem.ToString());
      string targetLanguageId = targetLanguage.LanguageId;

      using (FormLoadingStatus dialog = new FormLoadingStatus()) {
        dialog.StartPosition = FormStartPosition.CenterScreen;
        dialog.Show(this);
        TranslationsManager.PopulateCultureWithMachineTranslations(targetLanguageId, dialog);
        dialog.Close();
      }

      PopulateGridWithTranslations();
    }

    private void FillEmptyTranslations(object sender, EventArgs e) {

      Language targetLanguage = SupportedLanguages.GetLanguageFromFullName(listCultureToPopulate.SelectedItem.ToString());
      string targetLanguageId = targetLanguage.LanguageId;

      using (FormLoadingStatus dialog = new FormLoadingStatus()) {
        dialog.StartPosition = FormStartPosition.CenterScreen;
        dialog.Show(this);
        TranslationsManager.PopulateEmptyTranslations(targetLanguageId, dialog);
        dialog.Close();
      }

      PopulateGridWithTranslations();
    }

    private void GenenrateAllMachineTranslations(object sender, EventArgs e) {

      using (FormLoadingStatus dialog = new FormLoadingStatus()) {

        dialog.StartPosition = FormStartPosition.CenterScreen;
        dialog.Show(this);

        foreach (var language in TranslationsManager.GetSecondaryLanguagesInDataModel()) {
          TranslationsManager.PopulateCultureWithMachineTranslations(language, dialog);
          PopulateGridWithTranslations();
        }

        dialog.Close();
      }

      PopulateGridWithTranslations();

    }

    private void FillAllEmptyTranslations(object sender, EventArgs e) {

      labelStatusBar.Text = "Executing FillAllEmptyTranslations operation...";

      using (FormLoadingStatus dialog = new FormLoadingStatus()) {

        dialog.StartPosition = FormStartPosition.CenterScreen;
        dialog.Show(this);

        foreach (var language in TranslationsManager.GetSecondaryLanguagesInDataModel()) {
          TranslationsManager.PopulateEmptyTranslations(language, dialog);
          PopulateGridWithTranslations();
        }

        dialog.Close();
      }
      PopulateGridWithTranslations();
      labelStatusBar.Text = "FillAllEmptyTranslations operation completed successfully";
    }

    private void RefreshTranslationsTable(object sender, EventArgs e) {
      PopulateGridWithTranslations();
    }

    private void CreateLocalizedLabelsTable(object sender, EventArgs e) {
      labelStatusBar.Text = "Creating Localized Labels table...";
      TranslationsManager.CreateLocalizedLabelsTable();
      SetMenuCommands();
      PopulateGridWithTranslations();
      labelStatusBar.Text = "Localized Labels table generated at " + DateTime.Now.ToShortTimeString();
      UserInteraction.PromptOnLocalizedLabelsTableCreate();
    }

    private void GenerateTranslatedLocalizedLabelsTable(object sender, EventArgs e) {
      labelStatusBar.Text = "Creating Translated Localized Labels table...";
      TranslationsManager.GenerateTranslatedLocalizedLabelsTable();
      labelStatusBar.Text = "[Translated Localized Labels] table generated at " + DateTime.Now.ToShortTimeString();

    }

    private void GenerateTranslatedDatasetObjectNamesTable(object sender, EventArgs e) {
      labelStatusBar.Text = "Creating Translated Database Object Names table...";
      TranslationsManager.GenerateTranslatedDatasetObjectsTable();
      labelStatusBar.Text = "[Translated Database Object Names] table generated at " + DateTime.Now.ToShortTimeString();
    }

    private void SyncDataModel(object sender, EventArgs e) {
      PopulateGridWithTranslations();
      SetMenuCommands();
    }

    private void AddLocalizedLabels(object sender, EventArgs e) {

      using (FormAddLocalizedLabels dialog = new FormAddLocalizedLabels()) {

        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.frmMain = this;
        dialog.ShowDialog(this);
        if (dialog.DialogResult == DialogResult.OK) {
          labelStatusBar.Text = "Localized Label added at " + DateTime.Now.ToShortTimeString();
          PopulateGridWithTranslations();
        }
      }
    }

    private void gridTranslations_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {

      if (e.ColumnIndex == -1 && TranslatorService.IsAvailable) {
        // allow double click on row heading to generate machine translation for all secondary languages across row

        string objectType = gridTranslations.Rows[e.RowIndex].Cells[0]?.Value.ToString();
        string propertyName = gridTranslations.Rows[e.RowIndex].Cells[1]?.Value.ToString();
        string objectName = gridTranslations.Rows[e.RowIndex].Cells[2]?.Value.ToString();

        using (FormLoadingStatus dialog = new FormLoadingStatus()) {
          dialog.StartPosition = FormStartPosition.CenterScreen;
          dialog.Show(this);
          TranslationsManager.PopulateDatasetObjectWithMachineTranslations(objectType, propertyName, objectName, dialog);
          dialog.Close();
        }
        PopulateGridWithTranslations();

      }

      if (e.ColumnIndex == 2) {
        var cell = gridTranslations.Rows[e.RowIndex].Cells[e.ColumnIndex];
        string cellContents = (cell.Value != null) ? cell.Value.ToString() : "";

        if (cellContents.Contains("Localized Labels")) {
          string labelName = cellContents.Replace("Localized Labels[", "").Replace("]", "");
          using (FormEditLocalizedLabel dialog = new FormEditLocalizedLabel(labelName)) {
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
            if (dialog.DialogResult == DialogResult.OK) {
              labelStatusBar.Text = "Measure (" + labelName + ") generated at " + DateTime.Now.ToShortTimeString();
              PopulateGridWithTranslations();
            }
          }
        }
      }

      if (e.ColumnIndex == 3 && e.RowIndex >= 0) {
        var cell = gridTranslations.Rows[e.RowIndex].Cells[e.ColumnIndex];
        string cellContents = (cell.Value != null) ? cell.Value.ToString() : "";

        // this is the default translation when column index is 3
        var ObjectType = gridTranslations.Rows[e.RowIndex].Cells[0].Value.ToString();
        var PropertyName = gridTranslations.Rows[e.RowIndex].Cells[1].Value.ToString();
        var ObjectName = gridTranslations.Rows[e.RowIndex].Cells[2].Value.ToString();
        var Language = SupportedLanguages.GetLanguageFromFullName(gridTranslations.Columns[e.ColumnIndex].HeaderText).LanguageId;


        using (FormEditDefaultTranslation dialog = new FormEditDefaultTranslation(ObjectType, PropertyName, ObjectName, Language, cellContents)) {
          dialog.StartPosition = FormStartPosition.CenterParent;
          dialog.ShowDialog(this);
          if (dialog.DialogResult == DialogResult.OK) {
            labelStatusBar.Text = ObjectName + " updated at " + DateTime.Now.ToShortTimeString();
            PopulateGridWithTranslations();
          }
        }

      }

      if (e.ColumnIndex >= 4 && e.RowIndex >= 0) {
        var cell = gridTranslations.Rows[e.RowIndex].Cells[e.ColumnIndex];
        string cellContents = (cell.Value != null) ? cell.Value.ToString() : "";

        // this is a secondary translation when column index is 4 or greater
        var ObjectType = gridTranslations.Rows[e.RowIndex].Cells[0].Value.ToString();
        var PropertyName = gridTranslations.Rows[e.RowIndex].Cells[1].Value.ToString();
        var ObjectName = gridTranslations.Rows[e.RowIndex].Cells[2].Value.ToString();
        var Language = SupportedLanguages.GetLanguageFromFullName(gridTranslations.Columns[e.ColumnIndex].HeaderText).LanguageId;


        using (FormEditTranslation dialog = new FormEditTranslation(ObjectType, PropertyName, ObjectName, Language, cellContents)) {
          dialog.StartPosition = FormStartPosition.CenterParent;
          dialog.ShowDialog(this);
          if (dialog.DialogResult == DialogResult.OK) {
            labelStatusBar.Text = ObjectName + " updated at " + DateTime.Now.ToShortTimeString();
            PopulateGridWithTranslations();
          }
        }

      }

    }

    private void gridTranslations_CellEndEdit(object sender, DataGridViewCellEventArgs e) {

      // get updated cell
      var cell = gridTranslations.Rows[e.RowIndex].Cells[e.ColumnIndex];

      // update translation value in model
      var ObjectType = gridTranslations.Rows[e.RowIndex].Cells[0].Value.ToString();
      var PropertyName = gridTranslations.Rows[e.RowIndex].Cells[1].Value.ToString();
      var ObjectName = gridTranslations.Rows[e.RowIndex].Cells[2].Value.ToString();
      var Language = SupportedLanguages.GetLanguageFromFullName(gridTranslations.Columns[e.ColumnIndex].HeaderText).LanguageId;
      string Translation = (cell.Value != null) ? cell.Value.ToString() : "";
      TranslationsManager.SetDatasetObjectTranslation(ObjectType, PropertyName, ObjectName, Language, Translation);
      cell.Style.BackColor = System.Drawing.Color.White;
    }

    private void gridTranslations_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {

      var cell = gridTranslations.Rows[e.RowIndex].Cells[e.ColumnIndex];
      cell.Style.BackColor = System.Drawing.Color.Yellow;

    }

    private void exportModelAsJson_Click(object sender, EventArgs e) {
      TranslationsManager.ExportModelAsBim();
    }

    private void menuCommandDeleteSecondaryLanguage_MouseDown(object sender, MouseEventArgs e) {

      ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
      ToolStrip contextMenu = menuItem.GetCurrentParent() as ToolStrip;
      Point pScreen = contextMenu.PointToScreen(new Point(0, 0)); // e.X, e.Y)); // ;
      Point pGrid = gridTranslations.PointToClient(pScreen);
      var columnIndex = gridTranslations.HitTest(pGrid.X, pGrid.Y).ColumnIndex;
      string language = gridTranslations.Columns[columnIndex].HeaderText;

      contextMenu.Hide();

      string confirmDeleteMessage = "Are you sure you want to delete the language - " + language + "?\r\n" +
                              "Deleting this secondary language will delete all its translations.";

      contextMenu.Hide();

      bool userConfirmedOperation = UserInteraction.PromptUserToConfirmOperation(confirmDeleteMessage, "Delete Secondary Language Operation");
      if (userConfirmedOperation) {

        string LanguageId = SupportedLanguages.GetLanguageFromFullName(language).LanguageId;
        TranslationsManager.DeleteSecondaryLanguage(LanguageId);

        listSecondaryLanguages.Items.Clear();
        listSecondaryLanguages.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());

        listLanguageForTransation.Items.Remove(language);
        if (listLanguageForTransation.SelectedIndex == -1 && listLanguageForTransation.Items.Count > 0) {
          listLanguageForTransation.SelectedIndex = 0;
        }

        listCultureToPopulate.Items.Remove(language);
        if (listCultureToPopulate.SelectedIndex == -1 && listCultureToPopulate.Items.Count > 0) {
          listCultureToPopulate.SelectedIndex = 0;
        }

        PopulateGridWithTranslations();
      }

    }

    private void menuCommandDuplicateSecondaryLanguage_MouseDown(object sender, MouseEventArgs e) {

      ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
      ToolStrip contextMenu = menuItem.GetCurrentParent() as ToolStrip;
      Point pScreen = contextMenu.PointToScreen(new Point(0, 0)); // e.X, e.Y)); // ;
      Point pGrid = gridTranslations.PointToClient(pScreen);
      var columnIndex = gridTranslations.HitTest(pGrid.X, pGrid.Y).ColumnIndex;

      contextMenu.Hide();

      string sourceCultureFullName = gridTranslations.Columns[columnIndex].HeaderText;
      string sourceCultureIdentifier = SupportedLanguages.GetLanguageFromFullName(sourceCultureFullName).LanguageId;
      string sourceLanguageGroup = SupportedLanguages.GetLanguageFromFullName(sourceCultureFullName).LanguageGroup;
      Culture sourceCulture = TranslationsManager.model.Cultures[sourceCultureIdentifier];

      using (FormAddLanguage dialog = new FormAddLanguage(sourceLanguageGroup)) {

        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.ShowDialog(this);

        if (dialog.DialogResult == DialogResult.OK) {

          string[] languages = dialog.GetSelectedLanguages();
          string lastLanguage = "";
          foreach (string language in languages) {
            TranslationsManager.DuplicateLanguage(sourceCulture, language);
            lastLanguage = language;
          }
          listSecondaryLanguages.Items.Clear();
          listSecondaryLanguages.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
          PopulateGridWithTranslations();

          string lastLanguageFullName = SupportedLanguages.AllLanguages[lastLanguage].FullName;

          listCultureToPopulate.Items.Clear();
          listCultureToPopulate.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
          listCultureToPopulate.SelectedIndex = listCultureToPopulate.Items.IndexOf(lastLanguageFullName);

          listLanguageForTransation.Items.Clear();
          listLanguageForTransation.Items.AddRange(TranslationsManager.GetSecondaryLanguageFullNamesInDataModel().ToArray());
          listLanguageForTransation.SelectedIndex = listLanguageForTransation.Items.IndexOf(lastLanguageFullName);
          labelStatusBar.Text = "New Culture[" + lastLanguageFullName + "] successfully added";
        }
      }




    }

    private void menuCommandCopyToSecondaryLanguage_MouseDown(object sender, MouseEventArgs e) {

      ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
      ToolStrip contextMenu = menuItem.GetCurrentParent() as ToolStrip;
      Point pScreen = contextMenu.PointToScreen(new Point(0, 0)); // e.X, e.Y)); // ;
      Point pGrid = gridTranslations.PointToClient(pScreen);
      var columnIndex = gridTranslations.HitTest(pGrid.X, pGrid.Y).ColumnIndex;

      contextMenu.Hide();

      string sourceLanguageFullName = gridTranslations.Columns[columnIndex].HeaderText;
      string sourceLanguageId = SupportedLanguages.GetLanguageFromFullName(sourceLanguageFullName).LanguageId;

      using (FormSelectExisitngLanguage dialog = new FormSelectExisitngLanguage(sourceLanguageId)) {

        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.ShowDialog(this);

        if (dialog.DialogResult == DialogResult.OK) {

          string[] languages = dialog.getLanguages();
          string lastLanguage = "";
          foreach (string targetLanguageId in languages) {
            TranslationsManager.CopyTranslationsToExisitngLanguage(sourceLanguageId, targetLanguageId);
            lastLanguage = targetLanguageId;
          }

          PopulateGridWithTranslations();

          labelStatusBar.Text = "Exist Culture[" + lastLanguage + "] successfully added";
        }
      }



    }

    private void menuCommandExportLanguageToTranslationSheet_MouseDown(object sender, MouseEventArgs e) {

      ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
      ToolStrip contextMenu = menuItem.GetCurrentParent() as ToolStrip;
      Point pScreen = contextMenu.PointToScreen(new Point(0, 0)); // e.X, e.Y)); // ;
      Point pGrid = gridTranslations.PointToClient(pScreen);
      var columnIndex = gridTranslations.HitTest(pGrid.X, pGrid.Y).ColumnIndex;

      contextMenu.Hide();

      labelStatusBar.Text = "Exporting Translations";
      string targetLanguageFullName = gridTranslations.Columns[columnIndex].HeaderText;
      string targetLanguageId = SupportedLanguages.GetLanguageFromFullName(targetLanguageFullName).LanguageId;
      TranslationsManager.ExportTranslations(targetLanguageId, chkOpenExportInExcel.Checked);
      labelStatusBar.Text = "Translations export Complete";


    }
  }
}
