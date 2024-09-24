Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_PA
    '================================================================================
    ' This module contains code to analyze tables used with the Project Controller Module
    '================================================================================

    Public Sub Analyze_PA()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "PA"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValDbl1 As Double
        Dim retValStr1 As String = String.Empty
        Dim calcValDbl1 As Double
        Dim sqlQuery As String = String.Empty


        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing

        Try
            Call oEventLog.LogMessage(0, "PROJECT CONTROLLER")
            Call oEventLog.LogMessage(0, "")

            '===== Project Controller =====

            '=== Module Usage ===
            Call oEventLog.LogMessage(0, "Analyzing Project Controller Module Usage")

            Form1.AnalysisStatusLbl.Text = "Analyzing Project Controller Module Usage"
            sAnalysisType = "Module Usage"

            RecID = RecID + 1
            sDescr = "Is the module being used?"
            sqlStmt = "SELECT COUNT(Init) FROM PCSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM PJTran"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If sResult = "NO" Then
                Exit Sub
            End If

            RecID = RecID + 1
            sDescr = "Financial modules Integrated:"
            sqlStmt = "SELECT Control_Data FROM PJCONTRL WHERE control_type = 'PA' AND control_code = 'INTERFACE-LIST'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetPJContrlValues(sqlReader, bPJContrlInfo)
            Else
                bPJContrlInfo.Control_Data = String.Empty
            End If
            Call sqlReader.Close()
            If bPJContrlInfo.Control_Data.Trim <> "" Then
                sResult = bPJContrlInfo.Control_Data.Trim
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Revenue Recognition based on:"
            sqlStmt = "SELECT * FROM PJCONTRL WHERE control_type = 'PA' AND control_code = 'REV-BASIS'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetPJContrlValues(sqlReader, bPJContrlInfo)
            Else
                bPJContrlInfo.Control_Data = String.Empty
            End If
            Call sqlReader.Close()


            If bPJContrlInfo.Control_Data.Trim <> "" Then
                If bPJContrlInfo.Control_Data.Trim = "R" Then
                    sResult = "Revenue Budget"
                ElseIf bPJContrlInfo.Control_Data.Trim = "C" Then
                    sResult = "Contract Value"
                Else
                    sResult = "Error"
                End If
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Billed to Date Acct Category:"
            sqlStmt = "SELECT * FROM PJCONTRL WHERE control_type = 'PA' AND control_code = 'BTD'"

            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetPJContrlValues(sqlReader, bPJContrlInfo)
            Else
                bPJContrlInfo.Control_Data = String.Empty
            End If
            Call sqlReader.Close()

            If bPJContrlInfo.Control_Data.Trim <> "" Then
                sResult = bPJContrlInfo.Control_Data.Trim
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unreleased Project Charge Entry batches:"
            sqlStmt = "SELECT COUNT(batch_id) FROM PJCHARGH WHERE batch_status <> 'P'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects using Revenue Recognition:"
            sqlStmt = "SELECT COUNT(project) FROM PJPROJEX WHERE rev_flag = 'Y'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Current period for the module:"
            sqlStmt = "SELECT control_data FROM PJCONTRL WHERE control_type = 'PA' AND control_code = 'CURRENT-PERIOD'"

            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetPJContrlValues(sqlReader, bPJContrlInfo)
            Else
                bPJContrlInfo.Control_Data = String.Empty
            End If
            Call sqlReader.Close()

            If bPJContrlInfo.Control_Data.Trim <> "" Then
                sResult = bPJContrlInfo.Control_Data.Substring(4, 2) + "-" + bPJContrlInfo.Control_Data.Substring(0, 4)
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Oldest Period used on Project transactions:"
            sqlStmt = "SELECT MIN(fiscalno) FROM PJTran"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Try
                    retValStr1 = sqlReader(0)
                Catch ex As Exception
                    retValStr1 = String.Empty
                End Try
            Else
                retValStr1 = String.Empty
            End If
            Call sqlReader.Close()

            If retValStr1.Trim <> "" Then
                sResult = retValStr1.Substring(4, 2) + "-" + retValStr1.Substring(0, 4)
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Newest Period used on Project transactions:"
            sqlStmt = "SELECT MAX(fiscalno) FROM PJTran"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Try
                    retValStr1 = sqlReader(0)
                Catch ex As Exception
                    retValStr1 = String.Empty
                End Try
            Else
                retValStr1 = String.Empty
            End If
            Call sqlReader.Close()
            If retValStr1.Trim <> "" Then
                sResult = retValStr1.Substring(4, 2) + "-" + retValStr1.Substring(0, 4)
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Module Usage section *****

            Call oEventLog.LogMessage(0, "")


            '=== Master Table Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Project Controller Master Table Counts")
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Total number of Projects:"
            sqlStmt = "SELECT COUNT(project) FROM PJPROJ"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of active Projects:"
            sqlStmt = "SELECT COUNT(project) FROM PJPROJ WHERE status_pa = 'A'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of active Projects billed to a Customer:"
            sqlStmt = "SELECT COUNT(project) FROM PJPROJ WHERE status_pa = 'A' AND RTRIM(customer) <> ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account Categories:"
            sqlStmt = "SELECT COUNT(acct) FROM PJACCT"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Tasks:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENT"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of active Tasks:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENT WHERE status_pa = 'A'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of active Tasks with a Task ID longer than 20 characters:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENT WHERE status_pa = 'A' AND LEN(pjt_entity) > 20"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                'Display to Project and Task IDs
                sqlStmt = "SELECT project, pjt_entity FROM PJPENT WHERE status_pa = 'A' AND LEN(pjt_entity) > 20"
                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                If (sqlReader.HasRows()) Then


                    While sqlReader.Read()
                        Call SetProjTaskListValues(sqlReader, bProjTaskListInfo)
                        RecID = RecID + 1
                        sDescr = " - " + "Project: " + bProjTaskListInfo.ProjectID.Trim + " Task: " + bProjTaskListInfo.TaskID.Trim
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

                Call sqlReader.Close()

            End If

            RecID = RecID + 1
            sDescr = "Number of Equipment/Resource IDs:"
            sqlStmt = "SELECT COUNT(equip_id) FROM PJEQUIP"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Fiscal Period Maintenance records:"
            sqlStmt = "SELECT COUNT(fiscalno) FROM PJFISCAL"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Site Maintenance records:"
            sqlStmt = "SELECT COUNT(TerminalCode) FROM PJSITE"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Address records:"
            sqlStmt = "SELECT COUNT(*) FROM PJADDR"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Master Table Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Document/Transaction Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Project Controller Document/Transaction Counts")
            sAnalysisType = "Document/Transaction Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry batches:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGH"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project-Task Current Summary (PJPTDSUM) records:"
            sqlStmt = "SELECT COUNT(*) FROM PJPTDSUM"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Budget by Period Summary (PJBUDSUM) records:"
            sqlStmt = "SELECT COUNT(*) FROM PJBUDSUM"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Resource Assignment records:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENTEM"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Total number of PJTRAN records:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Modules creating PJTRAN records:"
            sqlStmt = "SELECT COUNT(DISTINCT(system_cd)) FROM PJTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Company IDs used on PJTRAN transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CpnyID)) FROM PJTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs used on PJTRAN transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryID)) FROM PJTran WHERE RTRIM(CuryID) <> ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of PJTRAN records per day over last 365 days:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            calcValDbl1 = (retValInt1 / 365)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of PJTRAN records per week over last 365 days:"
            calcValDbl1 = (retValInt1 / 52)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of PJTRAN records per month over last 365 days:"
            calcValDbl1 = (retValInt1 / 12)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Document/Transaction Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Data Integrity Checks ===
            Call oEventLog.LogMessage(0, "Performing Project Controller Data Integrity Checks")
            sAnalysisType = "Data Integrity Checks"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Projects with an orphaned Customer ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE customer NOT IN (SELECT CustID FROM Customer) AND RTRIM(customer) <> ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects with an orphaned Project Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE ProjCuryId NOT IN (SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects with an orphaned Billing Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE billcuryid NOT IN (SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Tasks with an orphaned Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENT WHERE project NOT IN (SELECT project FROM PJPROJ)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Employee and Maintenance records with an orphaned Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJEMPLOY WHERE RTRIM(em_id01) <> '' AND RTRIM(em_id01) NOT IN (SELECT RTRIM(VendID) FROM Vendor)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions with an orphaned Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE project NOT IN (SELECT project FROM PJPROJ)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions with an orphaned Task ID:"
            sqlQuery = "SELECT COUNT(*) FROM PJCHARGD"
            sqlQuery = sqlQuery + " LEFT OUTER JOIN PJPENT ON PJPENT.project = PJCHARGD.project AND PJPENT.pjt_entity = PJCHARGD.pjt_entity"
            sqlQuery = sqlQuery + " WHERE PJPENT.pjt_entity IS NULL"
            Call sqlFetch_Num(retValInt1, sqlQuery, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions with an orphaned Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE acct NOT IN (SELECT acct FROM PJACCT)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJPTDSUM records with an orphaned Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPTDSUM WHERE project NOT IN (SELECT project FROM PJPROJ)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJPTDSUM records with an orphaned Task ID:"
            sqlQuery = "SELECT COUNT(*) FROM PJPTDSUM"
            sqlQuery = sqlQuery + " LEFT OUTER JOIN PJPENT ON PJPENT.project = PJPTDSUM.project AND PJPENT.pjt_entity = PJPTDSUM.pjt_entity"
            sqlQuery = sqlQuery + " WHERE PJPENT.pjt_entity IS NULL"
            Call sqlFetch_Num(retValInt1, sqlQuery, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJPTDSUM records with an orphaned Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJPTDSUM WHERE acct NOT IN (SELECT acct FROM PJACCT)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJBUDSUM records with an orphaned Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJBUDSUM WHERE project NOT IN (SELECT project FROM PJPROJ)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJBUDSUM records with an orphaned Task ID:"
            sqlQuery = "SELECT COUNT(*) FROM PJBUDSUM"
            sqlQuery = sqlQuery + " LEFT OUTER JOIN PJPENT ON PJPENT.project = PJBUDSUM.project AND PJPENT.pjt_entity = PJBUDSUM.pjt_entity"
            sqlQuery = sqlQuery + " WHERE PJPENT.pjt_entity IS NULL"
            Call sqlFetch_Num(retValInt1, sqlQuery, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJBUDSUM records with an orphaned Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJBUDSUM WHERE acct NOT IN (SELECT acct FROM PJACCT)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Resource Assignment records with an orphaned Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENTEM WHERE project NOT IN (SELECT project FROM PJPROJ)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Resource Assignment records with an orphaned Task ID:"
            sqlQuery = "SELECT COUNT(*) FROM PJPENTEM"
            sqlQuery = sqlQuery + " LEFT OUTER JOIN PJPENT ON PJPENT.project = PJPENTEM.project AND PJPENT.pjt_entity = PJPENTEM.pjt_entity"
            sqlQuery = sqlQuery + " WHERE PJPENT.pjt_entity IS NULL"
            Call sqlFetch_Num(retValInt1, sqlQuery, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Resource Assignment records with an orphaned Employee ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENTEM WHERE Employee NOT IN (SELECT employee FROM PJEMPLOY)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records with an orphaned Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE project NOT IN (SELECT project FROM PJPROJ)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records with an orphaned Task ID:"
            sqlQuery = "SELECT COUNT(*) FROM PJTRAN"
            sqlQuery = sqlQuery + " LEFT OUTER JOIN PJPENT ON PJPENT.project = PJTRAN.project AND PJPENT.pjt_entity = PJTRAN.pjt_entity"
            sqlQuery = sqlQuery + " WHERE PJPENT.pjt_entity IS NULL"
            Call sqlFetch_Num(retValInt1, sqlQuery, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records with an orphaned Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE acct NOT IN (SELECT acct FROM PJACCT)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects missing a Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE RTRIM(project) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects missing a Project Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE RTRIM(ProjCuryId) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects missing a Billing Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE RTRIM(billcuryid) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Projects missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPROJ WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Tasks missing a Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENT WHERE RTRIM(project) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Tasks missing a Task ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENT WHERE RTRIM(pjt_entity) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions missing a Batch ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE RTRIM(batch_id) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions missing a Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE RTRIM(project) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions missing a Task ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE RTRIM(pjt_entity) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions missing an Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE RTRIM(acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Project Charge Entry transactions missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJCHARGD WHERE RTRIM(cpnyId) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJPTDSUM records missing a Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPTDSUM WHERE RTRIM(project) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJPTDSUM records missing a Task ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPTDSUM WHERE RTRIM(pjt_entity) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJPTDSUM records missing an Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJPTDSUM WHERE RTRIM(acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJBUDSUM records missing a Fiscal Year:"
            sqlStmt = "SELECT COUNT(*) FROM PJBUDSUM WHERE RTRIM(fsyear_num) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJBUDSUM records missing a Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJBUDSUM WHERE RTRIM(project) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJBUDSUM records missing a Task ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJBUDSUM WHERE RTRIM(pjt_entity) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJBUDSUM records missing an Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJBUDSUM WHERE RTRIM(acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Resource Assignment records missing a Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENTEM WHERE RTRIM(project) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Resource Assignment records missing a Task ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENTEM WHERE RTRIM(pjt_entity) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Resource Assignment records missing a Employee ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJPENTEM WHERE RTRIM(Employee) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records missing a Fiscal Year:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE RTRIM(fiscalno) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records missing a system_cd value:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE RTRIM(system_cd) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records missing a Batch ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE RTRIM(batch_id) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records missing a Project ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE RTRIM(project) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records missing a Task ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE RTRIM(pjt_entity) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records missing an Account Category:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE RTRIM(acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PJTRAN records missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PJTRAN WHERE RTRIM(CpnyId) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")


        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - PA")
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Project Controller data"
            OkToContinue = False

        End Try

    End Sub

End Module
