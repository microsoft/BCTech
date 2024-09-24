Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Module Analyze_AP
    '================================================================================
    ' This module contains code to analyze tables used with the Accounts Payable Module
    '
    '================================================================================

    Public Sub Analyze_AP()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "AP"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValInt2 As Integer
        Dim retValDbl1 As Double
        Dim retValStr1 As String = String.Empty
        Dim calcValDbl1 As Double
        Dim currYearAP As String = String.Empty
        Dim fiscYearDelete As String = String.Empty
        Dim currPerNbrAP As String = String.Empty
        Dim perNbrDelete As String = String.Empty

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing

        Try
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Payable"


            '===== Accounts Payable =====

            '=== Module Usage ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Payable Module Usage"
            sAnalysisType = "Module Usage"

            Call oEventLog.LogMessage(0, "ACCOUNTS PAYABLE")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Accounts Payable Module Usage")
            RecID = RecID + 1
            sDescr = "Is the module being used?"
            sqlStmt = "SELECT COUNT(Init) FROM APSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM APTran WHERE Rlsed = 1"
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
            sqlStmt = "SELECT COUNT(Rlsed) FROM Batch WHERE Module = 'AP' AND Rlsed = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = CStr(retValInt1)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Loop through each of the companies
            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'AP' AND Rlsed = 0 AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Are recurring Vouchers being used?"
            sqlStmt = "Select COUNT(*) FROM APDoc WHERE DocType = 'RC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 Then
                sResult = "YES"
            Else
                sResult = "NO"
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            RecID = RecID + 1
            sDescr = "Current period for the module:"
            sqlStmt = "SELECT CurrPerNbr, PerNbr, PerRetHist, PerRetTran FROM APSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then
                Call sqlReader.Read()
                Call SetAPSetupValues(sqlReader, bAPSetupInfo)
            End If
            Call sqlReader.Close()


            If bAPSetupInfo.PerNbr.Trim <> "" Then
                sResult = bAPSetupInfo.PerNbr.Substring(4, 2) + "-" + bAPSetupInfo.PerNbr.Substring(0, 4)
                currPerNbrAP = bAPSetupInfo.PerNbr
                currYearAP = bAPSetupInfo.PerNbr.Substring(0, 4)
            Else
                sResult = ""
                currPerNbrAP = ""
                currYearAP = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Years to Retain Vendor History:"
            sResult = bAPSetupInfo.PerRetHist
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Periods to Retain Transactions:"
            sResult = bAPSetupInfo.PerRetTran
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Oldest Period to Post used on AP documents:"
            sqlStmt = "SELECT MIN(PerPost) FROM APDoc WHERE RTRIM(PerPost) <> ''"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Try
                    sqlReader.Read()
                    retValStr1 = sqlReader(0)
                Catch ex As Exception
                    retValStr1 = String.Empty
                End Try
            End If
            If retValStr1.Trim <> "" Then
                sResult = retValStr1.Substring(4, 2) + "-" + retValStr1.Substring(0, 4)
            Else
                sResult = ""
            End If
            Call sqlReader.Close()

            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Newest Period to Post used on AP documents:"
            sqlStmt = "SELECT MAX(PerPost) FROM APDoc WHERE RTRIM(PerPost) <> ''"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Try
                    sqlReader.Read()
                    retValStr1 = sqlReader(0)
                Catch ex As Exception
                    retValStr1 = String.Empty
                End Try
            End If

            If retValStr1.Trim <> "" Then
                sResult = retValStr1.Substring(4, 2) + "-" + retValStr1.Substring(0, 4)
            Else
                sResult = ""
            End If
            Call sqlReader.Close()

            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            Call oEventLog.LogMessage(0, "")

            '***** End of Module Usage section *****

            Call oEventLog.LogMessage(0, "")


            '=== Master Table Counts ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Payable Master Table Counts"
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Vendors:"
            sqlStmt = "SELECT COUNT(VendID) FROM Vendor"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Active Vendors:"
            sqlStmt = "SELECT COUNT(VendID) FROM Vendor WHERE Status = 'A'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of One Time Vendors:"
            sqlStmt = "SELECT COUNT(VendID) FROM Vendor WHERE Status = 'O'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Vendors on Hold:"
            sqlStmt = "SELECT COUNT(VendID) FROM Vendor WHERE Status = 'H'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of 1099 Vendors:"
            sqlStmt = "SELECT COUNT(VendID) FROM Vendor WHERE Vend1099 = '1'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            RecID = RecID + 1
            sDescr = "Number of Vendor Classes:"
            sqlStmt = "SELECT COUNT(ClassID) FROM VendClass"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique AP Accounts assigned to Vendors:"
            sqlStmt = "SELECT COUNT(DISTINCT(APAcct)) FROM Vendor"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Master Table Counts section *****

            Call oEventLog.LogMessage(0, "")


            '=== Document/Transaction Counts ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Payable Document/Transaction Counts"
            sAnalysisType = "Document/Transaction Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of AP batches:"
            sqlStmt = "SELECT COUNT(Module) FROM Batch WHERE Module = 'AP'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny In CpnyDBList

                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'AP' AND CpnyID =" + SParm(cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If


                Next
            End If

            If currPerNbrAP.Trim <> "" Then
                perNbrDelete = PeriodPlusPerNum(currPerNbrAP, -1 * (bAPSetupInfo.PerRetTran + 1))
                RecID = RecID + 1
                sDescr = "Number of AP batches outside retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
                sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'AP' AND STATUS IN ('V', 'C', 'D', 'P') AND PerPost <=" + SParm(perNbrDelete) + "AND PerPost <=" + SParm(GLRetention)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of Total Documents:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE CpnyID =" + SParm(cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            If currPerNbrAP.Trim <> "" Then
                perNbrDelete = PeriodPlusPerNum(currPerNbrAP, -1 * (bAPSetupInfo.PerRetTran + 1))
                RecID = RecID + 1
                sDescr = "Number of AP documents outside retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
                sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE OpenDoc = 0 AND Rlsed = 1 AND PerClosed <> '' AND PerPost <=" + SParm(perNbrDelete)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of fully open Vouchers:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'VO' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'VO' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Vouchers:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'VO' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'VO' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of fully open Credit Adjustments:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AC' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AC' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Credit Adjustments:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AC' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AC' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of fully open Debit Adjustments:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AD' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AD' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Debit Adjustments:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AD' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE OpenDoc = 1 AND DocType = 'AD' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of fully open Prepayments:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc LEFT OUTER JOIN APAdjust ON APAdjust.AdjdRefNbr = APDoc.RefNbr AND APAdjust.AdjdDocType = APDoc.DocType WHERE APDoc.DocType = 'PP' AND APDoc.Rlsed = 1 AND (APDoc.OpenDoc = 1 OR (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt <> 0)) AND (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt = APDoc.OrigDocAmt)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc LEFT OUTER JOIN APAdjust ON APAdjust.AdjdRefNbr = APDoc.RefNbr AND APAdjust.AdjdDocType = APDoc.DocType WHERE APDoc.DocType = 'PP' AND APDoc.Rlsed = 1 AND (APDoc.OpenDoc = 1 OR (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt <> 0)) AND (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt = APDoc.OrigDocAmt) AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Prepayments:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc LEFT OUTER JOIN APAdjust ON APAdjust.AdjdRefNbr = APDoc.RefNbr AND APAdjust.AdjdDocType = APDoc.DocType WHERE APDoc.DocType = 'PP' AND APDoc.Rlsed = 1 AND (APDoc.OpenDoc = 1 OR (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt <> 0)) AND (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt <> APDoc.OrigDocAmt)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc LEFT OUTER JOIN APAdjust ON APAdjust.AdjdRefNbr = APDoc.RefNbr AND APAdjust.AdjdDocType = APDoc.DocType WHERE APDoc.DocType = 'PP' AND APDoc.Rlsed = 1 AND (APDoc.OpenDoc = 1 OR (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt <> 0)) AND (APDoc.OpenDoc = 0 AND APAdjust.AdjAmt <> APDoc.OrigDocAmt) AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of open AP documents with an orphaned Terms ID:"
            sqlStmt = "SELECT COUNT(DISTINCT(Terms)) FROM APDoc WHERE OpenDoc = 1 AND DocBal = OrigDocAmt AND DocType IN ('VO', 'AD', 'AC') AND Terms NOT IN (SELECT TermsId FROM Terms WHERE ApplyTo IN ('B', 'V'))"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Outstanding (non-reconciled) Checks:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE DocType IN ('CK', 'HC', 'MC', 'SC', 'ZC', 'QC') AND Status = 'O' AND RTRIM(PerClosed) = '' AND NOT EXISTS (SELECT 1 FROM APTran WHERE APTran.RefNbr = APDoc.RefNbr AND APTran.TranType = APDoc.DocType AND APTran.VendID = APDoc.VendID)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE DocType IN ('CK', 'HC', 'MC', 'SC', 'ZC', 'QC') AND Status = 'O' AND RTRIM(PerClosed) = '' AND NOT EXISTS (SELECT 1 FROM APTran WHERE APTran.RefNbr = APDoc.RefNbr AND APTran.TranType = APDoc.DocType AND APTran.VendID = APDoc.VendID) AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of Recurring Vouchers:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE DocType = 'RC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE DocType = 'RC' AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of Landed Cost Vouchers:"
            sqlStmt = "SELECT COUNT(*) FROM LCVoucher"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM LCVoucher WHERE CpnyID =" + SParm(cpny.CompanyId.Trim)
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
            sDescr = "Number of unique Company IDs used on AP transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CpnyID)) FROM APTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM APTran WHERE CpnyID =" + SParm(cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            If currPerNbrAP.Trim <> "" Then
                perNbrDelete = PeriodPlusPerNum(currPerNbrAP, -1 * (bAPSetupInfo.PerRetTran + 1))
                RecID = RecID + 1
                sDescr = "Number of AP transactions outside retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
                sqlStmt = "SELECT COUNT(*) FROM APTran WHERE PerPost <=" + SParm(perNbrDelete) + "AND EXISTS (SELECT APDoc.RefNbr FROM APDoc WHERE APDoc.RefNbr = APTran.RefNbr AND OpenDoc = 0 AND Rlsed = 1 AND PerClosed <> '')"
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs used on AP transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryId)) FROM APTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 1 Then
                'Display the Currency IDs
                sqlStmt = "SELECT DISTINCT(CuryId) FROM APTran"
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
                Call sqlReader.Close()
            End If

            If MultiCpnyAppDB = True Then
                RecID = RecID + 1
                sDescr = "Average number of transactions per day over last 365 days:"
                sqlStmt = "SELECT COUNT(Rlsed) FROM APTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                calcValDbl1 = (retValInt1 / 365)
                calcValDbl1 = Decimal.Round(calcValDbl1, 2, MidpointRounding.AwayFromZero)
                sResult = calcValDbl1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                If retValInt1 > 0 Then
                    For Each cpny As CpnyDatabase In CpnyDBList
                        RecID = RecID + 1
                        sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                        sqlStmt = "SELECT COUNT(*) FROM APTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
                    For Each cpny As CpnyDatabase In CpnyDBList
                        RecID = RecID + 1
                        sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                        sqlStmt = "SELECT COUNT(*) FROM APTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
                    For Each cpny As CpnyDatabase In CpnyDBList
                        RecID = RecID + 1
                        sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                        sqlStmt = "SELECT COUNT(*) FROM APTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(cpny.CompanyId.Trim)
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
                sqlStmt = "SELECT COUNT(Rlsed) FROM APTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
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

            RecID = RecID + 1
            sDescr = "Number of AP History records:"
            sqlStmt = "SELECT COUNT(*) FROM APHist"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If currYearAP.Trim <> "" Then
                RecID = RecID + 1
                fiscYearDelete = CStr(FPSub(CDbl(currYearAP), bAPSetupInfo.PerRetHist + 1, 0))
                sDescr = "Number of AP History records outside of retention settings (" + fiscYearDelete.Trim + "):"
                sqlStmt = "SELECT COUNT(*) FROM APHist WHERE FiscYr <=" + SParm(fiscYearDelete)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of AP Balances records:"
            sqlStmt = "SELECT COUNT(*) FROM AP_Balances"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + cpny.CompanyId.Trim + " " + cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM AP_Balances WHERE CpnyID =" + SParm(cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            '***** End of Document/Transaction Counts section *****

            Call oEventLog.LogMessage(0, "")

            '=== Data Integrity Checks ===
            Form1.AnalysisStatusLbl.Text = "Performing Accounts Payable Data Integrity Checks"
            sAnalysisType = "Data Integrity Checks"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of open AP documents with a zero balance:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE DocType IN ('VO', 'AC', 'AD') AND OpenDoc = 1 AND DocBal = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of closed AP documents with a non-zero balance:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE DocType IN ('VO', 'AC', 'AD') AND OpenDoc = 0 AND DocBal <> 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents with a negative balance:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM APDoc WHERE DocType IN ('VO', 'AC', 'AD') AND DocBal < 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of duplicate checks by Account/Subaccount:"
            sqlStmt = "SELECT COUNT(*) FROM xv_PCLDUPAPCHKS"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents with an orphaned Account:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND Acct NOT IN(SELECT Acct FROM Account)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents with an orphaned Subaccount:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND Sub NOT IN(SELECT Sub FROM SubAcct)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents with an orphaned Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND VendID NOT IN(SELECT VendID FROM Vendor)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND CuryID NOT IN(SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions with an orphaned Account:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE Acct NOT IN(SELECT Acct FROM Account)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions with an orphaned Subaccount:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE Sub NOT IN(SELECT Sub FROM SubAcct)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions with an orphaned Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE VendID NOT IN(SELECT VendID FROM Vendor)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE CuryID NOT IN(SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents missing an Account:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND RTRIM(Acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents missing a Subaccount:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND RTRIM(Sub) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents missing a DocType:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND RTRIM(DocType) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents missing a Reference Number:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND RTRIM(RefNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents missing a Batch Number:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND RTRIM(BatNbr) = '' AND DocType <> 'RC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP documents missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE Status <> 'V' AND RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions missing a Batch Number:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE RTRIM(BatNbr) = '' AND TranType <> 'RC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions missing an Account:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE RTRIM(Acct) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions missing a Subaccount:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE RTRIM(Sub) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions missing a Reference Number:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE RTRIM(RefNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP transactions missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM APTran WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Vendor records missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM Vendor WHERE RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP History records with an orphaned Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM APHist WHERE VendID NOT IN (SELECT VendID FROM Vendor)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP History records missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM APHist WHERE RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP History records missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM APHist WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP History records missing a Fiscal Year:"
            sqlStmt = "SELECT COUNT(*) FROM APHist WHERE RTRIM(FiscYr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP Balances records with an orphaned Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM AP_Balances WHERE VendID NOT IN(SELECT VendID FROM Vendor)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP Balances records missing a Vendor ID:"
            sqlStmt = "SELECT COUNT(*) FROM AP_Balances WHERE RTRIM(VendID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AP Balances records missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM AP_Balances WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

        Catch ex As Exception
            Call MessageBox.Show("Error Encountered" + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - AP")

            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Accounts Payable data"

            OkToContinue = False
        End Try

    End Sub

End Module
