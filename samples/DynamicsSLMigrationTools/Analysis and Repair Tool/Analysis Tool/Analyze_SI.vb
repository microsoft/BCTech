Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_SI
    '================================================================================
    ' This module contains code to analyze tables used across multiple modules
     '================================================================================

    Public Sub Analyze_SI()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "SI"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing


        Try
            Form1.AnalysisStatusLbl.Text = "Analyzing Shared Information"
            Call oEventLog.LogMessage(0, "SHARED INFORMATION")
            Call oEventLog.LogMessage(0, "")



            '===== Shared Information =====

            '=== Master Table Counts ===
            Call oEventLog.LogMessage(0, "Analyzing Shared Information Master Table Counts")
            sAnalysisType = "Master Table Counts"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Terms IDs:"
            sqlStmt = "SELECT COUNT(*) FROM Terms"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Vendor only Terms IDs:"
            sqlStmt = "SELECT COUNT(*) FROM Terms WHERE ApplyTo = 'V'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Customer only Terms IDs:"
            sqlStmt = "SELECT COUNT(*) FROM Terms WHERE ApplyTo = 'C'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Multiple Installment Terms IDs:"
            sqlStmt = "SELECT COUNT(*) FROM Terms WHERE TermsType = 'M'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Tax IDs:"
            sqlStmt = "SELECT COUNT(*) FROM SalesTax"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Tax Categories:"
            sqlStmt = "SELECT COUNT(*) FROM SlsTaxCat"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Tax Groups:"
            sqlStmt = "SELECT COUNT(DISTINCT(GroupID)) FROM SlsTaxGrp"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of State/Province IDs:"
            sqlStmt = "SELECT COUNT(*) FROM State"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Country/Region IDs:"
            sqlStmt = "SELECT COUNT(*) FROM Country"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Currency IDs:"
            sqlStmt = "SELECT COUNT(*) FROM Currncy"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            '***** End of Master Table Counts section *****
            Call oEventLog.LogMessage(0, "")

            '=== Flexkey(Definitions) ===
            Call oEventLog.LogMessage(0, "Analyzing Flexkey Definitions")
            sAnalysisType = "Flexkey Definitions"
            sDescr = String.Empty
            sResult = String.Empty

            RecID = RecID + 1
            sDescr = "Number of Subaccount segments:"
            sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'SUBACCOUNT'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then

                Call sqlReader.Read()
                Call SetFlexDefValues(sqlReader, bFlexDefInfo)
            End If

            Call sqlReader.Close()
            sResult = bFlexDefInfo.NumberSegments.ToString
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Vendor ID segments:"
            sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'VENDORID'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then

                Call sqlReader.Read()
                Call SetFlexDefValues(sqlReader, bFlexDefInfo)
            End If

            Call sqlReader.Close()

            sResult = bFlexDefInfo.NumberSegments.ToString
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Customer ID segments:"
            sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'CUSTOMERID'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then

                Call sqlReader.Read()
                Call SetFlexDefValues(sqlReader, bFlexDefInfo)
            End If

            Call sqlReader.Close()

            sResult = bFlexDefInfo.NumberSegments.ToString
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Employee ID segments:"
            sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'EMPLOYEEID'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then

                Call sqlReader.Read()
                Call SetFlexDefValues(sqlReader, bFlexDefInfo)
            End If

            Call sqlReader.Close()

            sResult = bFlexDefInfo.NumberSegments.ToString
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Inventory ID segments:"
            sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'INVENTORYITEM'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then

                Call sqlReader.Read()
                Call SetFlexDefValues(sqlReader, bFlexDefInfo)
            End If

            Call sqlReader.Close()

            sResult = bFlexDefInfo.NumberSegments.ToString
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            RecID = RecID + 1
            sDescr = "Number of Task ID segments:"
            sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'TASKID'"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then

                Call sqlReader.Read()
                Call SetFlexDefValues(sqlReader, bFlexDefInfo)
            End If

            Call sqlReader.Close()
            sResult = bFlexDefInfo.NumberSegments.ToString
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")
        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - SI")
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Shared Information data"
            OkToContinue = False

        End Try

    End Sub

End Module
