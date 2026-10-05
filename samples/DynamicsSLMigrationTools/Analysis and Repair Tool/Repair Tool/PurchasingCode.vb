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
        oEventLog.FileName = "SL-PO-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")

        '=====================================================================================
        ' SScatliffe 10/8/2025 - VSTS 134249/145393 - Check for voided batches and warn user
        '=====================================================================================

        Call VoidedBatchCheck("PO")

        If VBatchesExistPO = True Then

            Call LogMessage("", oEventLog)
            Call LogMessage("WARNING: Voided batches exist in Purchasing.", oEventLog)
            Call LogMessage("", oEventLog)

            ' list voided batches
            sqlStmt = "SELECT BatNbr, EditScrnNbr, PerPost FROM Batch WHERE Module = 'PO' AND Status = 'V' AND CpnyID = " + SParm(CpnyId) + " AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim)

            Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

            While sqlReader.Read()

                Call SetBatchValues(sqlReader, bBatchInfo)
                'Write Batch info to event log
                Call LogMessage("Batch: " + bBatchInfo.BatNbr.Trim + vbTab + "Period: " + FormatPeriodNbr(bBatchInfo.PerPost) + vbTab + vbTab + "Screen: " + GetScreenName(bBatchInfo.EditScrnNbr), oEventLog)
                NbrOfWarnings_PO = NbrOfWarnings_PO + 1

            End While

            Call sqlReader.Close()
            Call LogMessage("", oEventLog)

        End If

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

        '*******************************************************************************************
        '*** Check for invalid BuyerEmail, ShipEmail, and VendEmail addresses on Purchase Orders ***
        '*******************************************************************************************
        sqlStmt = "SELECT PONbr, POType, Status, VendID, BuyerEmail, ShipEmail, VendEmail FROM PurchOrd WHERE RTRIM(PONbr) <> '' AND (RTRIM(BuyerEmail) <> '' OR RTRIM(ShipEmail) <> '' OR RTRIM(VendEmail) <> '')"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        Dim firstInvalidEmailFound As Boolean = False

        While (sqlReader.Read())

            Call SetPurchOrdValues(sqlReader, bPurchOrdInfo)

            Dim hasInvalidEmail As Boolean = False

            ' Check BuyerEmail
            If Not String.IsNullOrWhiteSpace(bPurchOrdInfo.BuyerEmail) AndAlso Not HelperFunctions.IsValidEmail(bPurchOrdInfo.BuyerEmail) Then
                hasInvalidEmail = True
            End If

            ' Check ShipEmail
            If Not String.IsNullOrWhiteSpace(bPurchOrdInfo.ShipEmail) AndAlso Not HelperFunctions.IsValidEmail(bPurchOrdInfo.ShipEmail) Then
                hasInvalidEmail = True
            End If

            ' Check VendEmail
            If Not String.IsNullOrWhiteSpace(bPurchOrdInfo.VendEmail) AndAlso Not HelperFunctions.IsValidEmail(bPurchOrdInfo.VendEmail) Then
                hasInvalidEmail = True
            End If

            ' If any email is invalid, log it
            If hasInvalidEmail Then

                ' Check if this is the first occurrence of an invalid email address
                If Not firstInvalidEmailFound Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)
                    msgText = "WARNING: Invalid email address(es) found on Purchase Orders. Email addresses must be in a valid format."
                    msgText = msgText + vbNewLine + "List of Purchase Orders with invalid email addresses:"
                    Call LogMessage(msgText, oEventLog)
                    firstInvalidEmailFound = True
                End If

                ' Write PO details and invalid email addresses to event log
                Call LogMessage("PO Number: " + bPurchOrdInfo.PONbr + vbTab + "Vendor ID: " + bPurchOrdInfo.VendID, oEventLog)

                If Not String.IsNullOrWhiteSpace(bPurchOrdInfo.BuyerEmail) AndAlso Not HelperFunctions.IsValidEmail(bPurchOrdInfo.BuyerEmail) Then
                    Call LogMessage("  - Other Information tab: " + bPurchOrdInfo.BuyerEmail, oEventLog)
                End If

                If Not String.IsNullOrWhiteSpace(bPurchOrdInfo.ShipEmail) AndAlso Not HelperFunctions.IsValidEmail(bPurchOrdInfo.ShipEmail) Then
                    Call LogMessage("  - Shipping Information tab: " + bPurchOrdInfo.ShipEmail, oEventLog)
                End If

                If Not String.IsNullOrWhiteSpace(bPurchOrdInfo.VendEmail) AndAlso Not HelperFunctions.IsValidEmail(bPurchOrdInfo.VendEmail) Then
                    Call LogMessage("  - Vendor Information tab: " + bPurchOrdInfo.VendEmail, oEventLog)
                End If

                Call LogMessage("", oEventLog)
                NbrOfWarnings_PO = NbrOfWarnings_PO + 1
            End If

        End While

        Call sqlReader.Close()


        '****************************************************************
        '*** Check for invalid Purchase Order Address email addresses ***
        '****************************************************************
        sqlStmt = "SELECT VendId, OrdFromId, EmailAddr, Phone FROM POAddress WHERE RTRIM(EmailAddr) <> ''"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        Dim firstInvalidPOAddressEmailFound As Boolean = False

        While sqlReader.Read()
            Dim vendorId As String = Convert.ToString(sqlReader("VendId")).Trim
            Dim orderFromId As String = Convert.ToString(sqlReader("OrdFromId")).Trim
            Dim emailAddress As String = Convert.ToString(sqlReader("EmailAddr")).Trim

            If Not HelperFunctions.IsValidEmail(emailAddress) Then

                If Not firstInvalidPOAddressEmailFound Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)
                    msgText = "WARNING: Invalid Purchase Order Address email address(es) found. Email addresses must be in a valid format."
                    msgText = msgText + vbNewLine + "List of PO Addresses with invalid email addresses:"
                    Call LogMessage(msgText, oEventLog)
                    firstInvalidPOAddressEmailFound = True
                End If

                Call LogMessage("Vendor ID: " + vendorId + vbTab + "Order From ID: " + orderFromId + vbTab + "Email Address: " + emailAddress, oEventLog)
                NbrOfWarnings_PO = NbrOfWarnings_PO + 1
            End If
        End While

        Call sqlReader.Close()


        '**************************************************************
        '*** Check for invalid Purchase Order Setup email addresses ***
        '**************************************************************
        sqlStmt = "SELECT SetupID, BillEmail, ShipEmail FROM POSetup WHERE RTRIM(BillEmail) <> '' OR RTRIM(ShipEmail) <> ''"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        Dim firstInvalidPOSetupEmailFound As Boolean = False

        While (sqlReader.Read())

            Call SetPOSetupValues(sqlReader, bPOSetupInfo)

            Dim hasInvalidEmail As Boolean = False

            ' Check BillEmail
            If Not String.IsNullOrEmpty(bPOSetupInfo.BillEmail) AndAlso Not HelperFunctions.IsValidEmail(bPOSetupInfo.BillEmail) Then
                hasInvalidEmail = True
            End If

            ' Check ShipEmail
            If Not String.IsNullOrEmpty(bPOSetupInfo.ShipEmail) AndAlso Not HelperFunctions.IsValidEmail(bPOSetupInfo.ShipEmail) Then
                hasInvalidEmail = True
            End If

            ' If any email is invalid, log it
            If hasInvalidEmail Then

                ' Check if this is the first occurrence of an invalid email address
                If Not firstInvalidPOSetupEmailFound Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)
                    msgText = "WARNING: Invalid Purchase Order Setup email address(es) found. Email addresses must be in a valid format."
                    msgText = msgText + vbNewLine + "List of PO Setup records with invalid email addresses:"
                    Call LogMessage(msgText, oEventLog)
                    firstInvalidPOSetupEmailFound = True
                End If

                ' Write PO Setup details and invalid email addresses to event log
                Call LogMessage("Setup ID: " + bPOSetupInfo.SetupID, oEventLog)

                If Not String.IsNullOrEmpty(bPOSetupInfo.BillEmail) AndAlso Not HelperFunctions.IsValidEmail(bPOSetupInfo.BillEmail) Then
                    Call LogMessage("  - Bill Email: " + bPOSetupInfo.BillEmail, oEventLog)
                End If

                If Not String.IsNullOrEmpty(bPOSetupInfo.ShipEmail) AndAlso Not HelperFunctions.IsValidEmail(bPOSetupInfo.ShipEmail) Then
                    Call LogMessage("  - Ship Email: " + bPOSetupInfo.ShipEmail, oEventLog)
                End If

                Call LogMessage("", oEventLog)
                NbrOfWarnings_PO = NbrOfWarnings_PO + 1
            End If

        End While

        Call sqlReader.Close()


        '***********************************************************************************************************************
        '*** Check for invalid Purchase Order Address phone numbers ***
        '***********************************************************************************************************************
        sqlStmt = "SELECT VendId, OrdFromId, Phone FROM POAddress WHERE TRIM(Phone) <> ''"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        Dim firstInvalidPOAddressPhoneFound As Boolean = False

        While sqlReader.Read()
            Dim vendorId As String = Convert.ToString(sqlReader("VendId")).Trim
            Dim orderFromId As String = Convert.ToString(sqlReader("OrdFromId")).Trim
            Dim phoneNumber As String = Convert.ToString(sqlReader("Phone")).Trim

            'Check if phone number is valid using HelperFunctions.IsValidPhoneNumber
            If Not HelperFunctions.IsValidPhoneNumber(phoneNumber) Then

                'Check if this is the first occurrence of an invalid phone number
                If Not firstInvalidPOAddressPhoneFound Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)
                    msgText = "WARNING: Invalid Purchase Order Address phone number(s) found. Phone numbers must be in a valid format."
                    msgText = msgText + vbNewLine + "List of PO Addresses with invalid phone numbers:"
                    Call LogMessage(msgText, oEventLog)
                    firstInvalidPOAddressPhoneFound = True
                End If

                'Write PO Address details and phone number to event log
                Call LogMessage("Vendor ID: " + vendorId + vbTab + "Order From ID: " + orderFromId + vbTab + "Phone: " + phoneNumber, oEventLog)
                NbrOfWarnings_PO = NbrOfWarnings_PO + 1
            End If

        End While

        Call sqlReader.Close()


        '*******************************************
        '*** Remove time values from date fields ***
        '*******************************************

        Try
            Call UpdateDates_PO(oEventLog)

        Catch ex As Exception
            Call MessageBox.Show(ex.Message + vbNewLine + ex.StackTrace, "Error", MessageBoxButtons.OK)

            Call LogMessage("", oEventLog)
            Call LogMessage("Error in removing time values in date fields - Purchasing", oEventLog)
            Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
            Call LogMessage("", oEventLog)
            OkToContinue = False
            NbrOfWarnings_PO = NbrOfWarnings_PO + 1
        End Try

        'Call oEventLog.LogMessage(EndProcess, "Validate Purchasing")
        Call oEventLog.LogMessage(EndProcess, "Repair Tool " & gcReleaseVersion.Trim & vbNewLine & "Validate Purchasing")

        Call MessageBox.Show("Purchasing validation complete.", "Purchasing Validation")

        ' Display the event log just created.
        'Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.POEventLogName = oEventLog.LogFile.FullName
        End If



    End Sub


End Module
