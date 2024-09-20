
Imports System.Data.SqlClient

Module AccountCode
    '================================================================================
    ' This module contains code to prepare the Chart of Accounts data for migration
    '
    '================================================================================

    Public Sub Validate()
        '=============================================================================
        ' Validate Chart of Accounts records
        '   - Account.Acct is not blank
        '   - Account.AcctType is not blank
        '   - Check for recurring Invoices
        '   - Verify posted tran amounts match AcctHist amount
        '=============================================================================
        Dim nbrAcctBlank As Integer
        Dim nbrAcctTypeBlank As Integer
        Dim retValInt As Integer
        Dim recCorrected As Boolean
        Dim acctCurrBal As Double
        Dim msgText As String = String.Empty
        Dim sqlString As String = String.Empty
        Dim perNumber As String = String.Empty
        Dim recurBatches As Integer


        Dim glTranCount As Integer = 0
        Dim fetchGLTran As Integer = 0
        Dim sqlStmt As String = String.Empty
        Dim nbrKeysBlk As Integer = 0
        Dim KeyString As String = String.Empty

        Dim oEventLog As clsEventLog
        Dim sqlReader As SqlDataReader = Nothing

        Dim AcctList As List(Of String) = New List(Of String)
        Dim AcctNum As String = ""

        Dim delTran As SqlTransaction = Nothing
        Dim updTran As SqlTransaction = Nothing

        Dim updConn As SqlConnection = Nothing
        Dim delConn As SqlConnection = Nothing

        Dim fmtDate As String

        fmtDate = Date.Now.ToString
        fmtDate = fmtDate.Replace(":", "")
        fmtDate = fmtDate.Replace("/", "")
        fmtDate = fmtDate.Remove(fmtDate.Length - 3)
        fmtDate = fmtDate.Replace(" ", "-")
        fmtDate = fmtDate & Date.Now.Millisecond

        oEventLog = New clsEventLog
        oEventLog.FileName = "SL" & "-GL-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")


        '**********************************************************************************************************************************************
        '*** Delete any Account records where Acct field is blank since this is the only key field in this table (Applies to all migration methods) ***
        '**********************************************************************************************************************************************
        ' Export all GL Accounts regardless of Status 
        Call sqlFetch_Num(nbrAcctBlank, "SELECT COUNT(*) FROM Account WHERE RTRIM(Acct) = ''", SqlAppDbConn)
        If nbrAcctBlank > 0 Then
            Call LogMessage("Number of Account records found with a blank Acct Field: " + CStr(nbrAcctBlank), oEventLog)


            'Delete Account records with a blank Acct field and log the number Account records deleted
            Try


                updConn = New SqlConnection(AppDbConnStr)

                updConn.Open()


                sqlStmt = "DELETE FROM Account WHERE RTRIM(Acct) = ''"
                Call sql_1(sqlReader, sqlStmt, updConn, OperationType.DeleteOp, CommandType.Text)


                    Call LogMessage("Deleted " + CStr(nbrAcctBlank) + " Account record(s) with a blank Acct field.", oEventLog)
                    Call LogMessage("", oEventLog)

                updConn.Close()


            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting Account record(s) with a blank Acct field", oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_COA = NbrOfErrors_COA + 1

            End Try

        End If

        '*****************************************************************************************************
        '*** Look for any Account records where AcctType field is blank (Applies to all migration methods) ***
        '*****************************************************************************************************
        If OkToContinue = True Then

            Call sqlFetch_Num(nbrAcctTypeBlank, "SELECT COUNT(*) FROM Account WHERE RTRIM(AcctType) = ''", SqlAppDbConn)

            If nbrAcctTypeBlank > 0 Then
                Try
                    Call LogMessage("Number of Account records found with a blank AcctType Field: " + CStr(nbrAcctTypeBlank), oEventLog)

                    sqlStmt = "SELECT Acct FROM Account WHERE RTRIM(AcctType) = ''"

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    ' Determine if the status record exists.
                    If (sqlReader.HasRows = True) Then

                        While sqlReader.Read()
                            'Insert record into list
                            AcctNum = sqlReader("Acct")
                            AcctList.Add(AcctNum.Trim)
                        End While

                        sqlReader.Close()

                        For Each Acct As String In AcctList
                            recCorrected = False

                            'Check to see if there are any AcctHist records
                            Call sqlFetch_Num(retValInt, "SELECT COUNT(*) FROM AcctHist WHERE Acct =" + SParm(Acct), SqlAppDbConn)

                            If retValInt = 0 Then


                                delConn = New SqlConnection(AppDbConnStr)
                                delConn.Open()


                                'Delete Account record since there are no posted amounts
                                sqlStmt = "DELETE FROM Account WHERE Acct =" + SParm(Acct)

                                Call sql_1(sqlReader, sqlStmt, delConn, OperationType.DeleteOp, CommandType.Text)

                                Call LogMessage("Deleted Account record (" + Acct.Trim + ") with a blank AcctType field and no AcctHist records", oEventLog)

                                recCorrected = True

                                delConn.Close()



                            End If

                            If recCorrected = False Then
                                'Check to see if the current balance is zero
                                acctCurrBal = CalcCurrBalanceAcctSub(Acct.Trim, "%")

                                If acctCurrBal = 0 Then
                                    'Delete Account record since the current balance is zero
                                    sqlStmt = "DELETE FROM Account WHERE Acct =" + SParm(Acct)
                                    Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                                    Call LogMessage("Deleted Account record (" + Acct.Trim + ") with a blank AcctType field and a current balance of 0", oEventLog)

                                    recCorrected = True

                                Else
                                    'Write information to the log file since the current balance is not zero
                                    msgText = "ERROR: Account " + Acct.Trim + " is missing a required field (AcctType). "
                                    msgText = msgText + "This account record cannot be deleted because the current balance is not zero. "
                                    Call LogMessage("", oEventLog)
                                    Call LogMessage(msgText, oEventLog)
                                    NbrOfErrors_COA = NbrOfErrors_COA + 1
                                End If
                            End If

                        Next

                    End If

                Catch ex As Exception
                    Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                    Call LogMessage("", oEventLog)
                    Call LogMessage("Error encountered while deleting Account record(s) with a blank AcctType field", oEventLog)
                    Call LogMessage("", oEventLog)
                    OkToContinue = False
                    NbrOfErrors_COA = NbrOfErrors_COA + 1
                End Try
            End If

        End If




        '***********************************************************************************'
        '*** Check for Subaccount segment values in GLTran that are not the SegDef table ***'
        '***********************************************************************************'

        If OkToContinue = True Then

            Call ValidateSegDefValue("GLTran", "Sub", False, oEventLog)

        End If  'If OkToContinue = True


        '**************************************
        '*** Check for recurring GL Batches ***
        '**************************************
        Call sqlFetch_Num(recurBatches, "SELECT COUNT(*) FROM Batch WHERE Module = 'GL' AND BatType = 'R' AND NbrCycle > 0", SqlAppDbConn)

        If recurBatches > 0 Then
            'Display a warning message
            LogMessage("", oEventLog)
            msgText = "WARNING: Open Recurring GL Batches exists. Recurring batches will not be migrated and will need to be manually entered in the new system."
            msgText = msgText + " To assist with the move of your recurring batches, the details of your recurring batches can be identified using the"
            msgText = msgText + " Generate Recurring (01.530.00) screen to identify the GL recurring batches identified by this utility."
            Call LogMessage(msgText, oEventLog)

            NbrOfWarnings_COA = NbrOfWarnings_COA + 1
            Call LogMessage("", oEventLog)
        End If


        '  Detect AcctHist records with blank key fields - keys are CpnyId, Acct, Sub, LedgerID, FiscYr
        If OkToContinue = True Then
            sqlStmt = "SELECT COUNT(*) FROM AcctHist WHERE RTRIM(CpnyId) = '' or RTRIM(Acct) = '' or RTRIM(Sub) = '' or RTRIM(LedgerID) = '' or RTRIM(FiscYr) = ''"
            Call sqlFetch_Num(nbrKeysBlk, sqlStmt, SqlAppDbConn)


            If nbrKeysBlk > 0 Then
                Try
                    Call LogMessage("", oEventLog)
                    Call LogMessage("Number of AcctHist records found with a blank Key Field: " + CStr(nbrKeysBlk), oEventLog)

                    ' Get the list of accthist records with one or more blank keys.
                    sqlStmt = "SELECT Acct,Cpnyid,FiscYr,LedgerID,Sub FROM AcctHist WHERE RTRIM(CpnyId) = '' or RTRIM(Acct) = '' or RTRIM(Sub) = '' or RTRIM(LedgerID) = '' or RTRIM(FiscYr) = ''"
                    KeyString = String.Empty

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    ' Determine if the status record exists.
                    If (sqlReader.HasRows = True) Then

                        While sqlReader.Read()

                            Call SetAcctHistValues(sqlReader, bAcctHistList)

                            ' Report in the Event Log.  
                            Call LogMessage("AcctHist record has blank key fields: ", oEventLog)
                            Call LogMessage("  Account:" + bAcctHistList.Acct.Trim, oEventLog)
                            Call LogMessage("  Company:" + bAcctHistList.CpnyId.Trim, oEventLog)
                            Call LogMessage("  Fiscal Year:" + bAcctHistList.FiscYr.Trim, oEventLog)
                            Call LogMessage("  Ledger:" + bAcctHistList.LedgerID.Trim, oEventLog)
                            Call LogMessage("  Subaccout:" + bAcctHistList.SubAcct.Trim, oEventLog)
                            Call LogMessage("", oEventLog)

                        End While
                    End If

                    Call sqlReader.Close()
                Catch
                End Try

            End If
        End If

        '  Detect GLTran records with blank key fields - keys are BatNbr, LineNbr, Module
        If OkToContinue = True Then
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE RTRIM(BatNbr) = '' or RTRIM(Module) = ''"

            Call sqlFetch_Num(nbrKeysBlk, sqlStmt, SqlAppDbConn)

            If nbrKeysBlk > 0 Then
                Try
                    Call LogMessage("", oEventLog)
                    Call LogMessage("Number of GLTran records found with a blank Key Field: " + CStr(nbrKeysBlk), oEventLog)

                    ' Get the list of accthist records with one or more blank keys.
                    sqlStmt = "SELECT Acct, Module, BatNbr, LineNbr, FiscYr, PerPost, CpnyId FROM GLTran WHERE RTRIM(BatNbr) = '' or RTRIM(Module) = ''"

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                    If (sqlReader.HasRows = True) Then

                        While sqlReader.Read()

                            Call SetGLTranValues(sqlReader, bGLTranList)


                            ' Report in the Event Log.  
                            Call LogMessage("GLTran record has blank key fields: ", oEventLog)
                            Call LogMessage("  BatNbr:" + bGLTranList.BatNbr.Trim, oEventLog)
                            Call LogMessage("  Module:" + bGLTranList.ModuleID.Trim, oEventLog)
                            Call LogMessage("  Linenbr:" + CStr(bGLTranList.LineNbr), oEventLog)
                            Call LogMessage("", oEventLog)
                        End While
                    End If
                Catch
                End Try

                sqlReader.Close()
            End If
        End If

        '**************************************************************************
        '*** Verify GL is in balance (Assets = Income - Expenses + Liabilities) ***
        '**************************************************************************
        Dim curBal_A As Double = 0
        Dim curBal_I As Double
        Dim curBal_E As Double
        Dim curBal_L As Double
        Dim checkVal As Double
        Dim diffVal As Double
        Dim RecordParmList As New List(Of ParmList)
        Dim ParmValues As ParmList


        'Get current balance for Assets

        ' Add the stored procedure parameters to the list.
        ParmValues = New ParmList
        ParmValues.ParmName = "parm1"
        ParmValues.ParmType = SqlDbType.VarChar
        ParmValues.ParmValue = CpnyId
        RecordParmList.Add(ParmValues)

        ParmValues = New ParmList
        ParmValues.ParmName = "parm2"
        ParmValues.ParmType = SqlDbType.VarChar
        ParmValues.ParmValue = bGLSetupInfo.PerNbr.Substring(0, 4)
        RecordParmList.Add(ParmValues)


        ParmValues = New ParmList
        ParmValues.ParmName = "parm3"
        ParmValues.ParmType = SqlDbType.VarChar
        ParmValues.ParmValue = bGLSetupInfo.YtdNetIncAcct
        RecordParmList.Add(ParmValues)

        ParmValues = New ParmList
        ParmValues.ParmName = "parm4"
        ParmValues.ParmType = SqlDbType.VarChar
        ParmValues.ParmValue = "A"
        RecordParmList.Add(ParmValues)


        Call sqlFetch_1(sqlReader, "xSLMPT_AcctCurBalByType", SqlAppDbConn, CommandType.StoredProcedure, RecordParmList)
        If (sqlReader.HasRows = True) Then
            Call sqlReader.Read()
            curBal_A = sqlReader("CurrBal")
        End If

        sqlReader.Close()

        curBal_A = FPRnd(curBal_A, 2)

        'Get current balance for Income
        RecordParmList.Item(3).ParmValue = "I"

        Call sqlFetch_1(sqlReader, "xSLMPT_AcctCurBalByType", SqlAppDbConn, CommandType.StoredProcedure, RecordParmList)
        If (sqlReader.HasRows = True) Then
            Call sqlReader.Read()
            curBal_I = sqlReader("CurrBal")
        End If

        sqlReader.Close()

        curBal_I = FPRnd(curBal_I, 2)

        'Get current balance for Expense
        RecordParmList.Item(3).ParmValue = "E"

        Call sqlFetch_1(sqlReader, "xSLMPT_AcctCurBalByType", SqlAppDbConn, CommandType.StoredProcedure, RecordParmList)
        If (sqlReader.HasRows = True) Then
            Call sqlReader.Read()
            curBal_E = sqlReader("CurrBal")
        End If

        sqlReader.Close()

        curBal_E = FPRnd(curBal_E, 2)

        'Get current balance for Liabilities
        RecordParmList.Item(3).ParmValue = "L"

        Call sqlFetch_1(sqlReader, "xSLMPT_AcctCurBalByType", SqlAppDbConn, CommandType.StoredProcedure, RecordParmList)
        If (sqlReader.HasRows = True) Then
            Call sqlReader.Read()
            curBal_L = sqlReader("CurrBal")
        End If

        sqlReader.Close()

        curBal_L = FPRnd(curBal_L, 2)

        checkVal = FPRnd((curBal_I - curBal_E + curBal_L), 2)

        If checkVal <> curBal_A Then
            diffVal = (checkVal - curBal_A)
            diffVal = FPRnd(diffVal, 2)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: The General Ledger is out of balance by " + diffVal.ToString("N2") + ". Income (" + curBal_I.ToString("N2") + ") minus Expenses (" + curBal_E.ToString("N2")
            msgText = msgText + ") plus Liabilities (" + curBal_L.ToString("N2") + ") does not equal Assets (" + curBal_A.ToString("N2") + ")."
            Call LogMessage(msgText, oEventLog)

            msgText = "Suggested actions for correcting GL balances are listed below:" + vbNewLine
            msgText = msgText + " - Turn on Initialize Mode and use Journal Transactions (01.010.00) to enter a one-sided, Adjustment type batch to bring the GL in balance." + vbNewLine
            msgText = msgText + " - Turn on Initialize Mode and use Account History (01.300.00) to make adjustments to the Beginning Balance or PTD Balance amounts to bring the GL in balance." + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
            NbrOfErrors_COA = NbrOfErrors_COA + 1

        End If



        '*********************************************************************************************
        '*** Verify posted GLTran records match amounts in AcctHist for Open Balance migrations ***
        '*********************************************************************************************
        Dim myFiscYear As String = String.Empty

        If OkToContinue = True Then

            Try

                sqlStmt = "SELECT Acct FROM Account"

                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                ' Determine if the status record exists.
                If (sqlReader.HasRows = True) Then
                    AcctList.Clear()

                    While sqlReader.Read()
                        'Insert record into list
                        AcctNum = sqlReader("Acct")
                        AcctList.Add(AcctNum.Trim)
                    End While
                End If

                sqlReader.Close()


                'Loop through Fiscal Years from Beginning Fiscal Year to Ending Fiscal Year
                myFiscYear = bSLMPTStatus.FiscYr_Beg
                'If there are no GLTran records for the Fiscal Year, skip to the next Fiscal Year
                sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE FiscYr =" + SParm(myFiscYear)

                Call sqlFetch_Num(glTranCount, sqlStmt, SqlAppDbConn)

                If glTranCount > 0 Then

                    'Set perNumber to the max period number for previous years or the current period for the current year
                    If myFiscYear = bSLMPTStatus.FiscYr_End Then
                        perNumber = bGLSetupInfo.PerNbr.Substring(4, 2)
                    Else
                        perNumber = bGLSetupInfo.NbrPer
                    End If


                    'Loop through Account list
                    Dim CurAcctType As String = ""
                    Dim SqlBalConn As SqlConnection = Nothing
                    For Each Acct As String In AcctList
                        'Skip if the Account is the YTD Net Income or Retained Earnings Account
                        If (Acct = bGLSetupInfo.YtdNetIncAcct.Trim) Or (Acct = bGLSetupInfo.RetEarnAcct.Trim) Then
                            'Skip checking posted transaction amounts
                            Continue For
                        Else
                            'Look up the Account record to get the Type
                            sqlStmt = "SELECT AcctType FROM Account WHERE Acct =" + SParm(Acct)
                            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                            If (sqlReader.HasRows = True) Then
                                sqlReader.Read()
                                CurAcctType = sqlReader("AcctType")
                            End If

                            sqlReader.Close()

                            'Loop through AcctHist for each Subaccount associated with the Account
                            sqlString = "SELECT Acct, Sub, PtdBal00, PtdBal01, PtdBal02, PtdBal03, PtdBal04, PtdBal05, PtdBal06, PtdBal07, PtdBal08, PtdBal09, PtdBal10, PtdBal11, PtdBal12"
                            sqlString = String.Concat(sqlString, " FROM AcctHist WHERE CpnyID =" + SParm(CpnyId) + "AND Acct =" + SParm(Acct) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID) + "AND FiscYr =" + SParm(myFiscYear.Trim))


                            ' Create a new connection for this reader since we need to do additional queries during processing.
                            SqlBalConn = New SqlClient.SqlConnection(AppDbConnStr)
                            SqlBalConn.Open()

                            Call sqlFetch_1(sqlReader, sqlString, SqlBalConn, CommandType.Text)


                            While sqlReader.Read()
                                Call SetAcctHistBufValues(sqlReader, bAcctHistBuf)
                                'Verify posted period transaction amounts match the PTD amounts from AcctHist
                                Select Case perNumber

                                    Case "01"
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)

                                    Case "02"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                    Case "03"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                    Case "04"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                    Case "05"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                    Case "06"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                    Case "07"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("06", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                    Case "08"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("06", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("07", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)

                                    Case "09"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("06", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("07", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("08", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)

                                    Case "10"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("06", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("07", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("08", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("09", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)



                                    Case "11"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("06", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("07", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("08", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("09", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("10", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                    Case "12"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("06", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("07", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("08", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("09", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("10", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("11", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)



                                    Case "13"
                                        Call VerifyTranAmt("01", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("02", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("03", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("04", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("05", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("06", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("07", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("08", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("09", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("10", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("11", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt("12", Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)
                                        Call VerifyTranAmt(perNumber, Acct, bAcctHistBuf.AcctSub, CurAcctType, myFiscYear, oEventLog, bAcctHistBuf)


                                End Select

                            End While
                            sqlReader.Close()
                            SqlBalConn.Close()
                        End If

                    Next
                End If  'If glTranCount > 0 Then


            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while validating posted transaction amounts", oEventLog)
                Call LogMessage("", oEventLog)
                    OkToContinue = False
                    NbrOfErrors_COA = NbrOfErrors_COA + 1
                End Try

            '   End If
        End If


        '*********************************************************************************************
        '*** Identify GLTran records with Fiscal Year different from period to post year
        '*********************************************************************************************
        Dim SqlTranConn As SqlConnection = Nothing
        Dim cmdText As String = ""
        Dim Operation As OperationType
        Dim sqlUpdate As SqlDataReader = Nothing
        Dim retval As Integer



        sqlString = "Select Acct,  BatNbr, CpnyId, FiscYr,  LineNbr, Module, PerPost from GLTran where Fiscyr <> left(Perpost,4) and CpnyId = " + SParm(CpnyId)
        Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)

        ' If any rows are found, then open a new connection for updating.
        If (sqlReader.HasRows = True) Then
            ' Open the connection to the app database.

            SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
            SqlTranConn.Open()

        End If

        While (sqlReader.Read())


            Call SetGLTranValues(sqlReader, bGLTranList)


            ' Update the fiscal year field.
            cmdText = "Update GLTran set FiscYr = @FiscYr where Acct = @Acct and Module = @Module and BatNbr = @BatNbr and LineNbr = @LineNbr"
            Operation = OperationType.UpdateOp

            RecordParmList.Clear()

            ParmValues = New ParmList
            ParmValues.ParmName = "FiscYr"
            ParmValues.ParmType = SqlDbType.Char
            ParmValues.ParmValue = bGLTranList.PerPost.Substring(0, 4)
            RecordParmList.Add(ParmValues)


            ' Define the parameters for the query.
            ParmValues = New ParmList
            ParmValues.ParmName = "Acct"
            ParmValues.ParmType = SqlDbType.Char
            ParmValues.ParmValue = bGLTranList.AcctNum
            RecordParmList.Add(ParmValues)

            ParmValues = New ParmList
            ParmValues.ParmName = "Module"
            ParmValues.ParmType = SqlDbType.Char
            ParmValues.ParmValue = bGLTranList.ModuleID
            RecordParmList.Add(ParmValues)

            ParmValues = New ParmList
            ParmValues.ParmName = "BatNbr"
            ParmValues.ParmType = SqlDbType.Char
            ParmValues.ParmValue = bGLTranList.BatNbr
            RecordParmList.Add(ParmValues)

            ParmValues = New ParmList
            ParmValues.ParmName = "LineNbr"
            ParmValues.ParmType = SqlDbType.Int
            ParmValues.ParmValue = bGLTranList.LineNbr
            RecordParmList.Add(ParmValues)

            Try

                If (SqlTranConn.State = ConnectionState.Closed) Then

                    SqlTranConn.Open()
                End If

                updTran = TranBeg(SqlTranConn)

                retval = sql_1(sqlUpdate, cmdText, SqlTranConn, Operation, CommandType.Text, updTran, RecordParmList)

                'Write to event log
                If (retval = 1) Then

                        msgText = "WARNING: GLTran for Batch: " + bGLTranList.BatNbr.Trim + vbTab + " Period to Post does not match Fiscal Year:" + bGLTranList.PerPost.Trim + vbTab + " Fiscal Year: " + bGLTranList.FiscYr.Trim + "13"
                        msgText = msgText + vbNewLine
                        msgText = msgText + "The Fiscal Year has been updated to match the Period to Post"
                        Call LogMessage("", oEventLog)
                        Call LogMessage(msgText, oEventLog)
                    End If


                    Call TranEnd(updTran)
                    SqlTranConn.Close()



            Catch ex As Exception
                Call LogMessage("Error in updating Fiscal Year value for " + bGLTranList.BatNbr.Trim + vbNewLine, oEventLog)
                Call LogMessage("Error:  " + ex.Message + vbNewLine, oEventLog)

            End Try


        End While

        If (SqlTranConn IsNot Nothing) Then
            If (SqlTranConn.State = ConnectionState.Open) Then
                SqlTranConn.Close()
                SqlTranConn = Nothing
            End If
        End If

        sqlReader.Close()



        '  Determine which Terms IDs are in use.  Delete any entries that are not used in the system.
        Dim termsList As New List(Of String)
        Dim sqlTranReader As SqlDataReader = Nothing

        ' For each table that contains terms, add the distinct entries to the list.
        Call AddTerms(termsList, "APDoc", "Terms")
        Call AddTerms(termsList, "ARDoc", "Terms")
        Call AddTerms(termsList, "APSetup", "Terms")
        Call AddTerms(termsList, "CustClass", "Terms")
        Call AddTerms(termsList, "Customer", "Terms")
        Call AddTerms(termsList, "Vendor", "Terms")
        Call AddTerms(termsList, "VendClass", "Terms")
        Call AddTerms(termsList, "SOHeader", "TermsId")
        Call AddTerms(termsList, "SOShipHeader", "TermsId")
        Call AddTerms(termsList, "PurchOrd", "Terms")
        Call AddTerms(termsList, "PJSubCon", "Termsid")
        Call AddTerms(termsList, "PJCont", "Termsid")

        ' Get the list of terms and determine whether each is used.
        sqlString = "Select Termsid from Terms"
        Call LogMessage("", oEventLog)
        Call LogMessage("Identifying and deleting unused Terms", oEventLog)
        Call LogMessage(" ", oEventLog)

        ' Open the connection to the app database.
        SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
        SqlTranConn.Open()
        Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)

        ' Add all results to the list of terms.
        While (sqlReader.Read())

            Call SetTermsListValues(sqlReader, bTermsListInfo, "TermsId")
            Try

                If (termsList.Contains(bTermsListInfo.TermId.Trim) = False) And (String.IsNullOrEmpty(bTermsListInfo.TermId.Trim) = False) Then
                    msgText = "Term " + bTermsListInfo.TermId.Trim + " is not used in the system.  This entry will be removed."
                    Call LogMessage(msgText, oEventLog)


                    If (SqlTranConn.State = ConnectionState.Closed) Then
                        SqlTranConn.Open()
                    End If


                    delTran = TranBeg(SqlTranConn)

                    sqlStmt = "DELETE FROM Terms WHERE Termsid =" + SParm(bTermsListInfo.TermId)


                    Call sql_1(sqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text, delTran)

                    Call TranEnd(delTran)
                End If

            Catch ex As Exception

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting unused Terms" + vbNewLine, oEventLog)
                Call LogMessage("Error: " + ex.Message, oEventLog)
                Call LogMessage("", oEventLog)

            End Try
        End While

        Call sqlReader.Close()
        Call SqlTranConn.Close()
        If (sqlTranReader IsNot Nothing) Then
            sqlTranReader.Close()
        End If


        Call oEventLog.LogMessage(EndProcess, "Validate General Ledger")

        ' Indicte that the processing has completed.
        Call MessageBox.Show("Account validation complete", "Account Validation")

        ' Display the event log just created.
        Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.COAEventLogName = oEventLog.LogFile.FullName
        End If

        termsList.Clear()

    End Sub


End Module
