namespace TranslationsBuilder {
    partial class FormEditLocalizedLabel {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditLocalizedLabel));
      this.btnUpdate = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.txtLabelName = new System.Windows.Forms.TextBox();
      this.lblLabelTextLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnUpdate
      // 
      this.btnUpdate.Location = new System.Drawing.Point(381, 8);
      this.btnUpdate.Name = "btnUpdate";
      this.btnUpdate.Size = new System.Drawing.Size(158, 29);
      this.btnUpdate.TabIndex = 1;
      this.btnUpdate.Text = "Update Label";
      this.btnUpdate.UseVisualStyleBackColor = true;
      this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
      // 
      // btnDelete
      // 
      this.btnDelete.Location = new System.Drawing.Point(381, 78);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(158, 29);
      this.btnDelete.TabIndex = 3;
      this.btnDelete.Text = "Delete Label";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(381, 43);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(158, 29);
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // txtLabelName
      // 
      this.txtLabelName.Location = new System.Drawing.Point(15, 38);
      this.txtLabelName.Name = "txtLabelName";
      this.txtLabelName.Size = new System.Drawing.Size(349, 27);
      this.txtLabelName.TabIndex = 0;
      // 
      // lblLabelTextLabel
      // 
      this.lblLabelTextLabel.AutoSize = true;
      this.lblLabelTextLabel.Location = new System.Drawing.Point(15, 15);
      this.lblLabelTextLabel.Name = "lblLabelTextLabel";
      this.lblLabelTextLabel.Size = new System.Drawing.Size(146, 20);
      this.lblLabelTextLabel.TabIndex = 4;
      this.lblLabelTextLabel.Text = "Localized Label Text:";
      // 
      // FormEditLocalizedLabel
      // 
      this.AcceptButton = this.btnUpdate;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(553, 130);
      this.Controls.Add(this.lblLabelTextLabel);
      this.Controls.Add(this.txtLabelName);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnDelete);
      this.Controls.Add(this.btnUpdate);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormEditLocalizedLabel";
      this.Text = "Edit Localized Label";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtLabelName;
        private System.Windows.Forms.Label lblLabelTextLabel;
    }
}