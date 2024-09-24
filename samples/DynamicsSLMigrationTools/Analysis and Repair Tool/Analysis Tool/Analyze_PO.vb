Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_PO
    '================================================================================
    ' This module contains code to analyze tables used with the Purchasing Module
    '
    '================================================================================

    Public Sub Analyze_PO()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "PO"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValDbl1 As Double
        Dim retValStr1 As String = String.Empty
        Dim calcValDbl1 As Double
        Dim valMultPOAllowed As Short = 0
        Dim strPerPost As String = String.Empty


        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing

        Try
            Call oEventLog.LogMessage(0, "PURCHASING")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Purchasing Module Usage")



            '===== Purchasing =====

            '=== Module Usage ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Purchasing Module Usage"
            sAnalysisType = "Module Usage"

            RecID = RecID + 1
            sDescr = "Is the module being used?"
            sqlStmt = "SELECT COUNT(Init) FROM POSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM POReceipt WHERE Rlsed = 1"
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
            sqlStmt = "SELECT COUNT(Rlsed) FROM Batch WHERE Module = 'PO' AND Rlsed = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = CStr(retValInt1)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Are multiple POs allowed on one receipt?"
            sqlStmt = "SELECT MultPOAllowed FROM POSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Try
                    valMultPOAllowed = sqlReader("MultPOAllowed")
                Catch ex As Exception
                    valMultPOAllowed = 0
                End Try
            End If
            Call sqlReader.Close()
            If valMultPOAllowed = 1 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Is the Landed Cost being used?"
            sqlStmt = "SELECT COUNT(*) FROM LCCode"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM LCReceipt WHERE TranStatus = 'P'"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Current period for the module (AP):"
            sqlStmt = "SELECT CurrPerNbr, PerNbr, PerRetHist, PerRetTran FROM APSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetAPSetupValues(sqlReader, bAPSetupInfo)
            Else
                bAPSetupInfo.PerNbr = String.Empty
            End If
            Call sqlReader.Close()

            If bAPSetupInfo.PerNbr.Trim <> "" Then
                sResult = bAPSetupInfo.PerNbr.Substring(4, 2) + "-" + bAPSetupInfo.PerNbr.Substring(0, 4)
            Else
                sResult = ""
            End If


            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Oldest Period to Post used on PO transactions:"
            sqlStmt = "SELECT MIN(PerPost) FROM POTran WHERE RTRIM(PerPost) <> ''"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If sqlReader.HasRows() Then
                Try
                    Call sqlReader.Read()
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
            sDescr = "Newest Period to Post used on PO transactions:"
            sqlStmt = "SELECT MAX(PerPost) FROM POTran WHERE RTRIM(PerPost) <> ''"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If sqlReader.HasRows() Then
                Try
                    Call sqlReader.Read()
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

            '***** End of Module Usage section *****

            Call oEventLog.LogMessage(0, "")

            '=== Master Table Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Purchasing Master Table Counts")
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Landed Cost codes:"
            sqlStmt = "SELECT COUNT(LCCode) FROM LCCode"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Address records:"
            sqlStmt = "SELECT COUNT(*) FROM POAddress"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Master Table Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Document/Transaction Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Purchasing Document/Transaction Counts")
            sAnalysisType = "Document/Transaction Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders:"
            sqlStmt = "SELECT COUNT(PONbr) FROM PurchOrd"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of open Purchase Orders:"
            sqlStmt = "SELECT COUNT(PONbr) FROM PurchOrd WHERE Status IN ('O','P')"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Blanket Order Purchase Orders:"
            sqlStmt = "SELECT COUNT(PONbr) FROM PurchOrd WHERE POType = 'BL'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Drop Ship Purchase Orders:"
            sqlStmt = "SELECT COUNT(PONbr) FROM PurchOrd WHERE POType = 'DP'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Standard Order Purchase Orders:"
            sqlStmt = "SELECT COUNT(PONbr) FROM PurchOrd WHERE POType = 'ST'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Order detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Goods for Inventory PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'GI'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Goods for Project PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'GP'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Goods for Project Inventory PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'PI'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Goods for Sales Order PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'GS'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Goods for Project Sales Order PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'PS'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Goods for Non-Inventory PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'GN'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Services for Expense PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'SE'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Services for Project PO detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType = 'SP'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of open Purchase Order detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE OpenLine = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of partially received Purchase Order detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE RcptStage = 'P'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of partially vouchered Purchase Order detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE VouchStage = 'P'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE RcptType = 'R'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Landed Cost PO Receipts:"
            sqlStmt = "SELECT COUNT(*) FROM LCReceipt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transaction detail lines:"
            sqlStmt = "SELECT COUNT(*) FROM POTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unreleased PO Receipt batches:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE Rlsed = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of PO Receipts per day over last 365 days:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM POReceipt WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            calcValDbl1 = (retValInt1 / 365)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of PO Receipts per week over last 365 days:"
            calcValDbl1 = (retValInt1 / 52)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Average number of PO Receipts per month over last 365 days:"
            calcValDbl1 = (retValInt1 / 12)
            calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
            sResult = calcValDbl1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Company IDs used on Purchase Orders:"
            sqlStmt = "SELECT COUNT(DISTINCT(CpnyID)) FROM PurchOrd"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Company IDs used on PO transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CpnyID)) FROM POTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs used on Purchase Orders:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryId)) FROM PurchOrd"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs used on PO transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryId)) FROM POTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Document/Transaction Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Data Integrity Checks ===
            Call oEventLog.LogMessage(0, "Performing Purchasing Data Integrity Checks")
            sAnalysisType = "Data Integrity Checks"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders with an orphaned Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE VendID NOT IN(SELECT VendID FROM Vendor) AND RTRIM(VendID) <> ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE CuryID NOT IN(SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE CpnyID NOT IN(SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines with an orphaned PO Number:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PONbr NOT IN (SELECT PONbr FROM PurchOrd)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines with an orphaned Inventory ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND InvtID NOT IN (SELECT InvtID FROM Inventory)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines with an orphaned Site ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND SiteID NOT IN (SELECT SiteID FROM Site)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE CuryID NOT IN (SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts with an orphaned PO Number:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE PONbr NOT IN (SELECT PONbr FROM PurchOrd) AND RTRIM(PONbr) <> '*'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts with an orphaned Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE VendID NOT IN(SELECT VendID FROM Vendor)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE CuryID NOT IN (SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions with an orphaned PO Number:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE PONbr NOT IN (SELECT PONbr FROM PurchOrd)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions with an orphaned Inventory ID:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND InvtID NOT IN (SELECT InvtID FROM Inventory)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions with an orphaned Site ID:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND SiteID NOT IN (SELECT SiteID FROM Site)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(PONbr) FROM POTran WHERE CuryID NOT IN (SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions with an orphaned Company ID:"
            sqlStmt = "SELECT COUNT(PONbr) FROM POTran WHERE CpnyID NOT IN (SELECT CpnyID FROM xSLAnalysisCpny)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders missing a PO Number:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE RTRIM(PONbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders missing a PO Type:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE RTRIM(POType) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Purchase Orders missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines missing a PO Number:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE RTRIM(PONbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines missing a Purchase For:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE RTRIM(PurchaseType) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines missing an Inventory ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND RTRIM(InvtID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines missing a Site ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND RTRIM(SiteID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines missing a Conversion Factor:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND CnvFact = 0.0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO detail lines missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts missing a Receipt Number:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE RTRIM(RcptNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts missing a PO Number:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE RTRIM(PONbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts missing a Batch Number:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE RTRIM(BatNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO Receipts missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM POReceipt WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a Receipt Number:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE RTRIM(RcptNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a PO Number:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE RTRIM(PONbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a Batch Number:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE RTRIM(BatNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing an Inventory ID:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND RTRIM(InvtID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a Site ID:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE PurchaseType NOT IN ('DL', 'FR', 'MI', 'GN', 'SE', 'SP') AND RTRIM(SiteID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a Conversion Factor:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE RTRIM(CnvFact) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of PO transactions missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM POTran WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - PO")
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Purchasing data"
            OkToContinue = False

        End Try

    End Sub

End Module
