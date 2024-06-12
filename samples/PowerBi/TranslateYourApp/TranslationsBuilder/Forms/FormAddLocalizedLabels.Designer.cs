namespace TranslationsBuilder {
    partial class FormAddLocalizedLabels {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddLocalizedLabels));
      this.btnAddLabel = new System.Windows.Forms.Button();
      this.txtLabel = new System.Windows.Forms.TextBox();
      this.lblLabelTextboxLabel = new System.Windows.Forms.Label();
      this.chkAdvancedMode = new System.Windows.Forms.CheckBox();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnDeleteAllLabels = new System.Windows.Forms.Button();
      this.lblBuilderHint = new System.Windows.Forms.Label();
      this.btnUploadLabelsFromFile = new System.Windows.Forms.Button();
      this.lblDangerZone = new System.Windows.Forms.Label();
      this.dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
      this.SuspendLayout();
      // 
      // btnAddLabel
      // 
      this.btnAddLabel.Location = new System.Drawing.Point(444, 30);
      this.btnAddLabel.Name = "btnAddLabel";
      this.btnAddLabel.Size = new System.Drawing.Size(146, 29);
      this.btnAddLabel.TabIndex = 1;
      this.btnAddLabel.Text = "Add Label";
      this.btnAddLabel.UseVisualStyleBackColor = true;
      this.btnAddLabel.Click += new System.EventHandler(this.AddLabel);
      // 
      // txtLabel
      // 
      this.txtLabel.Location = new System.Drawing.Point(69, 31);
      this.txtLabel.Name = "txtLabel";
      this.txtLabel.Size = new System.Drawing.Size(359, 27);
      this.txtLabel.TabIndex = 0;
      this.txtLabel.TextChanged += new System.EventHandler(this.SetAddButtonEnabled);
      // 
      // lblLabelTextboxLabel
      // 
      this.lblLabelTextboxLabel.AutoSize = true;
      this.lblLabelTextboxLabel.Location = new System.Drawing.Point(10, 34);
      this.lblLabelTextboxLabel.Name = "lblLabelTextboxLabel";
      this.lblLabelTextboxLabel.Size = new System.Drawing.Size(48, 20);
      this.lblLabelTextboxLabel.TabIndex = 1;
      this.lblLabelTextboxLabel.Text = "Label:";
      this.lblLabelTextboxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // chkAdvancedMode
      // 
      this.chkAdvancedMode.AutoSize = true;
      this.chkAdvancedMode.Location = new System.Drawing.Point(446, 103);
      this.chkAdvancedMode.Name = "chkAdvancedMode";
      this.chkAdvancedMode.Size = new System.Drawing.Size(140, 24);
      this.chkAdvancedMode.TabIndex = 3;
      this.chkAdvancedMode.Text = "Advanced Mode";
      this.chkAdvancedMode.UseVisualStyleBackColor = true;
      this.chkAdvancedMode.CheckedChanged += new System.EventHandler(this.chkAdvancedMode_CheckedChanged);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(444, 65);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(146, 29);
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnDeleteAllLabels
      // 
      this.btnDeleteAllLabels.Location = new System.Drawing.Point(446, 214);
      this.btnDeleteAllLabels.Name = "btnDeleteAllLabels";
      this.btnDeleteAllLabels.Size = new System.Drawing.Size(140, 29);
      this.btnDeleteAllLabels.TabIndex = 5;
      this.btnDeleteAllLabels.Text = "Delete All Labels";
      this.btnDeleteAllLabels.UseVisualStyleBackColor = true;
      this.btnDeleteAllLabels.Click += new System.EventHandler(this.btnDeleteAllLabels_Click);
      // 
      // lblBuilderHint
      // 
      this.lblBuilderHint.AutoSize = true;
      this.lblBuilderHint.Location = new System.Drawing.Point(68, 318);
      this.lblBuilderHint.Name = "lblBuilderHint";
      this.lblBuilderHint.Size = new System.Drawing.Size(263, 20);
      this.lblBuilderHint.TabIndex = 10;
      this.lblBuilderHint.Text = "Separate each label using a line break.";
      // 
      // btnUploadLabelsFromFile
      // 
      this.btnUploadLabelsFromFile.Location = new System.Drawing.Point(444, 140);
      this.btnUploadLabelsFromFile.Name = "btnUploadLabelsFromFile";
      this.btnUploadLabelsFromFile.Size = new System.Drawing.Size(146, 29);
      this.btnUploadLabelsFromFile.TabIndex = 4;
      this.btnUploadLabelsFromFile.Text = "Upload from File";
      this.btnUploadLabelsFromFile.UseVisualStyleBackColor = true;
      this.btnUploadLabelsFromFile.Click += new System.EventHandler(this.btnUploadLabelsFromFile_Click);
      // 
      // lblDangerZone
      // 
      this.lblDangerZone.AutoSize = true;
      this.lblDangerZone.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
      this.lblDangerZone.ForeColor = System.Drawing.Color.Firebrick;
      this.lblDangerZone.Location = new System.Drawing.Point(444, 186);
      this.lblDangerZone.Name = "lblDangerZone";
      this.lblDangerZone.Size = new System.Drawing.Size(143, 20);
      this.lblDangerZone.TabIndex = 11;
      this.lblDangerZone.Text = "--- Danger Zone ---";
      // 
      // dialogOpenFile
      // 
      this.dialogOpenFile.FileName = "";
      // 
      // FormAddLocalizedLabels
      // 
      this.AcceptButton = this.btnAddLabel;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(617, 353);
      this.Controls.Add(this.lblDangerZone);
      this.Controls.Add(this.btnUploadLabelsFromFile);
      this.Controls.Add(this.lblBuilderHint);
      this.Controls.Add(this.btnDeleteAllLabels);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.chkAdvancedMode);
      this.Controls.Add(this.lblLabelTextboxLabel);
      this.Controls.Add(this.txtLabel);
      this.Controls.Add(this.btnAddLabel);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormAddLocalizedLabels";
      this.Text = "Add Localized Labels";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddLabel;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.Label lblLabelTextboxLabel;
        private System.Windows.Forms.CheckBox chkAdvancedMode;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDeleteAllLabels;
        private System.Windows.Forms.Label lblBuilderHint;
        private System.Windows.Forms.Button btnUploadLabelsFromFile;
        private System.Windows.Forms.Label lblDangerZone;
        private System.Windows.Forms.OpenFileDialog dialogOpenFile;
    }
}