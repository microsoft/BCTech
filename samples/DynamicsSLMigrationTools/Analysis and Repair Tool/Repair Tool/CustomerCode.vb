
Imports System.Data.SqlClient

Module CustomerCode
    '=======================================================================================
    ' This module contains code to prepare the Customer data for migration
    '
    '=======================================================================================

    Public Sub Validate()
        '====================================================================================
        ' Validate Customer records
        '   - Customer.CustID is not blank
        '   - Check for recurring Invoices
        '   - Check for open Credit Memos, Payments and Pre-Payments
        '   - Verify document date falls within the date range for the period to post for open AR documents
        '   - Verify Customer balances using the stored procedures from the AR Integity Check
        '====================================================================================
        Dim nbrCustIDBlank As Integer
        Dim sqlString As String = String.Empty
        Dim perNumber As String = String.Empty
        Dim recInvoices As Integer
        Dim msgText As String = String.Empty
        Dim docDateStr As String = String.Empty
        Dim docPerNbr As String = String.Empty
		Dim docOutsidePerPostFound As Boolean = False
        Dim custIDSpecCharExists As Boolean = False

        Dim sqlStmt As String = String.Empty
        Dim nbrKeysBlk As Integer = 0
        Dim KeyString As String = String.Empty

        Dim oEventLog As clsEventLog
        Dim sqlReader As SqlDataReader = Nothing


        Dim fmtDate As String

        fmtDate = Date.Now.ToString
        fmtDate = fmtDate.Replace(":", "")
        fmtDate = fmtDate.Replace("/", "")
        fmtDate = fmtDate.Remove(fmtDate.Length - 3)
        fmtDate = fmtDate.Replace(" ", "-")
        fmtDate = fmtDate & Date.Now.Millisecond

        oEventLog = New clsEventLog
        oEventLog.FileName = "SL-AR-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")

        Call oEventLog.LogMessage(0, "Processing Customers")

        '*************************************************************************************************************************************************
        '*** Delete any Customer records where CustID field is blank since this is the only key field in this table (Applies to all migration methods) ***
        '*************************************************************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM Customer WHERE Status <> 'I' AND RTRIM(CustID) = ''"
        Call sqlFetch_Num(nbrCustIDBlank, sqlStmt, SqlAppDbConn)

        If nbrCustIDBlank > 0 Then
            Call LogMessage("Number of Customer records found with a blank CustID Field: " + CStr(nbrCustIDBlank), oEventLog)

            'Delete Customer records with a blank CustID field and log the number of Customer records deleted
            Try
                sqlStmt = "DELETE FROM Customer WHERE Status <> 'I' AND RTRIM(CustID) = ''"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                Call LogMessage("Deleted " + CStr(nbrCustIDBlank) + " Customer record(s) with a blank CustID field.", oEventLog)


                Call LogMessage("", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting Customer record(s) with a blank CustID field", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
				NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                Exit Sub
            End Try
		End If

        ' Check the segdef values used in the AR trans.
        Call ValidateSegDefValue("ARTran", "Sub", True, oEventLog)

        '***************************************************************************************************************
        '*** Check for special characters in CustID that are not valid in D365 BC (Applies to all migration methods) ***
        '***************************************************************************************************************
        sqlStmt = "SELECT CustId, Name FROM Customer WHERE Status <> 'I' AND CHARINDEX('&', CustId) <> 0"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: Customer IDs with '&' character found. The '&' character is not a valid character for the Customer No. in Microsoft Dynamics 365 Business Central."
            msgText = msgText + vbNewLine + "List of Customer IDs with '&' character:"
            Call LogMessage(msgText, oEventLog)
            custIDSpecCharExists = True
        End If
        While sqlReader.Read()

            Call SetCustomerValues(sqlReader, bCustomerInfo)
            'Write CustID to event log
            Call LogMessage("Customer ID: " + bCustomerInfo.CustId, oEventLog)
            NbrOfErrors_Cust = NbrOfErrors_Cust + 1
        End While

        Call sqlReader.Close()
        'Display message in Event Log for suggested actions
        If custIDSpecCharExists = True Then
            Call LogMessage("", oEventLog)
            msgText = "Suggested actions for updating Customer IDs are listed below:" + vbNewLine
			msgText = msgText + " - Use the Professional Services Tools Library (PSTL) application to modify the Customer IDs to remove the '&' character" + vbNewLine
			msgText = msgText + " - Set the Status on these Customers to Inactive in Customer Maintenance (08.260.00) to exclude these customers from the migration" + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
        End If

        '**********************************************************************
        '*** Check for Customer names greater than 50 characters in length  ***
        '**********************************************************************
        sqlStmt = "SELECT CustId, Name FROM Customer WHERE LEN(RTRIM(NAME)) > 50"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: D365 BC Customer name can only be 50 characters for migration."
            msgText = msgText + vbNewLine + "List of Customer IDs with names greater than 50 characters:"
            Call LogMessage(msgText, oEventLog)
            custIDSpecCharExists = True
        End If
        While sqlReader.Read()

            Call SetCustomerValues(sqlReader, bCustomerInfo)
            'Write CustID to event log
            Call LogMessage("Customer ID: " + bCustomerInfo.CustId, oEventLog)
            NbrOfErrors_Cust = NbrOfErrors_Cust + 1
        End While

        Call sqlReader.Close()



        '************************************
        '*** Check for recurring Invoices ***
        '************************************
        sqlStmt = "SELECT COUNT(*) FROM ARDoc WHERE DocType = 'RC' AND NbrCycle > 0"
        Call sqlFetch_Num(recInvoices, sqlStmt, SqlAppDbConn)

        If recInvoices > 0 Then
            'Display a warning message
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open Recurring Invoices exists. Recurring invoices will not be migrated and will need to be manually entered in the new system."
            msgText = msgText + " To assist with the move of your recurring batches, the details of your recurring batches can be identified using the"
            msgText = msgText + " Recurring Invoice (08.670.00) report, Standard format to identify the AR recurring batches identified by this utility."

            Call LogMessage(msgText, oEventLog)

            NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1
        End If





        '**************************************************************
        '*** Check for open Credit Memos, Payments and Pre-Payments ***
        '**************************************************************
        'Check for open Credit Memos
        sqlStmt = "SELECT CustId, DocBal, DocDate, DocType, OrigDocAmt, PerPost, RefNbr, Terms FROM ARDoc WHERE DocType = 'CM' AND OpenDoc = 1 AND CpnyID =" + SParm(CpnyId)
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open AR Credit Memos exist. It is recommended that all open Credit Memos are applied to Invoices prior to migrating data." + vbNewLine + "List of open AR Credit Memos:"

            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()
            Call SetARDocValues(sqlReader, bARDocInfo)
            'Write CustID, RefNbr, and DocBal to event log
            Call LogMessage("Customer: " + bARDocInfo.CustId + vbTab + "Reference Nbr: " + bARDocInfo.RefNbr + vbTab + "Open Balance: " + bARDocInfo.DocBal.ToString + vbTab + "Original Balance: " + bARDocInfo.OrigDocAmt.ToString, oEventLog)
            NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1

        End While
        Call sqlReader.Close()

        'Check for open Payments
        sqlStmt = "SELECT CustId, DocBal, DocDate, DocType, OrigDocAmt, PerPost, RefNbr, Terms FROM ARDoc WHERE DocType = 'PA' AND OpenDoc = 1 AND CpnyID =" + SParm(CpnyId)
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open AR Payments exist. It is recommended that all open Payments are applied to Invoices prior to migrating data." + vbNewLine + "List of open AR Payments:"

            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()
            Call SetARDocValues(sqlReader, bARDocInfo)
            'Write CustID, RefNbr, and DocBal to event log
            Call LogMessage("Customer: " + bARDocInfo.CustId + vbTab + "Reference Nbr: " + bARDocInfo.RefNbr + vbTab + "Open Balance: " + bARDocInfo.DocBal.ToString + vbTab + "Original Balance: " + bARDocInfo.OrigDocAmt.ToString, oEventLog)
            NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1

        End While
        Call sqlReader.Close()

        'Check for open Pre-Payments
        sqlStmt = "SELECT CustId, DocBal, DocDate, DocType,  OrigDocAmt, PerPost, RefNbr, Terms FROM ARDoc WHERE DocType = 'PP' AND OpenDoc = 1 AND CpnyID =" + SParm(CpnyId)
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open AR Prepayments exist. It is recommended that all open Prepayments are applied to Invoices prior to migrating data." + vbNewLine + "List of open AR Prepayments:"

            Call LogMessage(msgText, oEventLog)
        End If

        While sqlReader.Read()
            Call SetARDocValues(sqlReader, bARDocInfo)
            'Write CustID, RefNbr, and DocBal to event log
            Call LogMessage("Customer: " + bARDocInfo.CustId + vbTab + "Reference Nbr: " + bARDocInfo.RefNbr + vbTab + "Open Balance: " + bARDocInfo.DocBal.ToString + vbTab + "Original Balance: " + bARDocInfo.OrigDocAmt.ToString, oEventLog)
            NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1

        End While
        Call sqlReader.Close()


        '********************************************************
        '*** Check for open AR docs with an orphaned Terms ID ***
        '********************************************************
        sqlStmt = "SELECT CustId, DocBal, DocDate, DocType, OrigDocAmt, PerPost, RefNbr, Terms FROM ARDoc WHERE OpenDoc = 1 AND DocBal = OrigDocAmt AND DocType IN ('IN', 'DM', 'CM') AND Terms NOT IN (SELECT TermsId FROM Terms WHERE ApplyTo IN ('B', 'C')) AND CpnyID =" + SParm(CpnyId)
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Open AR documents exist with an invalid Payment Terms ID" + vbNewLine + "List of open documents with an invalid Payment Terms ID:"

            Call LogMessage(msgText, oEventLog)
        End If

        While sqlReader.Read()
            Call SetARDocValues(sqlReader, bARDocInfo)
            'Write CustID, RefNbr, and DocBal to event log
            Call LogMessage("Customer: " + bARDocInfo.CustId + vbTab + "Reference Nbr: " + bARDocInfo.RefNbr + vbTab + "Doc Type: " + bARDocInfo.DocType + vbTab + "Terms ID: " + bARDocInfo.Terms, oEventLog)
            NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1

        End While
        Call sqlReader.Close()


        '*********************************************************************************************
        'Verify document date falls within the date range for the period to post for open AR documents
        'Do not include documents with a DocType of 'RC' 
        '*********************************************************************************************
        Try

            Dim connStr As String = ""
            Dim sqlTranConn As SqlConnection = Nothing
            Dim sqlTranReader As SqlDataReader = Nothing

            Dim RecordParmList As New List(Of ParmList)
            Dim ParmValues As ParmList

            sqlStmt = "SELECT CustId, DocBal, DocDate, DocType, OrigDocAmt, PerPost, RefNbr, Terms FROM ARDoc WHERE CpnyID =" + SParm(CpnyId) + "AND DocType <> 'RC' AND OpenDoc = 1 AND DocBal = OrigDocAmt ORDER BY ARDoc.CustId, ARDoc.RefNbr"
            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

            Call LogMessage("", oEventLog)

            ' If any rows were found, then open a second connection for reading the GL Period info.
            If (sqlReader.HasRows) Then
                ' Open the connection to the app database.
                sqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                sqlTranConn.Open()

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



            End If

            While sqlReader.Read() And OkToContinue = True
                Call SetARDocValues(sqlReader, bARDocInfo)
                'Get Document Date and Document Period Number
                docDateStr = bARDocInfo.DocDate.ToShortDateString
                docPerNbr = bARDocInfo.PerPost.Substring(4, 2)



                'Get Period to Post based on Document Date
                RecordParmList.Item(0).ParmValue = bARDocInfo.DocDate
                Call sqlFetch_1(sqlTranReader, "ADG_GLPeriod_GetPeriodFromDate", sqlTranConn, CommandType.StoredProcedure, RecordParmList)
                If (sqlTranReader.HasRows = True) Then
                    Call sqlTranReader.Read()
                    bPerNbrCheckInfo.PerPost = sqlTranReader("Period")
                End If
                sqlTranReader.Close()


                'Write warnings to the event log
                If (bARDocInfo.PerPost <> bPerNbrCheckInfo.PerPost) Then
                    If docOutsidePerPostFound = False Then
                        'Display Warning message
                        msgText = "WARNING: Open AR documents exist where the Document Date does not fall within the date range for the Period to Post."
                        msgText = msgText + " Migrated open documents for the current year will be posted to the new system based on the Document Date, not the SL Period to Post."
                        msgText = msgText + " If any open documents exist from the prior fiscal year, they will be posted to the first day of the first accounting period in the new system."
                        msgText = msgText + vbNewLine + "List of open documents where Document Date does not fall within Period to Post date range: "
                        Call LogMessage("", oEventLog)
                        Call LogMessage(msgText, oEventLog)
                        docOutsidePerPostFound = True
                    End If

                    msgText = "Customer: " + bARDocInfo.CustId + " RefNbr: " + bARDocInfo.RefNbr + " Type: " + bARDocInfo.DocType + vbTab + vbTab + "DocDate: " + docDateStr.ToString + vbTab + "PerPost: " + bARDocInfo.PerPost
                    Call LogMessage(msgText, oEventLog)
                    NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1
                End If

            End While
            Call sqlReader.Close()

        Catch ex As Exception
            Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

            Call LogMessage("", oEventLog)
            Call LogMessage("Error encountered while validating open AR document dates fall within the assigned period to post date range.", oEventLog)
            Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
            Call LogMessage("", oEventLog)
            OkToContinue = False
            NbrOfErrors_Cust = NbrOfErrors_Cust + 1
            Exit Sub
        End Try


        '************************************************************
        '*** Verify Customer Balances for Open Balance migrations ***
        '************************************************************
        If OkToContinue = True Then

            Try
                Call CheckCustomerBalances(oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while validating Customer balances", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                Exit Sub
            End Try

        End If

        '  Detect AR_Balance records with blank key fields - keys are VendId, CpnyId
        If OkToContinue = True Then
            sqlStmt = "SELECT COUNT(*) FROM AR_Balances WHERE RTRIM(CpnyId) = '' or RTRIM(CustId) = ''"
            Call sqlFetch_Num(nbrKeysBlk, sqlStmt, SqlAppDbConn)

            If nbrKeysBlk > 0 Then
                Try
                    Call LogMessage("", oEventLog)
                    Call LogMessage("Number of AR_Balances records found with a blank Key Field: " + CStr(nbrKeysBlk), oEventLog)

                    Call LogMessage("", oEventLog)


                    ' Get the list of accthist records with one or more blank keys.
                    sqlStmt = "SELECT CpnyId, CustId FROM AR_Balances WHERE RTRIM(CpnyId) = '' or RTRIM(CustId) = ''"
                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    While sqlReader.Read()
                        Call SetARBalListvalues(sqlReader, bARBalList)

                        ' Report in the Event Log. 
                        Call LogMessage("AR_Balances record has blank key fields: ", oEventLog)
                        Call LogMessage("  CpnyID:" + bARBalList.CpnyId.Trim, oEventLog)
                        Call LogMessage("  CustId:" + bARBalList.CustID.Trim, oEventLog)

                        Call LogMessage("", oEventLog)


                        NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1

                    End While
                Catch
                End Try

                Call sqlReader.Close()
            End If
        End If

        Call oEventLog.LogMessage(EndProcess, "Validate Customer")

        Call MessageBox.Show("Customer validation complete", "Customer Validation")


        ' Display the event log just created.
        Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.CustEventLogName = oEventLog.LogFile.FullName
        End If



    End Sub


End Module
