Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_CM
    '================================================================================
    ' This module contains code to analyze tables used with the Currency Manager Module
    '
    '================================================================================

    Public Sub Analyze_CM()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "CM"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValDbl1 As Double
        Dim calcValDbl1 As Double

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing

        Try
            Call oEventLog.LogMessage(0, "CURRENCY MANAGER")
            Call oEventLog.LogMessage(0, "")


            '===== Currency Manager =====

            '=== Module Usage ===
            Call oEventLog.LogMessage(0, "Analyzing Currency Manager Module Usage")
            sAnalysisType = "Module Usage"

            RecID = RecID + 1
            sDescr = "Is the module being used?"
            sqlStmt = "SELECT COUNT(*) FROM CMSetup WHERE SetupId = 'CM' AND MCActivated = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE CuryId <> BaseCuryID AND Rlsed = 1"
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
            sDescr = "Base Currency ID application database (" + DBName.Trim + "):"
            sqlStmt = "SELECT BaseCuryID, Central_Cash_Cntl, COAOrder, CpnyId, LedgerID, MCActive, Mult_Cpny_Db, NbrPer, PerNbr, PerRetHist, PerRetModTran, PerRetTran, RetEarnAcct, ValidateAcctSub, YtdNetIncAcct FROM GLSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetGLSetupValues(sqlReader, bGLSetupInfo)
            Else
                Call InitGLSetupValues(bGLSetupInfo)
            End If
            Call sqlReader.Close()
            sResult = bGLSetupInfo.BaseCuryID.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Default Rate Type for General Ledger:"
            sqlStmt = "SELECT APRateDate, APRtTpDflt, ARRtTpDflt, CARtTpDflt, GLRtTpDflt, MCActivated, OERateDate  FROM CMSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetCMSetupValues(sqlReader, bCMSETUPInfo)
            Else
                Call InitCMSetupValues(bCMSETUPInfo)
            End If
            Call sqlReader.Close()
            sResult = bCMSETUPInfo.GLRtTpDflt.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Default Rate Type for Cash Manager:"
            sResult = bCMSETUPInfo.CARtTpDflt.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Default Rate Type for Accounts Payable/Purchasing:"
            sResult = bCMSETUPInfo.APRtTpDflt.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Accounts Payable Selection Date option:"
            If bCMSETUPInfo.APRateDate.Trim <> "" Then
                Select Case bCMSETUPInfo.APRateDate
                    Case "I"
                        sResult = "Invoice Date"
                    Case "D"
                        sResult = "Document Date"
                    Case Else
                        sResult = "ERROR"
                End Select
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Default Rate Type for Accounts Receivable/Order Management:"
            sResult = bCMSETUPInfo.ARRtTpDflt.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Order Management Selection Date option:"
            If bCMSETUPInfo.APRateDate.Trim <> "" Then
                Select Case bCMSETUPInfo.OERateDate
                    Case "O"
                        sResult = "Order Date"
                    Case "S"
                        sResult = "Ship Date"
                    Case Else
                        sResult = "ERROR"
                End Select
            Else
                sResult = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Module Usage section *****
            Call oEventLog.LogMessage(0, "")

            '=== Master Table Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Currency Manager Master Table Counts")
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Currencies (including the base Currency):"
            sqlStmt = "SELECT COUNT(*) FROM Currncy"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Currency Rate Types:"
            sqlStmt = "SELECT COUNT(*) FROM CuryRtTp"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Currency Rates:"
            sqlStmt = "SELECT COUNT(*) FROM CuryRate"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Master Table Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Document/Transaction Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Currency Manager Document/Transaction Counts")
            sAnalysisType = "Document/Transaction Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of CuryAcct records for non-base Currency IDs:"
            sqlStmt = "SELECT COUNT(*) FROM CuryAcct WHERE CuryId <> BaseCuryID"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of GLTran records for multi-currency transactions:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE CuryId <> BaseCuryID"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Modules on GLTran records for multi-currency transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(Module)) FROM GLTran WHERE CuryId <> BaseCuryID"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs on GLTran records for multi-currency transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryId)) FROM GLTran WHERE CuryId <> BaseCuryID"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of multi-currency transactions per day over last 365 days:"
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE CuryId <> BaseCuryID AND Crtd_DateTime >=" + SParm(LastYrDateStr)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            calcValDbl1 = (retValInt1 / 365)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of multi-currency transactions per week over last 365 days:"
            calcValDbl1 = (retValInt1 / 52)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of multi-currency transactions per month over last 365 days:"
            calcValDbl1 = (retValInt1 / 12)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Document/Transaction Counts section *****
            Call oEventLog.LogMessage(0, "")

            '=== Data Integrity Checks ===
            Call oEventLog.LogMessage(0, "Performing Currency Manager Data Integrity Checks")
            sAnalysisType = "Data Integrity Checks"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Currency records missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM Currncy WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Currency Rate Types missing a Type ID:"
            sqlStmt = "SELECT COUNT(*) FROM CuryRtTp WHERE RTRIM(RateTypeId) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Currency Rates missing a From Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM CuryRate WHERE RTRIM(FromCuryId) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Currency Rates missing a To Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM CuryRate WHERE RTRIM(ToCuryId) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Currency Rates missing a Rate Type:"
            sqlStmt = "SELECT COUNT(*) FROM CuryRate WHERE RTRIM(RateType) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - CM")
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Currency Manager data"
            OkToContinue = False

        End Try



    End Sub

End Module
