namespace Microsoft.GP.MigrationDiagnostic.UI.Views.TaskGroup;

partial class TaskGroupView
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
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
        this.dgvGroups = new System.Windows.Forms.DataGridView();
        ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
        this.SuspendLayout();
        // 
        // dgvGroups
        // 
        this.dgvGroups.AllowUserToAddRows = false;
        this.dgvGroups.AllowUserToDeleteRows = false;
        this.dgvGroups.AllowUserToResizeColumns = false;
        this.dgvGroups.AllowUserToResizeRows = false;
        this.dgvGroups.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
        this.dgvGroups.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(247)))));
        this.dgvGroups.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.dgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
        dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
        dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
        this.dgvGroups.DefaultCellStyle = dataGridViewCellStyle1;
        this.dgvGroups.Dock = System.Windows.Forms.DockStyle.Fill;
        this.dgvGroups.GridColor = System.Drawing.SystemColors.Control;
        this.dgvGroups.Location = new System.Drawing.Point(0, 0);
        this.dgvGroups.Margin = new System.Windows.Forms.Padding(2);
        this.dgvGroups.MinimumSize = new System.Drawing.Size(300, 300);
        this.dgvGroups.Name = "dgvGroups";
        this.dgvGroups.ReadOnly = true;
        this.dgvGroups.RowHeadersWidth = 62;
        this.dgvGroups.RowTemplate.Height = 60;
        this.dgvGroups.RowTemplate.ReadOnly = true;
        this.dgvGroups.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
        this.dgvGroups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvGroups.Size = new System.Drawing.Size(302, 302);
        this.dgvGroups.TabIndex = 3;
        // 
        // TaskView
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSize = true;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(247)))));
        this.Controls.Add(this.dgvGroups);
        this.DoubleBuffered = true;
        this.Margin = new System.Windows.Forms.Padding(2);
        this.Name = "TaskView";
        this.Size = new System.Drawing.Size(302, 302);
        ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dgvGroups;
}
