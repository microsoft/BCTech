
namespace TranslationsBuilder {
  partial class FormAddLanguage {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddLanguage));
      btnAddLanguage = new System.Windows.Forms.Button();
      btnCancel = new System.Windows.Forms.Button();
      listLanguages = new System.Windows.Forms.ListBox();
      chkShowAllLanguages = new System.Windows.Forms.CheckBox();
      lblMulitselectAdvice = new System.Windows.Forms.Label();
      comboLanguageGroupFilter = new System.Windows.Forms.ComboBox();
      lblFilterByLanguageGroup = new System.Windows.Forms.Label();
      SuspendLayout();
      // 
      // btnAddLanguage
      // 
      btnAddLanguage.Location = new System.Drawing.Point(394, 17);
      btnAddLanguage.Name = "btnAddLanguage";
      btnAddLanguage.Size = new System.Drawing.Size(164, 37);
      btnAddLanguage.TabIndex = 0;
      btnAddLanguage.Text = "Add Language";
      btnAddLanguage.UseVisualStyleBackColor = true;
      btnAddLanguage.Click += btnAddCulture_Click;
      // 
      // btnCancel
      // 
      btnCancel.Location = new System.Drawing.Point(394, 66);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new System.Drawing.Size(164, 40);
      btnCancel.TabIndex = 1;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      btnCancel.Click += btnCancel_Click;
      // 
      // listLanguages
      // 
      listLanguages.FormattingEnabled = true;
      listLanguages.ItemHeight = 20;
      listLanguages.Location = new System.Drawing.Point(12, 16);
      listLanguages.Name = "listLanguages";
      listLanguages.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      listLanguages.Size = new System.Drawing.Size(363, 504);
      listLanguages.Sorted = true;
      listLanguages.TabIndex = 2;
      // 
      // chkShowAllLanguages
      // 
      chkShowAllLanguages.AutoSize = true;
      chkShowAllLanguages.Location = new System.Drawing.Point(394, 122);
      chkShowAllLanguages.Name = "chkShowAllLanguages";
      chkShowAllLanguages.Size = new System.Drawing.Size(164, 24);
      chkShowAllLanguages.TabIndex = 3;
      chkShowAllLanguages.Text = "Show All Languages";
      chkShowAllLanguages.UseVisualStyleBackColor = true;
      chkShowAllLanguages.CheckedChanged += chkShowAllLanguages_CheckedChanged;
      // 
      // lblMulitselectAdvice
      // 
      lblMulitselectAdvice.AutoSize = true;
      lblMulitselectAdvice.Location = new System.Drawing.Point(12, 525);
      lblMulitselectAdvice.Name = "lblMulitselectAdvice";
      lblMulitselectAdvice.Size = new System.Drawing.Size(363, 20);
      lblMulitselectAdvice.TabIndex = 4;
      lblMulitselectAdvice.Text = "Hold down the CTRL key to enable multiple selection.";
      // 
      // comboLanguageGroupFilter
      // 
      comboLanguageGroupFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      comboLanguageGroupFilter.FormattingEnabled = true;
      comboLanguageGroupFilter.Location = new System.Drawing.Point(181, 19);
      comboLanguageGroupFilter.Name = "comboLanguageGroupFilter";
      comboLanguageGroupFilter.Size = new System.Drawing.Size(192, 28);
      comboLanguageGroupFilter.TabIndex = 5;
      comboLanguageGroupFilter.SelectedIndexChanged += comboLanguageGroupFilter_SelectedIndexChanged;
      // 
      // lblFilterByLanguageGroup
      // 
      lblFilterByLanguageGroup.AutoSize = true;
      lblFilterByLanguageGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      lblFilterByLanguageGroup.Location = new System.Drawing.Point(16, 21);
      lblFilterByLanguageGroup.Name = "lblFilterByLanguageGroup";
      lblFilterByLanguageGroup.Size = new System.Drawing.Size(159, 20);
      lblFilterByLanguageGroup.TabIndex = 6;
      lblFilterByLanguageGroup.Text = "Language Group Filter:";
      // 
      // FormAddLanguage
      // 
      AcceptButton = btnAddLanguage;
      AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      BackColor = System.Drawing.SystemColors.ActiveCaption;
      CancelButton = btnCancel;
      ClientSize = new System.Drawing.Size(579, 555);
      Controls.Add(lblFilterByLanguageGroup);
      Controls.Add(comboLanguageGroupFilter);
      Controls.Add(lblMulitselectAdvice);
      Controls.Add(chkShowAllLanguages);
      Controls.Add(listLanguages);
      Controls.Add(btnCancel);
      Controls.Add(btnAddLanguage);
      Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
      Name = "FormAddLanguage";
      Text = "Add Language";
      Load += AddLanguageDialog_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Button btnAddLanguage;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ListBox listLanguages;
    private System.Windows.Forms.CheckBox chkShowAllLanguages;
    private System.Windows.Forms.Label lblMulitselectAdvice;
    private System.Windows.Forms.ComboBox comboLanguageGroupFilter;
    private System.Windows.Forms.Label lblFilterByLanguageGroup;
  }
}