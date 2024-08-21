namespace TranslationsBuilder.Forms {
    partial class FormConnect {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConnect));
      this.lstSessions = new System.Windows.Forms.ListBox();
      this.btnConnect = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lstSessions
      // 
      this.lstSessions.FormattingEnabled = true;
      this.lstSessions.ItemHeight = 20;
      this.lstSessions.Location = new System.Drawing.Point(12, 12);
      this.lstSessions.Name = "lstSessions";
      this.lstSessions.Size = new System.Drawing.Size(440, 184);
      this.lstSessions.TabIndex = 0;
      this.lstSessions.SelectedIndexChanged += new System.EventHandler(this.lstSessions_SelectedIndexChanged);
      // 
      // btnConnect
      // 
      this.btnConnect.Location = new System.Drawing.Point(467, 12);
      this.btnConnect.Name = "btnConnect";
      this.btnConnect.Size = new System.Drawing.Size(94, 29);
      this.btnConnect.TabIndex = 1;
      this.btnConnect.Text = "Connect";
      this.btnConnect.UseVisualStyleBackColor = true;
      this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(467, 47);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(94, 29);
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // FormConnect
      // 
      this.AcceptButton = this.btnConnect;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(574, 208);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnConnect);
      this.Controls.Add(this.lstSessions);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormConnect";
      this.Text = "Connect to a PBIX Session";
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstSessions;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCancel;
    }
}