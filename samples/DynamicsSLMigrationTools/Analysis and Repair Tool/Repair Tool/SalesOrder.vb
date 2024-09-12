
Imports System.Data.SqlClient




Module SalesOrder

    ' This module contains code to prepare the Vendor data for migration
    '=======================================================================================

    Public Sub Validate()

        Dim sqlStmt As String = String.Empty
        Dim nbrKeysBlk As Integer = 0
        Dim KeyString As String = String.Empty

        Dim nbrOrdNbrBlank As Integer
        Dim msgText As String = String.Empty

        Dim strOrdNbr As String = String.Empty

        Dim oEventLog As clsEventLog
        Dim SqlReader As SqlDataReader = Nothing


        Dim fmtDate As String

        fmtDate = Date.Now.ToString
        fmtDate = fmtDate.Replace(":", "")
        fmtDate = fmtDate.Replace("/", "")
        fmtDate = fmtDate.Remove(fmtDate.Length - 3)
        fmtDate = fmtDate.Replace(" ", "-")
        fmtDate = fmtDate & Date.Now.Millisecond

        oEventLog = New clsEventLog
        oEventLog.FileName = "SL-SO-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")

        Call oEventLog.LogMessage(0, "Processing Sales Orders")
        Call oEventLog.LogMessage(0, "")

        '***********************************************************************************************************************************************
        '*** Delete any Sales Order records where OrdNbr field is blank  ***
        '***********************************************************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM SOHeader WHERE RTRIM(OrdNbr) = '' and CpnyId = " & SParm(CpnyId)
        Call sqlFetch_Num(nbrOrdNbrBlank, sqlStmt, SqlAppDbConn)

        If nbrOrdNbrBlank > 0 Then
            Call LogMessage("Number of Sales Order records found with a blank OrdNbrD Field: " + CStr(nbrOrdNbrBlank), oEventLog)


            'Delete Sales Order records and associated SOLine records with a blank OrdNbr field and log the number of Sales Order records deleted
            Try
                sqlStmt = "Delete from SOHeader WHERE RTRIM(OrdNbr) = ''"
                Call sql_1(SqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                Call LogMessage("Deleted " + CStr(nbrOrdNbrBlank) + " Sales Order record(s) with a blank OrdNbr field.", oEventLog)
                Call LogMessage("", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)
                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting Sales Order record(s) with a blank OrdNbr field", oEventLog)
                Call LogMessage("", oEventLog)

                OkToContinue = False
                NbrOfErrors_SO = NbrOfErrors_SO + 1
            End Try
        End If

        '***********************************************************************************************************************************************
        '*** Delete any SOLine records where OrdNbr field is blank  ***
        '***********************************************************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM SOLine WHERE RTRIM(OrdNbr) = '' and CpnyId = " & SParm(CpnyId)
        Call sqlFetch_Num(nbrOrdNbrBlank, sqlStmt, SqlAppDbConn)


        If nbrOrdNbrBlank > 0 Then
            Call LogMessage("Number of SOLine records found with a blank OrdNbrD Field: " + CStr(nbrOrdNbrBlank), oEventLog)


            'Delete Sales Order records and associated SOLine records with a blank OrdNbr field and log the number of Sales Order records deleted
            Try
                sqlStmt = "Delete from SOLine WHERE RTRIM(OrdNbr) = ''"
                Call sql_1(SqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                Call LogMessage("Deleted " + CStr(nbrOrdNbrBlank) + " SOLine record(s) with a blank OrdNbr field.", oEventLog)


                Call LogMessage("", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting SOLine record(s) with a blank OrdNbr field", oEventLog)
                Call LogMessage("", oEventLog)

                OkToContinue = False
                NbrOfErrors_SO = NbrOfErrors_SO + 1
            End Try
        End If


        '***********************************************************************************************************************************************
        '*** Report any Sales Order records with no sales order lines  ***
        '***********************************************************************************************************************************************
        sqlStmt = "SELECT OrdNbr FROM SOHEADER H WHERE H.CpnyId = " & SParm(CpnyId) & " AND NOT EXISTS (SELECT * FROM SOLINE S WHERE S.OrdNbr = H.OrdNbr)"
        Call sqlFetch_1(SqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If SqlReader.HasRows() Then

            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "WARNING:  Sales Orders exist with no detail lines ."
            msgText = msgText + vbNewLine + "List of Sales Orders:"


            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
        End If
        While SqlReader.Read()

            Call SetOrdNbrListValues(SqlReader, bOrdNbrListInfo)

            'Write Sales Order to event log
            Call LogMessage("Sales Order: " + bOrdNbrListInfo.OrdNbr, oEventLog)

            NbrOfWarnings_SO = NbrOfWarnings_SO + 1
        End While
        Call SqlReader.Close()

        '***********************************************************************************************************************************************
        '*** Report any Shipper records with no SOShipLines  ***
        '***********************************************************************************************************************************************
        sqlStmt = "SELECT ShipperID FROM SOSHIPHEADER H WHERE H.CpnyId = " & SParm(CpnyId) & " AND NOT EXISTS (SELECT * FROM SOShipLine S WHERE S.ShipperID = H.ShipperID)"
        Call sqlFetch_1(SqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If SqlReader.HasRows() Then
            'Write Warning message to event logWhat
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "WARNING:  Shippers exist with no detail lines ."
            msgText = msgText + vbNewLine + "List of Shippers:"
            Call LogMessage(msgText, oEventLog)
        End If
        While SqlReader.Read()

            Call SetShipperListValues(SqlReader, bShipperListInfo)
            'Write Sales Order to event log
            Call LogMessage("Shipper: " + bShipperListInfo.ShipperID, oEventLog)

            NbrOfWarnings_SO = NbrOfWarnings_SO + 1
        End While
        Call SqlReader.Close()


        '***********************************************************************************************************************************************
        '*** Report any orders with invalid custoer ***
        '***********************************************************************************************************************************************
        sqlStmt = "Select OrdNbr, CustId From SOHeader H Where  CpnyId = " & SParm(CpnyId) & " And Status = 'O' And NOT EXISTS (SELECT * FROM Customer C WHERE C.CustID = H.CustId)"
        Call sqlFetch_1(SqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If SqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: SOHeader Record contains a non-existent customer."

            msgText = msgText + vbNewLine + "The following Sales Order(s) have an invalid Customer ID."
            msgText = msgText + vbNewLine + "List of Sales Orders and Customers:"


            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
        End If
        While SqlReader.Read()

            Call SetOrdNbrCustListValues(SqlReader, bOrdNbrCustListInfo)
            'Write Sales Order to event log
            Call LogMessage("Sales Order: " + bOrdNbrCustListInfo.OrdNbr, oEventLog)
            Call LogMessage("Customer: " + bOrdNbrCustListInfo.CustID + vbNewLine, oEventLog)


            NbrOfErrors_SO = NbrOfErrors_SO + 1
        End While
        Call SqlReader.Close()

        '***********************************************************************************************************************************************
        '*** Report any shippers with invalid customer ***
        '***********************************************************************************************************************************************
        sqlStmt = "Select ShipperId, CustId From SOShipHeader S Where  CpnyId = " & SParm(CpnyId) & " And Status = 'O' And NOT EXISTS (SELECT * FROM Customer C WHERE C.CustID = S.CustId)"
        Call sqlFetch_1(SqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If SqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: SOShipHeader Record contains a non-existent customer."

            msgText = msgText + vbNewLine + "The following Shipper(s) have an invalid Customer ID."
            msgText = msgText + vbNewLine + "List of Shippers and Customers:"


            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
        End If
        While SqlReader.Read()

            Call SetShipperCustListValues(SqlReader, bShipperCustListInfo)
            'Write Sales Order to event log
            Call LogMessage("Shipper: " + bShipperCustListInfo.ShipperID, oEventLog)
            Call LogMessage("Customer: " + bShipperCustListInfo.CustID + vbNewLine, oEventLog)


            NbrOfErrors_SO = NbrOfErrors_SO + 1
        End While
        Call SqlReader.Close()

        '***********************************************************************************************************************************************
        '*** Report any orders with inventory id > 20 characters ***
        '***********************************************************************************************************************************************
        sqlStmt = "Select OrdNbr, InvtID From SOLine Where  CpnyId = " & SParm(CpnyId) & " And Status = 'O' And LEN(InvtID) > 20"
        Call sqlFetch_1(SqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)


        If SqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = " ERROR: The following Sales Order lines have an Inventory ID longer than 20 characters. Any Inventory ID longer than 20 characters will need to be updated to be 20 characters or less before data can be migrated."
            msgText = msgText + "Suggested actions for updating Inventory IDs are listed below:" + vbNewLine
            msgText = msgText + " - Use the Professional Services Tools Library (PSTL) application to modify the Inventory IDs to 20 characters or less" + vbNewLine
            msgText = msgText + " - Set the Transaction Status on these items to Inactive or Delete in Inventory Items (10.250.00) to exclude these items from the migration" + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)

            msgText = "List of Sales Orders and Inventory Items:"

            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
        End If
        While SqlReader.Read()

            Call SetOrdNbrInvtListValues(SqlReader, bOrdNbrInvtListInfo)
            'Write Sales Order to event log
            Call LogMessage("Sales Order: " + bOrdNbrInvtListInfo.OrdNbr, oEventLog)
            Call LogMessage("Inventory ID: " + bOrdNbrInvtListInfo.InvtID + vbNewLine, oEventLog)


            NbrOfErrors_SO = NbrOfErrors_SO + 1
        End While
        Call SqlReader.Close()


        '***********************************************************************************************************************************************
        '*** Report any shippers with inventory id > 20 characters ***
        '***********************************************************************************************************************************************
        sqlStmt = "Select ShipperID, InvtID From SOShipLine Where  CpnyId = " & SParm(CpnyId) & " And Status = 'O' And LEN(InvtID) > 20"
        Call sqlFetch_1(SqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If SqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = " ERROR: The following Shipper lines have an Inventory ID longer than 20 characters. Any Inventory ID longer than 20 characters will need to be updated to be 20 characters or less before data can be migrated."
            msgText = msgText + "Suggested actions for updating Inventory IDs are listed below:" + vbNewLine
            msgText = msgText + " - Use the Professional Services Tools Library (PSTL) application to modify the Inventory IDs to 20 characters or less" + vbNewLine
            msgText = msgText + " - Set the Transaction Status on these items to Inactive or Delete in Inventory Items (10.250.00) to exclude these items from the migration" + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)

            msgText = "List of Shippers and Inventory Items:"

            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
        End If
        While SqlReader.Read()

            Call setShipperInvtListValues(SqlReader, bShipperInvtListInfo)
            'Write Sales Order to event log
            Call LogMessage("Shipper: " + bShipperInvtListInfo.ShipperID, oEventLog)
            Call LogMessage("Inventory ID: " + bShipperInvtListInfo.InvtID + vbNewLine, oEventLog)


            NbrOfErrors_SO = NbrOfErrors_SO + 1
        End While
        Call SqlReader.Close()



        Call oEventLog.LogMessage(EndProcess, "Validate Sales Order")


        Call MessageBox.Show("Sales Order Validation Complete", "Sales Order Validation")

        ' Display the event log just created.
        Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.SOEventLogName = oEventLog.LogFile.FullName
        End If




    End Sub

End Module
