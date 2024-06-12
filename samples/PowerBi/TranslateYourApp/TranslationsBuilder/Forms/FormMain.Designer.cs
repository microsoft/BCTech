
using System.Drawing;
using System.Windows.Forms;

namespace TranslationsBuilder {
  partial class FormMain {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      gridTranslations = new DataGridView();
      dialogOpenFile = new OpenFileDialog();
      groupDatasetProperties = new GroupBox();
      lbldefaultLanguageNameLabel = new Label();
      txtdefaultLanguage = new TextBox();
      txtCompatibilityLevel = new TextBox();
      lblCompatibilityLevel = new Label();
      txtDatasetName = new TextBox();
      txtServerConnection = new TextBox();
      lblDatasetName = new Label();
      lblServerConnection = new Label();
      panel2 = new Panel();
      grpMachineTranslationsAllLanguages = new GroupBox();
      btnFillAllEmptyTranslations = new Button();
      btnGenenrateAllMachineTranslations = new Button();
      grpImportExportTranslations = new GroupBox();
      btnExportAllTranslationSheets = new Button();
      chkOpenExportInExcel = new CheckBox();
      listLanguageForTransation = new ComboBox();
      btnExportTranslations = new Button();
      btnImportTranslations = new Button();
      btnExportTranslationsSheet = new Button();
      grpMachineTranslationsSingleLanguage = new GroupBox();
      btnFillEmptyTranslations = new Button();
      btnGenenrateMachineTranslations = new Button();
      listCultureToPopulate = new ComboBox();
      grpSecondaryCultures = new GroupBox();
      btnAddSecondaryLanguage = new Button();
      listSecondaryLanguages = new ListBox();
      menuStrip = new MenuStrip();
      menuConnection = new ToolStripMenuItem();
      menuConnect = new ToolStripMenuItem();
      menuDisconnect = new ToolStripMenuItem();
      toolStripSeparator2 = new ToolStripSeparator();
      menuConfigureSettings = new ToolStripMenuItem();
      toolStripSeparator1 = new ToolStripSeparator();
      exportModelAsJson = new ToolStripMenuItem();
      menuTranslatedTables = new ToolStripMenuItem();
      menuCreateLocalizedLabelsTable = new ToolStripMenuItem();
      menuAddLabelsToLocalizedLabelsTable = new ToolStripMenuItem();
      menuGenerateTranslatedLocalizedLabelsTable = new ToolStripMenuItem();
      menuGenerateTranslatedDatasetObjectNamesTable = new ToolStripMenuItem();
      menuSyncDataModel = new ToolStripMenuItem();
      statusStrip1 = new StatusStrip();
      labelStatusBar = new ToolStripStatusLabel();
      toolStripStatusLabel1 = new ToolStripStatusLabel();
      toolStripStatusLabel2 = new ToolStripStatusLabel();
      tooltipDatasetName = new ToolTip(components);
      tooltipServiceConnection = new ToolTip(components);
      contextMenuSecondaryLanguageHeader = new ContextMenuStrip(components);
      toolStripSeparator5 = new ToolStripSeparator();
      menuCommandDuplicateSecondaryLanguage = new ToolStripMenuItem();
      menuCommandCopyToSecondaryLanguage = new ToolStripMenuItem();
      toolStripSeparator3 = new ToolStripSeparator();
      menuCommandExportLanguageToTranslationSheet = new ToolStripMenuItem();
      toolStripSeparator4 = new ToolStripSeparator();
      menuCommandDeleteSecondaryLanguage = new ToolStripMenuItem();
      toolStripSeparator6 = new ToolStripSeparator();
      ((System.ComponentModel.ISupportInitialize)gridTranslations).BeginInit();
      groupDatasetProperties.SuspendLayout();
      panel2.SuspendLayout();
      grpMachineTranslationsAllLanguages.SuspendLayout();
      grpImportExportTranslations.SuspendLayout();
      grpMachineTranslationsSingleLanguage.SuspendLayout();
      grpSecondaryCultures.SuspendLayout();
      menuStrip.SuspendLayout();
      statusStrip1.SuspendLayout();
      contextMenuSecondaryLanguageHeader.SuspendLayout();
      SuspendLayout();
      // 
      // gridTranslations
      // 
      gridTranslations.AllowUserToAddRows = false;
      gridTranslations.AllowUserToDeleteRows = false;
      gridTranslations.AllowUserToResizeColumns = false;
      gridTranslations.AllowUserToResizeRows = false;
      dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = Color.Black;
      dataGridViewCellStyle1.Font = new Font("Arial Black", 7F, FontStyle.Bold, GraphicsUnit.Point);
      dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      gridTranslations.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      gridTranslations.ColumnHeadersHeight = 29;
      gridTranslations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      gridTranslations.Dock = DockStyle.Fill;
      gridTranslations.EditMode = DataGridViewEditMode.EditProgrammatically;
      gridTranslations.EnableHeadersVisualStyles = false;
      gridTranslations.Location = new Point(0, 323);
      gridTranslations.MultiSelect = false;
      gridTranslations.Name = "gridTranslations";
      gridTranslations.RowHeadersWidth = 24;
      gridTranslations.RowTemplate.Height = 28;
      gridTranslations.Size = new Size(1519, 394);
      gridTranslations.TabIndex = 0;
      gridTranslations.TabStop = false;
      gridTranslations.CellBeginEdit += gridTranslations_CellBeginEdit;
      gridTranslations.CellDoubleClick += gridTranslations_CellDoubleClick;
      gridTranslations.CellEndEdit += gridTranslations_CellEndEdit;
      // 
      // groupDatasetProperties
      // 
      groupDatasetProperties.Controls.Add(lbldefaultLanguageNameLabel);
      groupDatasetProperties.Controls.Add(txtdefaultLanguage);
      groupDatasetProperties.Controls.Add(txtCompatibilityLevel);
      groupDatasetProperties.Controls.Add(lblCompatibilityLevel);
      groupDatasetProperties.Controls.Add(txtDatasetName);
      groupDatasetProperties.Controls.Add(txtServerConnection);
      groupDatasetProperties.Controls.Add(lblDatasetName);
      groupDatasetProperties.Controls.Add(lblServerConnection);
      groupDatasetProperties.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
      groupDatasetProperties.Location = new Point(9, 47);
      groupDatasetProperties.Name = "groupDatasetProperties";
      groupDatasetProperties.Size = new Size(472, 269);
      groupDatasetProperties.TabIndex = 0;
      groupDatasetProperties.TabStop = false;
      groupDatasetProperties.Text = "Dataset Properties";
      // 
      // lbldefaultLanguageNameLabel
      // 
      lbldefaultLanguageNameLabel.AutoSize = true;
      lbldefaultLanguageNameLabel.Location = new Point(6, 115);
      lbldefaultLanguageNameLabel.Name = "lbldefaultLanguageNameLabel";
      lbldefaultLanguageNameLabel.Size = new Size(130, 20);
      lbldefaultLanguageNameLabel.TabIndex = 33;
      lbldefaultLanguageNameLabel.Text = "Default Language:";
      // 
      // txtdefaultLanguage
      // 
      txtdefaultLanguage.Location = new Point(141, 112);
      txtdefaultLanguage.Name = "txtdefaultLanguage";
      txtdefaultLanguage.Size = new Size(307, 27);
      txtdefaultLanguage.TabIndex = 32;
      txtdefaultLanguage.TabStop = false;
      // 
      // txtCompatibilityLevel
      // 
      txtCompatibilityLevel.Location = new Point(141, 152);
      txtCompatibilityLevel.Name = "txtCompatibilityLevel";
      txtCompatibilityLevel.ReadOnly = true;
      txtCompatibilityLevel.Size = new Size(307, 27);
      txtCompatibilityLevel.TabIndex = 22;
      txtCompatibilityLevel.TabStop = false;
      // 
      // lblCompatibilityLevel
      // 
      lblCompatibilityLevel.AutoSize = true;
      lblCompatibilityLevel.Location = new Point(33, 153);
      lblCompatibilityLevel.Name = "lblCompatibilityLevel";
      lblCompatibilityLevel.Size = new Size(103, 20);
      lblCompatibilityLevel.TabIndex = 21;
      lblCompatibilityLevel.Text = "Compat Level:";
      lblCompatibilityLevel.TextAlign = ContentAlignment.MiddleRight;
      // 
      // txtDatasetName
      // 
      txtDatasetName.Location = new Point(141, 72);
      txtDatasetName.Name = "txtDatasetName";
      txtDatasetName.ReadOnly = true;
      txtDatasetName.Size = new Size(307, 27);
      txtDatasetName.TabIndex = 20;
      txtDatasetName.TabStop = false;
      // 
      // txtServerConnection
      // 
      txtServerConnection.Location = new Point(141, 32);
      txtServerConnection.Margin = new Padding(10, 3, 3, 3);
      txtServerConnection.Name = "txtServerConnection";
      txtServerConnection.ReadOnly = true;
      txtServerConnection.Size = new Size(307, 27);
      txtServerConnection.TabIndex = 0;
      txtServerConnection.TabStop = false;
      // 
      // lblDatasetName
      // 
      lblDatasetName.AutoSize = true;
      lblDatasetName.Location = new Point(29, 75);
      lblDatasetName.Name = "lblDatasetName";
      lblDatasetName.Size = new Size(107, 20);
      lblDatasetName.TabIndex = 18;
      lblDatasetName.Text = "Dataset Name:";
      lblDatasetName.TextAlign = ContentAlignment.MiddleRight;
      // 
      // lblServerConnection
      // 
      lblServerConnection.AutoSize = true;
      lblServerConnection.Location = new Point(49, 36);
      lblServerConnection.Name = "lblServerConnection";
      lblServerConnection.Size = new Size(87, 20);
      lblServerConnection.TabIndex = 17;
      lblServerConnection.Text = "Connection:";
      lblServerConnection.TextAlign = ContentAlignment.MiddleRight;
      // 
      // panel2
      // 
      panel2.BackColor = SystemColors.ActiveCaption;
      panel2.Controls.Add(grpMachineTranslationsAllLanguages);
      panel2.Controls.Add(grpImportExportTranslations);
      panel2.Controls.Add(grpMachineTranslationsSingleLanguage);
      panel2.Controls.Add(grpSecondaryCultures);
      panel2.Controls.Add(groupDatasetProperties);
      panel2.Controls.Add(menuStrip);
      panel2.Dock = DockStyle.Top;
      panel2.Location = new Point(0, 0);
      panel2.Name = "panel2";
      panel2.Size = new Size(1519, 323);
      panel2.TabIndex = 8;
      // 
      // grpMachineTranslationsAllLanguages
      // 
      grpMachineTranslationsAllLanguages.Controls.Add(btnFillAllEmptyTranslations);
      grpMachineTranslationsAllLanguages.Controls.Add(btnGenenrateAllMachineTranslations);
      grpMachineTranslationsAllLanguages.Location = new Point(981, 191);
      grpMachineTranslationsAllLanguages.Name = "grpMachineTranslationsAllLanguages";
      grpMachineTranslationsAllLanguages.Size = new Size(294, 125);
      grpMachineTranslationsAllLanguages.TabIndex = 10;
      grpMachineTranslationsAllLanguages.TabStop = false;
      grpMachineTranslationsAllLanguages.Text = "Machine Translations - All Languages";
      // 
      // btnFillAllEmptyTranslations
      // 
      btnFillAllEmptyTranslations.BackColor = SystemColors.ControlLight;
      btnFillAllEmptyTranslations.Location = new Point(11, 72);
      btnFillAllEmptyTranslations.Name = "btnFillAllEmptyTranslations";
      btnFillAllEmptyTranslations.Size = new Size(267, 37);
      btnFillAllEmptyTranslations.TabIndex = 15;
      btnFillAllEmptyTranslations.Text = "Fill All Empty Translations";
      btnFillAllEmptyTranslations.UseVisualStyleBackColor = false;
      btnFillAllEmptyTranslations.Click += FillAllEmptyTranslations;
      // 
      // btnGenenrateAllMachineTranslations
      // 
      btnGenenrateAllMachineTranslations.BackColor = SystemColors.ControlLight;
      btnGenenrateAllMachineTranslations.Location = new Point(10, 28);
      btnGenenrateAllMachineTranslations.Name = "btnGenenrateAllMachineTranslations";
      btnGenenrateAllMachineTranslations.Size = new Size(269, 36);
      btnGenenrateAllMachineTranslations.TabIndex = 14;
      btnGenenrateAllMachineTranslations.Text = "Generate All Translations";
      btnGenenrateAllMachineTranslations.UseVisualStyleBackColor = false;
      btnGenenrateAllMachineTranslations.Click += GenenrateAllMachineTranslations;
      // 
      // grpImportExportTranslations
      // 
      grpImportExportTranslations.Controls.Add(btnExportAllTranslationSheets);
      grpImportExportTranslations.Controls.Add(chkOpenExportInExcel);
      grpImportExportTranslations.Controls.Add(listLanguageForTransation);
      grpImportExportTranslations.Controls.Add(btnExportTranslations);
      grpImportExportTranslations.Controls.Add(btnImportTranslations);
      grpImportExportTranslations.Controls.Add(btnExportTranslationsSheet);
      grpImportExportTranslations.Location = new Point(730, 47);
      grpImportExportTranslations.Name = "grpImportExportTranslations";
      grpImportExportTranslations.Size = new Size(235, 269);
      grpImportExportTranslations.TabIndex = 4;
      grpImportExportTranslations.TabStop = false;
      grpImportExportTranslations.Text = "Export/Import Translations";
      // 
      // btnExportAllTranslationSheets
      // 
      btnExportAllTranslationSheets.BackColor = SystemColors.ControlLight;
      btnExportAllTranslationSheets.Location = new Point(13, 144);
      btnExportAllTranslationSheets.Name = "btnExportAllTranslationSheets";
      btnExportAllTranslationSheets.Size = new Size(209, 36);
      btnExportAllTranslationSheets.TabIndex = 10;
      btnExportAllTranslationSheets.Text = "Export All Translation Sheets";
      btnExportAllTranslationSheets.UseVisualStyleBackColor = false;
      btnExportAllTranslationSheets.Click += ExportAllTranslationSheets;
      // 
      // chkOpenExportInExcel
      // 
      chkOpenExportInExcel.AutoSize = true;
      chkOpenExportInExcel.Checked = true;
      chkOpenExportInExcel.CheckState = CheckState.Checked;
      chkOpenExportInExcel.Location = new Point(31, 187);
      chkOpenExportInExcel.Name = "chkOpenExportInExcel";
      chkOpenExportInExcel.Size = new Size(168, 24);
      chkOpenExportInExcel.TabIndex = 9;
      chkOpenExportInExcel.Text = "Open Export in Excel";
      chkOpenExportInExcel.UseVisualStyleBackColor = true;
      // 
      // listLanguageForTransation
      // 
      listLanguageForTransation.BackColor = SystemColors.Window;
      listLanguageForTransation.FormattingEnabled = true;
      listLanguageForTransation.Location = new Point(13, 68);
      listLanguageForTransation.Name = "listLanguageForTransation";
      listLanguageForTransation.Size = new Size(207, 28);
      listLanguageForTransation.TabIndex = 6;
      // 
      // btnExportTranslations
      // 
      btnExportTranslations.BackColor = SystemColors.ControlLight;
      btnExportTranslations.Location = new Point(11, 101);
      btnExportTranslations.Name = "btnExportTranslations";
      btnExportTranslations.Size = new Size(210, 36);
      btnExportTranslations.TabIndex = 7;
      btnExportTranslations.Text = "Export All Translations";
      btnExportTranslations.UseVisualStyleBackColor = false;
      btnExportTranslations.Click += ExportAllTranslations;
      // 
      // btnImportTranslations
      // 
      btnImportTranslations.BackColor = SystemColors.ControlLight;
      btnImportTranslations.Location = new Point(11, 216);
      btnImportTranslations.Name = "btnImportTranslations";
      btnImportTranslations.Size = new Size(210, 36);
      btnImportTranslations.TabIndex = 8;
      btnImportTranslations.Text = "Import Translations";
      btnImportTranslations.UseVisualStyleBackColor = false;
      btnImportTranslations.Click += ImportTranslations;
      // 
      // btnExportTranslationsSheet
      // 
      btnExportTranslationsSheet.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      btnExportTranslationsSheet.BackColor = SystemColors.ControlLight;
      btnExportTranslationsSheet.Location = new Point(11, 27);
      btnExportTranslationsSheet.Name = "btnExportTranslationsSheet";
      btnExportTranslationsSheet.Size = new Size(210, 36);
      btnExportTranslationsSheet.TabIndex = 5;
      btnExportTranslationsSheet.Text = "Export Translations Sheet";
      btnExportTranslationsSheet.UseVisualStyleBackColor = false;
      btnExportTranslationsSheet.Click += ExportTranslationsSheet;
      // 
      // grpMachineTranslationsSingleLanguage
      // 
      grpMachineTranslationsSingleLanguage.Controls.Add(btnFillEmptyTranslations);
      grpMachineTranslationsSingleLanguage.Controls.Add(btnGenenrateMachineTranslations);
      grpMachineTranslationsSingleLanguage.Controls.Add(listCultureToPopulate);
      grpMachineTranslationsSingleLanguage.Location = new Point(978, 47);
      grpMachineTranslationsSingleLanguage.Name = "grpMachineTranslationsSingleLanguage";
      grpMachineTranslationsSingleLanguage.Size = new Size(294, 140);
      grpMachineTranslationsSingleLanguage.TabIndex = 9;
      grpMachineTranslationsSingleLanguage.TabStop = false;
      grpMachineTranslationsSingleLanguage.Text = "Machine Translations - Single Language";
      // 
      // btnFillEmptyTranslations
      // 
      btnFillEmptyTranslations.BackColor = SystemColors.ControlLight;
      btnFillEmptyTranslations.ForeColor = SystemColors.ControlText;
      btnFillEmptyTranslations.Location = new Point(8, 99);
      btnFillEmptyTranslations.Name = "btnFillEmptyTranslations";
      btnFillEmptyTranslations.Size = new Size(271, 36);
      btnFillEmptyTranslations.TabIndex = 14;
      btnFillEmptyTranslations.Text = "Fill Empty Translations";
      btnFillEmptyTranslations.UseVisualStyleBackColor = false;
      btnFillEmptyTranslations.Click += FillEmptyTranslations;
      // 
      // btnGenenrateMachineTranslations
      // 
      btnGenenrateMachineTranslations.BackColor = SystemColors.ControlLight;
      btnGenenrateMachineTranslations.ForeColor = SystemColors.ControlText;
      btnGenenrateMachineTranslations.Location = new Point(8, 27);
      btnGenenrateMachineTranslations.Name = "btnGenenrateMachineTranslations";
      btnGenenrateMachineTranslations.Size = new Size(271, 36);
      btnGenenrateMachineTranslations.TabIndex = 10;
      btnGenenrateMachineTranslations.Text = "Generate Translations";
      btnGenenrateMachineTranslations.UseVisualStyleBackColor = false;
      btnGenenrateMachineTranslations.Click += GenenrateMachineTranslations;
      // 
      // listCultureToPopulate
      // 
      listCultureToPopulate.FormattingEnabled = true;
      listCultureToPopulate.Location = new Point(8, 67);
      listCultureToPopulate.Name = "listCultureToPopulate";
      listCultureToPopulate.Size = new Size(271, 28);
      listCultureToPopulate.TabIndex = 11;
      // 
      // grpSecondaryCultures
      // 
      grpSecondaryCultures.Controls.Add(btnAddSecondaryLanguage);
      grpSecondaryCultures.Controls.Add(listSecondaryLanguages);
      grpSecondaryCultures.Location = new Point(494, 45);
      grpSecondaryCultures.Name = "grpSecondaryCultures";
      grpSecondaryCultures.Size = new Size(224, 269);
      grpSecondaryCultures.TabIndex = 2;
      grpSecondaryCultures.TabStop = false;
      grpSecondaryCultures.Text = "Secondary Languages";
      // 
      // btnAddSecondaryLanguage
      // 
      btnAddSecondaryLanguage.Location = new Point(8, 27);
      btnAddSecondaryLanguage.Name = "btnAddSecondaryLanguage";
      btnAddSecondaryLanguage.Size = new Size(200, 36);
      btnAddSecondaryLanguage.TabIndex = 3;
      btnAddSecondaryLanguage.Text = "Add Language";
      btnAddSecondaryLanguage.UseVisualStyleBackColor = true;
      btnAddSecondaryLanguage.Click += AddSecondaryLanguage;
      // 
      // listSecondaryLanguages
      // 
      listSecondaryLanguages.FormattingEnabled = true;
      listSecondaryLanguages.ItemHeight = 20;
      listSecondaryLanguages.Location = new Point(9, 69);
      listSecondaryLanguages.Name = "listSecondaryLanguages";
      listSecondaryLanguages.Size = new Size(201, 184);
      listSecondaryLanguages.TabIndex = 0;
      listSecondaryLanguages.TabStop = false;
      listSecondaryLanguages.UseTabStops = false;
      // 
      // menuStrip
      // 
      menuStrip.BackColor = Color.FromArgb(221, 221, 221);
      menuStrip.ImageScalingSize = new Size(20, 20);
      menuStrip.Items.AddRange(new ToolStripItem[] { menuConnection, menuTranslatedTables, menuSyncDataModel });
      menuStrip.Location = new Point(0, 0);
      menuStrip.Name = "menuStrip";
      menuStrip.Padding = new Padding(6, 3, 6, 3);
      menuStrip.Size = new Size(1519, 38);
      menuStrip.TabIndex = 1;
      menuStrip.Text = "menuMain";
      // 
      // menuConnection
      // 
      menuConnection.DropDownItems.AddRange(new ToolStripItem[] { menuConnect, menuDisconnect, toolStripSeparator2, menuConfigureSettings, toolStripSeparator1, exportModelAsJson });
      menuConnection.Name = "menuConnection";
      menuConnection.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
      menuConnection.Size = new Size(153, 32);
      menuConnection.Text = "Dataset Connection";
      // 
      // menuConnect
      // 
      menuConnect.Name = "menuConnect";
      menuConnect.ShortcutKeys = Keys.Control | Keys.Shift | Keys.C;
      menuConnect.Size = new Size(281, 26);
      menuConnect.Text = "Connect...";
      menuConnect.Click += menuConnect_Click;
      // 
      // menuDisconnect
      // 
      menuDisconnect.Name = "menuDisconnect";
      menuDisconnect.ShortcutKeys = Keys.Control | Keys.Shift | Keys.D;
      menuDisconnect.Size = new Size(281, 26);
      menuDisconnect.Text = "Disconnect";
      menuDisconnect.Click += menuDisconnect_Click;
      // 
      // toolStripSeparator2
      // 
      toolStripSeparator2.Name = "toolStripSeparator2";
      toolStripSeparator2.Size = new Size(278, 6);
      // 
      // menuConfigureSettings
      // 
      menuConfigureSettings.Name = "menuConfigureSettings";
      menuConfigureSettings.ShortcutKeys = Keys.Control | Keys.S;
      menuConfigureSettings.Size = new Size(281, 26);
      menuConfigureSettings.Text = "Configure Settings...";
      menuConfigureSettings.Click += ConfigureSettings;
      // 
      // toolStripSeparator1
      // 
      toolStripSeparator1.Name = "toolStripSeparator1";
      toolStripSeparator1.Size = new Size(278, 6);
      // 
      // exportModelAsJson
      // 
      exportModelAsJson.Name = "exportModelAsJson";
      exportModelAsJson.ShortcutKeys = Keys.Control | Keys.E;
      exportModelAsJson.Size = new Size(281, 26);
      exportModelAsJson.Text = "Export As Model.bim";
      exportModelAsJson.Click += exportModelAsJson_Click;
      // 
      // menuTranslatedTables
      // 
      menuTranslatedTables.DropDownItems.AddRange(new ToolStripItem[] { menuCreateLocalizedLabelsTable, menuAddLabelsToLocalizedLabelsTable, menuGenerateTranslatedLocalizedLabelsTable, menuGenerateTranslatedDatasetObjectNamesTable });
      menuTranslatedTables.Name = "menuTranslatedTables";
      menuTranslatedTables.ShortcutKeys = Keys.Control | Keys.Shift | Keys.T;
      menuTranslatedTables.Size = new Size(200, 32);
      menuTranslatedTables.Text = "Generate Translated Tables";
      // 
      // menuCreateLocalizedLabelsTable
      // 
      menuCreateLocalizedLabelsTable.Name = "menuCreateLocalizedLabelsTable";
      menuCreateLocalizedLabelsTable.ShortcutKeys = Keys.Control | Keys.C;
      menuCreateLocalizedLabelsTable.Size = new Size(469, 26);
      menuCreateLocalizedLabelsTable.Text = "Create Localized Labels Table";
      menuCreateLocalizedLabelsTable.Click += CreateLocalizedLabelsTable;
      // 
      // menuAddLabelsToLocalizedLabelsTable
      // 
      menuAddLabelsToLocalizedLabelsTable.Name = "menuAddLabelsToLocalizedLabelsTable";
      menuAddLabelsToLocalizedLabelsTable.ShortcutKeys = Keys.Control | Keys.A;
      menuAddLabelsToLocalizedLabelsTable.Size = new Size(469, 26);
      menuAddLabelsToLocalizedLabelsTable.Text = "Add Labels to Localized Labels Table...";
      menuAddLabelsToLocalizedLabelsTable.Click += AddLocalizedLabels;
      // 
      // menuGenerateTranslatedLocalizedLabelsTable
      // 
      menuGenerateTranslatedLocalizedLabelsTable.Name = "menuGenerateTranslatedLocalizedLabelsTable";
      menuGenerateTranslatedLocalizedLabelsTable.ShortcutKeys = Keys.Control | Keys.L;
      menuGenerateTranslatedLocalizedLabelsTable.Size = new Size(469, 26);
      menuGenerateTranslatedLocalizedLabelsTable.Text = "Generate Translated Localized Labels Table";
      menuGenerateTranslatedLocalizedLabelsTable.Click += GenerateTranslatedLocalizedLabelsTable;
      // 
      // menuGenerateTranslatedDatasetObjectNamesTable
      // 
      menuGenerateTranslatedDatasetObjectNamesTable.Name = "menuGenerateTranslatedDatasetObjectNamesTable";
      menuGenerateTranslatedDatasetObjectNamesTable.ShortcutKeys = Keys.Control | Keys.D;
      menuGenerateTranslatedDatasetObjectNamesTable.Size = new Size(469, 26);
      menuGenerateTranslatedDatasetObjectNamesTable.Text = "Generate Translated Dataset Object Names Table";
      menuGenerateTranslatedDatasetObjectNamesTable.Click += GenerateTranslatedDatasetObjectNamesTable;
      // 
      // menuSyncDataModel
      // 
      menuSyncDataModel.Alignment = ToolStripItemAlignment.Right;
      menuSyncDataModel.BackColor = Color.LemonChiffon;
      menuSyncDataModel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
      menuSyncDataModel.ForeColor = Color.DarkRed;
      menuSyncDataModel.Margin = new Padding(4);
      menuSyncDataModel.Name = "menuSyncDataModel";
      menuSyncDataModel.Size = new Size(140, 24);
      menuSyncDataModel.Text = "Sync Data Model";
      menuSyncDataModel.Click += SyncDataModel;
      // 
      // statusStrip1
      // 
      statusStrip1.ImageScalingSize = new Size(20, 20);
      statusStrip1.Items.AddRange(new ToolStripItem[] { labelStatusBar, toolStripStatusLabel1, toolStripStatusLabel2 });
      statusStrip1.Location = new Point(0, 717);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Padding = new Padding(1, 0, 16, 0);
      statusStrip1.Size = new Size(1519, 22);
      statusStrip1.TabIndex = 9;
      statusStrip1.Text = "statusStrip1";
      // 
      // labelStatusBar
      // 
      labelStatusBar.Name = "labelStatusBar";
      labelStatusBar.Size = new Size(0, 16);
      // 
      // toolStripStatusLabel1
      // 
      toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      toolStripStatusLabel1.Size = new Size(0, 16);
      // 
      // toolStripStatusLabel2
      // 
      toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      toolStripStatusLabel2.Size = new Size(0, 16);
      // 
      // contextMenuSecondaryLanguageHeader
      // 
      contextMenuSecondaryLanguageHeader.ImageScalingSize = new Size(20, 20);
      contextMenuSecondaryLanguageHeader.Items.AddRange(new ToolStripItem[] { toolStripSeparator5, menuCommandDuplicateSecondaryLanguage, menuCommandCopyToSecondaryLanguage, toolStripSeparator3, menuCommandExportLanguageToTranslationSheet, toolStripSeparator4, menuCommandDeleteSecondaryLanguage, toolStripSeparator6 });
      contextMenuSecondaryLanguageHeader.Name = "contextMenuSecondaryLanguageHeader";
      contextMenuSecondaryLanguageHeader.Size = new Size(341, 124);
      contextMenuSecondaryLanguageHeader.Text = "Actions on This Language";
      // 
      // toolStripSeparator5
      // 
      toolStripSeparator5.Name = "toolStripSeparator5";
      toolStripSeparator5.Size = new Size(337, 6);
      // 
      // menuCommandDuplicateSecondaryLanguage
      // 
      menuCommandDuplicateSecondaryLanguage.Name = "menuCommandDuplicateSecondaryLanguage";
      menuCommandDuplicateSecondaryLanguage.Size = new Size(340, 24);
      menuCommandDuplicateSecondaryLanguage.Text = "Copy Translations to New Language";
      menuCommandDuplicateSecondaryLanguage.MouseDown += menuCommandDuplicateSecondaryLanguage_MouseDown;
      // 
      // menuCommandCopyToSecondaryLanguage
      // 
      menuCommandCopyToSecondaryLanguage.Name = "menuCommandCopyToSecondaryLanguage";
      menuCommandCopyToSecondaryLanguage.Size = new Size(340, 24);
      menuCommandCopyToSecondaryLanguage.Text = "Copy Translations to Existing Language";
      menuCommandCopyToSecondaryLanguage.MouseDown += menuCommandCopyToSecondaryLanguage_MouseDown;
      // 
      // toolStripSeparator3
      // 
      toolStripSeparator3.Name = "toolStripSeparator3";
      toolStripSeparator3.Size = new Size(337, 6);
      // 
      // menuCommandExportLanguageToTranslationSheet
      // 
      menuCommandExportLanguageToTranslationSheet.Name = "menuCommandExportLanguageToTranslationSheet";
      menuCommandExportLanguageToTranslationSheet.Size = new Size(340, 24);
      menuCommandExportLanguageToTranslationSheet.Text = "Export Language to Translation Sheet";
      menuCommandExportLanguageToTranslationSheet.MouseDown += menuCommandExportLanguageToTranslationSheet_MouseDown;
      // 
      // toolStripSeparator4
      // 
      toolStripSeparator4.Name = "toolStripSeparator4";
      toolStripSeparator4.Size = new Size(337, 6);
      // 
      // menuCommandDeleteSecondaryLanguage
      // 
      menuCommandDeleteSecondaryLanguage.Name = "menuCommandDeleteSecondaryLanguage";
      menuCommandDeleteSecondaryLanguage.Size = new Size(340, 24);
      menuCommandDeleteSecondaryLanguage.Text = "Delete This Language from Data Model";
      menuCommandDeleteSecondaryLanguage.MouseDown += menuCommandDeleteSecondaryLanguage_MouseDown;
      // 
      // toolStripSeparator6
      // 
      toolStripSeparator6.Name = "toolStripSeparator6";
      toolStripSeparator6.Size = new Size(337, 6);
      // 
      // FormMain
      // 
      AutoScaleDimensions = new SizeF(8F, 20F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1519, 739);
      Controls.Add(gridTranslations);
      Controls.Add(statusStrip1);
      Controls.Add(panel2);
      Icon = (Icon)resources.GetObject("$this.Icon");
      MainMenuStrip = menuStrip;
      Name = "FormMain";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "Translations Builder";
      Load += onLoad;
      ((System.ComponentModel.ISupportInitialize)gridTranslations).EndInit();
      groupDatasetProperties.ResumeLayout(false);
      groupDatasetProperties.PerformLayout();
      panel2.ResumeLayout(false);
      panel2.PerformLayout();
      grpMachineTranslationsAllLanguages.ResumeLayout(false);
      grpImportExportTranslations.ResumeLayout(false);
      grpImportExportTranslations.PerformLayout();
      grpMachineTranslationsSingleLanguage.ResumeLayout(false);
      grpSecondaryCultures.ResumeLayout(false);
      menuStrip.ResumeLayout(false);
      menuStrip.PerformLayout();
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      contextMenuSecondaryLanguageHeader.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private System.Windows.Forms.DataGridView gridTranslations;
    private OpenFileDialog dialogOpenFile;
    private GroupBox groupDatasetProperties;
    private Panel panel2;
    private GroupBox grpSecondaryCultures;
    private Button btnAddSecondaryLanguage;
    private ListBox listSecondaryLanguages;
    private GroupBox grpMachineTranslationsSingleLanguage;
    private GroupBox grpImportExportTranslations;
    private Button btnExportTranslations;
    private Button btnImportTranslations;
    private Button btnExportTranslationsSheet;
    private ComboBox listLanguageForTransation;
    private ComboBox listCultureToPopulate;
    private Button btnGenenrateMachineTranslations;
    private TextBox txtCompatibilityLevel;
    private Label lblCompatibilityLevel;
    private TextBox txtDatasetName;
    private TextBox txtServerConnection;
    private Label lblDatasetName;
    private Label lblServerConnection;
    private MenuStrip menuStrip;
    private ToolStripMenuItem menuConnection;
    private ToolStripMenuItem menuConfigureSettings;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem menuTranslatedTables;
    private ToolStripMenuItem menuCreateLocalizedLabelsTable;
    private ToolStripMenuItem menuGenerateTranslatedLocalizedLabelsTable;
    private ToolStripMenuItem menuSyncDataModel;
    private ToolStripMenuItem menuGenerateTranslatedDatasetObjectNamesTable;
    private Button btnFillEmptyTranslations;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel labelStatusBar;
    private ToolStripMenuItem menuAddLabelsToLocalizedLabelsTable;
    private ToolStripMenuItem menuConnect;
    private ToolStripMenuItem menuDisconnect;
    private ToolTip tooltipDatasetName;
    private ToolTip tooltipServiceConnection;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ToolStripStatusLabel toolStripStatusLabel2;
    private ContextMenuStrip contextMenuSecondaryLanguageHeader;
    private ToolStripMenuItem menuCommandDeleteSecondaryLanguage;
    private ToolStripMenuItem exportModelAsJson;
    private ToolStripSeparator toolStripSeparator2;
    private Label lbldefaultLanguageNameLabel;
    private TextBox txtdefaultLanguage;
    private GroupBox grpMachineTranslationsAllLanguages;
    private Button btnFillAllEmptyTranslations;
    private Button btnGenenrateAllMachineTranslations;
    private CheckBox chkOpenExportInExcel;
    private Button btnExportAllTranslationSheets;
    private ToolStripMenuItem menuCommandDuplicateSecondaryLanguage;
    private ToolStripMenuItem menuCommandCopyToSecondaryLanguage;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem menuCommandExportLanguageToTranslationSheet;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripSeparator toolStripSeparator5;
    private ToolStripSeparator toolStripSeparator6;
  }
}

