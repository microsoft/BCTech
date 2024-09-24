Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_MC
    '================================================================================
    ' This module contains code to analyze tables used with the Multi-Company Module
    '================================================================================

    Public Sub Analyze_MC()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "MC"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValInt2 As Integer
        Dim retValDbl1 As Double
        Dim retValStr1 As String = String.Empty
        Dim calcValDbl1 As Double
        Dim perNbrDelete As String = String.Empty
        Dim sqlStringRet As String = String.Empty

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing

        Try

            '===== Multi-Company =====

            '=== Module Usage ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Multi-Company Module Usage"

            Call oEventLog.LogMessage(0, "MULTI-COMPANY")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Multi-Company Module Usage")

            sAnalysisType = "Module Usage"

            RecID = RecID + 1
            sDescr = "Is the module being used?"
            sqlStmt = "SELECT COUNT(MCActivated) FROM MCSetup WHERE MCActivated = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)

            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND Rlsed = 1"
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
            sDescr = "Is Multi-company processing enabled in Multi-Company Setup?"
            sqlStmt = "SELECT COUNT(*) FROM MCSetup WHERE MCActivated = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)


            If retValInt1 > 0 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Application Database Name:"
            sResult = DBName.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "- Is Multi-company processing enabled for this application database?"

            sqlStmt = "SELECT BaseCuryID, Central_Cash_Cntl, COAOrder, CpnyId, LedgerID, MCActive, Mult_Cpny_Db, NbrPer, PerNbr, PerRetHist, PerRetModTran, PerRetTran, RetEarnAcct, ValidateAcctSub, YtdNetIncAcct FROM GLSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetGLSetupValues(sqlReader, bGLSetupInfo)
            End If
            Call sqlReader.Close()
            If bGLSetupInfo.MCActive = 1 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "- Are multiple companies allowed in a single database for this application database?"
            If bGLSetupInfo.Mult_Cpny_Db = 1 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "- Is Centralized Cash Processing enabled for this application database?"
            If bGLSetupInfo.Central_Cash_Cntl = 1 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "- Master Company ID for this application database?"
            sResult = bGLSetupInfo.CpnyId.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Period of the oldest intercompany transaction:"
            sqlStmt = "SELECT MIN(PerPost) FROM GLTran WHERE TranType = 'IC'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Try
                    retValStr1 = sqlReader(0)
                Catch ex As Exception
                    retValStr1 = String.Empty
                End Try
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
            sDescr = "Period of the newest intercompany transaction:"
            sqlStmt = "SELECT MAX(PerPost) FROM GLTran WHERE TranType = 'IC'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Try
                    retValStr1 = sqlReader(0)
                Catch ex As Exception
                    retValStr1 = String.Empty
                End Try
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

            Call oEventLog.LogMessage(0, "")

            '***** End of Module Usage section *****

            '=== Master Table Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Multi-Company Master Table Counts")
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Companies in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(CpnyID) FROM xSLAnalysisCpny"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then

                'Display the names of all Company IDs linked to the system DB

                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sResult = ""
                    sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                    sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                    Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                Next
            End If

            RecID = RecID + 1
            sDescr = "Number of active Companies in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(CpnyID) FROM xSLAnalysisCpny WHERE Active = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                'Display the names of all active Company IDs linked to the system DB
                For Each Cpny As CpnyDatabase In CpnyDBList
                    If (Cpny.Active = 1) Then
                        RecID = RecID + 1
                        sDescr = " - " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If

                Next
            End If

            RecID = RecID + 1
            sDescr = "Number of inactive Companies in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(CpnyID) FROM xSLAnalysisCpny WHERE Active = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                'Display the names of all active Company IDs linked to the system DB
                For Each Cpny As CpnyDatabase In CpnyDBList
                    If (Cpny.Active = 0) Then

                        RecID = RecID + 1
                        sDescr = " - " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If

                Next

            End If

            RecID = RecID + 1
            sDescr = "Number of application databases linked to system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(DISTINCT DatabaseName) FROM xSLAnalysisCpny"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 1 Then
                'Display the names of all application databases linked to the system DB
                Dim DBNameString As String = String.Empty

                RecID = RecID + 1
                sDescr = "Name of application databases linked to system DB (" + SysDBName.Trim + "):"
                sResult = ""
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                sqlStmt = "SELECT Distinct(DatabaseName) FROM xSLAnalysisCpny"
                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                If (sqlReader.HasRows()) Then

                    ' Get the unique database names from the company list.

                    While sqlReader.Read()

                        Try
                            bMCDatabases.MCDatabaseName = sqlReader(0)
                        Catch ex As Exception
                            bMCDatabases.MCDatabaseName = String.Empty
                        End Try
                        RecID = RecID + 1
                        sDescr = "     - " + bMCDatabases.MCDatabaseName.ToUpper
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

                Call sqlReader.Close()

            End If

            RecID = RecID + 1
            sDescr = "Number of Companies in the logged in application DB (" + DBName.Trim + "):"
            sqlStmt = "SELECT COUNT(CpnyID) FROM vs_Company WHERE DatabaseName =" + SParm(DBName.Trim)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 1 Then
                MultiCpnyAppDB = True
            Else
                MultiCpnyAppDB = False
            End If
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then

                For Each Cpny As CpnyDatabase In CpnyDBList
                    If (String.Compare(Cpny.DatabaseName.Trim, DBName.Trim) = 0) Then
                        RecID = RecID + 1
                        sDescr = " - " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If

                Next
            End If

            RecID = RecID + 1
            sDescr = "Number of active Companies in the logged in application DB (" + DBName.Trim + "):"
            sqlStmt = "SELECT COUNT(CpnyID) FROM xSLAnalysisCpny WHERE Active = 1 AND DatabaseName =" + SParm(DBName.Trim)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                'Display the names of all active Company IDs linked to the application DB
                sqlStmt = "SELECT Distinct(CpnyID), CpnyName FROM xSLAnalysisCpny WHERE Active = 1 AND DatabaseName =" + SParm(DBName.Trim) + "ORDER BY CpnyID"
                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()
                        Try
                            bCpnyIDListAppTemp.CompanyID = sqlReader(0)
                            bCpnyIDListAppTemp.CompanyName = sqlReader(1)
                        Catch ex As Exception
                            bCpnyIDListAppTemp.CompanyID = String.Empty
                            bCpnyIDListAppTemp.CompanyName = String.Empty

                        End Try
                        RecID = RecID + 1
                        sDescr = " - " + bCpnyIDListAppTemp.CompanyID.Trim + " " + bCpnyIDListAppTemp.CompanyName.Trim()
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If
                Call sqlReader.Close()

            End If

            RecID = RecID + 1
            sDescr = "Number of inactive Companies in the logged in application DB (" + DBName.Trim + "):"
            sqlStmt = "SELECT COUNT(CpnyID) FROM xSLAnalysisCpny WHERE Active = 0 AND DatabaseName =" + SParm(DBName.Trim)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                'Display the names of all active Company IDs linked to the application DB
                sqlStmt = "SELECT Distinct(CpnyID), CpnyName FROM xSLAnalysisCpny WHERE Active = 0 AND DatabaseName =" + SParm(DBName.Trim) + "ORDER BY CpnyID"
                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()
                        Try
                            bCpnyIDListAppTemp.CompanyID = sqlReader(0)
                            bCpnyIDListAppTemp.CompanyName = sqlReader(1)
                        Catch ex As Exception
                            bCpnyIDListAppTemp.CompanyID = String.Empty
                            bCpnyIDListAppTemp.CompanyName = String.Empty

                        End Try

                        RecID = RecID + 1
                        sDescr = " - " + bCpnyIDListAppTemp.CompanyID.Trim + " " + bCpnyIDListAppTemp.CompanyName.Trim()
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If

            RecID = RecID + 1
            sDescr = "Number of intercompany account / subaccount records:"
            sqlStmt = "SELECT COUNT(*) FROM Intercompany WHERE RTRIM(FromAcct) <> ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)

            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                For Each Cpny As CpnyDatabase In CpnyDBList

                    RecID = RecID + 1
                    sDescr = " - From CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Intercompany WHERE RTRIM(FromAcct) <> '' AND FromCompany =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)

                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            Call oEventLog.LogMessage(0, "")


            '***** End of Master Table Counts section *****

            '=== Document/Transaction Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Multi-Company Document/Transaction Counts")
            sAnalysisType = "Document/Transaction Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of unique modules creating intercompany transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(Module)) FROM GLTran WHERE TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 1 Then
                'Display the Module IDs
                Call InitUnboundList30Values(bUnboundList30)
                sqlStmt = "SELECT DISTINCT(Module) FROM GLTran WHERE TranType = 'IC'"
                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                While sqlReader.Read()
                    Call SetUnboundList30Values(sqlReader, bUnboundList30)
                    RecID = RecID + 1
                    sDescr = " - " + bUnboundList30.ListID.Trim
                    sResult = ""
                    sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                    sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                    Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                End While
            End If
            Call sqlReader.Close()

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs used on intercompany transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryID)) FROM GLTran WHERE TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 1 Then
                'Display the Currency IDs
                Call InitUnboundList30Values(bUnboundList30)
                sqlStmt = "SELECT DISTINCT(CuryId) FROM GLTran WHERE TranType = 'IC'"
                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                While sqlReader.Read()

                    RecID = RecID + 1
                    sDescr = " - " + bUnboundList30.ListID.Trim
                    sResult = ""
                    sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                    sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                    Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                End While
                Call sqlReader.Close()
            End If

            RecID = RecID + 1
            sDescr = "Number of unique Company IDs used on intercompany transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CpnyID)) FROM GLTran WHERE TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transactions:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt2, sqlStmt, SqlAppDbConn)

                    sResult = retValInt2
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            If retValInt1 > 0 Then
                RecID = RecID + 1
                perNbrDelete = PeriodPlusPerNum(bGLSetupInfo.PerNbr, -1 * (bGLSetupInfo.PerRetTran + 1))
                sDescr = "Number of intercompany trans outside of retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
                sqlStringRet = "SELECT COUNT(*) FROM GLTran WHERE Module = 'GL' AND Posted = 'P' AND TranType = 'IC' AND PerPost <=" + SParm(perNbrDelete) + "AND PerEnt <" + SParm(bGLSetupInfo.PerNbr)
                Call sqlFetch_Num(retValInt1, sqlStringRet, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Total number of intercompany transactions posted to a Company that exists in another application DB:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE IC_Distribution IN (1, 2)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE IC_Distribution IN (1, 2) AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            RecID = RecID + 1
            sDescr = "- Number of these transactions that have not been processed by Inter-company Export/Import:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE IC_Distribution = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE IC_Distribution = 1 AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            RecID = RecID + 1
            sDescr = "Number of intercompany transactions imported from a Company that exists in another application DB:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE IC_Distribution = 3"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 0 Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - OrigCpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE IC_Distribution = 3 AND OrigCpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            If MultiCpnyAppDB = True Then
                RecID = RecID + 1
                sDescr = "Average number of intercompany transactions per day over last 365 days:"
                sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND Crtd_DateTime >=" + SParm(LastYrDateStr)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                calcValDbl1 = (retValInt1 / 365)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                If retValInt1 > 0 Then
                    For Each Cpny As CpnyDatabase In CpnyDBList
                        RecID = RecID + 1
                        sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                        sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
                        Call sqlFetch_Num(retValInt2, sqlStmt, SqlAppDbConn)
                        calcValDbl1 = (retValInt2 / 365)
                        calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                        sResult = calcValDbl1
                        If calcValDbl1 > 0 Then
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                        End If
                    Next
                End If

                RecID = RecID + 1
                sDescr = "Average number of intercompany transactions per week over last 365 days:"
                calcValDbl1 = (retValInt1 / 52)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                If retValInt1 > 0 Then
                    For Each Cpny As CpnyDatabase In CpnyDBList
                        RecID = RecID + 1
                        sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                        sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
                        Call sqlFetch_Num(retValInt2, sqlStmt, SqlAppDbConn)
                        calcValDbl1 = (retValInt2 / 52)
                        calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                        sResult = calcValDbl1
                        If calcValDbl1 > 0 Then
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                        End If
                    Next
                End If

                RecID = RecID + 1
                sDescr = "Average number of intercompany transactions per month over last 365 days:"
                calcValDbl1 = (retValInt1 / 12)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                If retValInt1 > 0 Then
                    For Each Cpny As CpnyDatabase In CpnyDBList
                        RecID = RecID + 1
                        sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                        sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)

                        Call sqlFetch_Num(retValInt2, sqlStmt, SqlAppDbConn)
                        calcValDbl1 = (retValInt2 / 12)
                        calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                        sResult = calcValDbl1
                        If calcValDbl1 > 0 Then
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                        End If
                    Next
                End If

            Else
                RecID = RecID + 1
                sDescr = "Average number of intercompany transactions per day over last 365 days:"
                sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND Crtd_DateTime >=" + SParm(LastYrDateStr)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                calcValDbl1 = (retValInt1 / 365)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                RecID = RecID + 1
                sDescr = "Average number of intercompany transactions per week over last 365 days:"
                calcValDbl1 = (retValInt1 / 52)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                RecID = RecID + 1
                sDescr = "Average number of intercompany transactions per month over last 365 days:"
                calcValDbl1 = (retValInt1 / 12)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If


            '***** End of Document/Transaction Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Data Integrity Checks ===
            Call oEventLog.LogMessage(0, "Performing Multi-Company Data Integrity Checks")
            sAnalysisType = "Data Integrity Checks"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records with an orphaned Account:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE Acct NOT IN (SELECT Acct FROM Account) AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records with an orphaned Subaccount:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE Sub NOT IN (SELECT Sub FROM Subacct) AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records with an orphaned Ledger ID:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE LedgerID NOT IN (SELECT LedgerID FROM Ledger) AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE CuryID NOT IN (SELECT CuryID FROM Currncy) AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny) AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records missing a Module ID:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(Module) = '' AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records missing a Batch Number:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(BatNbr) = '' AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records missing an Account:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(Acct) = '' AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records missing an Subaccount:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(Sub) = '' AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records missing an Ledger ID:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(LedgerID) = '' AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records missing an Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(CuryID) = '' AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of intercompany transaction records missing an Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(CpnyID) = '' AND TranType = 'IC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - MC")
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Multi-Company data"
            OkToContinue = False

        End Try

    End Sub

End Module
