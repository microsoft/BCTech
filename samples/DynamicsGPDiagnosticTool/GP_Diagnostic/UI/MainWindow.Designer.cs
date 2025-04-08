namespace Microsoft.GP.MigrationDiagnostic.UI;

partial class MainWindow
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.databaseSettings = new System.Windows.Forms.ToolStripMenuItem();
        this.runEvaluationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.statusStrip = new System.Windows.Forms.StatusStrip();
        this.currentStateStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
        this.btnRunEvaluation = new System.Windows.Forms.Button();
        this.tabTaskViews = new System.Windows.Forms.TabControl();
        this.tabSummary = new System.Windows.Forms.TabPage();
        this.tabIssues = new System.Windows.Forms.TabPage();
        this.btnCancelEvaluation = new System.Windows.Forms.Button();
        this.menuStrip1.SuspendLayout();
        this.statusStrip.SuspendLayout();
        this.tabTaskViews.SuspendLayout();
        this.SuspendLayout();
        // 
        // menuStrip1
        // 
        this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileToolStripMenuItem, this.toolsToolStripMenuItem, this.helpToolStripMenuItem });
        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
        this.menuStrip1.Size = new System.Drawing.Size(784, 24);
        this.menuStrip1.TabIndex = 3;
        this.menuStrip1.Text = "menuStrip1";
        // 
        // fileToolStripMenuItem
        // 
        this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.printPreviewToolStripMenuItem, this.toolStripSeparator2, this.exitToolStripMenuItem });
        this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
        this.fileToolStripMenuItem.Text = "&File";
        // 
        // printPreviewToolStripMenuItem
        // 
        this.printPreviewToolStripMenuItem.Enabled = false;
        this.printPreviewToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("printPreviewToolStripMenuItem.Image");
        this.printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
        this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
        this.printPreviewToolStripMenuItem.Text = "Save Results Pre&view";
        // 
        // toolStripSeparator2
        // 
        this.toolStripSeparator2.Name = "toolStripSeparator2";
        this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
        // 
        // exitToolStripMenuItem
        // 
        this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        this.exitToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
        this.exitToolStripMenuItem.Text = "E&xit";
        this.exitToolStripMenuItem.Click += this.exitToolStripMenuItem_Click;
        // 
        // toolsToolStripMenuItem
        // 
        this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.databaseSettings, this.runEvaluationMenuItem });
        this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
        this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 22);
        this.toolsToolStripMenuItem.Text = "&Tools";
        // 
        // databaseSettings
        // 
        this.databaseSettings.Name = "databaseSettings";
        this.databaseSettings.Size = new System.Drawing.Size(153, 22);
        this.databaseSettings.Text = "Settings";
        // 
        // runEvaluationMenuItem
        // 
        this.runEvaluationMenuItem.Name = "runEvaluationMenuItem";
        this.runEvaluationMenuItem.Size = new System.Drawing.Size(153, 22);
        this.runEvaluationMenuItem.Text = "Run Evaluation";
        // 
        // helpToolStripMenuItem
        // 
        this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.aboutToolStripMenuItem });
        this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
        this.helpToolStripMenuItem.Text = "&Help";
        // 
        // aboutToolStripMenuItem
        // 
        this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
        this.aboutToolStripMenuItem.Text = "&About...";
        this.aboutToolStripMenuItem.Click += this.aboutToolStripMenuItem_Click;
        // 
        // statusStrip
        // 
        this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
        this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { currentStateStatusLabel });
        this.statusStrip.Location = new System.Drawing.Point(0, 475);
        this.statusStrip.Name = "statusStrip";
        this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
        this.statusStrip.Size = new System.Drawing.Size(784, 22);
        this.statusStrip.TabIndex = 5;
        this.statusStrip.Text = "statusStrip1";
        // 
        // currentStateStatusLabel
        // 
        this.currentStateStatusLabel.Name = "currentStateStatusLabel";
        this.currentStateStatusLabel.Size = new System.Drawing.Size(131, 17);
        this.currentStateStatusLabel.Text = "currentStateStatusLabel";
        // 
        // btnRunEvaluation
        // 
        this.btnRunEvaluation.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        this.btnRunEvaluation.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
        this.btnRunEvaluation.ForeColor = System.Drawing.Color.White;
        this.btnRunEvaluation.Location = new System.Drawing.Point(644, 433);
        this.btnRunEvaluation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        this.btnRunEvaluation.Name = "btnRunEvaluation";
        this.btnRunEvaluation.Size = new System.Drawing.Size(128, 38);
        this.btnRunEvaluation.TabIndex = 6;
        this.btnRunEvaluation.Text = "Run";
        this.btnRunEvaluation.UseVisualStyleBackColor = false;
        this.btnRunEvaluation.Click += this.btnRunEvaluation_Click;
        // 
        // tabTaskViews
        // 
        this.tabTaskViews.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.tabTaskViews.Controls.Add(this.tabSummary);
        this.tabTaskViews.Controls.Add(this.tabIssues);
        this.tabTaskViews.Location = new System.Drawing.Point(13, 28);
        this.tabTaskViews.Name = "tabTaskViews";
        this.tabTaskViews.SelectedIndex = 0;
        this.tabTaskViews.Size = new System.Drawing.Size(759, 398);
        this.tabTaskViews.TabIndex = 12;
        // 
        // tabSummary
        // 
        this.tabSummary.Location = new System.Drawing.Point(4, 24);
        this.tabSummary.Name = "tabSummary";
        this.tabSummary.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
        this.tabSummary.Size = new System.Drawing.Size(751, 370);
        this.tabSummary.TabIndex = 0;
        this.tabSummary.Text = "Summary";
        this.tabSummary.UseVisualStyleBackColor = true;
        // 
        // tabIssues
        // 
        this.tabIssues.Location = new System.Drawing.Point(4, 24);
        this.tabIssues.Name = "tabIssues";
        this.tabIssues.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
        this.tabIssues.Size = new System.Drawing.Size(758, 431);
        this.tabIssues.TabIndex = 1;
        this.tabIssues.Text = "Issues";
        this.tabIssues.UseVisualStyleBackColor = true;
        // 
        // btnCancelEvaluation
        // 
        this.btnCancelEvaluation.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        this.btnCancelEvaluation.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
        this.btnCancelEvaluation.ForeColor = System.Drawing.Color.White;
        this.btnCancelEvaluation.Location = new System.Drawing.Point(644, 433);
        this.btnCancelEvaluation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        this.btnCancelEvaluation.Name = "btnCancelEvaluation";
        this.btnCancelEvaluation.Size = new System.Drawing.Size(128, 38);
        this.btnCancelEvaluation.TabIndex = 13;
        this.btnCancelEvaluation.Text = "Cancel";
        this.btnCancelEvaluation.UseVisualStyleBackColor = false;
        this.btnCancelEvaluation.Click += this.btnCancelEvaluation_Click;
        // 
        // MainWindow
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.BackColor = System.Drawing.Color.FromArgb(248, 248, 247);
        this.ClientSize = new System.Drawing.Size(784, 497);
        this.Controls.Add(this.tabTaskViews);
        this.Controls.Add(this.statusStrip);
        this.Controls.Add(this.menuStrip1);
        this.Controls.Add(this.btnRunEvaluation);
        this.Controls.Add(this.btnCancelEvaluation);
        this.MainMenuStrip = this.menuStrip1;
        this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
        this.MinimumSize = new System.Drawing.Size(795, 504);
        this.Name = "MainWindow";
        this.Text = "GP Migration Diagnostic Tool";
        this.Load += this.Main_Load;
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.tabTaskViews.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem databaseSettings;
    private System.Windows.Forms.ToolStripMenuItem runEvaluationMenuItem;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.Button btnRunEvaluation;
    private System.Windows.Forms.ToolStripStatusLabel currentStateStatusLabel;
    private System.Windows.Forms.TabControl tabTaskViews;
    private System.Windows.Forms.TabPage tabSummary;
    private System.Windows.Forms.TabPage tabIssues;
    private System.Windows.Forms.Button btnCancelEvaluation;
}
