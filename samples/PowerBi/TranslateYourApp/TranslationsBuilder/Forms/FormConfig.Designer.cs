
namespace TranslationsBuilder {
  partial class FormConfig {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfig));
      this.lblTranslationsOutboxFolderPath = new System.Windows.Forms.Label();
      this.txtTranslationsOutboxFolderPath = new System.Windows.Forms.TextBox();
      this.lblTranslationsInboxFolderPath = new System.Windows.Forms.Label();
      this.txtTranslationsInboxFolderPath = new System.Windows.Forms.TextBox();
      this.btnSaveConfigurationChanges = new System.Windows.Forms.Button();
      this.lblAzureTranslatorServiceKey = new System.Windows.Forms.Label();
      this.txtAzureTranslatorServiceKey = new System.Windows.Forms.TextBox();
      this.lblAzureTranslatorServiceLocation = new System.Windows.Forms.Label();
      this.txtAzureTranslatorServiceLocation = new System.Windows.Forms.TextBox();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnSetOutboxPath = new System.Windows.Forms.Button();
      this.btnSetInboxPath = new System.Windows.Forms.Button();
      this.dialogSelectFolder = new System.Windows.Forms.FolderBrowserDialog();
      this.SuspendLayout();
      // 
      // lblTranslationsOutboxFolderPath
      // 
      this.lblTranslationsOutboxFolderPath.AutoSize = true;
      this.lblTranslationsOutboxFolderPath.Location = new System.Drawing.Point(12, 123);
      this.lblTranslationsOutboxFolderPath.Name = "lblTranslationsOutboxFolderPath";
      this.lblTranslationsOutboxFolderPath.Size = new System.Drawing.Size(218, 20);
      this.lblTranslationsOutboxFolderPath.TabIndex = 4;
      this.lblTranslationsOutboxFolderPath.Text = "Translations Outbox Folder Path";
      // 
      // txtTranslationsOutboxFolderPath
      // 
      this.txtTranslationsOutboxFolderPath.Location = new System.Drawing.Point(12, 146);
      this.txtTranslationsOutboxFolderPath.Name = "txtTranslationsOutboxFolderPath";
      this.txtTranslationsOutboxFolderPath.Size = new System.Drawing.Size(540, 27);
      this.txtTranslationsOutboxFolderPath.TabIndex = 3;
      // 
      // lblTranslationsInboxFolderPath
      // 
      this.lblTranslationsInboxFolderPath.AutoSize = true;
      this.lblTranslationsInboxFolderPath.Location = new System.Drawing.Point(12, 179);
      this.lblTranslationsInboxFolderPath.Name = "lblTranslationsInboxFolderPath";
      this.lblTranslationsInboxFolderPath.Size = new System.Drawing.Size(206, 20);
      this.lblTranslationsInboxFolderPath.TabIndex = 6;
      this.lblTranslationsInboxFolderPath.Text = "Translations Inbox Folder Path";
      // 
      // txtTranslationsInboxFolderPath
      // 
      this.txtTranslationsInboxFolderPath.Location = new System.Drawing.Point(12, 204);
      this.txtTranslationsInboxFolderPath.Name = "txtTranslationsInboxFolderPath";
      this.txtTranslationsInboxFolderPath.Size = new System.Drawing.Size(540, 27);
      this.txtTranslationsInboxFolderPath.TabIndex = 5;
      // 
      // btnSaveConfigurationChanges
      // 
      this.btnSaveConfigurationChanges.Location = new System.Drawing.Point(580, 17);
      this.btnSaveConfigurationChanges.Name = "btnSaveConfigurationChanges";
      this.btnSaveConfigurationChanges.Size = new System.Drawing.Size(140, 37);
      this.btnSaveConfigurationChanges.TabIndex = 7;
      this.btnSaveConfigurationChanges.Text = "Save Changes";
      this.btnSaveConfigurationChanges.UseVisualStyleBackColor = true;
      this.btnSaveConfigurationChanges.Click += new System.EventHandler(this.btnSaveConfigurationChanges_Click);
      // 
      // lblAzureTranslatorServiceKey
      // 
      this.lblAzureTranslatorServiceKey.AutoSize = true;
      this.lblAzureTranslatorServiceKey.Location = new System.Drawing.Point(12, 10);
      this.lblAzureTranslatorServiceKey.Name = "lblAzureTranslatorServiceKey";
      this.lblAzureTranslatorServiceKey.Size = new System.Drawing.Size(195, 20);
      this.lblAzureTranslatorServiceKey.TabIndex = 9;
      this.lblAzureTranslatorServiceKey.Text = "Azure Translator Service Key";
      // 
      // txtAzureTranslatorServiceKey
      // 
      this.txtAzureTranslatorServiceKey.Location = new System.Drawing.Point(12, 33);
      this.txtAzureTranslatorServiceKey.Name = "txtAzureTranslatorServiceKey";
      this.txtAzureTranslatorServiceKey.Size = new System.Drawing.Size(540, 27);
      this.txtAzureTranslatorServiceKey.TabIndex = 1;
      // 
      // lblAzureTranslatorServiceLocation
      // 
      this.lblAzureTranslatorServiceLocation.AutoSize = true;
      this.lblAzureTranslatorServiceLocation.Location = new System.Drawing.Point(14, 66);
      this.lblAzureTranslatorServiceLocation.Name = "lblAzureTranslatorServiceLocation";
      this.lblAzureTranslatorServiceLocation.Size = new System.Drawing.Size(318, 20);
      this.lblAzureTranslatorServiceLocation.TabIndex = 11;
      this.lblAzureTranslatorServiceLocation.Text = "Azure Translator Service Location (e.g. eastus2)";
      // 
      // txtAzureTranslatorServiceLocation
      // 
      this.txtAzureTranslatorServiceLocation.Location = new System.Drawing.Point(14, 90);
      this.txtAzureTranslatorServiceLocation.Name = "txtAzureTranslatorServiceLocation";
      this.txtAzureTranslatorServiceLocation.Size = new System.Drawing.Size(540, 27);
      this.txtAzureTranslatorServiceLocation.TabIndex = 2;
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(580, 64);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(140, 36);
      this.btnCancel.TabIndex = 8;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnSetOutboxPath
      // 
      this.btnSetOutboxPath.Location = new System.Drawing.Point(571, 140);
      this.btnSetOutboxPath.Name = "btnSetOutboxPath";
      this.btnSetOutboxPath.Size = new System.Drawing.Size(53, 36);
      this.btnSetOutboxPath.TabIndex = 4;
      this.btnSetOutboxPath.Text = "set";
      this.btnSetOutboxPath.UseVisualStyleBackColor = true;
      this.btnSetOutboxPath.Click += new System.EventHandler(this.SetOutboxPath);
      // 
      // btnSetInboxPath
      // 
      this.btnSetInboxPath.Location = new System.Drawing.Point(571, 199);
      this.btnSetInboxPath.Name = "btnSetInboxPath";
      this.btnSetInboxPath.Size = new System.Drawing.Size(53, 36);
      this.btnSetInboxPath.TabIndex = 6;
      this.btnSetInboxPath.Text = "set";
      this.btnSetInboxPath.UseVisualStyleBackColor = true;
      this.btnSetInboxPath.Click += new System.EventHandler(this.SetInboxPath);
      // 
      // FormConfig
      // 
      this.AcceptButton = this.btnSaveConfigurationChanges;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(738, 251);
      this.Controls.Add(this.btnSetInboxPath);
      this.Controls.Add(this.btnSetOutboxPath);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.txtAzureTranslatorServiceLocation);
      this.Controls.Add(this.lblAzureTranslatorServiceLocation);
      this.Controls.Add(this.txtAzureTranslatorServiceKey);
      this.Controls.Add(this.lblAzureTranslatorServiceKey);
      this.Controls.Add(this.btnSaveConfigurationChanges);
      this.Controls.Add(this.txtTranslationsInboxFolderPath);
      this.Controls.Add(this.lblTranslationsInboxFolderPath);
      this.Controls.Add(this.txtTranslationsOutboxFolderPath);
      this.Controls.Add(this.lblTranslationsOutboxFolderPath);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormConfig";
      this.Text = "Configuration Options";
      this.Load += new System.EventHandler(this.FormConfig_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label lblTranslationsOutboxFolderPath;
    private System.Windows.Forms.TextBox txtTranslationsOutboxFolderPath;
    private System.Windows.Forms.Label lblTranslationsInboxFolderPath;
    private System.Windows.Forms.TextBox txtTranslationsInboxFolderPath;
    private System.Windows.Forms.Button btnSaveConfigurationChanges;
    private System.Windows.Forms.Label lblAzureTranslatorServiceKey;
    private System.Windows.Forms.TextBox txtAzureTranslatorServiceKey;
    private System.Windows.Forms.Label lblAzureTranslatorServiceLocation;
    private System.Windows.Forms.TextBox txtAzureTranslatorServiceLocation;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnSetOutboxPath;
    private System.Windows.Forms.Button btnSetInboxPath;
    private System.Windows.Forms.FolderBrowserDialog dialogSelectFolder;
  }
}