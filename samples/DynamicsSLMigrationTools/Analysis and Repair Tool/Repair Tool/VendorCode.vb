
Imports System.Data.SqlClient

Module VendorCode
    '=======================================================================================
    ' This module contains code to prepare the Vendor data for migration
    '
    '=======================================================================================

    Public Sub Validate()
        '====================================================================================
        ' Validate Vendor records
        '   - Vendor.VendID is not blank
        '   - Check for recurring Vouchers
        '   - Check for open Prepayments
        '   - Verify Customer balances using the stored procedures from the AP Integity Check
        '====================================================================================
        Dim nbrVendIDBlank As Integer
        Dim recVouchers As Integer
        Dim msgText As String = String.Empty
        Dim sqlString As String = String.Empty
        Dim vendIDSpecCharExists As Boolean = False

        Dim sqlStmt As String = String.Empty
        Dim nbrKeysBlk As Integer = 0
        Dim KeyString As String = String.Empty

        Dim sqlRowReader As SqlDataReader = Nothing


        Dim oEventLog As clsEventLog
        Dim sqlReader As SqlDataReader = Nothing

        Dim sqlPerConn As SqlConnection = Nothing
        Dim sqlPerReader As SqlDataReader = Nothing

        Dim fmtDate As String

        fmtDate = Date.Now.ToString
        fmtDate = fmtDate.Replace(":", "")
        fmtDate = fmtDate.Replace("/", "")
        fmtDate = fmtDate.Remove(fmtDate.Length - 3)
        fmtDate = fmtDate.Replace(" ", "-")
        fmtDate = fmtDate & Date.Now.Millisecond

        oEventLog = New clsEventLog
        oEventLog.FileName = "SL-AP-" & "-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")

        Call oEventLog.LogMessage(0, "Processing Vendors")

        '***********************************************************************************************************************************************
        '*** Delete any Vendor records where VendID field is blank since this is the only key field in this table (Applies to all migration methods) ***
        '***********************************************************************************************************************************************
        Call sqlFetch_Num(nbrVendIDBlank, "SELECT COUNT(*) FROM Vendor WHERE RTRIM(VendID) = ''", SqlAppDbConn)


        If nbrVendIDBlank > 0 Then
            Call LogMessage("Number of Vendor records found with a blank VendID Field: " + CStr(nbrVendIDBlank), oEventLog)


            'Delete Vendor records with a blank VendID field and log the number of Vendor records deleted
            Try
                sqlStmt = "DELETE FROM Vendor WHERE RTRIM(VendID) = ''"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)


                If nbrVendIDBlank = 1 Then
                    Call LogMessage("Deleted " + CStr(nbrVendIDBlank) + " Vendor record with a blank VendID field.", oEventLog)

                Else
                    Call LogMessage("Deleted " + CStr(nbrVendIDBlank) + " Vendor records with a blank VendID field.", oEventLog)


                End If
                Call LogMessage("", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting Vendor record(s) with a blank VendID field", oEventLog)
                Call LogMessage("", oEventLog)

                OkToContinue = False
                NbrOfErrors_Vend = NbrOfErrors_Vend + 1
            End Try
        End If

        ' Check the segdef values.
        Call ValidateSegDefValue("APTran", "Sub", True, oEventLog)

        '***************************************************************************************************************
        '*** Check for special characters in VendID that are not valid in D365 BC (Applies to all migration methods) ***
        '***************************************************************************************************************
        sqlStmt = "SELECT APAcct, ExpAcct, PerNbr, VendId, Name FROM Vendor WHERE CHARINDEX('&', VendID) <> 0"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)


        If sqlReader.HasRows = True Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: Vendor IDs with '&' character found. The '&' character is not a valid character for the Vendor No. in Microsoft Dynamics 365 Business Central."
            msgText = msgText + vbNewLine + "List of Vendor IDs with '&' character:"
            Call LogMessage(msgText, oEventLog)

            vendIDSpecCharExists = True
        End If
        While (sqlReader.Read())

            Call SetVendorValues(sqlReader, bVendorInfo)
            'Write VendID to event log
            Call LogMessage("Vendor ID: " + bVendorInfo.VendId, oEventLog)

            NbrOfErrors_Vend = NbrOfErrors_Vend + 1
        End While

        sqlReader.Close()

        'Display message in Event Log for suggested actions
        If vendIDSpecCharExists = True Then
            Call LogMessage("", oEventLog)
            msgText = "Suggested actions for updating Vendor IDs are listed below:" + vbNewLine
			msgText = msgText + " - Use the Professional Services Tools Library (PSTL) application to modify the Vendor IDs to remove the '&' character" + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)

        End If

        '***************************************************************************************************************
        '*** Check for Vendor Names that are greater than 50 characters in length ***
        '***************************************************************************************************************
        sqlStmt = "SELECT APAcct, ExpAcct, PerNbr, VendId, Name FROM Vendor WHERE LEN(RTRIM(Name)) > 50"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)


        If sqlReader.HasRows = True Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: D365 BC Vendor name can only be 50 characters for migration."
            msgText = msgText + vbNewLine + "List of Vendor IDs with name greater than 50 characters:"
            Call LogMessage(msgText, oEventLog)

        End If
        While (sqlReader.Read())

            Call SetVendorValues(sqlReader, bVendorInfo)
            'Write VendID to event log
            Call LogMessage("Vendor ID: " + bVendorInfo.VendId, oEventLog)

            NbrOfErrors_Vend = NbrOfErrors_Vend + 1
        End While

        sqlReader.Close()

        '************************************
        '*** Check for recurring Vouchers ***
        '************************************
        sqlStmt = "SELECT COUNT(*) FROM APDoc WHERE DocType = 'RC' AND NbrCycle > 0"
        Call sqlFetch_Num(recVouchers, sqlStmt, SqlAppDbConn)

        If recVouchers > 0 Then
            'Display a warning message
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open Recurring Vouchers exists. Recurring vouchers will not be migrated and will need to be manually entered in the new system."
            msgText = msgText + " To assist with the move of your recurring batches, the details of your recurring batches can be identified using the"
            msgText = msgText + " Recurring Vouchers (03.700.00) report, Standard format to identify the AP recurring batches identified by this utility."
            Call LogMessage(msgText, oEventLog)


            NbrOfWarnings_Vend = NbrOfWarnings_Vend + 1
            Call LogMessage("", oEventLog)
        End If



        '****************************************
        '*** Check for fully open Prepayments ***
        '****************************************
        sqlString = "SELECT d.CpnyId, d.DocBal, d.DocType, d.OpenDoc, d.OrigDocAmt, d.PerPost, d.RefNbr, d.Rlsed, d.Terms, d.Vendid FROM APDoc d LEFT OUTER JOIN APAdjust j ON j.AdjdRefNbr = d.RefNbr AND j.AdjdDocType = d.DocType"
        sqlString = sqlString + " WHERE d.CpnyID =" + SParm(CpnyId) + "AND d.DocType = 'PP' AND d.Rlsed = 1 AND (d.OpenDoc = 1 OR (d.OpenDoc = 0 AND j.AdjAmt <> 0))"
        sqlString = sqlString + "AND ((d.OpenDoc = 0) OR d.OpenDoc = 1)"
        sqlString = sqlString + " ORDER BY d.VendId, d.RefNbr"
        Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() = True Then
            'Write warning message to event log
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open AP Prepayments exist. It is recommended that all open Prepayments are applied to Vouchers prior to migrating data." + vbNewLine + "List of open AP Prepayments:"
            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()
            Call SetAPDocValues(sqlReader, bAPDocInfo)
            'Write VendID, RefNbr, and DocBal to event log
            If bAPDocInfo.OpenDoc = 1 Then
                'Pull open balance from APDoc.DocBal
                Call LogMessage("Vendor: " + bAPDocInfo.VendId + vbTab + "Reference Nbr: " + bAPDocInfo.RefNbr + vbTab + "Open Balance: " + bAPDocInfo.DocBal.ToString + vbTab + "Original Balance: " + bAPDocInfo.OrigDocAmt.ToString, oEventLog)

                NbrOfWarnings_Vend = NbrOfWarnings_Vend + 1
            Else
                'Pull open balance from APAdjust.AdjAmt
                Dim sqlAdjConn As SqlConnection = New SqlConnection(AppDbConnStr)
                sqlAdjConn.Open()
                Dim sqlAdjReader As SqlDataReader = Nothing

                sqlStmt = "Select j.AdjAmt, j.AdjgPerPost, j.AdjDiscAmt from APAdjust j where j.AdjdRefNbr = " + SParm(bAPDocInfo.RefNbr)
                Call sqlFetch_1(sqlAdjReader, sqlStmt, sqlAdjConn, CommandType.Text)
                If (sqlAdjReader.HasRows()) Then
                    Call sqlAdjReader.Read()
                    Call SetAPAdjustValues(sqlAdjReader, bAPAdjustInfo)
                    Call sqlAdjReader.Close()

                    Call LogMessage("Vendor: " + bAPDocInfo.VendId + vbTab + "Reference Nbr: " + bAPDocInfo.RefNbr + vbTab + "Open Balance: " + bAPAdjustInfo.AdjAmt.ToString + vbTab + "Original Balance: " + bAPDocInfo.OrigDocAmt.ToString, oEventLog)

                Else
                    Call LogMessage("Vendor: " + bAPDocInfo.VendId + vbTab + "Reference Nbr: " + bAPDocInfo.RefNbr + vbTab + "Original Balance: " + bAPDocInfo.OrigDocAmt.ToString, oEventLog)

                End If

                NbrOfWarnings_Vend = NbrOfWarnings_Vend + 1
            End If

        End While
        Call sqlReader.Close()
        Call LogMessage("", oEventLog)


        '********************************************************
        '*** Check for open AP docs with an orphaned Terms ID ***
        '********************************************************
        sqlStmt = "SELECT CpnyId, DocBal, DocType, OpenDoc, OrigDocAmt, PerPost, RefNbr, Rlsed, Terms, Vendid FROM APDoc WHERE OpenDoc = 1 AND DocBal = OrigDocAmt AND DocType IN ('VO', 'AD', 'AC') AND Terms NOT IN (SELECT TermsId FROM Terms WHERE ApplyTo IN ('B', 'V')) AND CpnyID = '" + CpnyId + "'"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open AP documents exist with an invalid Payment Terms ID." + vbNewLine + "List of open documents with an invalid Payment Terms ID:"
            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()

            Call SetAPDocValues(sqlReader, bAPDocInfo)
            'Write detail to event log
            Call LogMessage("Vendor: " + bAPDocInfo.VendId + vbTab + "Reference Nbr: " + bAPDocInfo.RefNbr + vbTab + "Doc Type: " + bAPDocInfo.DocType + vbTab + "Terms ID: " + bAPDocInfo.Terms, oEventLog)
            NbrOfWarnings_Vend = NbrOfWarnings_Vend + 1

        End While
        Call sqlReader.Close()
        Call LogMessage("", oEventLog)


        '*********************************************************************************************
        'Verify document date falls within the date range for the period to post for open AP documents
        'Do not include documents with a DocType of 'RC' or 'VT' 
        '*********************************************************************************************
        Dim sqlstrg As String = String.Empty
        Dim docDateStr As String = String.Empty
        Dim docPerNbr As String = String.Empty
        Dim docOutsidePerPostFound As Boolean = False

        Dim RecordParmList As New List(Of ParmList)
        Dim ParmValues As ParmList



        ' Add the stored procedure parameters to the list.
        ParmValues = New ParmList
        ParmValues.ParmName = "Date"
        ParmValues.ParmType = SqlDbType.SmallDateTime
        ParmValues.ParmValue = 0
        RecordParmList.Add(ParmValues)

        ParmValues = New ParmList
        ParmValues.ParmName = "UseCurrentOMPeriod"
        ParmValues.ParmType = SqlDbType.SmallInt
        ParmValues.ParmValue = 0
        RecordParmList.Add(ParmValues)


        'Use query from the Vendor Trial Balance report to gather open AP documents
        sqlstrg = "SELECT v.VendId, v.RefNbr, v.DocType, v.InvcDate, v.PerPost FROM vr_ShareAPVendorDetail v, APDoc d"
        sqlstrg = sqlstrg + " WHERE (d.DocBal <> 0 OR (v.docbal <> 0 and v.doctype = 'PP') OR (v.doctype not in ('CK', 'HC','EP', 'ZC') and v.PerClosed = (SELECT PerNbr FROM APSetup (NOLOCK)))"
        sqlstrg = sqlstrg + " OR (v.DocType in ('CK','HC','EP','ZC','BW') AND v.PerPost = (SELECT PerNbr FROM APSetup (NOLOCK))))"
        sqlstrg = sqlstrg + " AND v.Parent = d.RefNbr AND v.ParentType = d.DocType"
        sqlstrg = sqlstrg + " AND v.CpnyID =" + SParm(CpnyId.Trim) + "AND v.DocBal <> 0"
        sqlstrg = sqlstrg + " AND v.DocBal = v.OrigDocAmt"
		sqlstrg = sqlstrg + " AND d.DocType NOT IN ('RC', 'VT')"
        sqlstrg = sqlstrg + " ORDER BY v.VendID, v.Parent, v.Ord, v.RefNbr"

        Call sqlFetch_1(sqlReader, sqlstrg, SqlAppDbConn, CommandType.Text)
        sqlPerConn = New SqlClient.SqlConnection(AppDbConnStr)
        sqlPerConn.Open()


        While sqlReader.Read() And OkToContinue = True
            Try

                Call SetAPVendorDetailValues(sqlReader, bAPVendorDetailInfo)
                'Get Invoice Date and Document Period Number
                docDateStr = bAPVendorDetailInfo.InvcDate.ToShortDateString
                docPerNbr = bAPVendorDetailInfo.PerPost.Substring(4, 2)

                RecordParmList.Item(0).ParmValue = bAPVendorDetailInfo.InvcDate


                sqlStmt = "ADG_GLPeriod_GetPeriodFromDate"
                'Get Period to Post based on Invoice Date
                Call sqlFetch_1(sqlPerReader, sqlStmt, sqlPerConn, CommandType.StoredProcedure, RecordParmList)
                If (sqlPerReader.HasRows()) Then
                    Call sqlPerReader.Read()
                    Call SetPerNbrCheckValues(sqlPerReader, bPerNbrCheckInfo)
                Else
                    bPerNbrCheckInfo.PerPost = ""
                End If
                Call sqlPerReader.Close()

                'Write warnings to the event log
                If (bAPVendorDetailInfo.PerPost <> bPerNbrCheckInfo.PerPost) Then
                    If docOutsidePerPostFound = False Then
                        'Display Warning message
                        msgText = "WARNING: Open AP documents exist where the Invoice Date does not fall within the date range for the Period to Post."
                        msgText = msgText + " Migrated open documents will be posted to the new system based on the Invoice Date, not the SL Period to Post."
                        msgText = msgText + " If any open documents exist from the prior fiscal year, they will be posted to the first day of the first accounting period in the new system."
                        msgText = msgText + vbNewLine + "List of open documents where Invoice Date does not fall within Period to Post date range: "
                        Call LogMessage("", oEventLog)
                        Call LogMessage(msgText, oEventLog)
                        docOutsidePerPostFound = True
                    End If

                    msgText = "Vendor: " + bAPVendorDetailInfo.VendID + " RefNbr: " + bAPVendorDetailInfo.RefNbr + " Type: " + bAPVendorDetailInfo.DocType + vbTab + vbTab + "InvcDate: " + docDateStr.ToString + vbTab + "PerPost: " + bAPVendorDetailInfo.PerPost
                    Call LogMessage(msgText, oEventLog)
                    NbrOfWarnings_Vend = NbrOfWarnings_Vend + 1
                End If

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while validating open AP document dates fall within the assigned period to post date range.", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Vend = NbrOfErrors_Vend + 1
                Call MessageBox.Show("Error Encountered: " + ex.Message.Trim + " Operation ended.")

                Call oEventLog.LogMessage(EndProcess, "Validate Accounts Payable")

                Exit Sub
            End Try

        End While
        Call sqlReader.Close()


        '**********************************************************
        '*** Verify Vendor Balances for Open Balance migrations ***
        '**********************************************************

        If OkToContinue = True Then

            Try
                Call CheckVendorBalances(oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while validating Vendor balances", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Vend = NbrOfErrors_Vend + 1
                Exit Sub
            End Try

        End If

        '  Detect AP_Balance records with blank key fields - keys are VendId, CpnyId
        If OkToContinue = True Then
            sqlStmt = "SELECT COUNT(*) FROM AP_Balances WHERE RTRIM(CpnyId) = '' or RTRIM(VendId) = ''"
            Call sqlFetch_Num(nbrKeysBlk, sqlStmt, SqlAppDbConn)

            If nbrKeysBlk > 0 Then
                Try
                    Call LogMessage("Number of AP_Balances records found with a blank Key Field: " + CStr(nbrKeysBlk), oEventLog)

                    ' Get the list of accthist records with one or more blank keys.
                    sqlStmt = "SELECT CpnyId, VendId FROM AP_Balances WHERE RTRIM(CpnyId) = '' or RTRIM(VendId) = ''"

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
                    While sqlReader.Read
                        Call SetAPBalListValues(sqlReader, bAPBalListInfo)

                        ' Report in the Event Log.  
                        Call LogMessage("AP_Balances record has blank key fields: ", oEventLog)
                        Call LogMessage("  CpnyID:" + bAPBalListInfo.CpnyId.Trim, oEventLog)
                        Call LogMessage("  VendId:" + bAPBalListInfo.VendID.Trim, oEventLog)



                        KeyString = String.Concat(bAPBalListInfo.CpnyId.Trim, " ")
                        KeyString = String.Concat(KeyString, bAPBalListInfo.VendID.Trim, " ")

                    End While
                Catch
                End Try

                Call sqlReader.Close()
            End If
        End If

        Call oEventLog.LogMessage(EndProcess, "Validate Accounts Payable")

        Call MessageBox.Show("Vendor Validation Complete", "Vendor Validation")

        ' Display the event log just created.
        Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.VendEventLogName = oEventLog.LogFile.FullName
        End If



    End Sub

End Module
