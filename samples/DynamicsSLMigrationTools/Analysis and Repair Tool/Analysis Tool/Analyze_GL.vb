Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_GL
    '================================================================================
    ' This module contains code to analyze tables used with the General Ledger Module
    '
    '================================================================================

    Public Sub Analyze_GL()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "GL"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValInt2 As Integer
        Dim retValDbl1 As Double
        Dim retValStr1 As String = String.Empty
        Dim calcValDbl1 As Double
        Dim currYearGL As String = String.Empty
        Dim fiscYearDelete As String = String.Empty
        Dim perNbrDelete As String = String.Empty
        Dim sqlStringRet As String = String.Empty
        Dim sqlStmt As String = String.Empty

        Dim sqlReader As SqlDataReader = Nothing

        Try
            Form1.AnalysisStatusLbl.Text = "Analyzing General Ledger"

            '====== General Ledger ======

            '=== Module Usage ===
            Form1.AnalysisStatusLbl.Text = "Analyzing General Ledger Module Usage"

            Call oEventLog.LogMessage(0, "GENERAL LEDGER")
            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "Analyzing General Ledger Module Usage")

            sAnalysisType = "Module Usage"

            RecID = RecID + 1
            sDescr = "Is the module being used?"
            sqlStmt = "SELECT COUNT(Init) FROM GLSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM GLTran WHERE Rlsed = 1"
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
            sDescr = "Number of unreleased batches:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM Batch WHERE Module = 'GL' AND Rlsed = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = CStr(retValInt1)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unposted batches across all modules:"
            sqlStmt = "SELECT COUNT(Status) FROM Batch WHERE Status = 'U'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = CStr(retValInt1)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of periods being used?"
            sqlStmt = "SELECT BaseCuryID, Central_Cash_Cntl, COAOrder, CpnyId, LedgerID, MCActive, Mult_Cpny_Db, NbrPer, PerNbr, PerRetHist, PerRetModTran, PerRetTran, RetEarnAcct, ValidateAcctSub, YtdNetIncAcct FROM GLSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetGLSetupValues(sqlReader, bGLSetupInfo)
            Else
                Call InitGLSetupValues(bGLSetupInfo)
            End If
            Call sqlReader.Close()

            sResult = bGLSetupInfo.NbrPer
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "What Chart of Account order is being used?"
            Select Case bGLSetupInfo.COAOrder
                Case "A"
                    sResult = "Assets, Liabilities, Income & Expenses"
                Case "B"
                    sResult = "Assets, Liabilities, Income, Expenses"
                Case "C"
                    sResult = "Income, Expenses, Assets, Liabilities"
                Case "D"
                    sResult = "Income & Expenses, Assets, Liabilities"
                Case "E"
                    sResult = "Custom Order"
                Case Else
                    sResult = "ERROR"
            End Select
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Is validate account/subaccount enabled?"
            If bGLSetupInfo.ValidateAcctSub = 1 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Current period for the module:"
            sResult = bGLSetupInfo.PerNbr.Substring(4, 2) + "-" + bGLSetupInfo.PerNbr.Substring(0, 4)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Periods to Retain Module Transactions:"
            sResult = bGLSetupInfo.PerRetModTran
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ''''''''''''''''''''''''''
            'Set GLRetention and currYearGL variables
            GLRetention = PeriodPlusPerNum(bGLSetupInfo.PerNbr, -1 * (bGLSetupInfo.PerRetModTran + 1))
            currYearGL = bGLSetupInfo.PerNbr.Substring(0, 4)
            ''''''''''''''''''''''''''

            RecID = RecID + 1
            sDescr = "Periods to Retain GL Transactions:"
            sResult = bGLSetupInfo.PerRetTran
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Years to Retain GL Balances:"
            sResult = bGLSetupInfo.PerRetHist
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Oldest Period to Post used on GL Batches:"
            sqlStmt = "SELECT MIN(PerPost) FROM Batch WHERE Module = 'GL' AND Status NOT IN ('D', 'V')"
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
            If (String.IsNullOrEmpty(retValStr1) = False) Then
                sResult = retValStr1.Substring(4, 2) + "-" + retValStr1.Substring(0, 4)
            Else
                sResult = String.Empty
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Newest Period to Post used on GL Batches:"
            sqlStmt = "SELECT MAX(PerPost) FROM Batch WHERE Module = 'GL' AND Status NOT IN ('D', 'V')"
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
            If (String.IsNullOrEmpty(retValStr1) = False) Then
                sResult = retValStr1.Substring(4, 2) + "-" + retValStr1.Substring(0, 4)
            Else
                sResult = String.Empty
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Fiscal year of the oldest Account History record:"
            sqlStmt = "SELECT MIN(FiscYr) FROM AcctHist"
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
            sResult = retValStr1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            '**************************************************************************
            '*** Verify GL is in balance (Assets = Income - Expenses + Liabilities) ***
            '**************************************************************************
            Dim curBal_A As Double
            Dim curBal_I As Double
            Dim curBal_E As Double
            Dim curBal_L As Double
            Dim checkVal As Double
            Dim diffVal As Double

            'Get current balance for Assets
            sqlStmt = "xSLMPT_AcctCurBalByType" + SParm(CpnyId) + "," + SParm(bGLSetupInfo.PerNbr.Substring(0, 4)) + "," + SParm(bGLSetupInfo.YtdNetIncAcct) + ", 'A'"
            Call sqlFetch_Num(curBal_A, sqlStmt, SqlAppDbConn)
            curBal_A = FPRnd(curBal_A, 2)

            'Get current balance for Income
            sqlStmt = "xSLMPT_AcctCurBalByType" + SParm(CpnyId) + "," + SParm(bGLSetupInfo.PerNbr.Substring(0, 4)) + "," + SParm(bGLSetupInfo.YtdNetIncAcct) + ", 'I'"
            Call sqlFetch_Num(curBal_I, sqlStmt, SqlAppDbConn)
            curBal_I = FPRnd(curBal_I, 2)

            'Get current balance for Expense
            sqlStmt = "xSLMPT_AcctCurBalByType" + SParm(CpnyId) + "," + SParm(bGLSetupInfo.PerNbr.Substring(0, 4)) + "," + SParm(bGLSetupInfo.YtdNetIncAcct) + ", 'E'"
            Call sqlFetch_Num(curBal_E, sqlStmt, SqlAppDbConn)
            curBal_E = FPRnd(curBal_E, 2)

            'Get current balance for Liabilities
            sqlStmt = "xSLMPT_AcctCurBalByType" + SParm(CpnyId) + "," + SParm(bGLSetupInfo.PerNbr.Substring(0, 4)) + "," + SParm(bGLSetupInfo.YtdNetIncAcct) + ", 'L'"
            Call sqlFetch_Num(curBal_L, sqlStmt, SqlAppDbConn)
            curBal_L = FPRnd(curBal_L, 2)

            checkVal = FPRnd((curBal_I - curBal_E + curBal_L), 2)
            If checkVal <> curBal_A Then
                sResult = "NO"
                diffVal = (checkVal - curBal_A)
                diffVal = FPRnd(diffVal, 2)
            Else
                sResult = "YES"
                diffVal = 0
            End If
            RecID = RecID + 1
            sDescr = "Is the General Ledger in balance?"
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = " - Income"
            sResult = "$ " + curBal_I.ToString("N2")
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm("$ " + curBal_I.ToString("N2"))
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = " - Expenses"
            sResult = "$ " + curBal_E.ToString("N2")
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm("$ " + curBal_E.ToString("N2"))
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = " - Liabilities"
            sResult = "$ " + curBal_L.ToString("N2")
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm("$ " + curBal_L.ToString("N2"))
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = " - Assets"
            sResult = "$ " + curBal_A.ToString("N2")
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm("$ " + curBal_A.ToString("N2"))
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = " - Difference [(Income - Expenses + Liabilities) - Assets]"
            sResult = "$ " + diffVal.ToString("N2")
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm("$ " + diffVal.ToString("N2"))
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Module Usage section *****


            Call oEventLog.LogMessage(0, "")

            '=== Master Table Counts ===
            Call oEventLog.LogMessage(0, "Analyzing General Ledger Master Table Counts")
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Accounts:"
            sqlStmt = "SELECT COUNT(Acct) FROM Account"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of active Accounts:"
            sqlStmt = "SELECT COUNT(Acct) FROM Account WHERE Active = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Subaccounts:"
            sqlStmt = "SELECT COUNT(Sub) FROM Subacct"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of active Subaccounts:"
            sqlStmt = "SELECT COUNT(Sub) FROM Subacct WHERE Active = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of account/subaccount combinations:"
            sqlStmt = "SELECT COUNT(*) FROM AcctSub"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of active account/subaccount combinations:"
            sqlStmt = "SELECT COUNT(*) FROM AcctSub WHERE Active = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records:"
            sqlStmt = "SELECT COUNT(*) FROM AcctHist"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                RecID = RecID + 1
                sDescr = "Number of Account History records by Company ID:"
                sResult = ""
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM AcctHist WHERE CpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt2, sqlStmt, SqlAppDbConn)
                    sResult = retValInt2
                    If retValInt2 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If

                Next
            End If

            RecID = RecID + 1
            fiscYearDelete = CStr(FPSub(CDbl(currYearGL), bGLSetupInfo.PerRetHist + 1, 0))
            sDescr = "Number of Account History records outside of retention settings (" + fiscYearDelete.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM AcctHist WHERE FiscYr <=" + SParm(fiscYearDelete)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Ledgers:"
            sqlStmt = "SELECT COUNT(*) FROM Ledger"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Actual Ledgers:"
            sqlStmt = "SELECT COUNT(*) FROM Ledger WHERE BalanceType = 'A'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account Classes:"
            sqlStmt = "SELECT COUNT(*) FROM AcctClass"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Master Table Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Document/Transaction Counts ===
            Call oEventLog.LogMessage(0, "Analyzing General Ledger Document/Transaction Counts")
            sAnalysisType = "Document/Transaction Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of GL batches:"
            sqlStmt = "SELECT COUNT(Module) FROM Batch WHERE Module = 'GL'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'GL' AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            perNbrDelete = PeriodPlusPerNum(bGLSetupInfo.PerNbr, -1 * (bGLSetupInfo.PerRetTran + 1))
            sDescr = "Number of GL batches outside of retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
            sqlStringRet = "SELECT COUNT(*) FROM Batch WHERE Module = 'GL' AND STATUS IN ('V', 'D', 'P') AND PerPost <=" + SParm(perNbrDelete) + "AND PerEnt <" + SParm(bGLSetupInfo.PerNbr)
            Call sqlFetch_Num(retValInt1, sqlStringRet, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of open recurring GL batches:"
            sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'GL' AND BatType = 'R' AND Rlsed = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'GL' AND BatType = 'R' AND Rlsed = 0 AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of batches (all modules):"
            sqlStmt = "SELECT COUNT(BatNbr) FROM Batch"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of unique Company IDs used on GL transactions (Excluding intercompany trans):"
            sqlStmt = "SELECT COUNT(DISTINCT(CpnyID)) FROM GLTran WHERE Module = 'GL' AND TranType <> 'IC' AND IC_Distribution = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of GL transactions (GL module only, excluding intercompany trans):"
            sqlStmt = "SELECT COUNT(BatNbr) FROM GLTran WHERE Module = 'GL' AND TranType <> 'IC' AND IC_Distribution = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE Module = 'GL' AND TranType <> 'IC' AND IC_Distribution = 0 AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            perNbrDelete = PeriodPlusPerNum(bGLSetupInfo.PerNbr, -1 * (bGLSetupInfo.PerRetTran + 1))
            sDescr = "Number of GL module only trans outside of retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
            sqlStringRet = "SELECT COUNT(*) FROM GLTran WHERE Module = 'GL' AND Posted = 'P' AND TranType <> 'IC' AND IC_Distribution = 0 AND PerPost <=" + SParm(perNbrDelete) + "AND PerEnt <" + SParm(bGLSetupInfo.PerNbr)
            Call sqlFetch_Num(retValInt1, sqlStringRet, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs used on GL transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryId)) FROM GLTran WHERE Module = 'GL' AND TranType <> 'IC' AND IC_Distribution = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 1 Then
                'Display the Currency IDs
                sqlStmt = "SELECT DISTINCT(CuryId) FROM GLTran WHERE Module = 'GL' AND TranType <> 'IC' AND IC_Distribution = 0"
                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                If (sqlReader.HasRows()) Then
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
            End If

            RecID = RecID + 1
            sDescr = "Number of GL transactions (all modules, excluding intercompany trans):"
            sqlStmt = "SELECT COUNT(BatNbr) FROM GLTran WHERE TranType <> 'IC' AND IC_Distribution = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType <> 'IC' AND IC_Distribution = 0 AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            perNbrDelete = PeriodPlusPerNum(bGLSetupInfo.PerNbr, -1 * (bGLSetupInfo.PerRetTran + 1))
            sDescr = "Number of non-GL module only trans outside of retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE Module <> 'GL' AND Posted = 'P' AND TranType <> 'IC' AND IC_Distribution = 0 AND PerPost <=" + SParm(perNbrDelete) + "AND PerEnt <" + SParm(bGLSetupInfo.PerNbr)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If MultiCpnyAppDB = True Then
                RecID = RecID + 1
                sDescr = "Average number of transactions per day over last 365 days:"
                sqlStmt = "SELECT COUNT(Rlsed) FROM GLTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
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
                        sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
                sDescr = "Average number of transactions per week over last 365 days:"
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
                        sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)

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
                sDescr = "Average number of transactions per month over last 365 days:"
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
                        sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
                sDescr = "Average number of transactions per day over last 365 days:"
                sqlStmt = "SELECT COUNT(Rlsed) FROM GLTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                calcValDbl1 = (retValInt1 / 365)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                RecID = RecID + 1
                sDescr = "Average number of transactions per week over last 365 days:"
                calcValDbl1 = (retValInt1 / 52)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                RecID = RecID + 1
                sDescr = "Average number of transactions per month over last 365 days:"
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
            Call oEventLog.LogMessage(0, "Performing General Ledger Data Integrity Checks")
            sAnalysisType = "Data Integrity Checks"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Account History records with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(CpnyID) FROM AcctHist WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records with an orphaned Account:"
            sqlStmt = "SELECT COUNT(Acct) FROM AcctHist WHERE Acct NOT IN (SELECT Acct FROM Account)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records with an orphaned Subaccount:"
            sqlStmt = "SELECT COUNT(Sub) FROM AcctHist WHERE Sub NOT IN (SELECT Sub FROM Subacct)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records with an orphaned Ledger ID:"
            sqlStmt = "SELECT COUNT(LedgerID) FROM AcctHist WHERE LedgerID NOT IN (SELECT LedgerID FROM Ledger)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(CuryID) FROM AcctHist WHERE CuryID NOT IN (SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records with an orphaned Account:"
            sqlStmt = "SELECT COUNT(Acct) FROM GLTran WHERE Acct NOT IN (SELECT Acct FROM Account)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records with an orphaned Subaccount:"
            sqlStmt = "SELECT COUNT(Sub) FROM GLTran WHERE Sub NOT IN (SELECT Sub FROM Subacct)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(CuryID) FROM GLTran WHERE CuryID NOT IN (SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records with an orphaned Ledger ID:"
            sqlStmt = "SELECT COUNT(LedgerID) FROM GLTran WHERE LedgerID NOT IN (SELECT LedgerID FROM Ledger)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account records missing an Account:"
            sqlStmt = "SELECT COUNT(Acct) FROM Account WHERE RTRIM(Acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account records missing an Account Type:"
            sqlStmt = "SELECT COUNT(AcctType) FROM Account WHERE RTRIM(AcctType) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records missing a CompanyID:"
            sqlStmt = "SELECT COUNT(CpnyID) FROM AcctHist WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records missing an Account:"
            sqlStmt = "SELECT COUNT(Acct) FROM AcctHist WHERE RTRIM(Acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records missing a Subaccount:"
            sqlStmt = "SELECT COUNT(Sub) FROM AcctHist WHERE RTRIM(Sub) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records missing a Ledger ID:"
            sqlStmt = "SELECT COUNT(LedgerID) FROM AcctHist WHERE RTRIM(LedgerID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records missing a Fiscal Year:"
            sqlStmt = "SELECT COUNT(FiscYr) FROM AcctHist WHERE RTRIM(FiscYr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Account History records missing a Currency ID:"
            sqlStmt = "SELECT COUNT(CuryID) FROM AcctHist WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Batch records missing a Module ID:"
            sqlStmt = "SELECT COUNT(Module) FROM Batch WHERE Status NOT IN ('D', 'V') AND RTRIM(Module) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Batch records missing a Batch number:"
            sqlStmt = "SELECT COUNT(BatNbr) FROM Batch WHERE Status NOT IN ('D', 'V') AND RTRIM(BatNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Batch records missing a Currency ID:"
            sqlStmt = "SELECT COUNT(CuryID) FROM Batch WHERE Status NOT IN ('D', 'V') AND RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records missing a Module ID:"
            sqlStmt = "SELECT COUNT(Module) FROM GLTran WHERE RTRIM(Module) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records missing a Batch Number:"
            sqlStmt = "SELECT COUNT(BatNbr) FROM GLTran WHERE RTRIM(BatNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records missing an Account:"
            sqlStmt = "SELECT COUNT(Acct) FROM GLTran WHERE RTRIM(Acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records missing an Subaccount:"
            sqlStmt = "SELECT COUNT(Sub) FROM GLTran WHERE RTRIM(Sub) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records missing an Ledger ID:"
            sqlStmt = "SELECT COUNT(LedgerID) FROM GLTran WHERE RTRIM(LedgerID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transaction records missing an Currency ID:"
            sqlStmt = "SELECT COUNT(CuryID) FROM GLTran WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of batches that are posted but not released:"
            sqlStmt = "SELECT COUNT(BatNbr) FROM Batch WHERE Status = 'P' AND Rlsed = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of posted transactions missing a period to post:"
            sqlStmt = "SELECT COUNT(PerPost) FROM GLTran WHERE Posted = 'P' AND RTRIM(PerPost) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of posted transactions missing a fiscal year:"
            sqlStmt = "SELECT COUNT(FiscYr) FROM GLTran WHERE Posted = 'P' AND RTRIM(FiscYr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            '***********************************************************************************'
            '*** Check for Subaccount segment values in GLTran that are not the SegDef table ***'
            '***********************************************************************************'
            Dim sqlString As String = String.Empty
            Dim sqlString2 As String = String.Empty
            Dim sqlString3 As String = String.Empty

            Dim sqlDefConn As SqlConnection = Nothing
            Dim sqlDefReader As SqlDataReader = Nothing
            Dim sqlErrConn As SqlConnection = Nothing
            Dim sqlErrReader As SqlDataReader = Nothing

            Dim dbTran As SqlTransaction = Nothing
            Dim sqlInsertStmt As String = String.Empty

            Dim retVal As Integer

            Dim StartPos As Integer = 0

            Dim SegDefLengthArray As List(Of Short) = New List(Of Short)

            'Get FlexDef information

            sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'SUBACCOUNT'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then

                Call sqlReader.Read()
                Call SetFlexDefValues(sqlReader, bFlexDefInfo)
            Else
                Call InitFlexDefValues(bFlexDefInfo)
            End If

            ' Create an array of segment lengths.
            SegDefLengthArray.Add(bFlexDefInfo.SegLength00)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength01)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength02)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength03)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength04)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength05)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength06)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength07)

            sqlReader.Close()

            sqlDefConn = New SqlClient.SqlConnection(AppDbConnStr)


            sqlErrConn = New SqlClient.SqlConnection(AppDbConnStr)


            'Check Segment 1
            'Check Each Segment
            StartPos = 1
            For cnt As Integer = 1 To bFlexDefInfo.NumberSegments


                sqlString = "SELECT DISTINCT(SUBSTRING(Sub, " + StartPos.ToString + ", " + SegDefLengthArray(cnt - 1).ToString + ")) FROM GLTran"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()
                        Call SetSubAcctListValues(sqlReader, bSubAcctList)
                        If bSubAcctList.SubAcct.Trim IsNot String.Empty Then

                            If (sqlDefConn.State <> ConnectionState.Open) Then
                                Call sqlDefConn.Open()
                            End If
                            'Check to see if segment value is in the SegDef table
                            sqlString2 = "Select * FROM SegDef WHERE FieldClassName = 'SUBACCOUNT' AND SegNumber =  '" + CStr(cnt) + "' AND ID =" + SParm(bSubAcctList.SubAcct.Trim)

                            Call sqlFetch_1(sqlDefReader, sqlString2, sqlDefConn, CommandType.Text)

                            If sqlDefReader.HasRows() = False Then
                                'Write record to xSLMPTSubErrors table
                                sqlString3 = "SELECT * FROM xSLMPTSubErrors WHERE SegNumber = '1' AND ID =" + SParm(bSubAcctList.SubAcct.Trim)
                                Call sqlFetch_1(sqlErrReader, sqlString3, sqlErrConn, CommandType.Text)

                                If sqlErrReader.HasRows() = False Then
                                    Call sqlErrReader.Close()
                                    bxSLMPTSubErrors.SegNumber = cnt
                                    bxSLMPTSubErrors.ID = bSubAcctList.SubAcct.Trim

                                    If (sqlErrConn.State <> ConnectionState.Open) Then
                                        sqlErrConn.Open()
                                    End If

                                    dbTran = TranBeg(sqlErrConn)

                                    sqlInsertStmt = "Insert into xSLMPTSubErrors ([ID], [SegNumber]) Values(" + SParm(bSubAcctList.SubAcct.Trim) + "," + SParm(cnt.ToString) + ")"


                                    retVal = sql_1(sqlErrReader, sqlInsertStmt, sqlErrConn, OperationType.InsertOp, CommandType.Text, dbTran)
                                    If (retVal = 1) Then
                                        statusExists = True
                                    End If
                                    Call TranEnd(dbTran)

                                    sqlErrConn.Close()

                                End If
                                Call sqlErrReader.Close()
                            End If
                            Call sqlDefReader.Close()
                        End If

                    End While
                End If

                If (sqlDefConn.State = ConnectionState.Open) Then
                    Call sqlDefConn.Close()
                End If

                Call sqlReader.Close()

                StartPos = StartPos + SegDefLengthArray(cnt - 1)


            Next

            RecID = RecID + 1
            sDescr = "Number of Subaccount Segment IDs not found in the SegDef table:"
            sqlStmt = "SELECT COUNT(*) FROM xSLMPTSubErrors"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            'If records exist in xSLMPTSubErrors table, write record to the xSLAnalysisSum table
            sqlString = "SELECT * FROM xSLMPTSubErrors ORDER BY SegNumber, ID"
            Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                While sqlReader.Read()
                    Call SetxSLMPTSubValues(sqlReader, bxSLMPTSubErrors)
                    RecID = RecID + 1
                    sDescr = " - Segment Number: " + bxSLMPTSubErrors.SegNumber.Trim + "     Segment ID: " + bxSLMPTSubErrors.ID.Trim
                    sResult = ""
                    sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                    sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                    Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                End While
            End If
            Call sqlReader.Close()

            'Delete records in xSLMPTSubErrors table
            sqlStmt = "Delete from xSLMPTSubErrors"
            Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")


            '***********************************************************************************'
            '***********************************************************************************'            

        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - GL")
            ' Call TranEnd()
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing General Ledger data"
            OkToContinue = False
        End Try

    End Sub


End Module
