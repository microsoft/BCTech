namespace Microsoft.GP.MigrationDiagnostic.UI;

partial class AboutBox
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.labelProductName = new System.Windows.Forms.Label();
        this.labelVersion = new System.Windows.Forms.Label();
        this.labelCopyright = new System.Windows.Forms.Label();
        this.textBoxDescription = new System.Windows.Forms.TextBox();
        this.okButton = new System.Windows.Forms.Button();
        this.tableLayoutPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // tableLayoutPanel
        // 
        this.tableLayoutPanel.ColumnCount = 1;
        this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel.Controls.Add(this.labelProductName, 0, 0);
        this.tableLayoutPanel.Controls.Add(this.labelVersion, 0, 2);
        this.tableLayoutPanel.Controls.Add(this.labelCopyright, 0, 1);
        this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 0, 3);
        this.tableLayoutPanel.Controls.Add(this.okButton, 0, 4);
        this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel.Location = new System.Drawing.Point(19, 21);
        this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
        this.tableLayoutPanel.Name = "tableLayoutPanel";
        this.tableLayoutPanel.RowCount = 4;
        this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
        this.tableLayoutPanel.Size = new System.Drawing.Size(904, 656);
        this.tableLayoutPanel.TabIndex = 0;
        // 
        // labelProductName
        // 
        this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
        this.labelProductName.Location = new System.Drawing.Point(13, 0);
        this.labelProductName.Margin = new System.Windows.Forms.Padding(13, 0, 7, 0);
        this.labelProductName.MaximumSize = new System.Drawing.Size(0, 43);
        this.labelProductName.Name = "labelProductName";
        this.labelProductName.Size = new System.Drawing.Size(884, 43);
        this.labelProductName.TabIndex = 19;
        this.labelProductName.Text = "Product Name";
        this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // labelVersion
        // 
        this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
        this.labelVersion.Location = new System.Drawing.Point(13, 86);
        this.labelVersion.Margin = new System.Windows.Forms.Padding(13, 0, 7, 0);
        this.labelVersion.MaximumSize = new System.Drawing.Size(0, 43);
        this.labelVersion.Name = "labelVersion";
        this.labelVersion.Size = new System.Drawing.Size(884, 43);
        this.labelVersion.TabIndex = 0;
        this.labelVersion.Text = "Version";
        this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // labelCopyright
        // 
        this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
        this.labelCopyright.Location = new System.Drawing.Point(13, 43);
        this.labelCopyright.Margin = new System.Windows.Forms.Padding(13, 0, 7, 0);
        this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 43);
        this.labelCopyright.Name = "labelCopyright";
        this.labelCopyright.Size = new System.Drawing.Size(884, 43);
        this.labelCopyright.TabIndex = 21;
        this.labelCopyright.Text = "Copyright";
        this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // textBoxDescription
        // 
        this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
        this.textBoxDescription.Location = new System.Drawing.Point(13, 135);
        this.textBoxDescription.Margin = new System.Windows.Forms.Padding(13, 6, 7, 6);
        this.textBoxDescription.Multiline = true;
        this.textBoxDescription.Name = "textBoxDescription";
        this.textBoxDescription.ReadOnly = true;
        this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.textBoxDescription.Size = new System.Drawing.Size(884, 427);
        this.textBoxDescription.TabIndex = 23;
        this.textBoxDescription.TabStop = false;
        this.textBoxDescription.Text = "Description";
        // 
        // okButton
        // 
        this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        this.okButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
        this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.okButton.ForeColor = System.Drawing.Color.White;
        this.okButton.Location = new System.Drawing.Point(689, 574);
        this.okButton.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
        this.okButton.Name = "okButton";
        this.okButton.Size = new System.Drawing.Size(208, 76);
        this.okButton.TabIndex = 24;
        this.okButton.Text = "&OK";
        this.okButton.UseVisualStyleBackColor = false;
        this.okButton.Click += this.okButton_Click;
        // 
        // AboutBox
        // 
        this.AcceptButton = this.okButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(942, 698);
        this.Controls.Add(this.tableLayoutPanel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "AboutBox";
        this.Padding = new System.Windows.Forms.Padding(19, 21, 19, 21);
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "AboutBox";
        this.tableLayoutPanel.ResumeLayout(false);
        this.tableLayoutPanel.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.Label labelProductName;
    private System.Windows.Forms.Label labelVersion;
    private System.Windows.Forms.Label labelCopyright;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.TextBox textBoxDescription;
}
