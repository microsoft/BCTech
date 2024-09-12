
Imports System.Data.SqlClient


Module PurchasingCode
    '=======================================================================================
    ' This module contains code to prepare the Purchasing data for migration
    '
    '=======================================================================================

    Public Sub Validate()
        '====================================================================================
        ' Validate Purchasing related records
        '   - PurchOrd.PONbr is not blank
        '   - PurOrdDet.PONbr is not blank
        '   - Check for invalid Vendor IDs on POs
        '   - Purchase Orders with no detail lines
        '   - PurOrdDet.InvtID is not longer than 20 characters
        '   - PO Lines with InvtIDs that have a Transaction Status of Inactive or Delete
        '   - Verify a non-stock Inventory item is selected for Freight Charges if PO lines exist with the Purchase For set to Freight Charges
        '====================================================================================
        Dim nbrPONbrBlank As Integer
        Dim nbrPONbrBlank_Line As Integer
        Dim msgText As String = String.Empty
        Dim invtID20PlusExists As Boolean = False
        Dim poItemErrorExists As Boolean = False
        Dim sqlStmt As String = ""

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
        oEventLog.FileName = "SL-PO-" & "-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")

        Call oEventLog.LogMessage(0, "Processing Purchase Orders")

        '*************************************************************************************************************
        '*** Delete any PurchOrd records where PONbr field is blank since this is the only key field in this table ***
        '*************************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE POType = 'OR' AND Status IN ('O', 'P') AND RTRIM(PONbr) = ''"
        Call sqlFetch_Num(nbrPONbrBlank, sqlStmt, SqlAppDbConn)

        If nbrPONbrBlank > 0 Then
            Call LogMessage("Number of PurchOrd records found with a blank PONbr field: " + CStr(nbrPONbrBlank), oEventLog)

            'Delete PurchOrd records with a blank PONbr field and log the number of PurchOrd records deleted
            Try
                sqlStmt = "DELETE FROM PurchOrd WHERE POType = 'OR' AND Status IN ('O', 'P') AND RTRIM(PONbr) = ''"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                If nbrPONbrBlank = 1 Then
                    Call LogMessage("Deleted " + CStr(nbrPONbrBlank) + " PurchOrd record with a blank PONbr field.", oEventLog)

                Else
                    Call LogMessage("Deleted " + CStr(nbrPONbrBlank) + " PurchOrd records with a blank PONbr field.", oEventLog)

                End If
                Call LogMessage("", oEventLog)
                Call LogMessage("", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting PurchOrd record(s) with a blank PONbr field", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
				NbrOfErrors_PO = NbrOfErrors_PO + 1
                Call MessageBox.Show("Error Encountered: " + ex.Message.Trim + " Operation ended.")
                Exit Sub
			End Try
		End If


        '***********************************************************************************************************************
        '*** Delete any PurOrdDet records where PONbr field is blank since this is the main key field in the PurOrdDet table ***
        '***********************************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM PurOrdDet WHERE RTRIM(PONbr) = ''"
        Call sqlFetch_Num(nbrPONbrBlank_Line, sqlStmt, SqlAppDbConn)

        If nbrPONbrBlank_Line > 0 Then
            Call LogMessage("Number of PurOrdDet records found with a blank PONbr field: " + CStr(nbrPONbrBlank_Line), oEventLog)

            'Delete PurchOrd records with a blank PONbr field and log the number of PurchOrd records deleted
            Try
                sqlStmt = "DELETE FROM PurOrdDet WHERE RTRIM(PONbr) = ''"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                If nbrPONbrBlank_Line = 1 Then
                    Call LogMessage("Deleted " + CStr(nbrPONbrBlank_Line) + " PurOrdDet record with a blank PONbr field.", oEventLog)
                Else
                    Call LogMessage("Deleted " + CStr(nbrPONbrBlank_Line) + " PurOrdDet records with a blank PONbr field.", oEventLog)

                End If
                Call LogMessage("", oEventLog)
                Call LogMessage("", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                LogMessage("", oEventLog)
                LogMessage("Error encountered while deleting PurOrdDet record(s) with a blank PONbr field", oEventLog)
                LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                LogMessage("", oEventLog)
                OkToContinue = False
				NbrOfErrors_PO = NbrOfErrors_PO + 1
                Call MessageBox.Show("Error Encountered: " + ex.Message.Trim + " Operation ended.")
                Exit Sub
			End Try
		End If

        '*******************************************************
        '*** Check for invalid Vendor IDs on Purchase Orders ***
        '*******************************************************
        sqlStmt = "SELECT PONbr, POType, Status, VendID FROM PurchOrd WHERE PurchOrd.POType = 'OR' AND PurchOrd.Status IN ('O', 'P') AND RTRIM(PurchOrd.VendID) <> '' AND PurchOrd.VendID NOT IN (SELECT VendID FROM Vendor)"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() Then
            'Write Error message to event log
            msgText = "ERROR: The following Purchase Order(s) have an invalid Vendor ID."
            Call LogMessage(msgText, oEventLog)

            While sqlReader.Read()

                Call SetPurchOrdValues(sqlReader, bPurchOrdInfo)
                'Write PONbr and VendID to event log
                Call LogMessage("PO: " + bPurchOrdInfo.PONbr + vbTab + "Vendor ID: " + bPurchOrdInfo.VendID, oEventLog)
                NbrOfErrors_PO = NbrOfErrors_PO + 1

            End While
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)

        End If
        sqlReader.Close()


        '****************************************************************************
        '*** Check for PurOrdDet records with an InvtID longer than 20 characters ***
        '****************************************************************************
        sqlStmt = "SELECT InvtId, LineRef, PONbr FROM PurOrdDet WHERE LEN(PurOrdDet.InvtID) > 20 AND PurOrdDet.PONbr IN (SELECT PONbr FROM PurchOrd WHERE PurchOrd.POType = 'OR' AND PurchOrd.Status IN ('O', 'P'))"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows Then
            'Write Error message to event log
            msgText = "ERROR: The following Purchase Order lines have an Inventory ID longer than 20 characters. Any Inventory ID longer than 20 characters will need to be updated to be 20 characters or less before data can be migrated."
            Call LogMessage(msgText, oEventLog)

            invtID20PlusExists = True
        End If

        While sqlReader.Read()
            Call SetPurOrdDetValues(sqlReader, bPurOrdDetInfo)
            'Write PONbr, LineRef, InvtID to event log
            Call LogMessage("PO: " + bPurOrdDetInfo.PONbr + vbTab + "Line Ref: " + bPurOrdDetInfo.LineRef + vbTab + "Item: " + bPurOrdDetInfo.InvtID, oEventLog)

            NbrOfErrors_PO = NbrOfErrors_PO + 1

        End While
        Call sqlReader.Close()

        'Display message in Event Log for suggested actions
        If invtID20PlusExists = True Then
            Call LogMessage(" ", oEventLog)
            msgText = "Suggested actions for updating Inventory IDs are listed below:" + vbNewLine
			msgText = msgText + " - Use the Professional Services Tools Library (PSTL) application to modify the Inventory IDs to 20 characters or less" + vbNewLine
			msgText = msgText + " - Set the Transaction Status on these items to Inactive or Delete in Inventory Items (10.250.00) to exclude these items from the migration" + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner  for further assistance"
            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)

        End If


        '*********************************************************************************************
        '*** Check for PurOrdDet records with InvtID that have a Tran Status of Inactive or Delete ***
        '*********************************************************************************************
        sqlStmt = "SELECT d.InvtId, d.LineRef, d.PONbr FROM PurOrdDet d JOIN PurchOrd p ON p.PONbr = d.PONbr JOIN Inventory i ON i.InvtID = d.InvtID WHERE p.POType = 'OR' AND p.Status IN ('O', 'P') AND i.TranStatusCode IN ('IN', 'DE')"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            msgText = "ERROR: The following Purchase Order lines have an Inventory ID that has a Transaction Status of either Inactive or Delete. "
            msgText = msgText + "Items with a Transaction Status of Inactive or Delete are not migrated."
            Call LogMessage(msgText, oEventLog)

            poItemErrorExists = True
        End If

        While sqlReader.Read()

            Call SetPurOrdDetValues(sqlReader, bPurOrdDetInfo)
            'Write PONbr, LineRef, and InvtID to event log
            Call LogMessage("PO: " + bPurOrdDetInfo.PONbr + vbTab + "Line Ref: " + bPurOrdDetInfo.LineRef + vbTab + "Item: " + bPurOrdDetInfo.InvtID, oEventLog)
            NbrOfErrors_PO = NbrOfErrors_PO + 1

        End While
        Call sqlReader.Close()

        'Display message in Event Log for suggested actions
        If poItemErrorExists = True Then
            Call LogMessage("", oEventLog)
            msgText = "Suggested actions for resolving this error:" + vbNewLine
			msgText = msgText + " - Delete the listed line(s) from the Purchase Order." + vbNewLine
			msgText = msgText + " - Change the Transaction Status of the item to a value other than Inactive or Delete in Inventory Items (10.250.00)."
			msgText = msgText + " - Change the status of the Purchase Order to Completed."
            Call LogMessage(msgText, oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
        End If


        '******************************************************
        '*** Check for Purchase Orders without detail lines ***
        '******************************************************
        sqlStmt = "Select PONbr, POType, Status, VendId from PurchOrd WHERE PurchOrd.POType = 'OR' AND PurchOrd.Status IN ('O', 'P') AND PurchOrd.PONbr NOT IN (SELECT PONbr FROM PurOrdDet)"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() Then
            'Write Error message to event log
            msgText = "WARNING: The following Purchase Order(s) do not have any detail lines."
            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()
            Call SetPurchOrdValues(sqlReader, bPurchOrdInfo)
            'Write PONbr to event log
            Call LogMessage("PO: " + bPurchOrdInfo.PONbr, oEventLog)
            NbrOfWarnings_PO = NbrOfWarnings_PO + 1

        End While
        sqlReader.Close()

        Call oEventLog.LogMessage(EndProcess, "Validate Purchasing")

        Call MessageBox.Show("Purchasing Validation Complete", "Purchasing Validation")

        ' Display the event log just created.
        Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.POEventLogName = oEventLog.LogFile.FullName
        End If



    End Sub


End Module
