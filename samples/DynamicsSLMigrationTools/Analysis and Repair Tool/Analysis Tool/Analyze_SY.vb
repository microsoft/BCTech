Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module Analyze_SY
    '================================================================================
    ' This module contains code to analyze Administration type information
     '================================================================================

    Public Sub Analyze_SY()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "SY"
        Dim sResult As String = String.Empty
        Dim sqlStringRet As String = String.Empty
        Dim sqlString As String = String.Empty
        Dim retValInt1 As Integer
        Dim retValDbl1 As Double
        Dim retValDbl2 As Double

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing

        Dim MajorVersion As String = String.Empty
        Dim MinorVersion As String = String.Empty
        Dim remVersion As String = String.Empty
        Dim SepPos As Integer

        Try
            Form1.AnalysisStatusLbl.Text = "Analyzing Administration information"


            '====== Administration ======

            '=== General Information ===
            sAnalysisType = "General Information"

            Call oEventLog.LogMessage(0, "ADMINISTRATION - GENERAL INFORMATION")
            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

            RecID = RecID + 1
            sDescr = "Registration Customer ID:"
            Dim regStmt As String = "SELECT CustomerId, CustomerName FROM Registration"
            Call sqlFetch_1(sqlReader, regStmt, SqlSysDbConn, CommandType.Text)
            If (sqlReader.HasRows) Then
                Call sqlReader.Read()
                Call SetRegistrationValues(sqlReader, bRegistrationInfo)
            End If
            Call sqlReader.Close()

            sResult = DecodeCustId(bRegistrationInfo.CustomerId)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd


            Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            Call oEventLog.LogMessage(0, "")

            RecID = RecID + 1
            sDescr = "Registration Company Name:"
            sResult = DecodeCustName(bRegistrationInfo.CustomerName)
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd

            Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            Call oEventLog.LogMessage(0, "")

            RecID = RecID + 1
            sDescr = "Dynamics SL Version:"

            ' Determine the product name based on the version number.
            ProdName = "Microsoft Dynamics SL "
            SepPos = bVersionSL.VersionNbr.IndexOf(".")

            MajorVersion = bVersionSL.VersionNbr.Substring(0, SepPos)
            Select Case MajorVersion
                Case "10"
                    ProdName = String.Concat(ProdName, "2018")
                Case "9"
                    ProdName = String.Concat(ProdName, "2015")
                Case "8"
                    ProdName = String.Concat(ProdName, "2011")
            End Select

            remVersion = bVersionSL.VersionNbr.Substring(SepPos + 1)

            ' Get the position of the next separator.
            SepPos = remVersion.IndexOf(".")
            If (SepPos = -1) Then
                SepPos = remVersion.Length
            End If
            MinorVersion = remVersion.Substring(0, SepPos)
            Select Case MinorVersion
                Case "01"
                    ProdName = String.Concat(ProdName, " CU1")
                Case "02"
                    ProdName = String.Concat(ProdName, " CU2")
                Case "03"
                    ProdName = String.Concat(ProdName, " CU3")
                Case "04"
                    ProdName = String.Concat(ProdName, " CU4")
                Case "05"
                    ProdName = String.Concat(ProdName, " CU5")
                Case "06"
                    ProdName = String.Concat(ProdName, " CU6")
                Case "07"
                    ProdName = String.Concat(ProdName, " CU7")
                Case "08"
                    ProdName = String.Concat(ProdName, " CU8")
                Case "09"
                    ProdName = String.Concat(ProdName, " CU9")
                Case "10"
                    ProdName = String.Concat(ProdName, " CU10")
                Case "11"
                    ProdName = String.Concat(ProdName, " CU11")
            End Select



            sResult = bVersionSL.VersionNbr.Trim + " " + ProdName
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd


            Call AddStatusInfo(sqlStringExec, sDescr, sResult)



            RecID = RecID + 1
            sDescr = "SQL Server Information:"
            sqlStmt = "select CAST(SERVERPROPERTY('productversion') AS VARCHAR) , CAST(SERVERPROPERTY('machinename') AS VARCHAR), CAST(serverproperty('servername') AS VARCHAR),CAST(serverproperty('edition') AS VARCHAR)"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then
                Call sqlReader.Read()
                Call SetSQLInfoValues(sqlReader, bSQLInfo)

                ' Output the SQL information.
                sDescr = "-- Version: "
                sResult = "Microsoft SQL Server "
                Dim verMajor As String = bSQLInfo.ProductVersion.Substring(0, 2)
                Select Case verMajor

                    Case "10"
                        sResult = sResult + "2010"
                    Case "11"
                        sResult = sResult + "2012"
                    Case "12"
                        sResult = sResult + "2014"
                    Case "13"
                        sResult = sResult + "2016"
                    Case "14"
                        sResult = sResult + "2017"
                    Case "15"
                        sResult = sResult + "2019"

                End Select

                RecID = RecID + 1
                sResult = sResult + " (" + bSQLInfo.ProductVersion + ")"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)


                RecID = RecID + 1
                sDescr = "-- Edition: "
                sResult = bSQLInfo.Edition.Trim()
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)


                RecID = RecID + 1
                sDescr = "-- Machine/Server: "
                sResult = bSQLInfo.MachineName + "//" + bSQLInfo.ServerName
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            End If
            Call sqlReader.Close()

            RecID = RecID + 1
            sDescr = "Application Database Name:"
            sResult = DBName.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            RecID = RecID + 1
            sDescr = "Application Database Size:"

            Call InitUnboundList30Values(bUnboundList30)
            sqlStringRet = "SELECT STR(SUM(CONVERT(dec(17,2),size)) / 128,10,2) FROM " + DBName.Trim + ".dbo.sysfiles"

            Call sqlFetch_1(sqlReader, sqlStringRet, SqlAppDbConn, CommandType.Text)
            If (sqlReader.HasRows()) Then


                Call sqlReader.Read()
                Call SetUnboundList30Values(sqlReader, bUnboundList30)

                sResult = bUnboundList30.ListID.Trim + " MB"
            End If
            Call sqlReader.Close()

            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            RecID = RecID + 1
            sDescr = "Logged in User ID:"
            sResult = UserId.Trim
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            RecID = RecID + 1
            ' List the modules that are in use.
            sDescr = "Modules used"
            sResult = String.Empty
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)


            ' Check each module to see what is in use currently.
            sqlStmt = "SELECT COUNT(MCActivated) FROM MCSetup WHERE MCActivated = 1"

            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)

            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE TranType = 'IC' AND Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Multi-Company"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            End If



            sqlStmt = "SELECT COUNT(Init) FROM GLSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM GLTran WHERE Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)

            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- General Ledger"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(Init) FROM APSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM APTran WHERE Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Accounts Payable"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(Init) FROM ARSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM ARTran WHERE Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Accounts Receivable"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(*) FROM CASetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM CATran WHERE Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Cash Manager"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(Init) FROM INSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM INTran WHERE Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Inventory"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(*) FROM BMSetup"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM BOMDoc WHERE Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Bill of Material"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(Init) FROM POSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(Rlsed) FROM POReceipt WHERE Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Purchasing"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(Init) FROM RQSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM RQReqHdr WHERE Status IN ('PO', 'CP')"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Requisitions"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(Init) FROM SOSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM SOShipHeader WHERE Status = 'C' AND Cancelled = 0"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Order Management"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(Init) FROM PCSetup WHERE Init = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM PJTran"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Project Controller"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(*) FROM PJCONTRL WHERE control_type = 'BI' AND control_code = 'SETUP'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM PJINVHDR WHERE inv_status IN ('CO', 'PO', 'PR') "
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Flexible Billings"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(*) FROM PJCONTRL WHERE control_type = 'TM' AND control_code = 'SETUP'"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM PJLABHDR WHERE le_status ='P'"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM PJEXPHDR WHERE status_1 = 'P'"
            Call sqlFetch_Num(retValDbl2, sqlStmt, SqlAppDbConn)
            If (retValInt1 > 0 And retValDbl1 > 0) Or (retValInt1 > 0 And retValDbl2 > 0) Then
                RecID = RecID + 1
                sDescr = "-- Time and Expense for Projects"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If


            sqlStmt = "SELECT COUNT(*) FROM smProServSetup"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM smServCall WHERE ServiceCallCompleted = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Service Dispatch"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(*) FROM smConSetup"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM smContract WHERE Status = 'A'"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Service Contracts"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            sqlStmt = "SELECT COUNT(*) FROM CMSetup WHERE SetupId = 'CM' AND MCActivated = 1"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sqlStmt = "SELECT COUNT(*) FROM GLTran WHERE CuryId <> BaseCuryID AND Rlsed = 1"
            Call sqlFetch_Num(retValDbl1, sqlStmt, SqlAppDbConn)
            If retValInt1 > 0 And retValDbl1 > 0 Then
                RecID = RecID + 1
                sDescr = "-- Currency Manager"
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)
            End If

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

        Catch ex As Exception
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - SY")
            Form1.AnalysisStatusLbl.Text = "Error encountered while analyzing Administration data"
            OkToContinue = False

        End Try

    End Sub
End Module
