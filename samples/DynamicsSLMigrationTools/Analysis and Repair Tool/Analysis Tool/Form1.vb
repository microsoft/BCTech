'*********************************************************
'
'    Copyright (c) Microsoft. All rights reserved.
'    This code is licensed under the Microsoft Public License.
'    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
''
'*********************************************************
Option Explicit On
Option Strict Off
Imports Microsoft.VisualBasic.FileSystem
Imports System
Imports System.IO
Imports System.Collections

Imports System.Data.SqlClient
Friend Class Form1
	Inherits System.Windows.Forms.Form
	
	Protected m_IsInitializing As Boolean
	Protected ReadOnly Property IsInitializing() As Boolean
		Get
			Return m_IsInitializing
		End Get
	End Property

    Private Sub Form1_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load

        'Default the SQL Server name.
        NameOfServer.Text = System.Environment.MachineName
        SQLServerName = NameOfServer.Text

        CpnyDBList = New List(Of CpnyDatabase)

        ' Disable the Analysis tab until the company is selected.
        TabControl1.TabPages.Item("Analyze").Enabled = False

        cLastRunDate.Format = DateTimePickerFormat.Custom
        cLastRunDate.CustomFormat = "MM/dd/yyyy"


    End Sub

    Private Sub cmdSysAnalysis_Click(sender As System.Object, e As System.EventArgs) Handles cmdSysAnalysis.Click
        Dim result As DialogResult

        ' Require an output path for the log/report file before processing.
        If (String.IsNullOrEmpty(EventLogDir.Trim())) Then
            result = MessageBox.Show("Export folder path is required.  Please enter a valid directory.", "Error", MessageBoxButtons.OK)
            Exit Sub
        End If

        ' Set the current date/time.
        CurrDate = Date.Now
        CurrDateStr = CurrDate.ToString()

        LastYrDate = CurrDate.AddYears(-1)
        LastYrDateStr = LastYrDate.ToString()


        'Display confirmation message
        result = MessageBox.Show("This process will run an analysis on the Dynamics SL application database (" + DBName.Trim + "). Are you sure you would like to continue?", "System Analysis", MessageBoxButtons.YesNo)
        If (result = DialogResult.Yes) Then
            'Perform system analysis
            Call PerformAnalysis()
        Else
            Exit Sub
        End If

    End Sub

    Private Sub PerformAnalysis()

        Dim sqlReader As SqlDataReader = Nothing
        oEventLog = New clsEventLog
        Dim sqlStmt As String = String.Empty
        Dim ExportFilename As String

        Dim fmtDate As String

        fmtDate = Date.Now.ToString
        fmtDate = fmtDate.Replace(":", "")
        fmtDate = fmtDate.Replace("/", "")
        fmtDate = fmtDate.Remove(fmtDate.Length - 3)
        fmtDate = fmtDate.Replace(" ", "-")
        fmtDate = fmtDate & Date.Now.Millisecond


        Try
            ExportFilename = "SystemAnalysis_" + fmtDate + ".txt"
            oEventLog.FileName = ExportFilename

            ' Begin the output file for reporting results.
            Call oEventLog.LogMessage(StartProcess, "")


            UpdateStatusLbl.Text = "Begin Analysis"

            'Truncate data in the work tables
            sqlStmt = "Delete from xSLAnalysisSum"
            Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)
            sqlStmt = "Delete from xSLAnalysisCpny"
            Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)


            'Populate xSLAnalysisCpny table
            Call sql_1(sqlReader, "EXEC xp_SLAnalysisCpnyPop", SqlAppDbConn, OperationType.ExecProc, CommandType.Text)

            RecID = 0

            'Get date values
            CurrDate = Date.Now


            'Log Database and Company Information
            Dim strVersion As String = String.Empty


            ' Get the current version of Dynamics SL.
            Call sqlFetch_1(sqlReader, "getVersion", SqlSysDbConn, CommandType.StoredProcedure)
            If (sqlReader.HasRows) Then
                sqlReader.Read()
                bVersionSL.VersionNbr = sqlReader(0)
            End If

            Call sqlReader.Close()

            'Analyze Administration Information
            AnalysisStatusLbl.Text = "Analyzing Administration Information"
            Call Analyze_SY.Analyze_SY()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Administration Information"
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing Administration Information"
                Exit Sub
            End If

            'Analyze MC - Multi-Company
            Call Analyze_MC.Analyze_MC()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Multi-Company"
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing Multi-Company"
                Exit Sub
            End If

            'Analyze GL - General Ledger
            Call Analyze_GL.Analyze_GL()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing General Ledger at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the General Ledger Module"
                Exit Sub
            End If

            'Analyze AP - Accounts Payable
            Call Analyze_AP.Analyze_AP()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Accounts Payable at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Accounts Payable Module"
                Exit Sub
            End If

            'Analyze AR - Accounts Receivable
            Call Analyze_AR.Analyze_AR()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Accounts Receivable at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Accounts Receivable Module"
                Exit Sub
            End If

            'Analyze CA - Cash Manager
            Call Analyze_CA.Analyze_CA()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Cash Manager at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Cash Manager Module"
                Exit Sub
            End If

            'Analyze IN - Inventory
            Call Analyze_IN.Analyze_IN()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Inventory at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Inventory Module"
                Exit Sub
            End If

            'Analyze BM - Bill of Material
            Call Analyze_BM.Analyze_BM()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Bill of Material at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Bill of Material Module"
                Exit Sub
            End If

            'Analyze PO - Purchasing
            Call Analyze_PO.Analyze_PO()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Purchasing at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Purchasing Module"
                Exit Sub
            End If

            'Analyze RQ - Requisitions
            Call Analyze_RQ.Analyze_RQ()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Requisitions at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Requisitions Module"
                Exit Sub
            End If

            'Analyze OM - Order Management
            Call Analyze_OM.Analyze_OM()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Order Management at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Order Management Module"
                Exit Sub
            End If

            'Analyze PA - Project Controller
            Call Analyze_PA.Analyze_PA()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Project Controller at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Project Controller Module"
                Exit Sub
            End If

            'Analyze BI - Flexible Billings
            Call Analyze_BI.Analyze_BI()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Flexible Billings at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Flexible Billings Module"
                Exit Sub
            End If

            'Analyze TM - Time and Expense for Projects
            Call Analyze_TM.Analyze_TM()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Time and Expense for Projects at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Time and Expense for Projects Module"
                Exit Sub
            End If

            'Analyze SD - Service Dispatch
            Call Analyze_SD.Analyze_SD()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Service Dispatch at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Service Dispatch Module"
                Exit Sub
            End If

            'Analyze SN - Service Contracts
            Call Analyze_SN.Analyze_SN()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Service Contracts at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Service Contracts Module"
                Exit Sub
            End If

            'Analyze SI - Shared Information
            Call Analyze_SI.Analyze_SI()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Shared Information at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Shared Information Module"
                Exit Sub
            End If

            'Analyze CM - Currency Manager
            Call Analyze_CM.Analyze_CM()
            If OkToContinue = True Then
                AnalysisStatusLbl.Text = "Finished analyzing Currency Manager at " + Now.ToString("hh:mm:ss")
            Else
                AnalysisStatusLbl.Text = "Error encountered analyzing the Currency Manager Module"
                Exit Sub
            End If



            'Truncate table xSLAnalysisCpny
            sqlStmt = "Delete from xSLAnalysisCpny"
            Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)



            If OkToContinue = True Then
                'End process
                AnalysisStatusLbl.Text = ""

                Dim retValDte1 As Date
                Dim sResult As String = String.Empty

                sqlStmt = "SELECT MAX(LUpd_DateTime) FROM xSLAnalysisSum"
                Call sqlFetch_Num(retValDte1, sqlStmt, SqlAppDbConn)
                sResult = retValDte1.ToShortDateString()
                cLastRunDate.Value = sResult
                cmdViewAnalysis.Enabled = True
            End If


        Catch ex As Exception
            AnalysisStatusLbl.Text = ex.Message.Trim + vbNewLine + ex.StackTrace
            AnalysisStatusLbl.Text = "Error Encountered: "

        End Try

        Call oEventLog.LogMessage(EndProcess, "Microsoft Dynamics SL Analysis Report")
        ' Display message to indicate that analysis is complete.
        Call MessageBox.Show("Analysis Complete")

    End Sub

    Private Sub cmdViewAnalysis_Click(sender As System.Object, e As System.EventArgs) Handles cmdViewAnalysis.Click

        Dim fileOutput As String = oEventLog.GetLogFile().FullName
        Dim NPErr As Integer

        ' Open the log file generated by the previous run in notepad.
        ' Display the event log.
        If (My.Computer.FileSystem.FileExists(fileOutput)) Then

            ' See if the file also exists in the Virtual Store....
            NPErr = Shell("Notepad.exe " + fileOutput.Trim, AppWinStyle.NormalFocus, True)
            If NPErr > 0 Then
                Call MessageBox.Show("Unable to display log = " + fileOutput)
            End If
        End If


    End Sub



    Private Sub rbAuthenticationTypeWindows_CheckedChanged(sender As Object, e As EventArgs) Handles rbAuthenticationTypeWindows.CheckedChanged
        ' If this is selected, then indicate that the Windows Authentication is enabled.
        IsWindowsAuth = True
        txtUserId.Enabled = False
        txtPassword.Enabled = False
    End Sub

    Private Sub rbAuthenticationTypeSQL_CheckedChanged(sender As Object, e As EventArgs) Handles rbAuthenticationTypeSQL.CheckedChanged
        ' If this is selected, then indicate that the authentication type is SQL Authentication.
        IsWindowsAuth = False

        txtUserId.Enabled = True
        txtPassword.Enabled = True
    End Sub

    Private Sub btnConnectServer_Click(sender As Object, e As EventArgs) Handles btnConnectServer.Click
        Dim connStr As String
        Dim sqlString As String
        Dim sqlCpnySet As SqlDataReader
        Dim CpnyDBEntry As CpnyDatabase




        'Set the global variables that hold the userid and password
        'that the user entered.
        SQLAuthUser = Trim(txtUserId.Text)
        SQLAuthPwd = Trim(txtPassword.Text)

        ' Add the specified database.
        If (String.IsNullOrEmpty(SysDb.Text.Trim())) Then
            MsgBox("Database name is required")
        Else


            connStr = GetConnectionString(True)

            SqlSysDbConn = New SqlClient.SqlConnection(connStr)

            SQLCommand = New SqlClient.SqlCommand()
            SQLCommand.Connection = SqlSysDbConn


            SQLCommand.Connection.Open()

            ' If the connection opened successfully, then get the list of companies.
            If (SqlSysDbConn.State = ConnectionState.Open) Then
                CpnyIDList.Items.Clear()
                sqlString = "Select Active, CpnyId, CpnyName, DatabaseName From Company"
                SQLCommand.CommandText = sqlString
                sqlCpnySet = SQLCommand.ExecuteReader()



                If (sqlCpnySet.HasRows = True) Then
                    While (sqlCpnySet.Read())
                        CpnyDBEntry = New CpnyDatabase
                        CpnyDBEntry.Active = sqlCpnySet("Active")
                        CpnyDBEntry.CompanyId = sqlCpnySet("CpnyId")
                        CpnyDBEntry.CompanyName = sqlCpnySet("CpnyName")
                        CpnyDBEntry.DatabaseName = sqlCpnySet("DatabaseName")
                        CpnyIDList.Items.Add(CpnyDBEntry.CompanyId)
                        CpnyDBList.Add(CpnyDBEntry)
                    End While
                End If
            End If

            SQLCommand.Connection.Close()

            lblDbStatus.Text = "Connected"
            UserId = System.Environment.UserName()

        End If
    End Sub


    Private Sub cmdBrowse_Click(sender As Object, e As EventArgs) Handles cmdBrowse.Click
        Dim myResult As System.Windows.Forms.DialogResult
        Dim folderBrowserDialog1 As New FolderBrowserDialog
        Dim strPath As String = String.Empty
        Dim strSLRootPath As String = String.Empty

        folderBrowserDialog1.Description = "Select the directory for saving the export files."

        'Get SL path
        strSLRootPath = strPath.Replace("file:\", "")

        'Set selected path
        folderBrowserDialog1.SelectedPath = cExportFolder.Text

        'Open file dialog box
        myResult = folderBrowserDialog1.ShowDialog()
        If myResult = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If

        'Set folder selected and display the value
        cExportFolder.Text = folderBrowserDialog1.SelectedPath.Trim
        EventLogDir = cExportFolder.Text.Trim()

    End Sub



    Private Sub SysDb_TextChanged(sender As Object, e As EventArgs) Handles SysDb.TextChanged
        SysDBName = SysDb.Text
    End Sub

    Private Sub CpnyIDList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CpnyIDList.SelectedIndexChanged
        Dim retValDte As Date
        Dim sqlStmt As String = String.Empty
        Dim sResult As String = String.Empty

        CpnyId = CpnyIDList.Items(CpnyIDList.SelectedIndex)

        ' Get the name of the application database associated with the selected company.
        For Each CpnyDBEntry As CpnyDatabase In CpnyDBList
            If (String.Compare(CpnyId.Trim, CpnyDBEntry.CompanyId.Trim(), True) = 0) Then
                AppDBName = CpnyDBEntry.DatabaseName
                Exit For
            End If
        Next
        DBName = AppDBName

        If (String.IsNullOrEmpty(AppDBName) = True) Then
            MsgBox("Error:  Unable to open application database associated with company " & CpnyId.Trim())
        Else
            ' Open the connection to the app database.
            AppDbConnStr = GetConnectionString(False)

            SqlAppDbConn = New SqlClient.SqlConnection(AppDbConnStr)


            sqlStmt = "SELECT MAX(LUpd_DateTime) FROM xSLAnalysisSum"
            Call sqlFetch_Num(retValDte, sqlStmt, SqlAppDbConn)
            sResult = retValDte.ToShortDateString()
            If sResult.Trim <> "" Then
                cLastRunDate.Value = sResult
                cmdViewAnalysis.Enabled = True
            Else
                cLastRunDate.Value = ""
                cmdViewAnalysis.Enabled = False

            End If
            TabControl1.TabPages.Item("Analyze").Enabled = True

        End If
    End Sub



    Private Sub cExportFolder_Leave(sender As Object, e As EventArgs) Handles cExportFolder.Leave

        ' Verify the validity of the output folder.
        If (Directory.Exists(cExportFolder.Text.Trim())) Then

            EventLogDir = cExportFolder.Text.Trim()
        Else
            Call MessageBox.Show("Directory path is invalid.  Please enter a valid directory.", "Invalid Directory", MessageBoxButtons.OK)

        End If
    End Sub


    Private Sub NameOfServer_Leave(sender As Object, e As EventArgs) Handles NameOfServer.Leave
        SQLServerName = NameOfServer.Text

    End Sub


End Class
