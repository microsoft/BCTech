namespace Microsoft.GP.MigrationDiagnostic.UI;

partial class DatabaseSettings
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseSettings));
        this.txtServer = new System.Windows.Forms.TextBox();
        this.txtSystemDatabase = new System.Windows.Forms.TextBox();
        this.txtUsername = new System.Windows.Forms.TextBox();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.txtPassword = new System.Windows.Forms.MaskedTextBox();
        this.label5 = new System.Windows.Forms.Label();
        this.chkIntSecurity = new System.Windows.Forms.CheckBox();
        this.btnConnect = new System.Windows.Forms.Button();
        this.clbCompanyList = new System.Windows.Forms.CheckedListBox();
        this.btnSave = new System.Windows.Forms.Button();
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        this.lblServerExample = new System.Windows.Forms.Label();
        this.toolTip = new System.Windows.Forms.ToolTip(this.components);
        this.groupBox1.SuspendLayout();
        this.SuspendLayout();
        // 
        // txtServer
        // 
        this.txtServer.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.txtServer.Location = new System.Drawing.Point(220, 38);
        this.txtServer.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.txtServer.Name = "txtServer";
        this.txtServer.Size = new System.Drawing.Size(684, 39);
        this.txtServer.TabIndex = 0;
        this.toolTip.SetToolTip(this.txtServer, "Enter the name of your SQL server. (e.g. ServerName\\InstanceName)");
        this.txtServer.Leave += this.txtServer_Leave;
        // 
        // txtSystemDatabase
        // 
        this.txtSystemDatabase.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.txtSystemDatabase.Location = new System.Drawing.Point(220, 330);
        this.txtSystemDatabase.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.txtSystemDatabase.Name = "txtSystemDatabase";
        this.txtSystemDatabase.Size = new System.Drawing.Size(684, 39);
        this.txtSystemDatabase.TabIndex = 4;
        this.txtSystemDatabase.Leave += this.txtSystemDatabase_Leave;
        // 
        // txtUsername
        // 
        this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.txtUsername.Location = new System.Drawing.Point(220, 158);
        this.txtUsername.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.txtUsername.Name = "txtUsername";
        this.txtUsername.Size = new System.Drawing.Size(684, 39);
        this.txtUsername.TabIndex = 2;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.label1.Location = new System.Drawing.Point(10, 44);
        this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(157, 32);
        this.label1.TabIndex = 6;
        this.label1.Text = "Server Name:";
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.label2.Location = new System.Drawing.Point(10, 336);
        this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(200, 32);
        this.label2.TabIndex = 7;
        this.label2.Text = "System Database:";
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.label3.Location = new System.Drawing.Point(10, 164);
        this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(126, 32);
        this.label3.TabIndex = 9;
        this.label3.Text = "Username:";
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.label4.Location = new System.Drawing.Point(10, 220);
        this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(116, 32);
        this.label4.TabIndex = 10;
        this.label4.Text = "Password:";
        // 
        // txtPassword
        // 
        this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.txtPassword.Location = new System.Drawing.Point(220, 214);
        this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.PasswordChar = '*';
        this.txtPassword.Size = new System.Drawing.Size(684, 39);
        this.txtPassword.TabIndex = 3;
        // 
        // label5
        // 
        this.label5.AutoSize = true;
        this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.label5.Location = new System.Drawing.Point(18, 516);
        this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(138, 32);
        this.label5.TabIndex = 8;
        this.label5.Text = "Companies:";
        // 
        // chkIntSecurity
        // 
        this.chkIntSecurity.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkIntSecurity.Location = new System.Drawing.Point(220, 270);
        this.chkIntSecurity.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.chkIntSecurity.Name = "chkIntSecurity";
        this.chkIntSecurity.Size = new System.Drawing.Size(296, 48);
        this.chkIntSecurity.TabIndex = 1;
        this.chkIntSecurity.Text = "Use Integrated Security";
        this.toolTip.SetToolTip(this.chkIntSecurity, "Select this option if the user you are entering is a Domain or SQL user.");
        this.chkIntSecurity.UseVisualStyleBackColor = true;
        this.chkIntSecurity.CheckedChanged += this.chkIntSecurity_CheckedChanged;
        // 
        // btnConnect
        // 
        this.btnConnect.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
        this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.btnConnect.ForeColor = System.Drawing.Color.White;
        this.btnConnect.Location = new System.Drawing.Point(684, 388);
        this.btnConnect.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.btnConnect.Name = "btnConnect";
        this.btnConnect.Size = new System.Drawing.Size(224, 76);
        this.btnConnect.TabIndex = 5;
        this.btnConnect.Text = "Connect";
        this.btnConnect.UseVisualStyleBackColor = false;
        this.btnConnect.Click += this.btnConnect_Click;
        // 
        // clbCompanyList
        // 
        this.clbCompanyList.CheckOnClick = true;
        this.clbCompanyList.Enabled = false;
        this.clbCompanyList.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.clbCompanyList.FormattingEnabled = true;
        this.clbCompanyList.Location = new System.Drawing.Point(170, 516);
        this.clbCompanyList.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.clbCompanyList.Name = "clbCompanyList";
        this.clbCompanyList.Size = new System.Drawing.Size(772, 148);
        this.clbCompanyList.TabIndex = 6;
        this.toolTip.SetToolTip(this.clbCompanyList, "Select the company or companies you want to assess.");
        this.clbCompanyList.ItemCheck += this.clbCompanyList_ItemCheck;
        // 
        // btnSave
        // 
        this.btnSave.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
        this.btnSave.Enabled = false;
        this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.btnSave.ForeColor = System.Drawing.Color.White;
        this.btnSave.Location = new System.Drawing.Point(718, 676);
        this.btnSave.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(224, 76);
        this.btnSave.TabIndex = 8;
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = false;
        this.btnSave.Click += this.btnSave_Click;
        // 
        // groupBox1
        // 
        this.groupBox1.Controls.Add(this.lblServerExample);
        this.groupBox1.Controls.Add(this.txtSystemDatabase);
        this.groupBox1.Controls.Add(this.txtServer);
        this.groupBox1.Controls.Add(this.txtUsername);
        this.groupBox1.Controls.Add(this.label1);
        this.groupBox1.Controls.Add(this.label2);
        this.groupBox1.Controls.Add(this.btnConnect);
        this.groupBox1.Controls.Add(this.chkIntSecurity);
        this.groupBox1.Controls.Add(this.label3);
        this.groupBox1.Controls.Add(this.label4);
        this.groupBox1.Controls.Add(this.txtPassword);
        this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.groupBox1.Location = new System.Drawing.Point(16, 16);
        this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.groupBox1.Size = new System.Drawing.Size(936, 490);
        this.groupBox1.TabIndex = 14;
        this.groupBox1.TabStop = false;
        this.groupBox1.Text = "SQL Settings";
        // 
        // lblServerExample
        // 
        this.lblServerExample.AutoSize = true;
        this.lblServerExample.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
        this.lblServerExample.Location = new System.Drawing.Point(220, 90);
        this.lblServerExample.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.lblServerExample.Name = "lblServerExample";
        this.lblServerExample.Size = new System.Drawing.Size(342, 32);
        this.lblServerExample.TabIndex = 11;
        this.lblServerExample.Text = "e.g. ServerName\\InstanceName";
        // 
        // DatabaseSettings
        // 
        this.AcceptButton = this.btnSave;
        this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.BackColor = System.Drawing.Color.FromArgb(248, 248, 247);
        this.ClientSize = new System.Drawing.Size(970, 770);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.clbCompanyList);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.groupBox1);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "DatabaseSettings";
        this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "GP Migration Diagnostic Tool";
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }


    #endregion
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    internal System.Windows.Forms.MaskedTextBox txtPassword;
    internal System.Windows.Forms.TextBox txtServer;
    internal System.Windows.Forms.TextBox txtSystemDatabase;
    internal System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label label5;
    internal System.Windows.Forms.CheckBox chkIntSecurity;
    private System.Windows.Forms.Button btnConnect;
    public System.Windows.Forms.CheckedListBox clbCompanyList;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Label lblServerExample;
}