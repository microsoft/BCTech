'*********************************************************
'
'    Copyright (c) Microsoft. All rights reserved.
'    This code is licensed under the Microsoft Public License.
'    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class Form1
#Region "Windows Form Designer generated code "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'This call is required by the Windows Form Designer.
		m_IsInitializing = true
		InitializeComponent()
		m_IsInitializing = False
	End Sub
	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SAFHelpProvider = New System.Windows.Forms.HelpProvider()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.DBConnect = New System.Windows.Forms.TabPage()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.lblCpnyRequired = New System.Windows.Forms.Label()
        Me.btnConnectClose = New System.Windows.Forms.Button()
        Me.btnConnectServer = New System.Windows.Forms.Button()
        Me.lblDbStatus = New System.Windows.Forms.Label()
        Me.lblDbRequired = New System.Windows.Forms.Label()
        Me.CpnyIDList = New System.Windows.Forms.ComboBox()
        Me.lblCpnyId = New System.Windows.Forms.Label()
        Me.SysDb = New System.Windows.Forms.TextBox()
        Me.lblSysDBNames = New System.Windows.Forms.Label()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.rbAuthenticationTypeSQL = New System.Windows.Forms.RadioButton()
        Me.rbAuthenticationTypeWindows = New System.Windows.Forms.RadioButton()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.lblUserId = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.txtUserId = New System.Windows.Forms.TextBox()
        Me.lblSQLServerName = New System.Windows.Forms.Label()
        Me.NameOfServer = New System.Windows.Forms.TextBox()
        Me.Analyze = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cLastRunDate = New System.Windows.Forms.DateTimePicker()
        Me.lExportFolder = New System.Windows.Forms.Label()
        Me.cmdBrowse = New System.Windows.Forms.Button()
        Me.cExportFolder = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmdViewAnalysis = New System.Windows.Forms.Button()
        Me.cmdSysAnalysis = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.AnalysisStatusLbl = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusUpdateLbl = New System.Windows.Forms.ToolStripStatusLabel()
        Me.UpdateStatusLbl = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout()
        Me.DBConnect.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        Me.Analyze.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.DBConnect)
        Me.TabControl1.Controls.Add(Me.Analyze)
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(805, 425)
        Me.TabControl1.TabIndex = 2
        '
        'DBConnect
        '
        Me.DBConnect.Controls.Add(Me.GroupBox8)
        Me.DBConnect.Location = New System.Drawing.Point(4, 22)
        Me.DBConnect.Name = "DBConnect"
        Me.DBConnect.Padding = New System.Windows.Forms.Padding(3)
        Me.DBConnect.Size = New System.Drawing.Size(797, 399)
        Me.DBConnect.TabIndex = 0
        Me.DBConnect.Text = "Database Connection"
        Me.DBConnect.UseVisualStyleBackColor = True
        '
        'GroupBox8
        '
        Me.GroupBox8.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox8.Controls.Add(Me.lblCpnyRequired)
        Me.GroupBox8.Controls.Add(Me.btnConnectClose)
        Me.GroupBox8.Controls.Add(Me.btnConnectServer)
        Me.GroupBox8.Controls.Add(Me.lblDbStatus)
        Me.GroupBox8.Controls.Add(Me.lblDbRequired)
        Me.GroupBox8.Controls.Add(Me.CpnyIDList)
        Me.GroupBox8.Controls.Add(Me.lblCpnyId)
        Me.GroupBox8.Controls.Add(Me.SysDb)
        Me.GroupBox8.Controls.Add(Me.lblSysDBNames)
        Me.GroupBox8.Controls.Add(Me.GroupBox9)
        Me.GroupBox8.Controls.Add(Me.lblSQLServerName)
        Me.GroupBox8.Controls.Add(Me.NameOfServer)
        Me.GroupBox8.Location = New System.Drawing.Point(6, 3)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(774, 363)
        Me.GroupBox8.TabIndex = 1
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Database Connection"
        '
        'lblCpnyRequired
        '
        Me.lblCpnyRequired.AutoSize = True
        Me.lblCpnyRequired.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCpnyRequired.ForeColor = System.Drawing.Color.Red
        Me.lblCpnyRequired.Location = New System.Drawing.Point(458, 299)
        Me.lblCpnyRequired.Name = "lblCpnyRequired"
        Me.lblCpnyRequired.Size = New System.Drawing.Size(50, 13)
        Me.lblCpnyRequired.TabIndex = 79
        Me.lblCpnyRequired.Text = "Required"
        '
        'btnConnectClose
        '
        Me.btnConnectClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnConnectClose.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnConnectClose.Location = New System.Drawing.Point(306, 224)
        Me.btnConnectClose.Name = "btnConnectClose"
        Me.btnConnectClose.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnConnectClose.Size = New System.Drawing.Size(107, 25)
        Me.btnConnectClose.TabIndex = 66
        Me.btnConnectClose.Text = "Close"
        Me.btnConnectClose.UseVisualStyleBackColor = False
        '
        'btnConnectServer
        '
        Me.btnConnectServer.BackColor = System.Drawing.SystemColors.Control
        Me.btnConnectServer.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnConnectServer.Location = New System.Drawing.Point(115, 224)
        Me.btnConnectServer.Name = "btnConnectServer"
        Me.btnConnectServer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnConnectServer.Size = New System.Drawing.Size(99, 25)
        Me.btnConnectServer.TabIndex = 67
        Me.btnConnectServer.Text = "Connect"
        Me.btnConnectServer.UseVisualStyleBackColor = False
        '
        'lblDbStatus
        '
        Me.lblDbStatus.AutoSize = True
        Me.lblDbStatus.Location = New System.Drawing.Point(115, 262)
        Me.lblDbStatus.Name = "lblDbStatus"
        Me.lblDbStatus.Size = New System.Drawing.Size(79, 13)
        Me.lblDbStatus.TabIndex = 80
        Me.lblDbStatus.Text = "Not Connected"
        '
        'lblDbRequired
        '
        Me.lblDbRequired.AutoSize = True
        Me.lblDbRequired.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDbRequired.ForeColor = System.Drawing.Color.Red
        Me.lblDbRequired.Location = New System.Drawing.Point(448, 189)
        Me.lblDbRequired.Name = "lblDbRequired"
        Me.lblDbRequired.Size = New System.Drawing.Size(50, 13)
        Me.lblDbRequired.TabIndex = 76
        Me.lblDbRequired.Text = "Required"
        '
        'CpnyIDList
        '
        Me.CpnyIDList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CpnyIDList.FormattingEnabled = True
        Me.CpnyIDList.Location = New System.Drawing.Point(292, 296)
        Me.CpnyIDList.Name = "CpnyIDList"
        Me.CpnyIDList.Size = New System.Drawing.Size(121, 21)
        Me.CpnyIDList.TabIndex = 70
        '
        'lblCpnyId
        '
        Me.lblCpnyId.AutoSize = True
        Me.lblCpnyId.Location = New System.Drawing.Point(115, 296)
        Me.lblCpnyId.Name = "lblCpnyId"
        Me.lblCpnyId.Size = New System.Drawing.Size(88, 13)
        Me.lblCpnyId.TabIndex = 69
        Me.lblCpnyId.Text = "Select Company:"
        '
        'SysDb
        '
        Me.SysDb.Location = New System.Drawing.Point(235, 186)
        Me.SysDb.Margin = New System.Windows.Forms.Padding(2)
        Me.SysDb.Name = "SysDb"
        Me.SysDb.Size = New System.Drawing.Size(178, 21)
        Me.SysDb.TabIndex = 67
        '
        'lblSysDBNames
        '
        Me.lblSysDBNames.AutoSize = True
        Me.lblSysDBNames.Location = New System.Drawing.Point(112, 189)
        Me.lblSysDBNames.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSysDBNames.Name = "lblSysDBNames"
        Me.lblSysDBNames.Size = New System.Drawing.Size(95, 13)
        Me.lblSysDBNames.TabIndex = 66
        Me.lblSysDBNames.Text = "System Database:"
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.rbAuthenticationTypeSQL)
        Me.GroupBox9.Controls.Add(Me.rbAuthenticationTypeWindows)
        Me.GroupBox9.Controls.Add(Me.lblPassword)
        Me.GroupBox9.Controls.Add(Me.lblUserId)
        Me.GroupBox9.Controls.Add(Me.txtPassword)
        Me.GroupBox9.Controls.Add(Me.txtUserId)
        Me.GroupBox9.Location = New System.Drawing.Point(19, 55)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Size = New System.Drawing.Size(670, 121)
        Me.GroupBox9.TabIndex = 16
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "Authentication"
        '
        'rbAuthenticationTypeSQL
        '
        Me.rbAuthenticationTypeSQL.AutoSize = True
        Me.rbAuthenticationTypeSQL.Location = New System.Drawing.Point(96, 46)
        Me.rbAuthenticationTypeSQL.Name = "rbAuthenticationTypeSQL"
        Me.rbAuthenticationTypeSQL.Size = New System.Drawing.Size(152, 17)
        Me.rbAuthenticationTypeSQL.TabIndex = 62
        Me.rbAuthenticationTypeSQL.Text = "SQL Server Authentication"
        Me.rbAuthenticationTypeSQL.UseVisualStyleBackColor = True
        '
        'rbAuthenticationTypeWindows
        '
        Me.rbAuthenticationTypeWindows.AutoSize = True
        Me.rbAuthenticationTypeWindows.Checked = True
        Me.rbAuthenticationTypeWindows.Location = New System.Drawing.Point(96, 20)
        Me.rbAuthenticationTypeWindows.Name = "rbAuthenticationTypeWindows"
        Me.rbAuthenticationTypeWindows.Size = New System.Drawing.Size(141, 17)
        Me.rbAuthenticationTypeWindows.TabIndex = 61
        Me.rbAuthenticationTypeWindows.TabStop = True
        Me.rbAuthenticationTypeWindows.Text = "Windows Authentication"
        Me.rbAuthenticationTypeWindows.UseVisualStyleBackColor = True
        '
        'lblPassword
        '
        Me.lblPassword.BackColor = System.Drawing.SystemColors.Control
        Me.lblPassword.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblPassword.Enabled = False
        Me.lblPassword.Location = New System.Drawing.Point(123, 91)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblPassword.Size = New System.Drawing.Size(76, 17)
        Me.lblPassword.TabIndex = 60
        Me.lblPassword.Text = "Password:"
        '
        'lblUserId
        '
        Me.lblUserId.BackColor = System.Drawing.SystemColors.Control
        Me.lblUserId.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblUserId.Enabled = False
        Me.lblUserId.Location = New System.Drawing.Point(123, 71)
        Me.lblUserId.Name = "lblUserId"
        Me.lblUserId.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblUserId.Size = New System.Drawing.Size(72, 17)
        Me.lblUserId.TabIndex = 59
        Me.lblUserId.Text = "Login ID:"
        '
        'txtPassword
        '
        Me.txtPassword.AcceptsReturn = True
        Me.txtPassword.BackColor = System.Drawing.SystemColors.Window
        Me.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtPassword.Enabled = False
        Me.txtPassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPassword.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtPassword.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtPassword.Location = New System.Drawing.Point(201, 94)
        Me.txtPassword.MaxLength = 30
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(120)
        Me.txtPassword.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtPassword.Size = New System.Drawing.Size(153, 20)
        Me.txtPassword.TabIndex = 58
        '
        'txtUserId
        '
        Me.txtUserId.AcceptsReturn = True
        Me.txtUserId.BackColor = System.Drawing.SystemColors.Window
        Me.txtUserId.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtUserId.Enabled = False
        Me.txtUserId.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUserId.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtUserId.Location = New System.Drawing.Point(201, 68)
        Me.txtUserId.MaxLength = 0
        Me.txtUserId.Name = "txtUserId"
        Me.txtUserId.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtUserId.Size = New System.Drawing.Size(153, 20)
        Me.txtUserId.TabIndex = 57
        Me.txtUserId.Text = "sa"
        '
        'lblSQLServerName
        '
        Me.lblSQLServerName.AutoSize = True
        Me.lblSQLServerName.BackColor = System.Drawing.SystemColors.Control
        Me.lblSQLServerName.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblSQLServerName.Location = New System.Drawing.Point(16, 22)
        Me.lblSQLServerName.Name = "lblSQLServerName"
        Me.lblSQLServerName.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblSQLServerName.Size = New System.Drawing.Size(95, 13)
        Me.lblSQLServerName.TabIndex = 15
        Me.lblSQLServerName.Text = "SQL Server Name:"
        Me.lblSQLServerName.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'NameOfServer
        '
        Me.NameOfServer.AcceptsReturn = True
        Me.NameOfServer.BackColor = System.Drawing.SystemColors.Window
        Me.NameOfServer.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.NameOfServer.Location = New System.Drawing.Point(179, 19)
        Me.NameOfServer.MaxLength = 0
        Me.NameOfServer.Name = "NameOfServer"
        Me.NameOfServer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.NameOfServer.Size = New System.Drawing.Size(265, 21)
        Me.NameOfServer.TabIndex = 14
        '
        'Analyze
        '
        Me.Analyze.BackColor = System.Drawing.SystemColors.Control
        Me.Analyze.Controls.Add(Me.GroupBox1)
        Me.Analyze.Location = New System.Drawing.Point(4, 22)
        Me.Analyze.Name = "Analyze"
        Me.Analyze.Padding = New System.Windows.Forms.Padding(3)
        Me.Analyze.Size = New System.Drawing.Size(797, 399)
        Me.Analyze.TabIndex = 1
        Me.Analyze.Text = "Analyze"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.cLastRunDate)
        Me.GroupBox1.Controls.Add(Me.lExportFolder)
        Me.GroupBox1.Controls.Add(Me.cmdBrowse)
        Me.GroupBox1.Controls.Add(Me.cExportFolder)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.cmdViewAnalysis)
        Me.GroupBox1.Controls.Add(Me.cmdSysAnalysis)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(-3, 1)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(597, 288)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "System Analysis"
        '
        'cLastRunDate
        '
        Me.cLastRunDate.Enabled = False
        Me.cLastRunDate.Location = New System.Drawing.Point(430, 72)
        Me.cLastRunDate.Name = "cLastRunDate"
        Me.cLastRunDate.Size = New System.Drawing.Size(95, 21)
        Me.cLastRunDate.TabIndex = 88
        '
        'lExportFolder
        '
        Me.lExportFolder.AutoSize = True
        Me.lExportFolder.Location = New System.Drawing.Point(28, 107)
        Me.lExportFolder.Name = "lExportFolder"
        Me.lExportFolder.Size = New System.Drawing.Size(99, 13)
        Me.lExportFolder.TabIndex = 10
        Me.lExportFolder.Text = "Export folder path:"
        '
        'cmdBrowse
        '
        Me.cmdBrowse.Location = New System.Drawing.Point(342, 153)
        Me.cmdBrowse.Name = "cmdBrowse"
        Me.cmdBrowse.Size = New System.Drawing.Size(75, 23)
        Me.cmdBrowse.TabIndex = 9
        Me.cmdBrowse.Text = "Browse"
        Me.cmdBrowse.UseVisualStyleBackColor = True
        '
        'cExportFolder
        '
        Me.cExportFolder.Location = New System.Drawing.Point(28, 126)
        Me.cExportFolder.Name = "cExportFolder"
        Me.cExportFolder.Size = New System.Drawing.Size(470, 21)
        Me.cExportFolder.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Enabled = False
        Me.Label2.Location = New System.Drawing.Point(345, 75)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(79, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Last Run Date:"
        '
        'cmdViewAnalysis
        '
        Me.cmdViewAnalysis.Enabled = False
        Me.cmdViewAnalysis.Location = New System.Drawing.Point(186, 70)
        Me.cmdViewAnalysis.Name = "cmdViewAnalysis"
        Me.cmdViewAnalysis.Size = New System.Drawing.Size(153, 23)
        Me.cmdViewAnalysis.TabIndex = 4
        Me.cmdViewAnalysis.Text = "View Analysis Report"
        Me.cmdViewAnalysis.UseVisualStyleBackColor = True
        '
        'cmdSysAnalysis
        '
        Me.cmdSysAnalysis.Location = New System.Drawing.Point(28, 70)
        Me.cmdSysAnalysis.Name = "cmdSysAnalysis"
        Me.cmdSysAnalysis.Size = New System.Drawing.Size(152, 23)
        Me.cmdSysAnalysis.TabIndex = 3
        Me.cmdSysAnalysis.Text = "Perform System Analysis"
        Me.cmdSysAnalysis.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(25, 17)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(462, 40)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AnalysisStatusLbl, Me.StatusUpdateLbl, Me.UpdateStatusLbl})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 391)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(796, 22)
        Me.StatusStrip1.TabIndex = 68
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'AnalysisStatusLbl
        '
        Me.AnalysisStatusLbl.Name = "AnalysisStatusLbl"
        Me.AnalysisStatusLbl.Size = New System.Drawing.Size(0, 17)
        '
        'StatusUpdateLbl
        '
        Me.StatusUpdateLbl.Name = "StatusUpdateLbl"
        Me.StatusUpdateLbl.Size = New System.Drawing.Size(0, 17)
        '
        'UpdateStatusLbl
        '
        Me.UpdateStatusLbl.Name = "UpdateStatusLbl"
        Me.UpdateStatusLbl.Size = New System.Drawing.Size(0, 17)
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Red
        Me.Label3.Location = New System.Drawing.Point(517, 126)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(50, 13)
        Me.Label3.TabIndex = 89
        Me.Label3.Text = "Required"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(796, 413)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TabControl1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Location = New System.Drawing.Point(4, 23)
        Me.Name = "Form1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text = "Analysis Tool (Microsoft Dynamics SL)"
        Me.TabControl1.ResumeLayout(False)
        Me.DBConnect.ResumeLayout(False)
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox9.PerformLayout()
        Me.Analyze.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SAFHelpProvider As System.Windows.Forms.HelpProvider
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents Analyze As TabPage
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lExportFolder As Label
    Friend WithEvents cmdBrowse As Button
    Friend WithEvents cExportFolder As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents cmdViewAnalysis As Button
    Friend WithEvents cmdSysAnalysis As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents GroupBox8 As GroupBox
    Friend WithEvents CpnyIDList As ComboBox
    Friend WithEvents lblCpnyId As Label
    Friend WithEvents SysDb As TextBox
    Friend WithEvents lblSysDBNames As Label
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents rbAuthenticationTypeSQL As RadioButton
    Friend WithEvents rbAuthenticationTypeWindows As RadioButton
    Public WithEvents lblPassword As Label
    Public WithEvents lblUserId As Label
    Public WithEvents txtPassword As TextBox
    Public WithEvents txtUserId As TextBox
    Public WithEvents lblSQLServerName As Label
    Public WithEvents NameOfServer As TextBox
    Public WithEvents btnConnectClose As Button
    Public WithEvents btnConnectServer As Button
    Friend WithEvents cLastRunDate As DateTimePicker
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents AnalysisStatusLbl As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Private WithEvents DBConnect As TabPage
    Friend WithEvents lblDbRequired As Label
    Friend WithEvents lblCpnyRequired As Label
    Friend WithEvents lblDbStatus As Label
    Friend WithEvents StatusUpdateLbl As ToolStripStatusLabel
    Friend WithEvents UpdateStatusLbl As ToolStripStatusLabel
    Friend WithEvents Label3 As Label
#End Region
End Class
