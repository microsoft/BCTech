Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_AR
    '================================================================================
    ' This module contains code to analyze tables used with the Accounts Receivable Module
    '
    '================================================================================

    Public Sub Analyze_AR()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "AR"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValInt2 As Integer
        Dim retValDbl1 As Double
        Dim retValStr1 As String = String.Empty
        Dim calcValDbl1 As Double
        Dim currYearAR As String = String.Empty
        Dim fiscYearDelete As String = String.Empty
        Dim currPerNbrAR As String = String.Empty
        Dim perNbrDelete As String = String.Empty

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing


        Try
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Receivable"


            '===== Accounts Receivable ======

            '=== Module Usage ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Receivable Module Usage"
            sAnalysisType = "Module Usage"
            Call oEventLog.LogMessage(0, "ACCOUNTS RECEIVABLE")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Accounts Receivable Module Usage")

            RecID = RecID + 1
            sDescr = "Is the module being used?"
            sqlStmt = "SELECT COUNT(Init) FROM ARSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)

            sqlStmt = "SELECT COUNT(Rlsed) FROM ARTran WHERE Rlsed = 1"
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
            sqlStmt = "SELECT COUNT(Rlsed) FROM Batch WHERE Module = 'AR' AND Rlsed = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = CStr(retValInt1)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'AR' AND Rlsed = 0 AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Are recurring Invoices being used?"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE DocType = 'RC'"
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
            sDescr = "Are Salespeople being used?"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE RTRIM(SlsperID) <> ''"
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
            sqlStmt = "SELECT PerNbr, PerRetHist, PerRetTran FROM ARSetup"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then
                Call sqlReader.Read()
                Call SetARSetupValues(sqlReader, bARSetupInfo)
            End If
            Call sqlReader.Close()

            If bARSetupInfo.PerNbr.Trim <> "" Then
                sResult = bARSetupInfo.PerNbr.Substring(4, 2) + "-" + bARSetupInfo.PerNbr.Substring(0, 4)
                currPerNbrAR = bARSetupInfo.PerNbr
                currYearAR = bARSetupInfo.PerNbr.Substring(0, 4)
            Else
                sResult = ""
                currPerNbrAR = ""
                currYearAR = ""
            End If
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Years to Retain History:"
            sResult = bARSetupInfo.PerRetHist
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Periods to Retain Tran Detail:"
            sResult = bARSetupInfo.PerRetTran
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Oldest Period to Post used on AR documents:"
            sqlStmt = "SELECT MIN(PerPost) FROM ARDoc WHERE RTRIM(PerPost) <> ''"
            retValStr1 = String.Empty
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
            sDescr = "Newest Period to Post used on AR documents:"
            retValStr1 = String.Empty
            sqlStmt = "SELECT MAX(PerPost) FROM ARDoc WHERE RTRIM(PerPost) <> ''"
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

            '***** End of Module Usage section *****
            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "Master Table Counts")



            '=== Master Table Counts ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Receivable Master Table Counts"
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Customers:"
            sqlStmt = "SELECT COUNT(CustID) FROM Customer"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Active Customers:"
            sqlStmt = "SELECT COUNT(CustID) FROM Customer WHERE Status = 'A'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of One Time Customers:"
            sqlStmt = "SELECT COUNT(CustID) FROM Customer WHERE Status = 'O'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Customers on Admin Hold:"
            sqlStmt = "SELECT COUNT(CustID) FROM Customer WHERE Status = 'H'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Customer Classes:"
            sqlStmt = "SELECT COUNT(ClassID) FROM CustClass"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Salespeople:"
            sqlStmt = "SELECT COUNT(SlsperId) FROM Salesperson"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Sales Territories:"
            sqlStmt = "SELECT COUNT(Territory) FROM Territory"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Statement Cycles:"
            sqlStmt = "SELECT COUNT(StmtCycleId) FROM ARStmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of unique AR Accounts assigned to Customers:"
            sqlStmt = "SELECT COUNT(DISTINCT(ArAcct)) FROM Customer"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)



            '***** End of Master Table Counts section *****

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "Document / Transaction Counts")


            '=== Document/Transaction Counts ===
            Form1.AnalysisStatusLbl.Text = "Analyzing Accounts Receivable Document/Transaction Counts"
            sAnalysisType = "Document/Transaction Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of AR batches:"
            sqlStmt = "SELECT COUNT(Module) FROM Batch WHERE Module = 'AR'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'AR' AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next

            End If

            If currPerNbrAR.Trim <> "" Then
                perNbrDelete = PeriodPlusPerNum(currPerNbrAR, -1 * (bARSetupInfo.PerRetTran + 1))
                RecID = RecID + 1
                sDescr = "Number of AR batches outside retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
                sqlStmt = "SELECT COUNT(*) FROM Batch WHERE Module = 'AR' AND STATUS IN ('V', 'C', 'D', 'P') AND PerPost <=" + SParm(perNbrDelete) + "AND PerPost <=" + SParm(GLRetention)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of Total Documents:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE CpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next

            End If

            If currPerNbrAR.Trim <> "" Then
                perNbrDelete = PeriodPlusPerNum(currPerNbrAR, -1 * (bARSetupInfo.PerRetTran + 1))
                RecID = RecID + 1
                sDescr = "Number of AR documents outside retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
                sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE DocBal = 0 AND Rlsed = 1 AND PerClosed <> '' AND PerPost <=" + SParm(perNbrDelete)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of fully open Invoices:"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'IN' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'IN' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Invoices:"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'IN' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'IN' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of fully open Debit Memos:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'DM' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'DM' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Debit Memos:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'DM' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'DM' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of fully open Credit Memos:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'CM' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'CM' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Credit Memos:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'CM' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'CM' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of fully open Payments:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PA' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PA' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of partially open Payments:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PA' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PA' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PP' AND DocBal = OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PP' AND DocBal = OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PP' AND DocBal <> OrigDocAmt"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE OpenDoc = 1 and DocType = 'PP' AND DocBal <> OrigDocAmt AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of open AR documents with an orphaned Terms ID:"
            sqlStmt = "SELECT COUNT(DISTINCT(Terms)) FROM ARDoc WHERE OpenDoc = 1 AND DocBal = OrigDocAmt AND DocType IN ('IN', 'DM', 'CM') AND Terms NOT IN (SELECT TermsId FROM Terms WHERE ApplyTo IN ('B', 'C'))"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of recurring Invoices:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE DocType = 'RC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE DocType = 'RC' AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            sDescr = "Number of unique Company IDs used on AR Transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CpnyID)) FROM ARTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of transactions:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE CpnyID =" + SParm(Cpny.CompanyId.Trim)
                    Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                    sResult = retValInt1
                    If retValInt1 > 0 Then
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End If
                Next
            End If

            If currPerNbrAR.Trim <> "" Then
                perNbrDelete = PeriodPlusPerNum(currPerNbrAR, -1 * (bARSetupInfo.PerRetTran + 1))
                RecID = RecID + 1
                sDescr = "Number of AR transactions outside retention settings (" + perNbrDelete.Substring(4, 2) + "-" + perNbrDelete.Substring(0, 4) + "):"
                sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE PerPost <=" + SParm(perNbrDelete) + "AND EXISTS (SELECT ARDoc.RefNbr FROM ARDoc WHERE ARDoc.RefNbr = ARTran.RefNbr AND DocBal = 0 AND Rlsed = 1 AND PerClosed <> '')"
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of unique Currency IDs used on AR Transactions:"
            sqlStmt = "SELECT COUNT(DISTINCT(CuryId)) FROM ARTran"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If retValInt1 > 1 Then
                'Display the Currency IDs
                Call InitUnboundList30Values(bUnboundList30)
                sqlStmt = "SELECT DISTINCT(CuryId) FROM ARTran"
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
                sqlStmt = "SELECT COUNT(Rlsed) FROM ARTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
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
                        sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
                        sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
                        sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr) + "AND CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
                sqlStmt = "SELECT COUNT(Rlsed) FROM ARTran WHERE Crtd_DateTime >=" + SParm(LastYrDateStr)
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
            sDescr = "Number of AR History records:"
            sqlStmt = "SELECT COUNT(*) FROM ARHist"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If currYearAR.Trim <> "" Then
                RecID = RecID + 1
                fiscYearDelete = CStr(FPSub(CDbl(currYearAR), bARSetupInfo.PerRetHist + 1, 0))
                sDescr = "Number of AR History records outside of retention settings (" + fiscYearDelete.Trim + "):"
                sqlStmt = "SELECT COUNT(*) FROM ARHist WHERE FiscYr <=" + SParm(fiscYearDelete)
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            RecID = RecID + 1
            sDescr = "Number of AR Balances records:"
            sqlStmt = "SELECT COUNT(*) FROM AR_Balances"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            If (retValInt1 > 0 And MultiCpnyAppDB = True) Then
                For Each Cpny As CpnyDatabase In CpnyDBList
                    RecID = RecID + 1
                    sDescr = " - CpnyID: " + Cpny.CompanyId.Trim + " " + Cpny.CompanyName.Trim()
                    sqlStmt = "SELECT COUNT(*) FROM AR_Balances WHERE CpnyID =" + SParm(Cpny.CompanyId.Trim)
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
            Call oEventLog.LogMessage(0, "Data Integrity Checks")


            '=== Data Integrity Checks ===
            Form1.AnalysisStatusLbl.Text = "Performing Accounts Receivable Data Integrity Checks"
            sAnalysisType = "Data Integrity Checks"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of open AR documents with a zero balance:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE DocType NOT IN ('RC', 'VT') AND OpenDoc = 1 AND DocBal = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of closed AR documents with a non-zero balance:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE DocType NOT IN ('RC', 'VT', 'SB') AND OpenDoc = 0 AND DocBal <> 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents with a negative balance:"
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARDoc WHERE DocType IN ('RC', 'VT') AND DocBal < 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of duplicate Payments by Customer:"
            sqlStmt = "SELECT COUNT(*) FROM xv_PCLDUPARPAY"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents with an orphaned Customer ID:"
            sqlStmt = "SELECT COUNT(CustID) FROM ARDoc WHERE CustID NOT IN(SELECT CustID FROM Customer)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents with an orphaned Account:"
            sqlStmt = "SELECT COUNT(BankAcct) FROM ARDoc WHERE BankAcct NOT IN(SELECT Acct FROM Account)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents with an orphaned Subaccount:"
            sqlStmt = "SELECT COUNT(BankSub) FROM ARDoc WHERE BankSub NOT IN(SELECT Sub FROM SubAcct)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(CuryID) FROM ARDoc WHERE CuryID NOT IN(SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions with an orphaned Customer ID:"
            sqlStmt = "SELECT COUNT(CustID) FROM ARTran WHERE CustID NOT IN(SELECT CustID FROM Customer)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions with an orphaned Account:"
            sqlStmt = "SELECT COUNT(Acct) FROM ARTran WHERE Acct NOT IN(SELECT Acct FROM Account)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions with an orphaned Subaccount:"
            sqlStmt = "SELECT COUNT(Sub) FROM ARTran WHERE Sub NOT IN(SELECT Sub FROM SubAcct)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions with an orphaned Currency ID:"
            sqlStmt = "SELECT COUNT(CuryID) FROM ARTran WHERE CuryID NOT IN(SELECT CuryID FROM Currncy)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents missing a Customer ID:"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE RTRIM(CustID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents missing a Document Type:"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE RTRIM(DocType) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents missing a Reference Number:"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE RTRIM(RefNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents missing a Batch Number:"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE RTRIM(BatNbr) = '' AND DocType <> 'RC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR documents missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions missing a Customer ID:"
            sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE RTRIM(CustID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions missing a Transaction Type:"
            sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE RTRIM(TranType) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions missing a Reference Number:"
            sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE RTRIM(RefNbr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions missing a Batch Number:"
            sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE RTRIM(BatNbr) = '' AND TranType <> 'RC'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR transactions missing a Currency ID:"
            sqlStmt = "SELECT COUNT(*) FROM ARTran WHERE RTRIM(CuryID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of orphaned Customer records:"
            sqlStmt = "SELECT COUNT(*) FROM Customer WHERE RTRIM(CustID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR History records with an orphaned Customer ID:"
            sqlStmt = "SELECT COUNT(*) FROM ARHist WHERE CustID NOT IN(SELECT CustID FROM Customer)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR History records missing a Customer ID:"
            sqlStmt = "SELECT COUNT(*) FROM ARHist WHERE RTRIM(CustID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR History records missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM ARHist WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR History records missing a Fiscal Year:"
            sqlStmt = "SELECT COUNT(*) FROM ARHist WHERE RTRIM(FiscYr) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR Balances records with an orphaned Customer ID:"
            sqlStmt = "SELECT COUNT(*) FROM AR_Balances WHERE CustID NOT IN(SELECT CustID FROM Customer)"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR Balances records missing a Customer ID:"
            sqlStmt = "SELECT COUNT(*) FROM AR_Balances WHERE RTRIM(CustID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of AR Balances records missing a Company ID:"
            sqlStmt = "SELECT COUNT(*) FROM AR_Balances WHERE RTRIM(CpnyID) = ''"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            Call sqlReader.Close()

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")



        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - AR")
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Accounts Receivable data"
            OkToContinue = False
        End Try

    End Sub

End Module
