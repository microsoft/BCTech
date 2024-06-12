namespace TranslationsBuilder.Forms {
  partial class FormSelectExisitngLanguage {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectExisitngLanguage));
      lblMulitselectAdvice = new System.Windows.Forms.Label();
      listLanguages = new System.Windows.Forms.ListBox();
      btnCancel = new System.Windows.Forms.Button();
      btnCopyTanslations = new System.Windows.Forms.Button();
      SuspendLayout();
      // 
      // lblMulitselectAdvice
      // 
      lblMulitselectAdvice.AutoSize = true;
      lblMulitselectAdvice.Location = new System.Drawing.Point(12, 329);
      lblMulitselectAdvice.Name = "lblMulitselectAdvice";
      lblMulitselectAdvice.Size = new System.Drawing.Size(363, 20);
      lblMulitselectAdvice.TabIndex = 9;
      lblMulitselectAdvice.Text = "Hold down the CTRL key to enable multiple selection.";
      // 
      // listLanguages
      // 
      listLanguages.FormattingEnabled = true;
      listLanguages.ItemHeight = 20;
      listLanguages.Location = new System.Drawing.Point(12, 12);
      listLanguages.Name = "listLanguages";
      listLanguages.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      listLanguages.Size = new System.Drawing.Size(363, 304);
      listLanguages.Sorted = true;
      listLanguages.TabIndex = 7;
      // 
      // btnCancel
      // 
      btnCancel.Location = new System.Drawing.Point(394, 66);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new System.Drawing.Size(164, 40);
      btnCancel.TabIndex = 6;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      btnCancel.Click += btnCancel_Click;
      // 
      // btnCopyTanslations
      // 
      btnCopyTanslations.Location = new System.Drawing.Point(394, 17);
      btnCopyTanslations.Name = "btnCopyTanslations";
      btnCopyTanslations.Size = new System.Drawing.Size(164, 37);
      btnCopyTanslations.TabIndex = 5;
      btnCopyTanslations.Text = "Copy Translations";
      btnCopyTanslations.UseVisualStyleBackColor = true;
      btnCopyTanslations.Click += btnCopyTanslations_Click;
      // 
      // FormSelectExisitngLanguage
      // 
      AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      BackColor = System.Drawing.SystemColors.ActiveCaption;
      ClientSize = new System.Drawing.Size(575, 365);
      Controls.Add(lblMulitselectAdvice);
      Controls.Add(listLanguages);
      Controls.Add(btnCancel);
      Controls.Add(btnCopyTanslations);
      Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
      Name = "FormSelectExisitngLanguage";
      Text = "Select CopyTo Language";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label lblMulitselectAdvice;
    private System.Windows.Forms.ListBox listLanguages;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnCopyTanslations;
  }
}