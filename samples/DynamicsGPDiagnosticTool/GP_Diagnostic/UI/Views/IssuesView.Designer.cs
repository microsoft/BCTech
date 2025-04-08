namespace Microsoft.GP.MigrationDiagnostic.UI.Views;

partial class IssuesView
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.gbxTasks = new System.Windows.Forms.GroupBox();
        this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        this.dgvTasks = new System.Windows.Forms.DataGridView();
        this.gbxTasks.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
        this.splitContainer1.Panel1.SuspendLayout();
        this.splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.dgvTasks).BeginInit();
        this.SuspendLayout();
        // 
        // gbxTasks
        // 
        this.gbxTasks.AutoSize = true;
        this.gbxTasks.Controls.Add(this.splitContainer1);
        this.gbxTasks.Dock = System.Windows.Forms.DockStyle.Fill;
        this.gbxTasks.Location = new System.Drawing.Point(0, 0);
        this.gbxTasks.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.gbxTasks.Name = "gbxTasks";
        this.gbxTasks.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.gbxTasks.Size = new System.Drawing.Size(1486, 896);
        this.gbxTasks.TabIndex = 2;
        this.gbxTasks.TabStop = false;
        this.gbxTasks.Text = "Tasks";
        // 
        // splitContainer1
        // 
        this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer1.Location = new System.Drawing.Point(4, 36);
        this.splitContainer1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.splitContainer1.Name = "splitContainer1";
        this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
        // 
        // splitContainer1.Panel1
        // 
        this.splitContainer1.Panel1.Controls.Add(this.dgvTasks);
        this.splitContainer1.Panel1MinSize = 195;
        this.splitContainer1.Panel2MinSize = 195;
        this.splitContainer1.Size = new System.Drawing.Size(1478, 856);
        this.splitContainer1.SplitterDistance = 428;
        this.splitContainer1.SplitterWidth = 9;
        this.splitContainer1.TabIndex = 1;
        // 
        // dgvTasks
        // 
        this.dgvTasks.AllowUserToAddRows = false;
        this.dgvTasks.AllowUserToDeleteRows = false;
        this.dgvTasks.AllowUserToResizeRows = false;
        this.dgvTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvTasks.Dock = System.Windows.Forms.DockStyle.Fill;
        this.dgvTasks.Location = new System.Drawing.Point(0, 0);
        this.dgvTasks.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.dgvTasks.Name = "dgvTasks";
        this.dgvTasks.ReadOnly = true;
        this.dgvTasks.RowHeadersWidth = 62;
        this.dgvTasks.RowTemplate.Height = 33;
        this.dgvTasks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.dgvTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvTasks.Size = new System.Drawing.Size(1478, 428);
        this.dgvTasks.TabIndex = 1;
        // 
        // DebugTaskView
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.gbxTasks);
        this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.Name = "DebugTaskView";
        this.Size = new System.Drawing.Size(1486, 896);
        this.gbxTasks.ResumeLayout(false);
        this.splitContainer1.Panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
        this.splitContainer1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.dgvTasks).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.GroupBox gbxTasks;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.DataGridView dgvTasks;
}
