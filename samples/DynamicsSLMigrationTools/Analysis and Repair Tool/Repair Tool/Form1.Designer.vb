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
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SAFHelpProvider = New System.Windows.Forms.HelpProvider()
        Me.lCpnyID = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.lFiscYr_End = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.DBConnect = New System.Windows.Forms.TabPage()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.lblDirRequired = New System.Windows.Forms.Label()
        Me.lblCpnyRequired = New System.Windows.Forms.Label()
        Me.lblDbRequired = New System.Windows.Forms.Label()
        Me.lblDbStatus = New System.Windows.Forms.Label()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.txtEventLogPath = New System.Windows.Forms.TextBox()
        Me.lblEventOutput = New System.Windows.Forms.Label()
        Me.CpnyIDList = New System.Windows.Forms.ComboBox()
        Me.lblCpnyId = New System.Windows.Forms.Label()
        Me.SysDb = New System.Windows.Forms.TextBox()
        Me.lblSysDBNames = New System.Windows.Forms.Label()
        Me.btnConnectClose = New System.Windows.Forms.Button()
        Me.ConnectServer = New System.Windows.Forms.Button()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.rbAuthenticationTypeSQL = New System.Windows.Forms.RadioButton()
        Me.rbAuthenticationTypeWindows = New System.Windows.Forms.RadioButton()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.lblUserId = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.txtUserId = New System.Windows.Forms.TextBox()
        Me.lblSQLServerName = New System.Windows.Forms.Label()
        Me.NameOfServer = New System.Windows.Forms.TextBox()
        Me.TabPageSUM = New System.Windows.Forms.TabPage()
        Me.cErrorsSOVal = New System.Windows.Forms.TextBox()
        Me.cCompletedSOChk = New System.Windows.Forms.CheckBox()
        Me.cCompletedPAChk = New System.Windows.Forms.CheckBox()
        Me.cCompletedPOChk = New System.Windows.Forms.CheckBox()
        Me.cCompletedINChk = New System.Windows.Forms.CheckBox()
        Me.cCompletedARChk = New System.Windows.Forms.CheckBox()
        Me.cCompletedAPChk = New System.Windows.Forms.CheckBox()
        Me.cCompletedGLChk = New System.Windows.Forms.CheckBox()
        Me.cWarningsSOVal = New System.Windows.Forms.TextBox()
        Me.cWarningsGLVal = New System.Windows.Forms.TextBox()
        Me.cErrorsAPVal = New System.Windows.Forms.TextBox()
        Me.cWarningsARVal = New System.Windows.Forms.TextBox()
        Me.cErrorsARVal = New System.Windows.Forms.TextBox()
        Me.cWarningsINVal = New System.Windows.Forms.TextBox()
        Me.cErrorsINVal = New System.Windows.Forms.TextBox()
        Me.cWarningsPOVal = New System.Windows.Forms.TextBox()
        Me.cErrorsPOVal = New System.Windows.Forms.TextBox()
        Me.cWarningsPAVal = New System.Windows.Forms.TextBox()
        Me.cErrorsPAVal = New System.Windows.Forms.TextBox()
        Me.cWarningsAPVal = New System.Windows.Forms.TextBox()
        Me.cErrorsGLVal = New System.Windows.Forms.TextBox()
        Me.lSOTotal = New System.Windows.Forms.Label()
        Me.lSOActive = New System.Windows.Forms.Label()
        Me.lSalesOrder = New System.Windows.Forms.Label()
        Me.lCurrPerHdr = New System.Windows.Forms.Label()
        Me.lCurrPAPeriod = New System.Windows.Forms.Label()
        Me.lCurrINPeriod = New System.Windows.Forms.Label()
        Me.lCurrARPeriod = New System.Windows.Forms.Label()
        Me.lCurrAPPeriod = New System.Windows.Forms.Label()
        Me.lGLPeriod = New System.Windows.Forms.Label()
        Me.lOrderActive = New System.Windows.Forms.Label()
        Me.lOrderTotal = New System.Windows.Forms.Label()
        Me.lInvtActive = New System.Windows.Forms.Label()
        Me.lProjActive = New System.Windows.Forms.Label()
        Me.lCustActive = New System.Windows.Forms.Label()
        Me.lVendActive = New System.Windows.Forms.Label()
        Me.lAcctActive = New System.Windows.Forms.Label()
        Me.lProjTotal = New System.Windows.Forms.Label()
        Me.lItemTotal = New System.Windows.Forms.Label()
        Me.lCustTotal = New System.Windows.Forms.Label()
        Me.lVendTotal = New System.Windows.Forms.Label()
        Me.lAcctTotal = New System.Windows.Forms.Label()
        Me.lPurchasing = New System.Windows.Forms.Label()
        Me.lProjController = New System.Windows.Forms.Label()
        Me.lInventory = New System.Windows.Forms.Label()
        Me.lAcctsReceivable = New System.Windows.Forms.Label()
        Me.lAcctsPayable = New System.Windows.Forms.Label()
        Me.lGeneralLedger = New System.Windows.Forms.Label()
        Me.lCompleted = New System.Windows.Forms.Label()
        Me.lWarnings = New System.Windows.Forms.Label()
        Me.lErrors = New System.Windows.Forms.Label()
        Me.TabPagePA = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.cDateValidatedProj = New System.Windows.Forms.DateTimePicker()
        Me.cErrorsProj = New System.Windows.Forms.TextBox()
        Me.cWarningsProj = New System.Windows.Forms.TextBox()
        Me.cValCompletedProj = New System.Windows.Forms.CheckBox()
        Me.cmdProjEventLog = New System.Windows.Forms.Button()
        Me.lCurrPAPeriodDisp = New System.Windows.Forms.Label()
        Me.lCurrPAPeriodLabel = New System.Windows.Forms.Label()
        Me.lWarningsProj = New System.Windows.Forms.Label()
        Me.lErrorsProj = New System.Windows.Forms.Label()
        Me.lDateValidatedProj = New System.Windows.Forms.Label()
        Me.cmdValidateProj = New System.Windows.Forms.Button()
        Me.TabPageSO = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.cDateValidatedSO = New System.Windows.Forms.DateTimePicker()
        Me.cErrorsSO = New System.Windows.Forms.TextBox()
        Me.cWarningsSO = New System.Windows.Forms.TextBox()
        Me.cValCompletedSO = New System.Windows.Forms.CheckBox()
        Me.cmdSOEventLog = New System.Windows.Forms.Button()
        Me.cCurrSOPeriodDisp = New System.Windows.Forms.Label()
        Me.lCurrSOPeriodLabel = New System.Windows.Forms.Label()
        Me.lWarningSO = New System.Windows.Forms.Label()
        Me.lErrorsSO = New System.Windows.Forms.Label()
        Me.lValidatedSO = New System.Windows.Forms.Label()
        Me.cmdValidateSalesOrder = New System.Windows.Forms.Button()
        Me.TabPageIN = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.cDateValidatedItem = New System.Windows.Forms.DateTimePicker()
        Me.cErrorsItem = New System.Windows.Forms.TextBox()
        Me.cWarningsItem = New System.Windows.Forms.TextBox()
        Me.cValCompletedItem = New System.Windows.Forms.CheckBox()
        Me.cmdIntEventLog = New System.Windows.Forms.Button()
        Me.lCurrINPeriodDisp = New System.Windows.Forms.Label()
        Me.lCurrINPeriodLabel = New System.Windows.Forms.Label()
        Me.lWarningsItem = New System.Windows.Forms.Label()
        Me.lErrorsItem = New System.Windows.Forms.Label()
        Me.lDateValidatedItem = New System.Windows.Forms.Label()
        Me.cmdValidateItem = New System.Windows.Forms.Button()
        Me.TabPageAP = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cDateValidatedVend = New System.Windows.Forms.DateTimePicker()
        Me.cErrorsVend = New System.Windows.Forms.TextBox()
        Me.cWarningsVend = New System.Windows.Forms.TextBox()
        Me.cValCompletedVend = New System.Windows.Forms.CheckBox()
        Me.cmdAPEventLog = New System.Windows.Forms.Button()
        Me.lCurrAPPeriodDisp = New System.Windows.Forms.Label()
        Me.lblAPPeriod = New System.Windows.Forms.Label()
        Me.lWarningsVend = New System.Windows.Forms.Label()
        Me.lErrorsVend = New System.Windows.Forms.Label()
        Me.lDateValidatedVend = New System.Windows.Forms.Label()
        Me.cmdValidateVend = New System.Windows.Forms.Button()
        Me.TabPagePO = New System.Windows.Forms.TabPage()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.cDateValidatedPurch = New System.Windows.Forms.DateTimePicker()
        Me.cErrorsPurch = New System.Windows.Forms.TextBox()
        Me.cWarningsPurch = New System.Windows.Forms.TextBox()
        Me.cValCompletedPurch = New System.Windows.Forms.CheckBox()
        Me.cmdPOEventLog = New System.Windows.Forms.Button()
        Me.lPONote = New System.Windows.Forms.Label()
        Me.lWarningsPurch = New System.Windows.Forms.Label()
        Me.lErrorsPurch = New System.Windows.Forms.Label()
        Me.lDateValidatedPurch = New System.Windows.Forms.Label()
        Me.cmdValidatePurch = New System.Windows.Forms.Button()
        Me.TabPageAR = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cWarningsCust = New System.Windows.Forms.TextBox()
        Me.cValCompletedCust = New System.Windows.Forms.CheckBox()
        Me.cDateValidatedCust = New System.Windows.Forms.DateTimePicker()
        Me.cErrorsCust = New System.Windows.Forms.TextBox()
        Me.cmdAREventLog = New System.Windows.Forms.Button()
        Me.lCurrARPerLabelDisp = New System.Windows.Forms.Label()
        Me.lCurrARPeriodLabel = New System.Windows.Forms.Label()
        Me.lWarningsCust = New System.Windows.Forms.Label()
        Me.lErrorsCust = New System.Windows.Forms.Label()
        Me.lDateValidatedCust = New System.Windows.Forms.Label()
        Me.cmdValidateCust = New System.Windows.Forms.Button()
        Me.TabPageGL = New System.Windows.Forms.TabPage()
        Me.cmdGLEventLog = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cCompletedCOA = New System.Windows.Forms.CheckBox()
        Me.cErrorsCOAVal = New System.Windows.Forms.TextBox()
        Me.cWarningsCOAVal = New System.Windows.Forms.TextBox()
        Me.cDateCompletedCOA = New System.Windows.Forms.DateTimePicker()
        Me.lCurrPeriodDisp = New System.Windows.Forms.Label()
        Me.lWarningsCOA = New System.Windows.Forms.Label()
        Me.lCurrPeriodLabel = New System.Windows.Forms.Label()
        Me.lLedgerDisp = New System.Windows.Forms.Label()
        Me.lLedgerLabel = New System.Windows.Forms.Label()
        Me.lErrorsCOA = New System.Windows.Forms.Label()
        Me.lDateValidatedCOA = New System.Windows.Forms.Label()
        Me.cmdValidateAccts = New System.Windows.Forms.Button()
        Me.cCpnyIDTxt = New System.Windows.Forms.TextBox()
        Me.cFiscYr_End = New System.Windows.Forms.TextBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusLbl = New System.Windows.Forms.ToolStripStatusLabel()
        Me.FolderBrowserDialog2 = New System.Windows.Forms.FolderBrowserDialog()
        Me.TabControl1.SuspendLayout()
        Me.DBConnect.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        Me.TabPageSUM.SuspendLayout()
        Me.TabPagePA.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.TabPageSO.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.TabPageIN.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.TabPageAP.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TabPagePO.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.TabPageAR.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TabPageGL.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lCpnyID
        '
        Me.lCpnyID.AutoSize = True
        Me.lCpnyID.Location = New System.Drawing.Point(12, 12)
        Me.lCpnyID.Name = "lCpnyID"
        Me.lCpnyID.Size = New System.Drawing.Size(70, 13)
        Me.lCpnyID.TabIndex = 1
        Me.lCpnyID.Text = "Company ID:"
        '
        'lFiscYr_End
        '
        Me.lFiscYr_End.AutoSize = True
        Me.lFiscYr_End.Location = New System.Drawing.Point(256, 12)
        Me.lFiscYr_End.Name = "lFiscYr_End"
        Me.lFiscYr_End.Size = New System.Drawing.Size(62, 13)
        Me.lFiscYr_End.TabIndex = 7
        Me.lFiscYr_End.Text = "Fiscal Year:"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.DBConnect)
        Me.TabControl1.Controls.Add(Me.TabPageSUM)
        Me.TabControl1.Controls.Add(Me.TabPagePA)
        Me.TabControl1.Controls.Add(Me.TabPageSO)
        Me.TabControl1.Controls.Add(Me.TabPageIN)
        Me.TabControl1.Controls.Add(Me.TabPageAP)
        Me.TabControl1.Controls.Add(Me.TabPagePO)
        Me.TabControl1.Controls.Add(Me.TabPageAR)
        Me.TabControl1.Controls.Add(Me.TabPageGL)
        Me.TabControl1.Location = New System.Drawing.Point(7, 78)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1188, 395)
        Me.TabControl1.TabIndex = 78
        '
        'DBConnect
        '
        Me.DBConnect.Controls.Add(Me.GroupBox8)
        Me.DBConnect.Location = New System.Drawing.Point(4, 22)
        Me.DBConnect.Name = "DBConnect"
        Me.DBConnect.Size = New System.Drawing.Size(1180, 369)
        Me.DBConnect.TabIndex = 8
        Me.DBConnect.Text = "Database Connection"
        Me.DBConnect.UseVisualStyleBackColor = True
        '
        'GroupBox8
        '
        Me.GroupBox8.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox8.Controls.Add(Me.lblDirRequired)
        Me.GroupBox8.Controls.Add(Me.lblCpnyRequired)
        Me.GroupBox8.Controls.Add(Me.lblDbRequired)
        Me.GroupBox8.Controls.Add(Me.lblDbStatus)
        Me.GroupBox8.Controls.Add(Me.btnBrowse)
        Me.GroupBox8.Controls.Add(Me.txtEventLogPath)
        Me.GroupBox8.Controls.Add(Me.lblEventOutput)
        Me.GroupBox8.Controls.Add(Me.CpnyIDList)
        Me.GroupBox8.Controls.Add(Me.lblCpnyId)
        Me.GroupBox8.Controls.Add(Me.SysDb)
        Me.GroupBox8.Controls.Add(Me.lblSysDBNames)
        Me.GroupBox8.Controls.Add(Me.btnConnectClose)
        Me.GroupBox8.Controls.Add(Me.ConnectServer)
        Me.GroupBox8.Controls.Add(Me.GroupBox9)
        Me.GroupBox8.Controls.Add(Me.lblSQLServerName)
        Me.GroupBox8.Controls.Add(Me.NameOfServer)
        Me.GroupBox8.Location = New System.Drawing.Point(-1, 3)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(1181, 363)
        Me.GroupBox8.TabIndex = 0
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Database Connection"
        '
        'lblDirRequired
        '
        Me.lblDirRequired.AutoSize = True
        Me.lblDirRequired.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDirRequired.ForeColor = System.Drawing.Color.Red
        Me.lblDirRequired.Location = New System.Drawing.Point(660, 329)
        Me.lblDirRequired.Name = "lblDirRequired"
        Me.lblDirRequired.Size = New System.Drawing.Size(50, 13)
        Me.lblDirRequired.TabIndex = 77
        Me.lblDirRequired.Text = "Required"
        '
        'lblCpnyRequired
        '
        Me.lblCpnyRequired.AutoSize = True
        Me.lblCpnyRequired.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCpnyRequired.ForeColor = System.Drawing.Color.Red
        Me.lblCpnyRequired.Location = New System.Drawing.Point(487, 296)
        Me.lblCpnyRequired.Name = "lblCpnyRequired"
        Me.lblCpnyRequired.Size = New System.Drawing.Size(50, 13)
        Me.lblCpnyRequired.TabIndex = 76
        Me.lblCpnyRequired.Text = "Required"
        '
        'lblDbRequired
        '
        Me.lblDbRequired.AutoSize = True
        Me.lblDbRequired.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDbRequired.ForeColor = System.Drawing.Color.Red
        Me.lblDbRequired.Location = New System.Drawing.Point(487, 202)
        Me.lblDbRequired.Name = "lblDbRequired"
        Me.lblDbRequired.Size = New System.Drawing.Size(50, 13)
        Me.lblDbRequired.TabIndex = 75
        Me.lblDbRequired.Text = "Required"
        '
        'lblDbStatus
        '
        Me.lblDbStatus.AutoSize = True
        Me.lblDbStatus.Location = New System.Drawing.Point(94, 259)
        Me.lblDbStatus.Name = "lblDbStatus"
        Me.lblDbStatus.Size = New System.Drawing.Size(79, 13)
        Me.lblDbStatus.TabIndex = 74
        Me.lblDbStatus.Text = "Not Connected"
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(730, 329)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowse.TabIndex = 73
        Me.btnBrowse.Text = "Browse"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'txtEventLogPath
        '
        Me.txtEventLogPath.Location = New System.Drawing.Point(218, 329)
        Me.txtEventLogPath.Name = "txtEventLogPath"
        Me.txtEventLogPath.Size = New System.Drawing.Size(436, 21)
        Me.txtEventLogPath.TabIndex = 72
        '
        'lblEventOutput
        '
        Me.lblEventOutput.AutoSize = True
        Me.lblEventOutput.Location = New System.Drawing.Point(94, 329)
        Me.lblEventOutput.Name = "lblEventOutput"
        Me.lblEventOutput.Size = New System.Drawing.Size(116, 13)
        Me.lblEventOutput.TabIndex = 71
        Me.lblEventOutput.Text = "Event Log Destination:"
        '
        'CpnyIDList
        '
        Me.CpnyIDList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CpnyIDList.FormattingEnabled = True
        Me.CpnyIDList.Location = New System.Drawing.Point(218, 296)
        Me.CpnyIDList.Name = "CpnyIDList"
        Me.CpnyIDList.Size = New System.Drawing.Size(178, 21)
        Me.CpnyIDList.TabIndex = 70
        '
        'lblCpnyId
        '
        Me.lblCpnyId.AutoSize = True
        Me.lblCpnyId.Location = New System.Drawing.Point(94, 296)
        Me.lblCpnyId.Name = "lblCpnyId"
        Me.lblCpnyId.Size = New System.Drawing.Size(88, 13)
        Me.lblCpnyId.TabIndex = 69
        Me.lblCpnyId.Text = "Select Company:"
        '
        'SysDb
        '
        Me.SysDb.Location = New System.Drawing.Point(218, 195)
        Me.SysDb.Margin = New System.Windows.Forms.Padding(2)
        Me.SysDb.Name = "SysDb"
        Me.SysDb.Size = New System.Drawing.Size(178, 21)
        Me.SysDb.TabIndex = 67
        '
        'lblSysDBNames
        '
        Me.lblSysDBNames.AutoSize = True
        Me.lblSysDBNames.Location = New System.Drawing.Point(94, 194)
        Me.lblSysDBNames.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSysDBNames.Name = "lblSysDBNames"
        Me.lblSysDBNames.Size = New System.Drawing.Size(95, 13)
        Me.lblSysDBNames.TabIndex = 66
        Me.lblSysDBNames.Text = "System Database:"
        '
        'btnConnectClose
        '
        Me.btnConnectClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnConnectClose.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnConnectClose.Location = New System.Drawing.Point(292, 227)
        Me.btnConnectClose.Name = "btnConnectClose"
        Me.btnConnectClose.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnConnectClose.Size = New System.Drawing.Size(105, 25)
        Me.btnConnectClose.TabIndex = 65
        Me.btnConnectClose.Text = "Close"
        Me.btnConnectClose.UseVisualStyleBackColor = False
        '
        'ConnectServer
        '
        Me.ConnectServer.BackColor = System.Drawing.SystemColors.Control
        Me.ConnectServer.Cursor = System.Windows.Forms.Cursors.Default
        Me.ConnectServer.Location = New System.Drawing.Point(94, 227)
        Me.ConnectServer.Name = "ConnectServer"
        Me.ConnectServer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ConnectServer.Size = New System.Drawing.Size(105, 25)
        Me.ConnectServer.TabIndex = 64
        Me.ConnectServer.Text = "Connect"
        Me.ConnectServer.UseVisualStyleBackColor = False
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
        Me.GroupBox9.Size = New System.Drawing.Size(718, 121)
        Me.GroupBox9.TabIndex = 16
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "Authentication"
        '
        'rbAuthenticationTypeSQL
        '
        Me.rbAuthenticationTypeSQL.AutoSize = True
        Me.rbAuthenticationTypeSQL.Location = New System.Drawing.Point(218, 38)
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
        Me.rbAuthenticationTypeWindows.Location = New System.Drawing.Point(218, 6)
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
        Me.lblPassword.Location = New System.Drawing.Point(249, 94)
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
        Me.lblUserId.Location = New System.Drawing.Point(252, 68)
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
        Me.txtPassword.Location = New System.Drawing.Point(347, 94)
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
        Me.txtUserId.Location = New System.Drawing.Point(347, 68)
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
        Me.lblSQLServerName.Location = New System.Drawing.Point(94, 17)
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
        Me.NameOfServer.Location = New System.Drawing.Point(218, 14)
        Me.NameOfServer.MaxLength = 0
        Me.NameOfServer.Name = "NameOfServer"
        Me.NameOfServer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.NameOfServer.Size = New System.Drawing.Size(265, 21)
        Me.NameOfServer.TabIndex = 14
        '
        'TabPageSUM
        '
        Me.TabPageSUM.BackColor = System.Drawing.SystemColors.Control
        Me.TabPageSUM.Controls.Add(Me.cErrorsSOVal)
        Me.TabPageSUM.Controls.Add(Me.cCompletedSOChk)
        Me.TabPageSUM.Controls.Add(Me.cCompletedPAChk)
        Me.TabPageSUM.Controls.Add(Me.cCompletedPOChk)
        Me.TabPageSUM.Controls.Add(Me.cCompletedINChk)
        Me.TabPageSUM.Controls.Add(Me.cCompletedARChk)
        Me.TabPageSUM.Controls.Add(Me.cCompletedAPChk)
        Me.TabPageSUM.Controls.Add(Me.cCompletedGLChk)
        Me.TabPageSUM.Controls.Add(Me.cWarningsSOVal)
        Me.TabPageSUM.Controls.Add(Me.cWarningsGLVal)
        Me.TabPageSUM.Controls.Add(Me.cErrorsAPVal)
        Me.TabPageSUM.Controls.Add(Me.cWarningsARVal)
        Me.TabPageSUM.Controls.Add(Me.cErrorsARVal)
        Me.TabPageSUM.Controls.Add(Me.cWarningsINVal)
        Me.TabPageSUM.Controls.Add(Me.cErrorsINVal)
        Me.TabPageSUM.Controls.Add(Me.cWarningsPOVal)
        Me.TabPageSUM.Controls.Add(Me.cErrorsPOVal)
        Me.TabPageSUM.Controls.Add(Me.cWarningsPAVal)
        Me.TabPageSUM.Controls.Add(Me.cErrorsPAVal)
        Me.TabPageSUM.Controls.Add(Me.cWarningsAPVal)
        Me.TabPageSUM.Controls.Add(Me.cErrorsGLVal)
        Me.TabPageSUM.Controls.Add(Me.lSOTotal)
        Me.TabPageSUM.Controls.Add(Me.lSOActive)
        Me.TabPageSUM.Controls.Add(Me.lSalesOrder)
        Me.TabPageSUM.Controls.Add(Me.lCurrPerHdr)
        Me.TabPageSUM.Controls.Add(Me.lCurrPAPeriod)
        Me.TabPageSUM.Controls.Add(Me.lCurrINPeriod)
        Me.TabPageSUM.Controls.Add(Me.lCurrARPeriod)
        Me.TabPageSUM.Controls.Add(Me.lCurrAPPeriod)
        Me.TabPageSUM.Controls.Add(Me.lGLPeriod)
        Me.TabPageSUM.Controls.Add(Me.lOrderActive)
        Me.TabPageSUM.Controls.Add(Me.lOrderTotal)
        Me.TabPageSUM.Controls.Add(Me.lInvtActive)
        Me.TabPageSUM.Controls.Add(Me.lProjActive)
        Me.TabPageSUM.Controls.Add(Me.lCustActive)
        Me.TabPageSUM.Controls.Add(Me.lVendActive)
        Me.TabPageSUM.Controls.Add(Me.lAcctActive)
        Me.TabPageSUM.Controls.Add(Me.lProjTotal)
        Me.TabPageSUM.Controls.Add(Me.lItemTotal)
        Me.TabPageSUM.Controls.Add(Me.lCustTotal)
        Me.TabPageSUM.Controls.Add(Me.lVendTotal)
        Me.TabPageSUM.Controls.Add(Me.lAcctTotal)
        Me.TabPageSUM.Controls.Add(Me.lPurchasing)
        Me.TabPageSUM.Controls.Add(Me.lProjController)
        Me.TabPageSUM.Controls.Add(Me.lInventory)
        Me.TabPageSUM.Controls.Add(Me.lAcctsReceivable)
        Me.TabPageSUM.Controls.Add(Me.lAcctsPayable)
        Me.TabPageSUM.Controls.Add(Me.lGeneralLedger)
        Me.TabPageSUM.Controls.Add(Me.lCompleted)
        Me.TabPageSUM.Controls.Add(Me.lWarnings)
        Me.TabPageSUM.Controls.Add(Me.lErrors)
        Me.TabPageSUM.Location = New System.Drawing.Point(4, 22)
        Me.TabPageSUM.Name = "TabPageSUM"
        Me.TabPageSUM.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageSUM.Size = New System.Drawing.Size(1180, 369)
        Me.TabPageSUM.TabIndex = 5
        Me.TabPageSUM.Text = "Summary"
        '
        'cErrorsSOVal
        '
        Me.cErrorsSOVal.Enabled = False
        Me.cErrorsSOVal.Location = New System.Drawing.Point(203, 237)
        Me.cErrorsSOVal.Name = "cErrorsSOVal"
        Me.cErrorsSOVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsSOVal.TabIndex = 152
        '
        'cCompletedSOChk
        '
        Me.cCompletedSOChk.AutoSize = True
        Me.cCompletedSOChk.Enabled = False
        Me.cCompletedSOChk.Location = New System.Drawing.Point(386, 237)
        Me.cCompletedSOChk.Name = "cCompletedSOChk"
        Me.cCompletedSOChk.Size = New System.Drawing.Size(15, 14)
        Me.cCompletedSOChk.TabIndex = 151
        Me.cCompletedSOChk.UseVisualStyleBackColor = True
        '
        'cCompletedPAChk
        '
        Me.cCompletedPAChk.AutoSize = True
        Me.cCompletedPAChk.Enabled = False
        Me.cCompletedPAChk.Location = New System.Drawing.Point(386, 202)
        Me.cCompletedPAChk.Name = "cCompletedPAChk"
        Me.cCompletedPAChk.Size = New System.Drawing.Size(15, 14)
        Me.cCompletedPAChk.TabIndex = 150
        Me.cCompletedPAChk.UseVisualStyleBackColor = True
        '
        'cCompletedPOChk
        '
        Me.cCompletedPOChk.AutoSize = True
        Me.cCompletedPOChk.Enabled = False
        Me.cCompletedPOChk.Location = New System.Drawing.Point(386, 167)
        Me.cCompletedPOChk.Name = "cCompletedPOChk"
        Me.cCompletedPOChk.Size = New System.Drawing.Size(15, 14)
        Me.cCompletedPOChk.TabIndex = 149
        Me.cCompletedPOChk.UseVisualStyleBackColor = True
        '
        'cCompletedINChk
        '
        Me.cCompletedINChk.AutoSize = True
        Me.cCompletedINChk.Enabled = False
        Me.cCompletedINChk.Location = New System.Drawing.Point(386, 132)
        Me.cCompletedINChk.Name = "cCompletedINChk"
        Me.cCompletedINChk.Size = New System.Drawing.Size(15, 14)
        Me.cCompletedINChk.TabIndex = 148
        Me.cCompletedINChk.UseVisualStyleBackColor = True
        '
        'cCompletedARChk
        '
        Me.cCompletedARChk.AutoSize = True
        Me.cCompletedARChk.Enabled = False
        Me.cCompletedARChk.Location = New System.Drawing.Point(386, 97)
        Me.cCompletedARChk.Name = "cCompletedARChk"
        Me.cCompletedARChk.Size = New System.Drawing.Size(15, 14)
        Me.cCompletedARChk.TabIndex = 147
        Me.cCompletedARChk.UseVisualStyleBackColor = True
        '
        'cCompletedAPChk
        '
        Me.cCompletedAPChk.AutoSize = True
        Me.cCompletedAPChk.Enabled = False
        Me.cCompletedAPChk.Location = New System.Drawing.Point(386, 62)
        Me.cCompletedAPChk.Name = "cCompletedAPChk"
        Me.cCompletedAPChk.Size = New System.Drawing.Size(15, 14)
        Me.cCompletedAPChk.TabIndex = 146
        Me.cCompletedAPChk.UseVisualStyleBackColor = True
        '
        'cCompletedGLChk
        '
        Me.cCompletedGLChk.AutoSize = True
        Me.cCompletedGLChk.Enabled = False
        Me.cCompletedGLChk.Location = New System.Drawing.Point(386, 27)
        Me.cCompletedGLChk.Name = "cCompletedGLChk"
        Me.cCompletedGLChk.Size = New System.Drawing.Size(15, 14)
        Me.cCompletedGLChk.TabIndex = 145
        Me.cCompletedGLChk.UseVisualStyleBackColor = True
        '
        'cWarningsSOVal
        '
        Me.cWarningsSOVal.Enabled = False
        Me.cWarningsSOVal.Location = New System.Drawing.Point(276, 237)
        Me.cWarningsSOVal.Name = "cWarningsSOVal"
        Me.cWarningsSOVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsSOVal.TabIndex = 144
        '
        'cWarningsGLVal
        '
        Me.cWarningsGLVal.Enabled = False
        Me.cWarningsGLVal.Location = New System.Drawing.Point(276, 27)
        Me.cWarningsGLVal.Name = "cWarningsGLVal"
        Me.cWarningsGLVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsGLVal.TabIndex = 143
        '
        'cErrorsAPVal
        '
        Me.cErrorsAPVal.Enabled = False
        Me.cErrorsAPVal.Location = New System.Drawing.Point(203, 62)
        Me.cErrorsAPVal.Name = "cErrorsAPVal"
        Me.cErrorsAPVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsAPVal.TabIndex = 142
        '
        'cWarningsARVal
        '
        Me.cWarningsARVal.Enabled = False
        Me.cWarningsARVal.Location = New System.Drawing.Point(276, 97)
        Me.cWarningsARVal.Name = "cWarningsARVal"
        Me.cWarningsARVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsARVal.TabIndex = 141
        '
        'cErrorsARVal
        '
        Me.cErrorsARVal.Enabled = False
        Me.cErrorsARVal.Location = New System.Drawing.Point(203, 97)
        Me.cErrorsARVal.Name = "cErrorsARVal"
        Me.cErrorsARVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsARVal.TabIndex = 141
        '
        'cWarningsINVal
        '
        Me.cWarningsINVal.Enabled = False
        Me.cWarningsINVal.Location = New System.Drawing.Point(276, 132)
        Me.cWarningsINVal.Name = "cWarningsINVal"
        Me.cWarningsINVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsINVal.TabIndex = 140
        '
        'cErrorsINVal
        '
        Me.cErrorsINVal.Enabled = False
        Me.cErrorsINVal.Location = New System.Drawing.Point(203, 132)
        Me.cErrorsINVal.Name = "cErrorsINVal"
        Me.cErrorsINVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsINVal.TabIndex = 139
        '
        'cWarningsPOVal
        '
        Me.cWarningsPOVal.Enabled = False
        Me.cWarningsPOVal.Location = New System.Drawing.Point(276, 167)
        Me.cWarningsPOVal.Name = "cWarningsPOVal"
        Me.cWarningsPOVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsPOVal.TabIndex = 138
        '
        'cErrorsPOVal
        '
        Me.cErrorsPOVal.Enabled = False
        Me.cErrorsPOVal.Location = New System.Drawing.Point(203, 167)
        Me.cErrorsPOVal.Name = "cErrorsPOVal"
        Me.cErrorsPOVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsPOVal.TabIndex = 137
        '
        'cWarningsPAVal
        '
        Me.cWarningsPAVal.Enabled = False
        Me.cWarningsPAVal.Location = New System.Drawing.Point(276, 202)
        Me.cWarningsPAVal.Name = "cWarningsPAVal"
        Me.cWarningsPAVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsPAVal.TabIndex = 136
        '
        'cErrorsPAVal
        '
        Me.cErrorsPAVal.Enabled = False
        Me.cErrorsPAVal.Location = New System.Drawing.Point(203, 202)
        Me.cErrorsPAVal.Name = "cErrorsPAVal"
        Me.cErrorsPAVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsPAVal.TabIndex = 135
        '
        'cWarningsAPVal
        '
        Me.cWarningsAPVal.Enabled = False
        Me.cWarningsAPVal.Location = New System.Drawing.Point(276, 62)
        Me.cWarningsAPVal.Name = "cWarningsAPVal"
        Me.cWarningsAPVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsAPVal.TabIndex = 134
        '
        'cErrorsGLVal
        '
        Me.cErrorsGLVal.Enabled = False
        Me.cErrorsGLVal.Location = New System.Drawing.Point(203, 27)
        Me.cErrorsGLVal.Name = "cErrorsGLVal"
        Me.cErrorsGLVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsGLVal.TabIndex = 133
        '
        'lSOTotal
        '
        Me.lSOTotal.AutoSize = True
        Me.lSOTotal.Location = New System.Drawing.Point(567, 237)
        Me.lSOTotal.Name = "lSOTotal"
        Me.lSOTotal.Size = New System.Drawing.Size(99, 13)
        Me.lSOTotal.TabIndex = 132
        Me.lSOTotal.Text = "Total Sales Orders:"
        '
        'lSOActive
        '
        Me.lSOActive.AutoSize = True
        Me.lSOActive.Location = New System.Drawing.Point(922, 237)
        Me.lSOActive.Name = "lSOActive"
        Me.lSOActive.Size = New System.Drawing.Size(41, 13)
        Me.lSOActive.TabIndex = 127
        Me.lSOActive.Text = "Active:"
        '
        'lSalesOrder
        '
        Me.lSalesOrder.AutoSize = True
        Me.lSalesOrder.Location = New System.Drawing.Point(7, 237)
        Me.lSalesOrder.Name = "lSalesOrder"
        Me.lSalesOrder.Size = New System.Drawing.Size(67, 13)
        Me.lSalesOrder.TabIndex = 126
        Me.lSalesOrder.Text = "Sales Order:"
        '
        'lCurrPerHdr
        '
        Me.lCurrPerHdr.AutoSize = True
        Me.lCurrPerHdr.Location = New System.Drawing.Point(439, 0)
        Me.lCurrPerHdr.Name = "lCurrPerHdr"
        Me.lCurrPerHdr.Size = New System.Drawing.Size(81, 13)
        Me.lCurrPerHdr.TabIndex = 125
        Me.lCurrPerHdr.Text = "Current Period:"
        '
        'lCurrPAPeriod
        '
        Me.lCurrPAPeriod.AutoSize = True
        Me.lCurrPAPeriod.Location = New System.Drawing.Point(448, 202)
        Me.lCurrPAPeriod.Name = "lCurrPAPeriod"
        Me.lCurrPAPeriod.Size = New System.Drawing.Size(47, 13)
        Me.lCurrPAPeriod.TabIndex = 124
        Me.lCurrPAPeriod.Text = "00-0000"
        '
        'lCurrINPeriod
        '
        Me.lCurrINPeriod.AutoSize = True
        Me.lCurrINPeriod.Location = New System.Drawing.Point(448, 132)
        Me.lCurrINPeriod.Name = "lCurrINPeriod"
        Me.lCurrINPeriod.Size = New System.Drawing.Size(47, 13)
        Me.lCurrINPeriod.TabIndex = 123
        Me.lCurrINPeriod.Text = "00-0000"
        '
        'lCurrARPeriod
        '
        Me.lCurrARPeriod.AutoSize = True
        Me.lCurrARPeriod.Location = New System.Drawing.Point(448, 97)
        Me.lCurrARPeriod.Name = "lCurrARPeriod"
        Me.lCurrARPeriod.Size = New System.Drawing.Size(47, 13)
        Me.lCurrARPeriod.TabIndex = 122
        Me.lCurrARPeriod.Text = "00-0000"
        '
        'lCurrAPPeriod
        '
        Me.lCurrAPPeriod.AutoSize = True
        Me.lCurrAPPeriod.Location = New System.Drawing.Point(448, 62)
        Me.lCurrAPPeriod.Name = "lCurrAPPeriod"
        Me.lCurrAPPeriod.Size = New System.Drawing.Size(47, 13)
        Me.lCurrAPPeriod.TabIndex = 121
        Me.lCurrAPPeriod.Text = "00-0000"
        '
        'lGLPeriod
        '
        Me.lGLPeriod.AutoSize = True
        Me.lGLPeriod.Location = New System.Drawing.Point(448, 27)
        Me.lGLPeriod.Name = "lGLPeriod"
        Me.lGLPeriod.Size = New System.Drawing.Size(47, 13)
        Me.lGLPeriod.TabIndex = 120
        Me.lGLPeriod.Text = "00-0000"
        '
        'lOrderActive
        '
        Me.lOrderActive.AutoSize = True
        Me.lOrderActive.Location = New System.Drawing.Point(922, 167)
        Me.lOrderActive.Name = "lOrderActive"
        Me.lOrderActive.Size = New System.Drawing.Size(41, 13)
        Me.lOrderActive.TabIndex = 119
        Me.lOrderActive.Text = "Active:"
        '
        'lOrderTotal
        '
        Me.lOrderTotal.AutoSize = True
        Me.lOrderTotal.Location = New System.Drawing.Point(567, 167)
        Me.lOrderTotal.Name = "lOrderTotal"
        Me.lOrderTotal.Size = New System.Drawing.Size(71, 13)
        Me.lOrderTotal.TabIndex = 118
        Me.lOrderTotal.Text = "Total Orders:"
        '
        'lInvtActive
        '
        Me.lInvtActive.AutoSize = True
        Me.lInvtActive.Location = New System.Drawing.Point(922, 132)
        Me.lInvtActive.Name = "lInvtActive"
        Me.lInvtActive.Size = New System.Drawing.Size(41, 13)
        Me.lInvtActive.TabIndex = 117
        Me.lInvtActive.Text = "Active:"
        '
        'lProjActive
        '
        Me.lProjActive.AutoSize = True
        Me.lProjActive.Location = New System.Drawing.Point(922, 202)
        Me.lProjActive.Name = "lProjActive"
        Me.lProjActive.Size = New System.Drawing.Size(41, 13)
        Me.lProjActive.TabIndex = 116
        Me.lProjActive.Text = "Active:"
        '
        'lCustActive
        '
        Me.lCustActive.AutoSize = True
        Me.lCustActive.Location = New System.Drawing.Point(922, 97)
        Me.lCustActive.Name = "lCustActive"
        Me.lCustActive.Size = New System.Drawing.Size(41, 13)
        Me.lCustActive.TabIndex = 115
        Me.lCustActive.Text = "Active:"
        '
        'lVendActive
        '
        Me.lVendActive.AutoSize = True
        Me.lVendActive.Location = New System.Drawing.Point(922, 62)
        Me.lVendActive.Name = "lVendActive"
        Me.lVendActive.Size = New System.Drawing.Size(41, 13)
        Me.lVendActive.TabIndex = 114
        Me.lVendActive.Text = "Active:"
        '
        'lAcctActive
        '
        Me.lAcctActive.AutoSize = True
        Me.lAcctActive.Location = New System.Drawing.Point(922, 27)
        Me.lAcctActive.Name = "lAcctActive"
        Me.lAcctActive.Size = New System.Drawing.Size(41, 13)
        Me.lAcctActive.TabIndex = 113
        Me.lAcctActive.Text = "Active:"
        '
        'lProjTotal
        '
        Me.lProjTotal.AutoSize = True
        Me.lProjTotal.Location = New System.Drawing.Point(567, 202)
        Me.lProjTotal.Name = "lProjTotal"
        Me.lProjTotal.Size = New System.Drawing.Size(77, 13)
        Me.lProjTotal.TabIndex = 112
        Me.lProjTotal.Text = "Total Projects:"
        '
        'lItemTotal
        '
        Me.lItemTotal.AutoSize = True
        Me.lItemTotal.Location = New System.Drawing.Point(567, 132)
        Me.lItemTotal.Name = "lItemTotal"
        Me.lItemTotal.Size = New System.Drawing.Size(65, 13)
        Me.lItemTotal.TabIndex = 111
        Me.lItemTotal.Text = "Total Items:"
        '
        'lCustTotal
        '
        Me.lCustTotal.AutoSize = True
        Me.lCustTotal.Location = New System.Drawing.Point(567, 97)
        Me.lCustTotal.Name = "lCustTotal"
        Me.lCustTotal.Size = New System.Drawing.Size(89, 13)
        Me.lCustTotal.TabIndex = 110
        Me.lCustTotal.Text = "Total Customers:"
        '
        'lVendTotal
        '
        Me.lVendTotal.AutoSize = True
        Me.lVendTotal.Location = New System.Drawing.Point(567, 62)
        Me.lVendTotal.Name = "lVendTotal"
        Me.lVendTotal.Size = New System.Drawing.Size(77, 13)
        Me.lVendTotal.TabIndex = 109
        Me.lVendTotal.Text = "Total Vendors:"
        '
        'lAcctTotal
        '
        Me.lAcctTotal.AutoSize = True
        Me.lAcctTotal.Location = New System.Drawing.Point(567, 27)
        Me.lAcctTotal.Name = "lAcctTotal"
        Me.lAcctTotal.Size = New System.Drawing.Size(82, 13)
        Me.lAcctTotal.TabIndex = 108
        Me.lAcctTotal.Text = "Total Accounts:"
        '
        'lPurchasing
        '
        Me.lPurchasing.AutoSize = True
        Me.lPurchasing.Location = New System.Drawing.Point(7, 167)
        Me.lPurchasing.Name = "lPurchasing"
        Me.lPurchasing.Size = New System.Drawing.Size(63, 13)
        Me.lPurchasing.TabIndex = 100
        Me.lPurchasing.Text = "Purchasing:"
        '
        'lProjController
        '
        Me.lProjController.AutoSize = True
        Me.lProjController.Location = New System.Drawing.Point(7, 202)
        Me.lProjController.Name = "lProjController"
        Me.lProjController.Size = New System.Drawing.Size(95, 13)
        Me.lProjController.TabIndex = 104
        Me.lProjController.Text = "Project Controller:"
        '
        'lInventory
        '
        Me.lInventory.AutoSize = True
        Me.lInventory.Location = New System.Drawing.Point(7, 132)
        Me.lInventory.Name = "lInventory"
        Me.lInventory.Size = New System.Drawing.Size(59, 13)
        Me.lInventory.TabIndex = 96
        Me.lInventory.Text = "Inventory:"
        '
        'lAcctsReceivable
        '
        Me.lAcctsReceivable.AutoSize = True
        Me.lAcctsReceivable.Location = New System.Drawing.Point(6, 97)
        Me.lAcctsReceivable.Name = "lAcctsReceivable"
        Me.lAcctsReceivable.Size = New System.Drawing.Size(110, 13)
        Me.lAcctsReceivable.TabIndex = 92
        Me.lAcctsReceivable.Text = "Accounts Receivable:"
        '
        'lAcctsPayable
        '
        Me.lAcctsPayable.AutoSize = True
        Me.lAcctsPayable.Location = New System.Drawing.Point(6, 62)
        Me.lAcctsPayable.Name = "lAcctsPayable"
        Me.lAcctsPayable.Size = New System.Drawing.Size(96, 13)
        Me.lAcctsPayable.TabIndex = 88
        Me.lAcctsPayable.Text = "Accounts Payable:"
        '
        'lGeneralLedger
        '
        Me.lGeneralLedger.AutoSize = True
        Me.lGeneralLedger.Location = New System.Drawing.Point(7, 27)
        Me.lGeneralLedger.Name = "lGeneralLedger"
        Me.lGeneralLedger.Size = New System.Drawing.Size(84, 13)
        Me.lGeneralLedger.TabIndex = 84
        Me.lGeneralLedger.Text = "General Ledger:"
        '
        'lCompleted
        '
        Me.lCompleted.AutoSize = True
        Me.lCompleted.Location = New System.Drawing.Point(338, 0)
        Me.lCompleted.Name = "lCompleted"
        Me.lCompleted.Size = New System.Drawing.Size(62, 13)
        Me.lCompleted.TabIndex = 83
        Me.lCompleted.Text = "Completed:"
        '
        'lWarnings
        '
        Me.lWarnings.AutoSize = True
        Me.lWarnings.Location = New System.Drawing.Point(252, 3)
        Me.lWarnings.Name = "lWarnings"
        Me.lWarnings.Size = New System.Drawing.Size(56, 13)
        Me.lWarnings.TabIndex = 82
        Me.lWarnings.Text = "Warnings:"
        '
        'lErrors
        '
        Me.lErrors.AutoSize = True
        Me.lErrors.Location = New System.Drawing.Point(189, 3)
        Me.lErrors.Name = "lErrors"
        Me.lErrors.Size = New System.Drawing.Size(40, 13)
        Me.lErrors.TabIndex = 81
        Me.lErrors.Text = "Errors:"
        '
        'TabPagePA
        '
        Me.TabPagePA.BackColor = System.Drawing.SystemColors.Control
        Me.TabPagePA.Controls.Add(Me.GroupBox6)
        Me.TabPagePA.Location = New System.Drawing.Point(4, 22)
        Me.TabPagePA.Name = "TabPagePA"
        Me.TabPagePA.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPagePA.Size = New System.Drawing.Size(1180, 369)
        Me.TabPagePA.TabIndex = 4
        Me.TabPagePA.Text = "Project Controller"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.cDateValidatedProj)
        Me.GroupBox6.Controls.Add(Me.cErrorsProj)
        Me.GroupBox6.Controls.Add(Me.cWarningsProj)
        Me.GroupBox6.Controls.Add(Me.cValCompletedProj)
        Me.GroupBox6.Controls.Add(Me.cmdProjEventLog)
        Me.GroupBox6.Controls.Add(Me.lCurrPAPeriodDisp)
        Me.GroupBox6.Controls.Add(Me.lCurrPAPeriodLabel)
        Me.GroupBox6.Controls.Add(Me.lWarningsProj)
        Me.GroupBox6.Controls.Add(Me.lErrorsProj)
        Me.GroupBox6.Controls.Add(Me.lDateValidatedProj)
        Me.GroupBox6.Controls.Add(Me.cmdValidateProj)
        Me.GroupBox6.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(1178, 359)
        Me.GroupBox6.TabIndex = 65
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Projects"
        '
        'cDateValidatedProj
        '
        Me.cDateValidatedProj.Enabled = False
        Me.cDateValidatedProj.Location = New System.Drawing.Point(205, 58)
        Me.cDateValidatedProj.Name = "cDateValidatedProj"
        Me.cDateValidatedProj.Size = New System.Drawing.Size(100, 21)
        Me.cDateValidatedProj.TabIndex = 87
        '
        'cErrorsProj
        '
        Me.cErrorsProj.Enabled = False
        Me.cErrorsProj.Location = New System.Drawing.Point(400, 58)
        Me.cErrorsProj.Name = "cErrorsProj"
        Me.cErrorsProj.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsProj.TabIndex = 86
        '
        'cWarningsProj
        '
        Me.cWarningsProj.Enabled = False
        Me.cWarningsProj.Location = New System.Drawing.Point(525, 58)
        Me.cWarningsProj.Name = "cWarningsProj"
        Me.cWarningsProj.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsProj.TabIndex = 85
        '
        'cValCompletedProj
        '
        Me.cValCompletedProj.AutoSize = True
        Me.cValCompletedProj.Enabled = False
        Me.cValCompletedProj.Location = New System.Drawing.Point(595, 58)
        Me.cValCompletedProj.Name = "cValCompletedProj"
        Me.cValCompletedProj.Size = New System.Drawing.Size(77, 17)
        Me.cValCompletedProj.TabIndex = 84
        Me.cValCompletedProj.Text = "Completed"
        Me.cValCompletedProj.UseVisualStyleBackColor = True
        '
        'cmdProjEventLog
        '
        Me.cmdProjEventLog.Location = New System.Drawing.Point(14, 162)
        Me.cmdProjEventLog.Name = "cmdProjEventLog"
        Me.cmdProjEventLog.Size = New System.Drawing.Size(104, 23)
        Me.cmdProjEventLog.TabIndex = 83
        Me.cmdProjEventLog.Text = "Open Event Log"
        Me.cmdProjEventLog.UseVisualStyleBackColor = True
        '
        'lCurrPAPeriodDisp
        '
        Me.lCurrPAPeriodDisp.AutoSize = True
        Me.lCurrPAPeriodDisp.Location = New System.Drawing.Point(143, 116)
        Me.lCurrPAPeriodDisp.Name = "lCurrPAPeriodDisp"
        Me.lCurrPAPeriodDisp.Size = New System.Drawing.Size(47, 13)
        Me.lCurrPAPeriodDisp.TabIndex = 82
        Me.lCurrPAPeriodDisp.Text = "00-0000"
        '
        'lCurrPAPeriodLabel
        '
        Me.lCurrPAPeriodLabel.AutoSize = True
        Me.lCurrPAPeriodLabel.Location = New System.Drawing.Point(14, 116)
        Me.lCurrPAPeriodLabel.Name = "lCurrPAPeriodLabel"
        Me.lCurrPAPeriodLabel.Size = New System.Drawing.Size(81, 13)
        Me.lCurrPAPeriodLabel.TabIndex = 81
        Me.lCurrPAPeriodLabel.Text = "Current Period:"
        '
        'lWarningsProj
        '
        Me.lWarningsProj.AutoSize = True
        Me.lWarningsProj.Location = New System.Drawing.Point(450, 58)
        Me.lWarningsProj.Name = "lWarningsProj"
        Me.lWarningsProj.Size = New System.Drawing.Size(56, 13)
        Me.lWarningsProj.TabIndex = 70
        Me.lWarningsProj.Text = "Warnings:"
        '
        'lErrorsProj
        '
        Me.lErrorsProj.AutoSize = True
        Me.lErrorsProj.Location = New System.Drawing.Point(350, 58)
        Me.lErrorsProj.Name = "lErrorsProj"
        Me.lErrorsProj.Size = New System.Drawing.Size(40, 13)
        Me.lErrorsProj.TabIndex = 68
        Me.lErrorsProj.Text = "Errors:"
        '
        'lDateValidatedProj
        '
        Me.lDateValidatedProj.AutoSize = True
        Me.lDateValidatedProj.Location = New System.Drawing.Point(121, 58)
        Me.lDateValidatedProj.Name = "lDateValidatedProj"
        Me.lDateValidatedProj.Size = New System.Drawing.Size(78, 13)
        Me.lDateValidatedProj.TabIndex = 66
        Me.lDateValidatedProj.Text = "Last Validated:"
        '
        'cmdValidateProj
        '
        Me.cmdValidateProj.Location = New System.Drawing.Point(7, 58)
        Me.cmdValidateProj.Name = "cmdValidateProj"
        Me.cmdValidateProj.Size = New System.Drawing.Size(90, 21)
        Me.cmdValidateProj.TabIndex = 65
        Me.cmdValidateProj.Text = "Validate"
        Me.cmdValidateProj.UseVisualStyleBackColor = True
        '
        'TabPageSO
        '
        Me.TabPageSO.BackColor = System.Drawing.SystemColors.Control
        Me.TabPageSO.Controls.Add(Me.GroupBox4)
        Me.TabPageSO.Location = New System.Drawing.Point(4, 22)
        Me.TabPageSO.Name = "TabPageSO"
        Me.TabPageSO.Size = New System.Drawing.Size(1180, 369)
        Me.TabPageSO.TabIndex = 7
        Me.TabPageSO.Text = "Sales Order"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.cDateValidatedSO)
        Me.GroupBox4.Controls.Add(Me.cErrorsSO)
        Me.GroupBox4.Controls.Add(Me.cWarningsSO)
        Me.GroupBox4.Controls.Add(Me.cValCompletedSO)
        Me.GroupBox4.Controls.Add(Me.cmdSOEventLog)
        Me.GroupBox4.Controls.Add(Me.cCurrSOPeriodDisp)
        Me.GroupBox4.Controls.Add(Me.lCurrSOPeriodLabel)
        Me.GroupBox4.Controls.Add(Me.lWarningSO)
        Me.GroupBox4.Controls.Add(Me.lErrorsSO)
        Me.GroupBox4.Controls.Add(Me.lValidatedSO)
        Me.GroupBox4.Controls.Add(Me.cmdValidateSalesOrder)
        Me.GroupBox4.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(1174, 355)
        Me.GroupBox4.TabIndex = 66
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Sales Orders"
        '
        'cDateValidatedSO
        '
        Me.cDateValidatedSO.Enabled = False
        Me.cDateValidatedSO.Location = New System.Drawing.Point(205, 58)
        Me.cDateValidatedSO.Name = "cDateValidatedSO"
        Me.cDateValidatedSO.Size = New System.Drawing.Size(100, 21)
        Me.cDateValidatedSO.TabIndex = 87
        '
        'cErrorsSO
        '
        Me.cErrorsSO.Enabled = False
        Me.cErrorsSO.Location = New System.Drawing.Point(400, 58)
        Me.cErrorsSO.Name = "cErrorsSO"
        Me.cErrorsSO.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsSO.TabIndex = 86
        '
        'cWarningsSO
        '
        Me.cWarningsSO.Enabled = False
        Me.cWarningsSO.Location = New System.Drawing.Point(525, 58)
        Me.cWarningsSO.Name = "cWarningsSO"
        Me.cWarningsSO.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsSO.TabIndex = 85
        '
        'cValCompletedSO
        '
        Me.cValCompletedSO.AutoSize = True
        Me.cValCompletedSO.Enabled = False
        Me.cValCompletedSO.Location = New System.Drawing.Point(595, 58)
        Me.cValCompletedSO.Name = "cValCompletedSO"
        Me.cValCompletedSO.Size = New System.Drawing.Size(77, 17)
        Me.cValCompletedSO.TabIndex = 84
        Me.cValCompletedSO.Text = "Completed"
        Me.cValCompletedSO.UseVisualStyleBackColor = True
        '
        'cmdSOEventLog
        '
        Me.cmdSOEventLog.Location = New System.Drawing.Point(14, 162)
        Me.cmdSOEventLog.Name = "cmdSOEventLog"
        Me.cmdSOEventLog.Size = New System.Drawing.Size(104, 23)
        Me.cmdSOEventLog.TabIndex = 83
        Me.cmdSOEventLog.Text = "Open Event Log"
        Me.cmdSOEventLog.UseVisualStyleBackColor = True
        '
        'cCurrSOPeriodDisp
        '
        Me.cCurrSOPeriodDisp.AutoSize = True
        Me.cCurrSOPeriodDisp.Location = New System.Drawing.Point(143, 116)
        Me.cCurrSOPeriodDisp.Name = "cCurrSOPeriodDisp"
        Me.cCurrSOPeriodDisp.Size = New System.Drawing.Size(47, 13)
        Me.cCurrSOPeriodDisp.TabIndex = 82
        Me.cCurrSOPeriodDisp.Text = "00-0000"
        '
        'lCurrSOPeriodLabel
        '
        Me.lCurrSOPeriodLabel.AutoSize = True
        Me.lCurrSOPeriodLabel.Location = New System.Drawing.Point(14, 116)
        Me.lCurrSOPeriodLabel.Name = "lCurrSOPeriodLabel"
        Me.lCurrSOPeriodLabel.Size = New System.Drawing.Size(81, 13)
        Me.lCurrSOPeriodLabel.TabIndex = 81
        Me.lCurrSOPeriodLabel.Text = "Current Period:"
        '
        'lWarningSO
        '
        Me.lWarningSO.AutoSize = True
        Me.lWarningSO.Location = New System.Drawing.Point(450, 58)
        Me.lWarningSO.Name = "lWarningSO"
        Me.lWarningSO.Size = New System.Drawing.Size(56, 13)
        Me.lWarningSO.TabIndex = 70
        Me.lWarningSO.Text = "Warnings:"
        '
        'lErrorsSO
        '
        Me.lErrorsSO.AutoSize = True
        Me.lErrorsSO.Location = New System.Drawing.Point(350, 58)
        Me.lErrorsSO.Name = "lErrorsSO"
        Me.lErrorsSO.Size = New System.Drawing.Size(40, 13)
        Me.lErrorsSO.TabIndex = 68
        Me.lErrorsSO.Text = "Errors:"
        '
        'lValidatedSO
        '
        Me.lValidatedSO.AutoSize = True
        Me.lValidatedSO.Location = New System.Drawing.Point(121, 58)
        Me.lValidatedSO.Name = "lValidatedSO"
        Me.lValidatedSO.Size = New System.Drawing.Size(78, 13)
        Me.lValidatedSO.TabIndex = 66
        Me.lValidatedSO.Text = "Last Validated:"
        '
        'cmdValidateSalesOrder
        '
        Me.cmdValidateSalesOrder.Location = New System.Drawing.Point(7, 58)
        Me.cmdValidateSalesOrder.Name = "cmdValidateSalesOrder"
        Me.cmdValidateSalesOrder.Size = New System.Drawing.Size(87, 23)
        Me.cmdValidateSalesOrder.TabIndex = 65
        Me.cmdValidateSalesOrder.Text = "Validate"
        Me.cmdValidateSalesOrder.UseVisualStyleBackColor = True
        '
        'TabPageIN
        '
        Me.TabPageIN.BackColor = System.Drawing.SystemColors.Control
        Me.TabPageIN.Controls.Add(Me.GroupBox5)
        Me.TabPageIN.Location = New System.Drawing.Point(4, 22)
        Me.TabPageIN.Name = "TabPageIN"
        Me.TabPageIN.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageIN.Size = New System.Drawing.Size(1180, 369)
        Me.TabPageIN.TabIndex = 3
        Me.TabPageIN.Text = "Inventory"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.cDateValidatedItem)
        Me.GroupBox5.Controls.Add(Me.cErrorsItem)
        Me.GroupBox5.Controls.Add(Me.cWarningsItem)
        Me.GroupBox5.Controls.Add(Me.cValCompletedItem)
        Me.GroupBox5.Controls.Add(Me.cmdIntEventLog)
        Me.GroupBox5.Controls.Add(Me.lCurrINPeriodDisp)
        Me.GroupBox5.Controls.Add(Me.lCurrINPeriodLabel)
        Me.GroupBox5.Controls.Add(Me.lWarningsItem)
        Me.GroupBox5.Controls.Add(Me.lErrorsItem)
        Me.GroupBox5.Controls.Add(Me.lDateValidatedItem)
        Me.GroupBox5.Controls.Add(Me.cmdValidateItem)
        Me.GroupBox5.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(1168, 359)
        Me.GroupBox5.TabIndex = 46
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Items"
        '
        'cDateValidatedItem
        '
        Me.cDateValidatedItem.Enabled = False
        Me.cDateValidatedItem.Location = New System.Drawing.Point(205, 58)
        Me.cDateValidatedItem.Name = "cDateValidatedItem"
        Me.cDateValidatedItem.Size = New System.Drawing.Size(100, 21)
        Me.cDateValidatedItem.TabIndex = 88
        '
        'cErrorsItem
        '
        Me.cErrorsItem.Enabled = False
        Me.cErrorsItem.Location = New System.Drawing.Point(400, 58)
        Me.cErrorsItem.Name = "cErrorsItem"
        Me.cErrorsItem.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsItem.TabIndex = 87
        '
        'cWarningsItem
        '
        Me.cWarningsItem.Enabled = False
        Me.cWarningsItem.Location = New System.Drawing.Point(525, 58)
        Me.cWarningsItem.Name = "cWarningsItem"
        Me.cWarningsItem.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsItem.TabIndex = 86
        '
        'cValCompletedItem
        '
        Me.cValCompletedItem.AutoSize = True
        Me.cValCompletedItem.Enabled = False
        Me.cValCompletedItem.Location = New System.Drawing.Point(595, 58)
        Me.cValCompletedItem.Name = "cValCompletedItem"
        Me.cValCompletedItem.Size = New System.Drawing.Size(77, 17)
        Me.cValCompletedItem.TabIndex = 85
        Me.cValCompletedItem.Text = "Completed"
        Me.cValCompletedItem.UseVisualStyleBackColor = True
        '
        'cmdIntEventLog
        '
        Me.cmdIntEventLog.Location = New System.Drawing.Point(14, 162)
        Me.cmdIntEventLog.Name = "cmdIntEventLog"
        Me.cmdIntEventLog.Size = New System.Drawing.Size(104, 23)
        Me.cmdIntEventLog.TabIndex = 84
        Me.cmdIntEventLog.Text = "Open Event Log"
        Me.cmdIntEventLog.UseVisualStyleBackColor = True
        '
        'lCurrINPeriodDisp
        '
        Me.lCurrINPeriodDisp.AutoSize = True
        Me.lCurrINPeriodDisp.Location = New System.Drawing.Point(143, 116)
        Me.lCurrINPeriodDisp.Name = "lCurrINPeriodDisp"
        Me.lCurrINPeriodDisp.Size = New System.Drawing.Size(47, 13)
        Me.lCurrINPeriodDisp.TabIndex = 55
        Me.lCurrINPeriodDisp.Text = "00-0000"
        '
        'lCurrINPeriodLabel
        '
        Me.lCurrINPeriodLabel.AutoSize = True
        Me.lCurrINPeriodLabel.Location = New System.Drawing.Point(14, 116)
        Me.lCurrINPeriodLabel.Name = "lCurrINPeriodLabel"
        Me.lCurrINPeriodLabel.Size = New System.Drawing.Size(81, 13)
        Me.lCurrINPeriodLabel.TabIndex = 54
        Me.lCurrINPeriodLabel.Text = "Current Period:"
        '
        'lWarningsItem
        '
        Me.lWarningsItem.AutoSize = True
        Me.lWarningsItem.Location = New System.Drawing.Point(450, 58)
        Me.lWarningsItem.Name = "lWarningsItem"
        Me.lWarningsItem.Size = New System.Drawing.Size(56, 13)
        Me.lWarningsItem.TabIndex = 51
        Me.lWarningsItem.Text = "Warnings:"
        '
        'lErrorsItem
        '
        Me.lErrorsItem.AutoSize = True
        Me.lErrorsItem.Location = New System.Drawing.Point(350, 58)
        Me.lErrorsItem.Name = "lErrorsItem"
        Me.lErrorsItem.Size = New System.Drawing.Size(40, 13)
        Me.lErrorsItem.TabIndex = 49
        Me.lErrorsItem.Text = "Errors:"
        '
        'lDateValidatedItem
        '
        Me.lDateValidatedItem.AutoSize = True
        Me.lDateValidatedItem.Location = New System.Drawing.Point(121, 58)
        Me.lDateValidatedItem.Name = "lDateValidatedItem"
        Me.lDateValidatedItem.Size = New System.Drawing.Size(78, 13)
        Me.lDateValidatedItem.TabIndex = 47
        Me.lDateValidatedItem.Text = "Last Validated:"
        '
        'cmdValidateItem
        '
        Me.cmdValidateItem.Location = New System.Drawing.Point(7, 58)
        Me.cmdValidateItem.Name = "cmdValidateItem"
        Me.cmdValidateItem.Size = New System.Drawing.Size(87, 23)
        Me.cmdValidateItem.TabIndex = 46
        Me.cmdValidateItem.Text = "Validate"
        Me.cmdValidateItem.UseVisualStyleBackColor = True
        '
        'TabPageAP
        '
        Me.TabPageAP.BackColor = System.Drawing.SystemColors.Control
        Me.TabPageAP.Controls.Add(Me.GroupBox3)
        Me.TabPageAP.Location = New System.Drawing.Point(4, 22)
        Me.TabPageAP.Name = "TabPageAP"
        Me.TabPageAP.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageAP.Size = New System.Drawing.Size(1180, 369)
        Me.TabPageAP.TabIndex = 1
        Me.TabPageAP.Text = "Accounts Payable"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cDateValidatedVend)
        Me.GroupBox3.Controls.Add(Me.cErrorsVend)
        Me.GroupBox3.Controls.Add(Me.cWarningsVend)
        Me.GroupBox3.Controls.Add(Me.cValCompletedVend)
        Me.GroupBox3.Controls.Add(Me.cmdAPEventLog)
        Me.GroupBox3.Controls.Add(Me.lCurrAPPeriodDisp)
        Me.GroupBox3.Controls.Add(Me.lblAPPeriod)
        Me.GroupBox3.Controls.Add(Me.lWarningsVend)
        Me.GroupBox3.Controls.Add(Me.lErrorsVend)
        Me.GroupBox3.Controls.Add(Me.lDateValidatedVend)
        Me.GroupBox3.Controls.Add(Me.cmdValidateVend)
        Me.GroupBox3.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(1168, 349)
        Me.GroupBox3.TabIndex = 35
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Vendors"
        '
        'cDateValidatedVend
        '
        Me.cDateValidatedVend.Enabled = False
        Me.cDateValidatedVend.Location = New System.Drawing.Point(205, 58)
        Me.cDateValidatedVend.Name = "cDateValidatedVend"
        Me.cDateValidatedVend.Size = New System.Drawing.Size(100, 21)
        Me.cDateValidatedVend.TabIndex = 90
        '
        'cErrorsVend
        '
        Me.cErrorsVend.Enabled = False
        Me.cErrorsVend.Location = New System.Drawing.Point(400, 58)
        Me.cErrorsVend.Name = "cErrorsVend"
        Me.cErrorsVend.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsVend.TabIndex = 89
        '
        'cWarningsVend
        '
        Me.cWarningsVend.Enabled = False
        Me.cWarningsVend.Location = New System.Drawing.Point(525, 58)
        Me.cWarningsVend.Name = "cWarningsVend"
        Me.cWarningsVend.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsVend.TabIndex = 88
        '
        'cValCompletedVend
        '
        Me.cValCompletedVend.AutoSize = True
        Me.cValCompletedVend.Enabled = False
        Me.cValCompletedVend.Location = New System.Drawing.Point(595, 58)
        Me.cValCompletedVend.Name = "cValCompletedVend"
        Me.cValCompletedVend.Size = New System.Drawing.Size(77, 17)
        Me.cValCompletedVend.TabIndex = 87
        Me.cValCompletedVend.Text = "Completed"
        Me.cValCompletedVend.UseVisualStyleBackColor = True
        '
        'cmdAPEventLog
        '
        Me.cmdAPEventLog.Location = New System.Drawing.Point(14, 162)
        Me.cmdAPEventLog.Name = "cmdAPEventLog"
        Me.cmdAPEventLog.Size = New System.Drawing.Size(104, 23)
        Me.cmdAPEventLog.TabIndex = 86
        Me.cmdAPEventLog.Text = "Open Event Log"
        Me.cmdAPEventLog.UseVisualStyleBackColor = True
        '
        'lCurrAPPeriodDisp
        '
        Me.lCurrAPPeriodDisp.AutoSize = True
        Me.lCurrAPPeriodDisp.Location = New System.Drawing.Point(143, 116)
        Me.lCurrAPPeriodDisp.Name = "lCurrAPPeriodDisp"
        Me.lCurrAPPeriodDisp.Size = New System.Drawing.Size(47, 13)
        Me.lCurrAPPeriodDisp.TabIndex = 46
        Me.lCurrAPPeriodDisp.Text = "00-0000"
        '
        'lblAPPeriod
        '
        Me.lblAPPeriod.AutoSize = True
        Me.lblAPPeriod.Location = New System.Drawing.Point(14, 116)
        Me.lblAPPeriod.Name = "lblAPPeriod"
        Me.lblAPPeriod.Size = New System.Drawing.Size(81, 13)
        Me.lblAPPeriod.TabIndex = 45
        Me.lblAPPeriod.Text = "Current Period:"
        '
        'lWarningsVend
        '
        Me.lWarningsVend.AutoSize = True
        Me.lWarningsVend.Location = New System.Drawing.Point(450, 58)
        Me.lWarningsVend.Name = "lWarningsVend"
        Me.lWarningsVend.Size = New System.Drawing.Size(56, 13)
        Me.lWarningsVend.TabIndex = 40
        Me.lWarningsVend.Text = "Warnings:"
        '
        'lErrorsVend
        '
        Me.lErrorsVend.AutoSize = True
        Me.lErrorsVend.Location = New System.Drawing.Point(350, 58)
        Me.lErrorsVend.Name = "lErrorsVend"
        Me.lErrorsVend.Size = New System.Drawing.Size(40, 13)
        Me.lErrorsVend.TabIndex = 38
        Me.lErrorsVend.Text = "Errors:"
        '
        'lDateValidatedVend
        '
        Me.lDateValidatedVend.AutoSize = True
        Me.lDateValidatedVend.Location = New System.Drawing.Point(121, 58)
        Me.lDateValidatedVend.Name = "lDateValidatedVend"
        Me.lDateValidatedVend.Size = New System.Drawing.Size(78, 13)
        Me.lDateValidatedVend.TabIndex = 36
        Me.lDateValidatedVend.Text = "Last Validated:"
        '
        'cmdValidateVend
        '
        Me.cmdValidateVend.Location = New System.Drawing.Point(7, 58)
        Me.cmdValidateVend.Name = "cmdValidateVend"
        Me.cmdValidateVend.Size = New System.Drawing.Size(87, 23)
        Me.cmdValidateVend.TabIndex = 35
        Me.cmdValidateVend.Text = "Validate"
        Me.cmdValidateVend.UseVisualStyleBackColor = True
        '
        'TabPagePO
        '
        Me.TabPagePO.BackColor = System.Drawing.SystemColors.Control
        Me.TabPagePO.Controls.Add(Me.GroupBox7)
        Me.TabPagePO.Location = New System.Drawing.Point(4, 22)
        Me.TabPagePO.Name = "TabPagePO"
        Me.TabPagePO.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPagePO.Size = New System.Drawing.Size(1180, 369)
        Me.TabPagePO.TabIndex = 6
        Me.TabPagePO.Text = "Purchasing"
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.cDateValidatedPurch)
        Me.GroupBox7.Controls.Add(Me.cErrorsPurch)
        Me.GroupBox7.Controls.Add(Me.cWarningsPurch)
        Me.GroupBox7.Controls.Add(Me.cValCompletedPurch)
        Me.GroupBox7.Controls.Add(Me.cmdPOEventLog)
        Me.GroupBox7.Controls.Add(Me.lPONote)
        Me.GroupBox7.Controls.Add(Me.lWarningsPurch)
        Me.GroupBox7.Controls.Add(Me.lErrorsPurch)
        Me.GroupBox7.Controls.Add(Me.lDateValidatedPurch)
        Me.GroupBox7.Controls.Add(Me.cmdValidatePurch)
        Me.GroupBox7.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(1168, 349)
        Me.GroupBox7.TabIndex = 54
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Purchase Orders"
        '
        'cDateValidatedPurch
        '
        Me.cDateValidatedPurch.Enabled = False
        Me.cDateValidatedPurch.Location = New System.Drawing.Point(205, 58)
        Me.cDateValidatedPurch.Name = "cDateValidatedPurch"
        Me.cDateValidatedPurch.Size = New System.Drawing.Size(100, 21)
        Me.cDateValidatedPurch.TabIndex = 89
        '
        'cErrorsPurch
        '
        Me.cErrorsPurch.Enabled = False
        Me.cErrorsPurch.Location = New System.Drawing.Point(400, 58)
        Me.cErrorsPurch.Name = "cErrorsPurch"
        Me.cErrorsPurch.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsPurch.TabIndex = 88
        '
        'cWarningsPurch
        '
        Me.cWarningsPurch.Enabled = False
        Me.cWarningsPurch.Location = New System.Drawing.Point(525, 58)
        Me.cWarningsPurch.Name = "cWarningsPurch"
        Me.cWarningsPurch.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsPurch.TabIndex = 87
        '
        'cValCompletedPurch
        '
        Me.cValCompletedPurch.AutoSize = True
        Me.cValCompletedPurch.Enabled = False
        Me.cValCompletedPurch.Location = New System.Drawing.Point(595, 58)
        Me.cValCompletedPurch.Name = "cValCompletedPurch"
        Me.cValCompletedPurch.Size = New System.Drawing.Size(77, 17)
        Me.cValCompletedPurch.TabIndex = 86
        Me.cValCompletedPurch.Text = "Completed"
        Me.cValCompletedPurch.UseVisualStyleBackColor = True
        '
        'cmdPOEventLog
        '
        Me.cmdPOEventLog.Location = New System.Drawing.Point(14, 162)
        Me.cmdPOEventLog.Name = "cmdPOEventLog"
        Me.cmdPOEventLog.Size = New System.Drawing.Size(104, 23)
        Me.cmdPOEventLog.TabIndex = 85
        Me.cmdPOEventLog.Text = "Open Event Log"
        Me.cmdPOEventLog.UseVisualStyleBackColor = True
        '
        'lPONote
        '
        Me.lPONote.AutoSize = True
        Me.lPONote.Location = New System.Drawing.Point(4, 112)
        Me.lPONote.Name = "lPONote"
        Me.lPONote.Size = New System.Drawing.Size(556, 13)
        Me.lPONote.TabIndex = 65
        Me.lPONote.Text = "NOTE: Only POs with a PO Type of Regular Order and a Status of Purchase Order or " &
    "Open Order will be validated."
        '
        'lWarningsPurch
        '
        Me.lWarningsPurch.AutoSize = True
        Me.lWarningsPurch.Location = New System.Drawing.Point(450, 58)
        Me.lWarningsPurch.Name = "lWarningsPurch"
        Me.lWarningsPurch.Size = New System.Drawing.Size(56, 13)
        Me.lWarningsPurch.TabIndex = 59
        Me.lWarningsPurch.Text = "Warnings:"
        '
        'lErrorsPurch
        '
        Me.lErrorsPurch.AutoSize = True
        Me.lErrorsPurch.Location = New System.Drawing.Point(350, 58)
        Me.lErrorsPurch.Name = "lErrorsPurch"
        Me.lErrorsPurch.Size = New System.Drawing.Size(40, 13)
        Me.lErrorsPurch.TabIndex = 57
        Me.lErrorsPurch.Text = "Errors:"
        '
        'lDateValidatedPurch
        '
        Me.lDateValidatedPurch.AutoSize = True
        Me.lDateValidatedPurch.Location = New System.Drawing.Point(121, 58)
        Me.lDateValidatedPurch.Name = "lDateValidatedPurch"
        Me.lDateValidatedPurch.Size = New System.Drawing.Size(78, 13)
        Me.lDateValidatedPurch.TabIndex = 55
        Me.lDateValidatedPurch.Text = "Last Validated:"
        '
        'cmdValidatePurch
        '
        Me.cmdValidatePurch.Location = New System.Drawing.Point(7, 58)
        Me.cmdValidatePurch.Name = "cmdValidatePurch"
        Me.cmdValidatePurch.Size = New System.Drawing.Size(87, 23)
        Me.cmdValidatePurch.TabIndex = 54
        Me.cmdValidatePurch.Text = "Validate"
        Me.cmdValidatePurch.UseVisualStyleBackColor = True
        '
        'TabPageAR
        '
        Me.TabPageAR.BackColor = System.Drawing.SystemColors.Control
        Me.TabPageAR.Controls.Add(Me.GroupBox2)
        Me.TabPageAR.Location = New System.Drawing.Point(4, 22)
        Me.TabPageAR.Name = "TabPageAR"
        Me.TabPageAR.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageAR.Size = New System.Drawing.Size(1180, 369)
        Me.TabPageAR.TabIndex = 2
        Me.TabPageAR.Text = "Accounts Receivable"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.cWarningsCust)
        Me.GroupBox2.Controls.Add(Me.cValCompletedCust)
        Me.GroupBox2.Controls.Add(Me.cDateValidatedCust)
        Me.GroupBox2.Controls.Add(Me.cErrorsCust)
        Me.GroupBox2.Controls.Add(Me.cmdAREventLog)
        Me.GroupBox2.Controls.Add(Me.lCurrARPerLabelDisp)
        Me.GroupBox2.Controls.Add(Me.lCurrARPeriodLabel)
        Me.GroupBox2.Controls.Add(Me.lWarningsCust)
        Me.GroupBox2.Controls.Add(Me.lErrorsCust)
        Me.GroupBox2.Controls.Add(Me.lDateValidatedCust)
        Me.GroupBox2.Controls.Add(Me.cmdValidateCust)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(1178, 359)
        Me.GroupBox2.TabIndex = 24
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Customers"
        '
        'cWarningsCust
        '
        Me.cWarningsCust.Enabled = False
        Me.cWarningsCust.Location = New System.Drawing.Point(525, 58)
        Me.cWarningsCust.Name = "cWarningsCust"
        Me.cWarningsCust.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsCust.TabIndex = 32
        '
        'cValCompletedCust
        '
        Me.cValCompletedCust.AutoSize = True
        Me.cValCompletedCust.Enabled = False
        Me.cValCompletedCust.Location = New System.Drawing.Point(595, 58)
        Me.cValCompletedCust.Name = "cValCompletedCust"
        Me.cValCompletedCust.Size = New System.Drawing.Size(77, 17)
        Me.cValCompletedCust.TabIndex = 90
        Me.cValCompletedCust.Text = "Completed"
        Me.cValCompletedCust.UseVisualStyleBackColor = True
        '
        'cDateValidatedCust
        '
        Me.cDateValidatedCust.Enabled = False
        Me.cDateValidatedCust.Location = New System.Drawing.Point(205, 58)
        Me.cDateValidatedCust.Name = "cDateValidatedCust"
        Me.cDateValidatedCust.Size = New System.Drawing.Size(100, 21)
        Me.cDateValidatedCust.TabIndex = 89
        '
        'cErrorsCust
        '
        Me.cErrorsCust.Enabled = False
        Me.cErrorsCust.Location = New System.Drawing.Point(400, 58)
        Me.cErrorsCust.Name = "cErrorsCust"
        Me.cErrorsCust.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsCust.TabIndex = 88
        '
        'cmdAREventLog
        '
        Me.cmdAREventLog.Location = New System.Drawing.Point(14, 162)
        Me.cmdAREventLog.Name = "cmdAREventLog"
        Me.cmdAREventLog.Size = New System.Drawing.Size(104, 23)
        Me.cmdAREventLog.TabIndex = 87
        Me.cmdAREventLog.Text = "Open Event Log"
        Me.cmdAREventLog.UseVisualStyleBackColor = True
        '
        'lCurrARPerLabelDisp
        '
        Me.lCurrARPerLabelDisp.AutoSize = True
        Me.lCurrARPerLabelDisp.Location = New System.Drawing.Point(143, 116)
        Me.lCurrARPerLabelDisp.Name = "lCurrARPerLabelDisp"
        Me.lCurrARPerLabelDisp.Size = New System.Drawing.Size(47, 13)
        Me.lCurrARPerLabelDisp.TabIndex = 35
        Me.lCurrARPerLabelDisp.Text = "00-0000"
        '
        'lCurrARPeriodLabel
        '
        Me.lCurrARPeriodLabel.AutoSize = True
        Me.lCurrARPeriodLabel.Location = New System.Drawing.Point(14, 116)
        Me.lCurrARPeriodLabel.Name = "lCurrARPeriodLabel"
        Me.lCurrARPeriodLabel.Size = New System.Drawing.Size(81, 13)
        Me.lCurrARPeriodLabel.TabIndex = 34
        Me.lCurrARPeriodLabel.Text = "Current Period:"
        '
        'lWarningsCust
        '
        Me.lWarningsCust.AutoSize = True
        Me.lWarningsCust.Location = New System.Drawing.Point(450, 58)
        Me.lWarningsCust.Name = "lWarningsCust"
        Me.lWarningsCust.Size = New System.Drawing.Size(56, 13)
        Me.lWarningsCust.TabIndex = 29
        Me.lWarningsCust.Text = "Warnings:"
        '
        'lErrorsCust
        '
        Me.lErrorsCust.AutoSize = True
        Me.lErrorsCust.Location = New System.Drawing.Point(350, 58)
        Me.lErrorsCust.Name = "lErrorsCust"
        Me.lErrorsCust.Size = New System.Drawing.Size(40, 13)
        Me.lErrorsCust.TabIndex = 27
        Me.lErrorsCust.Text = "Errors:"
        '
        'lDateValidatedCust
        '
        Me.lDateValidatedCust.AutoSize = True
        Me.lDateValidatedCust.Location = New System.Drawing.Point(121, 58)
        Me.lDateValidatedCust.Name = "lDateValidatedCust"
        Me.lDateValidatedCust.Size = New System.Drawing.Size(78, 13)
        Me.lDateValidatedCust.TabIndex = 25
        Me.lDateValidatedCust.Text = "Last Validated:"
        '
        'cmdValidateCust
        '
        Me.cmdValidateCust.Location = New System.Drawing.Point(7, 58)
        Me.cmdValidateCust.Name = "cmdValidateCust"
        Me.cmdValidateCust.Size = New System.Drawing.Size(87, 23)
        Me.cmdValidateCust.TabIndex = 24
        Me.cmdValidateCust.Text = "Validate"
        Me.cmdValidateCust.UseVisualStyleBackColor = True
        '
        'TabPageGL
        '
        Me.TabPageGL.BackColor = System.Drawing.SystemColors.Control
        Me.TabPageGL.Controls.Add(Me.cmdGLEventLog)
        Me.TabPageGL.Controls.Add(Me.GroupBox1)
        Me.TabPageGL.Location = New System.Drawing.Point(4, 22)
        Me.TabPageGL.Name = "TabPageGL"
        Me.TabPageGL.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageGL.Size = New System.Drawing.Size(1180, 369)
        Me.TabPageGL.TabIndex = 0
        Me.TabPageGL.Text = "General Ledger"
        '
        'cmdGLEventLog
        '
        Me.cmdGLEventLog.Location = New System.Drawing.Point(14, 162)
        Me.cmdGLEventLog.Name = "cmdGLEventLog"
        Me.cmdGLEventLog.Size = New System.Drawing.Size(104, 23)
        Me.cmdGLEventLog.TabIndex = 88
        Me.cmdGLEventLog.Text = "Open Event Log"
        Me.cmdGLEventLog.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cCompletedCOA)
        Me.GroupBox1.Controls.Add(Me.cErrorsCOAVal)
        Me.GroupBox1.Controls.Add(Me.cWarningsCOAVal)
        Me.GroupBox1.Controls.Add(Me.cDateCompletedCOA)
        Me.GroupBox1.Controls.Add(Me.lCurrPeriodDisp)
        Me.GroupBox1.Controls.Add(Me.lWarningsCOA)
        Me.GroupBox1.Controls.Add(Me.lCurrPeriodLabel)
        Me.GroupBox1.Controls.Add(Me.lLedgerDisp)
        Me.GroupBox1.Controls.Add(Me.lLedgerLabel)
        Me.GroupBox1.Controls.Add(Me.lErrorsCOA)
        Me.GroupBox1.Controls.Add(Me.lDateValidatedCOA)
        Me.GroupBox1.Controls.Add(Me.cmdValidateAccts)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1171, 357)
        Me.GroupBox1.TabIndex = 11
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Chart of Accounts"
        '
        'cCompletedCOA
        '
        Me.cCompletedCOA.AutoSize = True
        Me.cCompletedCOA.Enabled = False
        Me.cCompletedCOA.Location = New System.Drawing.Point(595, 58)
        Me.cCompletedCOA.Name = "cCompletedCOA"
        Me.cCompletedCOA.Size = New System.Drawing.Size(77, 17)
        Me.cCompletedCOA.TabIndex = 92
        Me.cCompletedCOA.Text = "Completed"
        Me.cCompletedCOA.UseVisualStyleBackColor = True
        '
        'cErrorsCOAVal
        '
        Me.cErrorsCOAVal.Enabled = False
        Me.cErrorsCOAVal.Location = New System.Drawing.Point(400, 58)
        Me.cErrorsCOAVal.Name = "cErrorsCOAVal"
        Me.cErrorsCOAVal.Size = New System.Drawing.Size(26, 21)
        Me.cErrorsCOAVal.TabIndex = 90
        '
        'cWarningsCOAVal
        '
        Me.cWarningsCOAVal.Enabled = False
        Me.cWarningsCOAVal.Location = New System.Drawing.Point(525, 58)
        Me.cWarningsCOAVal.Name = "cWarningsCOAVal"
        Me.cWarningsCOAVal.Size = New System.Drawing.Size(26, 21)
        Me.cWarningsCOAVal.TabIndex = 91
        '
        'cDateCompletedCOA
        '
        Me.cDateCompletedCOA.CustomFormat = "MM/DD/YYYY"
        Me.cDateCompletedCOA.Enabled = False
        Me.cDateCompletedCOA.Location = New System.Drawing.Point(205, 58)
        Me.cDateCompletedCOA.Name = "cDateCompletedCOA"
        Me.cDateCompletedCOA.Size = New System.Drawing.Size(100, 21)
        Me.cDateCompletedCOA.TabIndex = 89
        '
        'lCurrPeriodDisp
        '
        Me.lCurrPeriodDisp.AutoSize = True
        Me.lCurrPeriodDisp.Location = New System.Drawing.Point(114, 116)
        Me.lCurrPeriodDisp.Name = "lCurrPeriodDisp"
        Me.lCurrPeriodDisp.Size = New System.Drawing.Size(47, 13)
        Me.lCurrPeriodDisp.TabIndex = 20
        Me.lCurrPeriodDisp.Text = "00-0000"
        '
        'lWarningsCOA
        '
        Me.lWarningsCOA.AutoSize = True
        Me.lWarningsCOA.Location = New System.Drawing.Point(450, 58)
        Me.lWarningsCOA.Name = "lWarningsCOA"
        Me.lWarningsCOA.Size = New System.Drawing.Size(56, 13)
        Me.lWarningsCOA.TabIndex = 16
        Me.lWarningsCOA.Text = "Warnings:"
        '
        'lCurrPeriodLabel
        '
        Me.lCurrPeriodLabel.AutoSize = True
        Me.lCurrPeriodLabel.Location = New System.Drawing.Point(14, 116)
        Me.lCurrPeriodLabel.Name = "lCurrPeriodLabel"
        Me.lCurrPeriodLabel.Size = New System.Drawing.Size(81, 13)
        Me.lCurrPeriodLabel.TabIndex = 19
        Me.lCurrPeriodLabel.Text = "Current Period:"
        '
        'lLedgerDisp
        '
        Me.lLedgerDisp.AutoSize = True
        Me.lLedgerDisp.Location = New System.Drawing.Point(290, 116)
        Me.lLedgerDisp.Name = "lLedgerDisp"
        Me.lLedgerDisp.Size = New System.Drawing.Size(38, 13)
        Me.lLedgerDisp.TabIndex = 22
        Me.lLedgerDisp.Text = "Label1"
        '
        'lLedgerLabel
        '
        Me.lLedgerLabel.AutoSize = True
        Me.lLedgerLabel.Location = New System.Drawing.Point(208, 116)
        Me.lLedgerLabel.Name = "lLedgerLabel"
        Me.lLedgerLabel.Size = New System.Drawing.Size(58, 13)
        Me.lLedgerLabel.TabIndex = 21
        Me.lLedgerLabel.Text = "Ledger ID:"
        '
        'lErrorsCOA
        '
        Me.lErrorsCOA.AutoSize = True
        Me.lErrorsCOA.Location = New System.Drawing.Point(350, 58)
        Me.lErrorsCOA.Name = "lErrorsCOA"
        Me.lErrorsCOA.Size = New System.Drawing.Size(40, 13)
        Me.lErrorsCOA.TabIndex = 14
        Me.lErrorsCOA.Text = "Errors:"
        '
        'lDateValidatedCOA
        '
        Me.lDateValidatedCOA.AutoSize = True
        Me.lDateValidatedCOA.Location = New System.Drawing.Point(121, 61)
        Me.lDateValidatedCOA.Name = "lDateValidatedCOA"
        Me.lDateValidatedCOA.Size = New System.Drawing.Size(78, 13)
        Me.lDateValidatedCOA.TabIndex = 12
        Me.lDateValidatedCOA.Text = "Last Validated:"
        '
        'cmdValidateAccts
        '
        Me.cmdValidateAccts.Location = New System.Drawing.Point(10, 58)
        Me.cmdValidateAccts.Name = "cmdValidateAccts"
        Me.cmdValidateAccts.Size = New System.Drawing.Size(87, 23)
        Me.cmdValidateAccts.TabIndex = 11
        Me.cmdValidateAccts.Text = "Validate"
        Me.cmdValidateAccts.UseVisualStyleBackColor = True
        '
        'cCpnyIDTxt
        '
        Me.cCpnyIDTxt.Enabled = False
        Me.cCpnyIDTxt.Location = New System.Drawing.Point(131, 9)
        Me.cCpnyIDTxt.Name = "cCpnyIDTxt"
        Me.cCpnyIDTxt.Size = New System.Drawing.Size(100, 21)
        Me.cCpnyIDTxt.TabIndex = 118
        '
        'cFiscYr_End
        '
        Me.cFiscYr_End.Enabled = False
        Me.cFiscYr_End.Location = New System.Drawing.Point(347, 12)
        Me.cFiscYr_End.Name = "cFiscYr_End"
        Me.cFiscYr_End.Size = New System.Drawing.Size(107, 21)
        Me.cFiscYr_End.TabIndex = 119
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLbl})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 582)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1248, 22)
        Me.StatusStrip1.TabIndex = 120
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusLbl
        '
        Me.StatusLbl.Name = "StatusLbl"
        Me.StatusLbl.Size = New System.Drawing.Size(0, 17)
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1248, 604)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.cFiscYr_End)
        Me.Controls.Add(Me.cCpnyIDTxt)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.lFiscYr_End)
        Me.Controls.Add(Me.lCpnyID)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Location = New System.Drawing.Point(4, 23)
        Me.Name = "Form1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text = "Repair Tool (Microsoft Dynamics SL)"
        Me.TabControl1.ResumeLayout(False)
        Me.DBConnect.ResumeLayout(False)
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox9.PerformLayout()
        Me.TabPageSUM.ResumeLayout(False)
        Me.TabPageSUM.PerformLayout()
        Me.TabPagePA.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.TabPageSO.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.TabPageIN.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.TabPageAP.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.TabPagePO.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.TabPageAR.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.TabPageGL.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SAFHelpProvider As System.Windows.Forms.HelpProvider
    Friend WithEvents lCpnyID As System.Windows.Forms.Label
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents lFiscYr_End As System.Windows.Forms.Label
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPageGL As TabPage
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lWarningsCOA As Label
    Friend WithEvents lCurrPeriodDisp As Label
    Friend WithEvents lCurrPeriodLabel As Label
    Friend WithEvents lLedgerDisp As Label
    Friend WithEvents lLedgerLabel As Label
    Friend WithEvents lErrorsCOA As Label
    Friend WithEvents lDateValidatedCOA As Label
    Friend WithEvents cmdValidateAccts As Button
    Friend WithEvents TabPageAP As TabPage
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents lWarningsVend As Label
    Friend WithEvents lErrorsVend As Label
    Friend WithEvents lDateValidatedVend As Label
    Friend WithEvents cmdValidateVend As Button
    Friend WithEvents TabPageAR As TabPage
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents lWarningsCust As Label
    Friend WithEvents lErrorsCust As Label
    Friend WithEvents lDateValidatedCust As Label
    Friend WithEvents cmdValidateCust As Button
    Friend WithEvents TabPageIN As TabPage
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents lWarningsItem As Label
    Friend WithEvents lErrorsItem As Label
    Friend WithEvents lDateValidatedItem As Label
    Friend WithEvents cmdValidateItem As Button
    Friend WithEvents TabPagePA As TabPage
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents lWarningsProj As Label
    Friend WithEvents lErrorsProj As Label
    Friend WithEvents lDateValidatedProj As Label
    Friend WithEvents cmdValidateProj As Button
    Friend WithEvents TabPageSUM As TabPage
    Friend WithEvents lGeneralLedger As Label
    Friend WithEvents lCompleted As Label
    Friend WithEvents lWarnings As Label
    Friend WithEvents lErrors As Label
    Friend WithEvents lAcctsPayable As Label
    Friend WithEvents lAcctsReceivable As Label
    Friend WithEvents lInventory As Label
    Friend WithEvents lProjController As Label
    Friend WithEvents TabPagePO As TabPage
    Friend WithEvents lPurchasing As Label
    Friend WithEvents lCurrAPPeriodDisp As Label
    Friend WithEvents lblAPPeriod As Label
    Friend WithEvents lCurrARPerLabelDisp As Label
    Friend WithEvents lCurrARPeriodLabel As Label
    Friend WithEvents lCurrINPeriodDisp As Label
    Friend WithEvents lCurrINPeriodLabel As Label
    Friend WithEvents lCurrPAPeriodDisp As Label
    Friend WithEvents lCurrPAPeriodLabel As Label
    Friend WithEvents lAcctTotal As Label
    Friend WithEvents lCustTotal As Label
    Friend WithEvents lVendTotal As Label
    Friend WithEvents lItemTotal As Label
    Friend WithEvents lProjTotal As Label
    Friend WithEvents lCustActive As Label
    Friend WithEvents lVendActive As Label
    Friend WithEvents lAcctActive As Label
    Friend WithEvents lProjActive As Label
    Friend WithEvents lInvtActive As Label
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents lPONote As Label
    Friend WithEvents lWarningsPurch As Label
    Friend WithEvents lErrorsPurch As Label
    Friend WithEvents lDateValidatedPurch As Label
    Friend WithEvents cmdValidatePurch As Button
    Friend WithEvents lOrderActive As Label
    Friend WithEvents lOrderTotal As Label
    Friend WithEvents lGLPeriod As Label
    Friend WithEvents lCurrARPeriod As Label
    Friend WithEvents lCurrAPPeriod As Label
    Friend WithEvents lCurrPAPeriod As Label
    Friend WithEvents lCurrINPeriod As Label
    Friend WithEvents lCurrPerHdr As Label
    Friend WithEvents cmdProjEventLog As Button
    Friend WithEvents cmdIntEventLog As Button
    Friend WithEvents cmdPOEventLog As Button
    Friend WithEvents cmdAPEventLog As Button
    Friend WithEvents cmdAREventLog As Button
    Friend WithEvents cmdGLEventLog As Button
    Friend WithEvents TabPageSO As TabPage
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents cmdSOEventLog As Button
    Friend WithEvents cCurrSOPeriodDisp As Label
    Friend WithEvents lCurrSOPeriodLabel As Label
    Friend WithEvents lWarningSO As Label
    Friend WithEvents lErrorsSO As Label
    Friend WithEvents lValidatedSO As Label
    Friend WithEvents cmdValidateSalesOrder As Button
    Friend WithEvents lSOTotal As Label
    Friend WithEvents lSOActive As Label
    Friend WithEvents lSalesOrder As Label
    Friend WithEvents cCpnyIDTxt As TextBox
    Friend WithEvents DBConnect As TabPage
    Friend WithEvents GroupBox8 As GroupBox
    Public WithEvents lblSQLServerName As Label
    Public WithEvents NameOfServer As TextBox
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents rbAuthenticationTypeSQL As RadioButton
    Friend WithEvents rbAuthenticationTypeWindows As RadioButton
    Public WithEvents lblPassword As Label
    Public WithEvents lblUserId As Label
    Public WithEvents txtPassword As TextBox
    Public WithEvents txtUserId As TextBox
    Friend WithEvents SysDb As TextBox
    Friend WithEvents lblSysDBNames As Label
    Public WithEvents btnConnectClose As Button
    Public WithEvents ConnectServer As Button
    Friend WithEvents lblCpnyId As Label
    Friend WithEvents cErrorsGLVal As TextBox
    Friend WithEvents cWarningsAPVal As TextBox
    Friend WithEvents cWarningsGLVal As TextBox
    Friend WithEvents cErrorsAPVal As TextBox
    Friend WithEvents cWarningsARVal As TextBox
    Friend WithEvents cErrorsARVal As TextBox
    Friend WithEvents cWarningsINVal As TextBox
    Friend WithEvents cErrorsINVal As TextBox
    Friend WithEvents cWarningsPOVal As TextBox
    Friend WithEvents cErrorsPOVal As TextBox
    Friend WithEvents cWarningsPAVal As TextBox
    Friend WithEvents cErrorsPAVal As TextBox
    Friend WithEvents cWarningsSOVal As TextBox
    Friend WithEvents cCompletedSOChk As CheckBox
    Friend WithEvents cCompletedPAChk As CheckBox
    Friend WithEvents cCompletedPOChk As CheckBox
    Friend WithEvents cCompletedINChk As CheckBox
    Friend WithEvents cCompletedARChk As CheckBox
    Friend WithEvents cCompletedAPChk As CheckBox
    Friend WithEvents cCompletedGLChk As CheckBox
    Friend WithEvents cDateCompletedCOA As DateTimePicker
    Friend WithEvents cCompletedCOA As CheckBox
    Friend WithEvents cWarningsCOAVal As TextBox
    Friend WithEvents cErrorsCOAVal As TextBox
    Friend WithEvents CpnyIDList As ComboBox
    Friend WithEvents cWarningsCust As TextBox
    Friend WithEvents cDateValidatedCust As DateTimePicker
    Friend WithEvents cErrorsCust As TextBox
    Friend WithEvents cDateValidatedPurch As DateTimePicker
    Friend WithEvents cErrorsPurch As TextBox
    Friend WithEvents cWarningsPurch As TextBox
    Friend WithEvents cValCompletedPurch As CheckBox
    Friend WithEvents cValCompletedCust As CheckBox
    Friend WithEvents cDateValidatedVend As DateTimePicker
    Friend WithEvents cErrorsVend As TextBox
    Friend WithEvents cWarningsVend As TextBox
    Friend WithEvents cValCompletedVend As CheckBox
    Friend WithEvents cDateValidatedItem As DateTimePicker
    Friend WithEvents cErrorsItem As TextBox
    Friend WithEvents cWarningsItem As TextBox
    Friend WithEvents cValCompletedItem As CheckBox
    Friend WithEvents cDateValidatedSO As DateTimePicker
    Friend WithEvents cErrorsSO As TextBox
    Friend WithEvents cWarningsSO As TextBox
    Friend WithEvents cValCompletedSO As CheckBox
    Friend WithEvents cDateValidatedProj As DateTimePicker
    Friend WithEvents cErrorsProj As TextBox
    Friend WithEvents cWarningsProj As TextBox
    Friend WithEvents cValCompletedProj As CheckBox
    Friend WithEvents cErrorsSOVal As TextBox
    Friend WithEvents cFiscYr_End As TextBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents btnBrowse As Button
    Friend WithEvents txtEventLogPath As TextBox
    Friend WithEvents lblEventOutput As Label
    Friend WithEvents FolderBrowserDialog2 As FolderBrowserDialog
    Friend WithEvents StatusLbl As ToolStripStatusLabel
    Friend WithEvents lblDirRequired As Label
    Friend WithEvents lblCpnyRequired As Label
    Friend WithEvents lblDbRequired As Label
    Friend WithEvents lblDbStatus As Label
#End Region
End Class
