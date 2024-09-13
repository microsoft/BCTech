'*********************************************************
'
'    Copyright (c) Microsoft. All rights reserved.
'    This code is licensed under the Microsoft Public License.
'    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************
Option Explicit On
Option Strict Off
Imports Microsoft.VisualBasic.FileSystem
Imports System
Imports System.IO
Imports System.Collections
Imports System.Data.SqlClient

Friend Class Form1
    Inherits System.Windows.Forms.Form

    Protected m_IsInitializing As Boolean
    Protected ReadOnly Property IsInitializing() As Boolean
        Get
            Return m_IsInitializing
        End Get
    End Property

    Private Sub Form1_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load

        StatusLbl.Text = "Starting Validation"



        ' Initialize fields for login.
        ' Default the server name to the current machine name.
        NameOfServer.Text = System.Environment.MachineName
        SQLServerName = NameOfServer.Text

        CpnyDBList = New List(Of CpnyDatabase)


        ' Default the authentication type to Windows.
        IsWindowsAuth = True
        rbAuthenticationTypeWindows.Checked = True

        SQLAuthUser = "sa"


        ' Set master password to blank so we can know later if it gets set
        Master_pwd = ""

        ' Set the format of the date controls.
        cDateCompletedCOA.Format = DateTimePickerFormat.Custom
        cDateCompletedCOA.CustomFormat = "MM/dd/yyyy"
        cDateValidatedCust.Format = DateTimePickerFormat.Custom
        cDateValidatedCust.CustomFormat = "MM/dd/yyyy"
        cDateValidatedVend.Format = DateTimePickerFormat.Custom
        cDateValidatedVend.CustomFormat = "MM/dd/yyyy"
        cDateValidatedItem.Format = DateTimePickerFormat.Custom
        cDateValidatedItem.CustomFormat = "MM/dd/yyyy"
        cDateValidatedProj.Format = DateTimePickerFormat.Custom
        cDateValidatedProj.CustomFormat = "MM/dd/yyyy"
        cDateValidatedPurch.Format = DateTimePickerFormat.Custom
        cDateValidatedPurch.CustomFormat = "MM/dd/yyyy"
        cDateValidatedSO.Format = DateTimePickerFormat.Custom
        cDateValidatedSO.CustomFormat = "MM/dd/yyyy"



        ' Disable all tab pages until the database connection is complete.
        DBConnect.Enabled = True
        TabPageAP.Enabled = False
        TabPageAR.Enabled = False
        TabPageGL.Enabled = False
        TabPageIN.Enabled = False
        TabPagePA.Enabled = False
        TabPagePO.Enabled = False
        TabPageSO.Enabled = False


        'Get date values
        CurrDate = Date.Now

    End Sub



    Private Sub cmdValidateAccts_Click(sender As System.Object, e As System.EventArgs) Handles cmdValidateAccts.Click
        Dim msgText As String = String.Empty
        Dim errPerNbr As Boolean = False

        Dim dlgResult As DialogResult
        StatusLbl.Text = "Validating Accounts"


        'Verify Beginning Fiscal Year is populated
        If bSLMPTStatus.FiscYr_Beg.Trim = "" Then
            bSLMPTStatus.FiscYr_Beg = bSLMPTStatus.FiscYr_End
        End If

        'Verify all modules are in the same period
        If ARExists = True Then
            If bARSetupInfo.PerNbr <> bGLSetupInfo.PerNbr Then
                errPerNbr = True
            End If
        End If

        If APExists = True Then
            If bAPSetupInfo.PerNbr <> bGLSetupInfo.PerNbr Then
                errPerNbr = True
            End If
        End If

        If errPerNbr = True Then
            msgText = "All modules must be in the same fiscal period before a validation can be performed."
            msgText = msgText + vbNewLine + vbNewLine + "General Ledger: " + vbTab + vbTab + CurrPeriodGL
            If ARExists = True Then
                msgText = msgText + vbNewLine + "Accounts Receivable: " + vbTab + CurrPeriodAR
            End If
            If APExists = True Then
                msgText = msgText + vbNewLine + "Accounts Payable: " + vbTab + vbTab + CurrPeriodAP
            End If

            Call MessageBox.Show(msgText, "Fiscal Period Error", MessageBoxButtons.OK)

            Exit Sub
        End If

        'Display message if Validation has already been completed successfully
        If bSLMPTStatus.COA_ValCmpltd = 1 Then
            msgText = "The Chart of Accounts validation process has already been completed. Are you sure you want to rerun this process?"

            dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

            If (dlgResult = DialogResult.No) Then
                Exit Sub
            End If
        End If

        'Display process message
        msgText = "This process will validate Chart of Accounts information to ensure the data is ready for migration. Issues found during the validation process will be written to an event log. "
        msgText = msgText + "Please make sure a current and valid backup of the application database is available prior to starting the validation process."
        msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"
        dlgResult = MessageBox.Show(msgText, "Validation Accounts", MessageBoxButtons.YesNo)

        If (dlgResult = DialogResult.Yes) Then

            'Check for unposted batches (CpnyID, LedgerID) and warn user
            Call UnpostedBatchCheck("%")

            If (UBatchesExistGL = True Or UBatchesExistAP = True Or UBatchesExistAR = True Or UBatchesExistIN = True) Then
                'Display message that unposted batches exist
                msgText = "WARNING: Unposted batches exist in the following modules."
                If UBatchesExistGL = True Then
                    msgText = msgText + vbNewLine + "General Ledger"
                End If
                If UBatchesExistAP = True Then
                    msgText = msgText + vbNewLine + "Accounts Payable"
                End If
                If UBatchesExistAR = True Then
                    msgText = msgText + vbNewLine + "Accounts Receivable"
                End If
                If UBatchesExistIN = True Then
                    msgText = msgText + vbNewLine + "Inventory"
                End If
                msgText = msgText + vbNewLine + vbNewLine + "Amounts from unposted balances will not be included in the calculated current balance of an Account." + vbNewLine + vbNewLine + "Do you want to continue?"

                dlgResult = MessageBox.Show(msgText, "Unposted Batches Exist", MessageBoxButtons.YesNo)

                If (dlgResult = DialogResult.No) Then
                    Exit Sub
                End If

            End If

            'Validate Chart of Accounts data
            NbrOfErrors_COA = 0
            NbrOfWarnings_COA = 0
            Call AccountCode.Validate()

            'End process
            If OkToContinue = True Then

                Dim eventLog As clsEventLog = New clsEventLog
                eventLog.FileName = System.IO.Path.GetFileName(bSLMPTStatus.COAEventLogName)



                'Set as Completed if no errors were found
                If NbrOfErrors_COA = 0 Then
                    bSLMPTStatus.COA_ValCmpltd = 1

                    cCompletedCOA.Checked = bSLMPTStatus.COA_ValCmpltd

                    bSLMPTStatus.COA_LValidated = Convert.ToDateTime(CurrDate)

                    cDateCompletedCOA.Value = bSLMPTStatus.COA_LValidated
                    cCompletedGLChk.Checked = bSLMPTStatus.COA_ValCmpltd

                    If NbrOfWarnings_COA > 0 Then
                        Call eventLog.LogMessage(StartProcess, "")

                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Chart of Accounts warnings found: " + NbrOfWarnings_COA.ToString, eventLog)
                        Call eventLog.LogMessage(EndProcess, "")

                    End If

                Else  'Errors found
                    Call eventLog.LogMessage(StartProcess, "")

                    Call LogMessage("", eventLog)
                    Call LogMessage("Number of Chart of Account errors found: " + CStr(NbrOfErrors_COA), eventLog)

                    cCompletedCOA.Checked = bSLMPTStatus.COA_ValCmpltd
                    cCompletedGLChk.Checked = bSLMPTStatus.COA_ValCmpltd



                    If NbrOfWarnings_COA > 0 Then
                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Chart of Accounts warnings found: " + NbrOfWarnings_COA.ToString, eventLog)

                    End If
                    Call eventLog.LogMessage(EndProcess, "")
                    eventLog = Nothing
                End If

            Else
                'Set Completed field to 0 (unchecked) since an error occurred at some point in the process
                bSLMPTStatus.COA_ValCmpltd = 0

                cCompletedCOA.Checked = bSLMPTStatus.COA_ValCmpltd
                cCompletedGLChk.Checked = bSLMPTStatus.COA_ValCmpltd


            End If

            'Update Last Validated Date
            bSLMPTStatus.COA_LValidated = Convert.ToDateTime(CurrDate)
            cDateCompletedCOA.Value = bSLMPTStatus.COA_LValidated

            'Update Number of Errors/Warnings
            bSLMPTStatus.COAErrors = NbrOfErrors_COA
            cErrorsCOAVal.Text = bSLMPTStatus.COAErrors

            bSLMPTStatus.COA_Warnings = NbrOfWarnings_COA
            cWarningsCOAVal.Text = bSLMPTStatus.COA_Warnings


            Call UpdateStatusRecord("Account")

            ' Update the control values
            Call UpdateGLControls()

            StatusLbl.Text = "Account Validation Complete"

        Else
            Exit Sub
        End If

    End Sub


    Private Sub cmdValidateCust_Click(sender As System.Object, e As System.EventArgs) Handles cmdValidateCust.Click
        Dim msgText As String = String.Empty
        Dim dlgResult As DialogResult


        StatusLbl.Text = "Validating Customers"

        'Verify Beginning Fiscal Year is populated
        If bSLMPTStatus.FiscYr_Beg.Trim = "" Then
            bSLMPTStatus.FiscYr_Beg = bSLMPTStatus.FiscYr_End
        End If

        'Verify Current Period Number is populated in ARSetup
        If bARSetupInfo.PerNbr.Trim = "" Then
            msgText = "Current Period Number is missing in AR Setup (08.950.00). The Current Period Number must be populated before a validation can be performed."
            Call MessageBox.Show(msgText, "AR Current Period Number Error", MessageBoxButtons.OK)
            Exit Sub
        End If

        'Verify all modules are in the same period
        If bARSetupInfo.PerNbr <> bGLSetupInfo.PerNbr Then
            msgText = "Accounts Receivable and General Ledger modules must be in the same fiscal period before a validation can be performed."
            msgText = msgText + vbNewLine + vbNewLine + "General Ledger: " + vbTab + vbTab + CurrPeriodGL
            msgText = msgText + vbNewLine + "Accounts Receivable: " + vbTab + CurrPeriodAR
            Call MessageBox.Show(msgText, "Fiscal Period Error", MessageBoxButtons.OK)

            Exit Sub
        End If

        'Display message if Validation has already been completed successfully
        If bSLMPTStatus.Cust_ValCmpltd = 1 Then
            msgText = "The Customer validation process has already been completed. Are you sure you want to rerun this process?"

            dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

            If (dlgResult = DialogResult.No) Then
                Exit Sub
            End If

        End If

        'Display process message
        msgText = "This process will validate Customer information to ensure the data is ready for migration. Issues found during the validation process will be written to an event log. "
        msgText = msgText + "Please make sure a current and valid backup of the application database is available prior to starting the validation process."
        msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"

        dlgResult = MessageBox.Show(msgText, "Validating Customers", MessageBoxButtons.YesNo)

        If (dlgResult = DialogResult.Yes) Then
            'Check for unposted AR batches (CpnyID, LedgerID) and warn user
            UBatchesExistAR = False
            Call UnpostedBatchCheck("AR")
            If UBatchesExistAR = True Then
                msgText = "WARNING: Unposted batches exist in Accounts Receivables."
                msgText = msgText + " Transaction amounts on unposted batches are not included in GL Account history amounts but are included in Customer/AR history amounts."
                msgText = msgText + " This could cause an out of balance situation between exported Customer open balances and exported balances of the GL Accounts associated with Customers"
                msgText = msgText + " on the Defaults tab of Customer Maintenance (08.260.00)."
                msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"

                dlgResult = MessageBox.Show(msgText, "Unposted AR Batches Exist", MessageBoxButtons.YesNo)

                If (dlgResult = DialogResult.No) Then
                    Exit Sub
                End If
            End If


            'Validate Customer data
            NbrOfErrors_Cust = 0
            NbrOfWarnings_Cust = 0
            Call CustomerCode.Validate()


            'End process
            If OkToContinue = True Then
                Dim eventLog As clsEventLog = New clsEventLog
                eventLog.FileName = System.IO.Path.GetFileName(bSLMPTStatus.CustEventLogName)

                'Set as Completed if no errors were found
                If NbrOfErrors_Cust = 0 Then
                    bSLMPTStatus.Cust_ValCmpltd = 1
                    cCompletedARChk.Text = bSLMPTStatus.Cust_ValCmpltd


                    If NbrOfWarnings_Cust > 0 Then
                        Call eventLog.LogMessage(StartProcess, "")

                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Customer warnings found: " + NbrOfWarnings_Cust.ToString, eventLog)
                        Call eventLog.LogMessage(EndProcess, "")

                    End If

                Else  'Errors found

                    Call eventLog.LogMessage(StartProcess, "")

                    Call LogMessage("", eventLog)
                    Call LogMessage("Number of Customer errors found: " + CStr(NbrOfErrors_Cust), eventLog)

                    bSLMPTStatus.Cust_ValCmpltd = 0

                    cCompletedARChk.Text = bSLMPTStatus.Cust_ValCmpltd

                    If NbrOfWarnings_Cust > 0 Then
                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Customer warnings found: " + NbrOfWarnings_Cust.ToString, eventLog)
                    End If
                    Call eventLog.LogMessage(EndProcess, "")
                End If

                eventLog = Nothing

            Else
                'Set Completed field to 0 (unchecked) since an error occurred at some point in the process
                bSLMPTStatus.Cust_ValCmpltd = 0

                cCompletedARChk.Text = bSLMPTStatus.Cust_ValCmpltd

            End If

            'Update Last Validated Date
            bSLMPTStatus.Cust_LValidated = CurrDate


            'Update Number of Errors/Warnings
            bSLMPTStatus.Cust_Errors = NbrOfErrors_Cust

            bSLMPTStatus.Cust_Warnings = NbrOfWarnings_Cust



            Call UpdateStatusRecord("Customer")

            ' Update the control values
            Call UpdateARControls()

            StatusLbl.Text = "Customer Validation Complete"

        Else
            Exit Sub
        End If

    End Sub

    Private Sub cmdValidateVend_Click(sender As System.Object, e As System.EventArgs) Handles cmdValidateVend.Click
        Dim msgText As String = String.Empty
        Dim dlgResult As DialogResult

        StatusLbl.Text = "Validating Vendors"

        'Verify Beginning Fiscal Year is populated
        If bSLMPTStatus.FiscYr_Beg.Trim = "" Then
            bSLMPTStatus.FiscYr_Beg = bSLMPTStatus.FiscYr_End
        End If

        'Verify Current Period Number is populated in APSetup
        If bAPSetupInfo.PerNbr.Trim = "" Then
            msgText = "Current Period Number is missing in AP Setup (03.950.00). The Current Period Number must be populated before a validation can be performed."
            Call MessageBox.Show(msgText, "AP Current Period Number Error", MessageBoxButtons.OK)

            Exit Sub
        End If

        'Verify all modules are in the same period
        If bAPSetupInfo.PerNbr <> bGLSetupInfo.PerNbr Then
            msgText = "Accounts Payable and General Ledger modules must be in the same fiscal period before a validation can be performed."
            msgText = msgText + vbNewLine + vbNewLine + "General Ledger: " + vbTab + vbTab + CurrPeriodGL
            msgText = msgText + vbNewLine + "Accounts Payable: " + vbTab + CurrPeriodAP
            Call MessageBox.Show(msgText, "Fiscal Period Error", MessageBoxButtons.OK)

            Exit Sub
        End If

        'Display message if Validation has already been completed successfully
        If bSLMPTStatus.Vend_ValCmpltd = 1 Then
            msgText = "The Vendor validation process has already been completed. Are you sure you want to rerun this process?"

            dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

            If (dlgResult = DialogResult.No) Then
                Exit Sub
            End If
        End If

        'Display process message
        msgText = "This process will validate Vendor information to ensure the data is ready for migration. Issues found during the validation process will be written to an event log. "
        msgText = msgText + "Please make sure a current and valid backup of the application database is available prior to starting the validation process."
        msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"
        dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

        If (dlgResult = DialogResult.Yes) Then
            'Check for unposted AP batches (CpnyID, LedgerID) and warn user
            UBatchesExistAP = False
            Call UnpostedBatchCheck("AP")
            If UBatchesExistAP = True Then
                msgText = "WARNING: Unposted batches exist in Accounts Payable."
                msgText = msgText + " Transaction amounts on unposted batches are not included in GL Account history amounts but are included in Vendor/AP history amounts."
                msgText = msgText + " This could cause an out of balance situation between exported Vendor open balances and exported balances of the GL Accounts associated with Vendors"
                msgText = msgText + " on the Defaults tab of Vendor Maintenance (03.270.00)."
                msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"
                dlgResult = MessageBox.Show(msgText, "Unposted AP Batches Exist", MessageBoxButtons.YesNo)

                If (dlgResult = DialogResult.No) Then
                    Exit Sub
                End If
            End If


            'Validate Vendor data
            NbrOfErrors_Vend = 0
            NbrOfWarnings_Vend = 0
            Call VendorCode.Validate()

            'End Process
            If OkToContinue = True Then
                Dim eventLog As clsEventLog = New clsEventLog
                eventLog.FileName = System.IO.Path.GetFileName(bSLMPTStatus.VendEventLogName)

                'Set as Completed if no errors were found
                If NbrOfErrors_Vend = 0 Then
                    bSLMPTStatus.Vend_ValCmpltd = 1
                    cCompletedAPChk.Checked = bSLMPTStatus.Vend_ValCmpltd

                    If NbrOfWarnings_Vend > 0 Then
                        Call eventLog.LogMessage(StartProcess, "")

                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Vendor warnings found:  " + NbrOfWarnings_Vend.ToString, eventLog)
                        Call eventLog.LogMessage(EndProcess, "")
                    End If

                Else  'Errors found
                    Call eventLog.LogMessage(StartProcess, "")
                    Call LogMessage("", eventLog)
                    Call LogMessage("Number of Vendor errors found: " + CStr(NbrOfErrors_Vend), eventLog)
                    bSLMPTStatus.Vend_ValCmpltd = 0


                    cCompletedAPChk.Checked = bSLMPTStatus.Vend_ValCmpltd

                    If NbrOfWarnings_Vend > 0 Then
                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Vendor warnings found:  " + NbrOfWarnings_Vend.ToString, eventLog)
                    End If
                    Call eventLog.LogMessage(EndProcess, "")


                End If

                eventLog = Nothing

            Else
                'Set Completed field to 0 (unchecked) since an error occurred at some point in the process
                bSLMPTStatus.Vend_ValCmpltd = 0


                cCompletedAPChk.Checked = bSLMPTStatus.Vend_ValCmpltd


            End If

            'Update Last Validated Date
            bSLMPTStatus.Vend_LValidated = CurrDate


            'Update Number of Errors/Warning
            bSLMPTStatus.Vend_Errors = NbrOfErrors_Vend

            bSLMPTStatus.Vend_Warnings = NbrOfWarnings_Vend


            Call UpdateStatusRecord("Vendor")

            ' Update the control values
            Call UpdateAPControls()

            StatusLbl.Text = "Vendor Validation Complete"

        Else
            Exit Sub
        End If

    End Sub


    Private Sub cmdValidateItem_Click(sender As Object, e As EventArgs) Handles cmdValidateItem.Click
        Dim msgText As String = String.Empty
        Dim reValidatePO As Boolean = False
        Dim dlgResult As DialogResult

        StatusLbl.Text = "Validating Inventory"


        'Verify Beginning Fiscal Year is populated
        If bSLMPTStatus.FiscYr_Beg.Trim = "" Then
            bSLMPTStatus.FiscYr_Beg = bSLMPTStatus.FiscYr_End
        End If

        'Verify Current Period Number is populated in INSetup
        If bINSetupInfo.PerNbr.Trim = "" Then
            msgText = "Current Period Number is missing in IN Setup (10.950.00). The Current Period Number must be populated before a validation can be performed."
            Call MessageBox.Show(msgText, "Inventory Current Period Number Error", MessageBoxButtons.OK)

            Exit Sub
        End If

        'Verify Inventory and GL modules are in the same period
        If bINSetupInfo.PerNbr <> bGLSetupInfo.PerNbr Then
            msgText = "Inventory and General Ledger modules must be in the same fiscal period before a validation can be performed."
            msgText = msgText + vbNewLine + vbNewLine + "General Ledger: " + vbTab + vbTab + CurrPeriodGL
            msgText = msgText + vbNewLine + "Inventory: " + vbTab + CurrPeriodIN
            Call MessageBox.Show(msgText, "Fiscal Period Error", MessageBoxButtons.OK)

            Return
        End If

        'Display message if Validation has already been completed successfully
        If bSLMPTStatus.Inv_ValCmpltd = 1 Then
            If POExists = True And bSLMPTStatus.PO_ValCmpltd = 1 Then
                msgText = "The Item validation process has already been completed. Are you sure you want to rerun this process?"

                reValidatePO = True
            Else
                msgText = "The Item validation process has already been completed. Are you sure you want to rerun this process?"
                reValidatePO = False
            End If

            dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

            If (dlgResult = DialogResult.No) Then
                Exit Sub
            End If
        End If

        'Display process message
        msgText = "This process will validate Inventory information to ensure the data is ready for migration. Issues found during the validation process will be written to an event log. "
        msgText = msgText + "Please make sure a current and valid backup of the application database is available prior to starting the validation process."
        msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"

        dlgResult = MessageBox.Show(msgText, "Validate Items", MessageBoxButtons.YesNo)

        If (dlgResult = DialogResult.Yes) Then
            'Check for unposted IN batches (CpnyID, LedgerID) and warn user
            Call UnpostedBatchCheck("IN")
            If UBatchesExistIN = True Then
                msgText = "WARNING: Unposted batches exist in Inventory."
                msgText = msgText + " Transaction amounts on unposted batches are not included in GL Account history amounts but are included in Inventory Item on-hand cost amounts."
                msgText = msgText + " This could cause an out of balance situation between exported Item on-hand costs and exported balances of the GL Accounts associated with Items"
                msgText = msgText + " on the GL Accts tab of Inventory Items (10.250.00)."
                msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"

                dlgResult = MessageBox.Show(msgText, "Unposted IN Batches Exist", MessageBoxButtons.YesNo)

                If (dlgResult = DialogResult.No) Then
                    Exit Sub
                End If

            End If

            'Validate Inventory data
            NbrOfErrors_Inv = 0
            NbrOfWarnings_Inv = 0
            Call InventoryCode.Validate()


            'End process
            If OkToContinue = True Then

                Dim eventLog As clsEventLog = New clsEventLog
                eventLog.FileName = System.IO.Path.GetFileName(bSLMPTStatus.InvEventLogName)

                'Set as Completed if no errors were found
                If NbrOfErrors_Inv = 0 Then
                    bSLMPTStatus.Inv_ValCmpltd = 1
                    cValCompletedItem.Checked = bSLMPTStatus.Inv_ValCmpltd

                    cCompletedINChk.Checked = bSLMPTStatus.Inv_ValCmpltd


                    If NbrOfWarnings_Inv > 0 Then
                        Call eventLog.LogMessage(StartProcess, "")

                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Item warnings found: " + NbrOfWarnings_Inv.ToString, eventLog)
                        Call eventLog.LogMessage(EndProcess, "")

                    End If

                Else  'Errors found
                    Call eventLog.LogMessage(StartProcess, "")
                    Call LogMessage("", eventLog)
                    Call LogMessage("Number of Item errors found: " + NbrOfErrors_Inv.ToString, eventLog)

                    bSLMPTStatus.Inv_ValCmpltd = 0

                    cCompletedINChk.Checked = bSLMPTStatus.Inv_ValCmpltd

                    If NbrOfWarnings_Inv > 0 Then
                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Vendor warnings found:  " + NbrOfWarnings_Vend.ToString, eventLog)
                    End If
                    Call eventLog.LogMessage(EndProcess, "")

                End If
                eventLog = Nothing

            Else
                'Set Completed field to 0 (unchecked) since an error occurred at some point in the process
                bSLMPTStatus.Inv_ValCmpltd = 0

                cCompletedINChk.Checked = bSLMPTStatus.Inv_ValCmpltd
            End If

            'Update Last Validated Date
            bSLMPTStatus.Inv_LValidated = CurrDate


            'Update Number of Errors/Warnings
            bSLMPTStatus.Inv_Errors = NbrOfErrors_Inv

            bSLMPTStatus.Inv_Warnings = NbrOfWarnings_Inv


            'Clear Purchasing data
            If reValidatePO = True Then
                bSLMPTStatus.PO_Warnings = 0
                bSLMPTStatus.PO_Errors = 0
                bSLMPTStatus.PO_ValCmpltd = 0

                Call UpdatePOControls()

            End If

            Call UpdateStatusRecord("Inventory")

            ' Update the control values
            Call UpdateINControls()

            StatusLbl.Text = "Inventory Validation Complete"


        Else
            Exit Sub
        End If

    End Sub

    Private Sub cmdValidateProj_Click(sender As Object, e As EventArgs) Handles cmdValidateProj.Click
        Dim msgText As String = String.Empty
        Dim dlgResult As DialogResult

        StatusLbl.Text = "Validating Projects"

        'Verify Beginning Fiscal Year is populated
        If bSLMPTStatus.FiscYr_Beg.Trim = "" Then
            bSLMPTStatus.FiscYr_Beg = bSLMPTStatus.FiscYr_End
        End If

        'Verify Current Period Number is populated in PCSetup
        If bPCSetupInfo.PerNbr.Trim = "" Then
            msgText = "Current Period Number is missing in Project Controller Setup (PA.SET.00). The Current Period Number must be populated before a validation can be performed."
            Call MessageBox.Show(msgText, "Project Controller Current Period Number Error", MessageBoxButtons.OK)
            Exit Sub
        End If

        'Verify Project Controller and GL modules are in the same period
        If bPCSetupInfo.PerNbr <> bGLSetupInfo.PerNbr Then
            msgText = "Project Controller and General Ledger modules must be in the same fiscal period before a validation can be performed."
            msgText = msgText + vbNewLine + vbNewLine + "General Ledger: " + vbTab + CurrPeriodGL
            msgText = msgText + vbNewLine + "Project Controller: " + vbTab + CurrPeriodPA
            Call MessageBox.Show(msgText, "Fiscal Period Error", MessageBoxButtons.OK)

            Exit Sub
        End If


        'Display message if Validation has already been completed successfully
        If bSLMPTStatus.Proj_ValCmpltd = 1 Then
            msgText = "The Project validation process has already been completed. Are you sure you want to rerun this process?"

            dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

            If (dlgResult = DialogResult.No) Then
                Exit Sub
            End If

        End If

        'Display process message
        msgText = "This process will validate Project information to ensure the data is ready for migration. Issues found during the validation process will be written to an event log. "
        msgText = msgText + "Please make sure a current and valid backup of the application database is available prior to starting the validation process."
        msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"
        dlgResult = MessageBox.Show(msgText, "Validate Projects", MessageBoxButtons.YesNo)

        If (dlgResult = DialogResult.Yes) Then

            'Validate Project data
            NbrOfErrors_Proj = 0
            NbrOfWarnings_Proj = 0
            Call ProjectCode.Validate()


            'End process
            If OkToContinue = True Then

                Dim eventLog As clsEventLog = New clsEventLog
                eventLog.FileName = System.IO.Path.GetFileName(bSLMPTStatus.ProjEventLogName)

                'Set as completed if no errors are found
                If NbrOfErrors_Proj = 0 Then
                    bSLMPTStatus.Proj_ValCmpltd = 1

                    cCompletedPAChk.Text = bSLMPTStatus.Proj_ValCmpltd

                    If NbrOfWarnings_Proj > 0 Then

                        Call eventLog.LogMessage(StartProcess, "")

                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Project warnings found: " + NbrOfWarnings_Proj.ToString, eventLog)
                        Call eventLog.LogMessage(EndProcess, "")

                    End If

                Else  'Errors found
                    Call eventLog.LogMessage(StartProcess, "")
                    Call LogMessage("", eventLog)
                    Call LogMessage("Number of Project errors found: " + NbrOfErrors_Proj.ToString, eventLog)

                    bSLMPTStatus.Proj_ValCmpltd = 0

                    cCompletedPAChk.Text = bSLMPTStatus.Proj_ValCmpltd


                    If NbrOfWarnings_Proj > 0 Then
                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Project warnings found: " + NbrOfWarnings_Proj.ToString, eventLog)
                    End If
                    Call eventLog.LogMessage(EndProcess, "")
                End If

                eventLog = Nothing

            Else
                'Set Completed field to 0 (unchecked) since an error occurred at some point in the process
                bSLMPTStatus.Proj_ValCmpltd = 0

                cCompletedPAChk.Text = bSLMPTStatus.Proj_ValCmpltd

            End If

            'Update Last Validated Date
            bSLMPTStatus.Proj_LValidated = CurrDate

            'Update Number of Errors/Warnings
            bSLMPTStatus.Proj_Errors = NbrOfErrors_Proj

            bSLMPTStatus.Proj_Warnings = NbrOfWarnings_Proj

            Call UpdateStatusRecord("Project")

            ' Update the control values
            Call UpdatePAControls()

            StatusLbl.Text = "Project Validation Complete"


        Else
            Exit Sub
        End If

    End Sub


    Private Sub cmdValidatePurch_Click(sender As Object, e As EventArgs) Handles cmdValidatePurch.Click
        Dim msgText As String = String.Empty
        Dim fetchRetVal As Short
        Dim retValInt As Integer
        Dim sqlStmt As String = ""

        Dim dlgResult As DialogResult

        StatusLbl.Text = "Validating Purchasing"

        'Verify Beginning Fiscal Year is populated
        If bSLMPTStatus.FiscYr_Beg.Trim = "" Then
            bSLMPTStatus.FiscYr_Beg = bSLMPTStatus.FiscYr_End
        End If

        'Verify Current Period Number is populated in APSetup
        If bAPSetupInfo.PerNbr.Trim = "" Then
            msgText = "Current Period Number is missing in AP Setup (03.950.00). The Current Period Number must be populated before a validation can be performed."
            Call MessageBox.Show(msgText, "Accounts Payable Current Period Number Error", MessageBoxButtons.OK)

            Exit Sub
        End If

        'Verify AP and GL modules are in the same period
        If bAPSetupInfo.PerNbr <> bGLSetupInfo.PerNbr Then
            msgText = "Accounts Payable and General Ledger modules must be in the same fiscal period before a validation can be performed."
            msgText = msgText + vbNewLine + vbNewLine + "General Ledger: " + vbTab + vbTab + CurrPeriodGL
            msgText = msgText + vbNewLine + "Accounts Payable: " + vbTab + CurrPeriodAP
            Call MessageBox.Show(msgText, "Fiscal Period Error", MessageBoxButtons.OK)

            Exit Sub
        End If

        'Display message if Validation has already been completed successfully
        If bSLMPTStatus.PO_ValCmpltd = 1 Then
            msgText = "The Puchase Order validation process has already been completed. Are you sure you want to rerun this process?"

            dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

            If (dlgResult = DialogResult.No) Then
                Exit Sub
            End If

        End If

        'Display process message
        msgText = "This process will validate Purchasing information to ensure the data is ready for migration. Issues found during the validation process will be written to an event log. "
        msgText = msgText + "Please make sure a current and valid backup of the application database is available prior to starting the validation process."
        msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"
        dlgResult = MessageBox.Show(msgText, "Validate Purchase Orders", MessageBoxButtons.YesNo)

        If (dlgResult = DialogResult.Yes) Then

            'Check for unposted PO batches (CpnyID, LedgerID) and warn user
            Call UnpostedBatchCheck("PO")
            If UBatchesExistPO = True Then
                msgText = "WARNING: Unposted batches exist in Purchasing."
                msgText = msgText + " Transaction amounts on unposted batches are not included in GL Account history amounts but are included in Inventory item cost amounts."
                msgText = msgText + " This could cause an out of balance situation between exported Item on-hand costs and exported balances of the GL Accounts associated with Items"
                msgText = msgText + " on the GL Accts tab of Inventory Items (10.250.00)."
                msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"
                dlgResult = MessageBox.Show(msgText, "Unposted PO Batches Exist", MessageBoxButtons.YesNo)

                If (dlgResult = DialogResult.No) Then
                    Exit Sub
                End If

            End If


            'Validate Purchasing data
            NbrOfErrors_PO = 0
            NbrOfWarnings_PO = 0
            Call PurchasingCode.Validate()

            'End process
            If OkToContinue = True Then

                Dim eventLog As clsEventLog = New clsEventLog
                eventLog.FileName = System.IO.Path.GetFileName(bSLMPTStatus.VendEventLogName)

                'Set as Completed if no error were found
                If NbrOfErrors_PO = 0 Then


                    bSLMPTStatus.PO_ValCmpltd = 1
                    cValCompletedPurch.Checked = bSLMPTStatus.PO_ValCmpltd
                    cCompletedPOChk.Checked = bSLMPTStatus.PO_ValCmpltd


                    If NbrOfWarnings_PO > 0 Then
                        Call eventLog.LogMessage(StartProcess, "")

                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Vendor warnings found:  " + NbrOfWarnings_Vend.ToString, eventLog)
                        Call eventLog.LogMessage(EndProcess, "")

                    End If

                Else  'Errors found
                    Call eventLog.LogMessage(StartProcess, "")

                    Call LogMessage("", eventLog)
                    Call LogMessage("Number of Purchasing errors found: " + NbrOfErrors_PO.ToString, eventLog)


                    bSLMPTStatus.PO_ValCmpltd = 0
                    cValCompletedPurch.Checked = bSLMPTStatus.PO_ValCmpltd

                    cCompletedPOChk.Checked = bSLMPTStatus.PO_ValCmpltd


                    If NbrOfWarnings_PO > 0 Then
                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Purchasing warnings found: " + NbrOfWarnings_PO.ToString, eventLog)

                    End If
                    Call eventLog.LogMessage(EndProcess, "")

                End If

                ' Update the label with the total purchase order count.
                sqlStmt = "SELECT COUNT(*) FROM PurchOrd where CpnyId = " + SParm(CpnyId)
                Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

                If (fetchRetVal = 0) Then
                    lOrderTotal.Text = String.Concat("Total Purchase Orders: " + CStr(retValInt))
                End If

                sqlStmt = "SELECT COUNT(*) FROM PurchOrd where CpnyId = " + SParm(CpnyId) + " And Status in (" + SParm("O") + "," + SParm("P") + ")"
                Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)



                If (fetchRetVal = 0) Then
                    lOrderActive.Text = String.Concat("Active: " + CStr(retValInt))
                End If


                eventLog = Nothing

            Else
                'Set Completed field to 0 (unchecked) since an error occurred at some point in the process
                bSLMPTStatus.PO_ValCmpltd = 0
                cValCompletedPurch.Checked = bSLMPTStatus.PO_ValCmpltd
                cCompletedPOChk.Checked = bSLMPTStatus.PO_ValCmpltd

            End If

            'Update Last Validated Date
            bSLMPTStatus.PO_LValidated = CurrDate
            cDateValidatedPurch.Value = bSLMPTStatus.PO_LValidated
            'Update Number of Errors/Warnings
            bSLMPTStatus.PO_Errors = NbrOfErrors_PO
            bSLMPTStatus.PO_Warnings = NbrOfWarnings_PO



            Call UpdateStatusRecord("Purchasing")

            ' Update the control values
            Call UpdatePOControls()

            StatusLbl.Text = "Purchasing Validation Complete"

        Else
            Exit Sub
        End If

    End Sub





    Private Sub cmdProjEventLog_Click(sender As Object, e As EventArgs) Handles cmdProjEventLog.Click
        ' Display the last event log created
        If (My.Computer.FileSystem.FileExists(bSLMPTStatus.ProjEventLogName)) Then

            Call DisplayLog(bSLMPTStatus.ProjEventLogName)
        Else
            Call MessageBox.Show("Event Log Not Found", "Event Log", MessageBoxButtons.OK)
        End If

    End Sub

    Private Sub cmdIntEventLog_Click(sender As Object, e As EventArgs) Handles cmdIntEventLog.Click
        ' Display the last event log created
        If (My.Computer.FileSystem.FileExists(bSLMPTStatus.InvEventLogName)) Then

            Call DisplayLog(bSLMPTStatus.InvEventLogName)
        Else
            Call MessageBox.Show("Event Log Not Found", "Event Log", MessageBoxButtons.OK)

        End If

    End Sub

    Private Sub cmdPOEventLog_Click(sender As Object, e As EventArgs) Handles cmdPOEventLog.Click
        ' Display the last event log created
        If (My.Computer.FileSystem.FileExists(bSLMPTStatus.POEventLogName)) Then

            Call DisplayLog(bSLMPTStatus.POEventLogName)
        Else
            Call MessageBox.Show("Event Log Not Found", "Event Log", MessageBoxButtons.OK)


        End If
    End Sub

    Private Sub cmdAPEventLog_Click(sender As Object, e As EventArgs) Handles cmdAPEventLog.Click
        ' Display the last event log created
        If (My.Computer.FileSystem.FileExists(bSLMPTStatus.VendEventLogName)) Then

            Call DisplayLog(bSLMPTStatus.VendEventLogName)
        Else
            Call MessageBox.Show("Event Log Not Found", "Event Log", MessageBoxButtons.OK)

        End If
    End Sub

    Private Sub cmdAREventLog_Click(sender As Object, e As EventArgs) Handles cmdAREventLog.Click
        ' Display the last event log created
        If (My.Computer.FileSystem.FileExists(bSLMPTStatus.CustEventLogName)) Then

            Call DisplayLog(bSLMPTStatus.CustEventLogName)
        Else
            Call MessageBox.Show("Event Log Not Found", "Event Log", MessageBoxButtons.OK)


        End If
    End Sub

    Private Sub cmdGLEventLog_Click(sender As Object, e As EventArgs) Handles cmdGLEventLog.Click
        ' Display the last event log created
        If (My.Computer.FileSystem.FileExists(bSLMPTStatus.COAEventLogName)) Then

            Call DisplayLog(bSLMPTStatus.COAEventLogName)
        Else
            Call MessageBox.Show("Event Log Not Found", "Event Log", MessageBoxButtons.OK)

        End If
    End Sub

    Private Sub cmdValidateSalesOrder_Click(sender As Object, e As EventArgs) Handles cmdValidateSalesOrder.Click
        Dim msgText As String = String.Empty
        Dim dlgResult As DialogResult

        'Verify Beginning Fiscal Year is populated
        If bSLMPTStatus.FiscYr_Beg.Trim = "" Then
            bSLMPTStatus.FiscYr_Beg = bSLMPTStatus.FiscYr_End
        End If


        'Display message if Validation has already been completed successfully
        If bSLMPTStatus.SO_ValCmpltd = 1 Then
            msgText = "The Sales Order validation process has already been completed. Are you sure you want to rerun this process?"

            dlgResult = MessageBox.Show(msgText, "Validation Warning", MessageBoxButtons.YesNo)

            If (dlgResult = DialogResult.No) Then
                Exit Sub
            End If
        End If

        'Display process message
        msgText = "This process will validate Sales Order information to ensure the data is ready for migration. Issues found during the validation process will be written to an event log. "
        msgText = msgText + "Please make sure a current and valid backup of the application database is available prior to starting the validation process."
        msgText = msgText + vbNewLine + vbNewLine + "Do you want to continue?"
        dlgResult = MessageBox.Show(msgText, "Validate Sales Orders", MessageBoxButtons.YesNo)

        If (dlgResult = DialogResult.Yes) Then

            'Validate Sales Order data
            NbrOfErrors_SO = 0
            NbrOfWarnings_SO = 0
            Call SalesOrder.Validate()

            'End Process
            If OkToContinue = True Then
                Dim eventLog As clsEventLog = New clsEventLog
                eventLog.FileName = System.IO.Path.GetFileName(bSLMPTStatus.SOEventLogName)

                'Set as Completed if no errors were found
                If NbrOfErrors_SO = 0 Then
                    bSLMPTStatus.SO_ValCmpltd = 1

                    cCompletedSOChk.Text = bSLMPTStatus.SO_ValCmpltd


                    If NbrOfWarnings_Vend > 0 Then
                        Call eventLog.LogMessage(StartProcess, "")

                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Sales Order warnings found:  " + NbrOfWarnings_SO.ToString, eventLog)
                        Call eventLog.LogMessage(EndProcess, "")
                    End If

                Else  'Errors found
                    Call eventLog.LogMessage(StartProcess, "")
                    Call LogMessage("", eventLog)
                    Call LogMessage("Number of Sales Order errors found: " + CStr(NbrOfErrors_SO), eventLog)
                    bSLMPTStatus.SO_ValCmpltd = 0

                    cCompletedSOChk.Text = bSLMPTStatus.SO_ValCmpltd


                    If NbrOfWarnings_SO > 0 Then
                        Call LogMessage("", eventLog)
                        Call LogMessage("Number of Sales Order warnings found:  " + NbrOfWarnings_SO.ToString, eventLog)
                    End If
                    Call eventLog.LogMessage(EndProcess, "")


                End If
                eventLog = Nothing

            Else
                'Set Completed field to 0 (unchecked) since an error occurred at some point in the process
                bSLMPTStatus.SO_ValCmpltd = 0

                cCompletedSOChk.Text = bSLMPTStatus.SO_ValCmpltd


            End If

            'Update Last Validated Date
            bSLMPTStatus.SO_LValidated = CurrDate


            'Update Number of Errors/Warning
            bSLMPTStatus.SO_Errors = NbrOfErrors_SO

            bSLMPTStatus.SO_Warnings = NbrOfWarnings_SO



            Call UpdateStatusRecord("SalesOrder")

            ' Update the control values
            Call UpdateSOControls()

        Else
            Exit Sub
        End If
    End Sub

    Private Sub cmdSOEventLog_Click(sender As Object, e As EventArgs) Handles cmdSOEventLog.Click
        ' Display the last event log created
        If (My.Computer.FileSystem.FileExists(bSLMPTStatus.SOEventLogName)) Then

            Call DisplayLog(bSLMPTStatus.SOEventLogName)
        Else
            Call MessageBox.Show("Event Log Not Found", "Event Log", MessageBoxButtons.OK)


        End If

    End Sub

    Private Sub ConnectServer_Click(sender As Object, e As EventArgs) Handles ConnectServer.Click
        Dim connStr As String
        Dim sqlString As String
        Dim sqlCpnySet As SqlDataReader
        Dim CpnyDBEntry As CpnyDatabase



        'Set the global variables that hold the userid and password
        'that the user entered.
        SQLAuthUser = Trim(txtUserId.Text)
        SQLAuthPwd = Trim(txtPassword.Text)

        ' Add the specified database.
        If (String.IsNullOrEmpty(SysDb.Text.Trim())) Then
            MsgBox("Database name is required")
        Else


            connStr = GetConnectionString(True)

            SqlSysDbConn = New SqlClient.SqlConnection(connStr)

            Using SQLCommand = New SqlClient.SqlCommand()
                SQLCommand.Connection = SqlSysDbConn

                Try
                    SQLCommand.Connection.Open()
                Catch ex As Exception
                    MsgBox("Database Connection failed: Error = " + ex.Message)
                    Exit Sub
                End Try


                ' If the connection opened successfully, then get the list of companies.
                If (SqlSysDbConn.State = ConnectionState.Open) Then
                    CpnyIDList.Items.Clear()
                    sqlString = "Select CpnyId, DatabaseName From Company"
                    SQLCommand.CommandText = sqlString
                    sqlCpnySet = SQLCommand.ExecuteReader()



                    If (sqlCpnySet.HasRows = True) Then
                        While (sqlCpnySet.Read())
                            CpnyDBEntry = New CpnyDatabase
                            CpnyDBEntry.CompanyId = sqlCpnySet("CpnyId")
                            CpnyDBEntry.DatabaseName = sqlCpnySet("DatabaseName")
                            CpnyIDList.Items.Add(sqlCpnySet(0).ToString())
                            CpnyDBList.Add(CpnyDBEntry)
                        End While
                    End If
                End If

                SQLCommand.Connection.Close()
            End Using


            UserId = System.Environment.UserName()

            ' Indicate that the connction completed successfully.
            lblDbStatus.Text = "Connected"

        End If
    End Sub

    Private Sub rbAuthenticationTypeWindows_CheckedChanged(sender As Object, e As EventArgs) Handles rbAuthenticationTypeWindows.CheckedChanged
        ' If this is selected, then indicate that the Windows Authentication is enabled.
        IsWindowsAuth = True
        txtUserId.Enabled = False
        txtPassword.Enabled = False

    End Sub

    Private Sub rbAuthenticationTypeSQL_CheckedChanged(sender As Object, e As EventArgs) Handles rbAuthenticationTypeSQL.CheckedChanged
        ' If this is selected, then indicate that the authentication type is SQL Authentication.
        IsWindowsAuth = False

        txtUserId.Enabled = True
        txtPassword.Enabled = True



    End Sub

    Private Sub SysDb_TextChanged(sender As Object, e As EventArgs) Handles SysDb.TextChanged
        SysDBName = SysDb.Text
    End Sub

    Private Sub UserId_TextChanged(sender As Object, e As EventArgs) Handles txtUserId.TextChanged
        SQLAuthUser = txtUserId.Text.Trim()
    End Sub

    Private Sub Password_TextChanged(sender As Object, e As EventArgs) Handles txtPassword.TextChanged
        SQLAuthPwd = txtPassword.Text
    End Sub

    Private Sub CpnyIDList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CpnyIDList.SelectedIndexChanged
        Dim retValInt As Integer
        Dim sqlStmt As String = ""


        CpnyId = CpnyIDList.Items(CpnyIDList.SelectedIndex)

        ' Get the name of the application database associated with the selected company.
        For Each CpnyDBEntry As CpnyDatabase In CpnyDBList
            If (String.Compare(CpnyId.Trim, CpnyDBEntry.CompanyId.Trim(), True) = 0) Then
                AppDBName = CpnyDBEntry.DatabaseName
                Exit For
            End If
        Next

        If (String.IsNullOrEmpty(AppDBName) = True) Then
            MsgBox("Error:  Unable to open application database associated with company " & CpnyId.Trim())
        Else
            ' Open the connection to the app database.
            AppDbConnStr = GetConnectionString(False)

            SqlAppDbConn = New SqlClient.SqlConnection(AppDbConnStr)

            Call Initialize_Controls()

            ' Set the value of the company control on the screen.
            cCpnyIDTxt.Text = CpnyId

            'Get module information.
            Call GetSLModuleInfo()

            ' If there are records in the master account work tabke, then update the label with the total account count.
            sqlStmt = "SELECT COUNT(*) FROM Account"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lAcctTotal.Text = "Total Accounts: " + CStr(retValInt)

            sqlStmt = "SELECT COUNT(*) FROM Account Where Active = 1"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lAcctActive.Text = String.Concat("Active: " + CStr(retValInt))

            ' Update the totals on the summary tab for the company-specific items.
            UpdateTotalLabels()

            ' If the event log output has been defined, then enable the appropriate tabs.
            If (String.IsNullOrEmpty(txtEventLogPath.Text.Trim) = False) Then
                Call EnableTabPages()
            End If

            '  Set the fiscyr to the default value.
            bSLMPTStatus.FiscYr_Beg = bGLSetupInfo.PerNbr.Substring(0, 4)
            bSLMPTStatus.FiscYr_End = bGLSetupInfo.PerNbr.Substring(0, 4)

            ' Set the value of the fiscal year control.
            cFiscYr_End.Text = bSLMPTStatus.FiscYr_Beg

            'Populate Current Period to Post and Ledger display labels in Chart of Accounts section
            lCurrPeriodDisp.Text = CurrPeriodGL
            lGLPeriod.Text = CurrPeriodGL
            lLedgerDisp.Text = bGLSetupInfo.LedgerID


        End If


    End Sub
    Public Sub DisbleTabPages()
        ' Disable all tab pages to prevent processing if the correct information has not been entered.
        TabPageAP.Enabled = False
        TabPageAR.Enabled = False
        TabPageGL.Enabled = False
        TabPageIN.Enabled = False
        TabPagePA.Enabled = False
        TabPagePO.Enabled = False
        TabPageSO.Enabled = False



    End Sub
    Private Sub EnableTabPages()

        Dim retValInt As Integer
        Dim sqlStmt As String = ""

        ' Enable the GL tab page.
        TabPageGL.Enabled = True

        'Disable Customers tab page if AR module is not set up
        If ARExists = False Then
            TabPageAR.Enabled = False
        Else
            TabPageAR.Enabled = True

            ' If there are records in the work table, update the label with the total customer count.
            sqlStmt = "SELECT COUNT(*) FROM Customer"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lCustTotal.Text = "Total Customers: " + CStr(retValInt)

            sqlStmt = "SELECT COUNT(*) FROM Customer where Status <> " + SParm("I")
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lCustActive.Text = "Active: " + CStr(retValInt)

        End If

        'Disable Vendors tab page if AP module is not set up
        If APExists = False Then
            TabPageAP.Enabled = False
        Else
            TabPageAP.Enabled = True

            '' If there are records in the work table, update the label with the total vendor count.
            sqlStmt = "SELECT COUNT(*) FROM Vendor"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lVendTotal.Text = "Total Vendors: " + CStr(retValInt)

            sqlStmt = "SELECT COUNT(*) FROM Vendor where Status <> 'I'"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lVendActive.Text = "Active: " + CStr(retValInt)

        End If

        'Disable Projects tab page is PA module is not set up
        If PAExists = False Then
            TabPagePA.Enabled = False

        Else

            TabPagePA.Enabled = True

            ' Update the label with the total item count.
            sqlStmt = "Select COUNT(*) FROM  PJProj Where CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)
            lProjTotal.Text = "Total Projects: " + CStr(retValInt)

            sqlStmt = "SELECT COUNT(*) FROM PJProj where status_pa = 'A' And CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lProjActive.Text = "Active: " + CStr(retValInt)

        End If

        'Disable Items tab page if IN module is not set up
        If INExists = False Then
            TabPageIN.Enabled = False
        Else
            TabPageIN.Enabled = True

            ' Update the label with the total item count.
            sqlStmt = "SELECT COUNT(*) FROM  Inventory"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lItemTotal.Text = String.Concat("Total Items: " + CStr(retValInt))

            sqlStmt = "SELECT COUNT(*) FROM Inventory where TranStatusCode NOT IN ('IN', 'DE')"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lInvtActive.Text = "Active: " + CStr(retValInt)

        End If

        'Disable Purchase Orders tab page if PO Module is not set up
        If POExists = False Then
            TabPagePO.Enabled = False
            lPurchasing.Enabled = False
        Else

            TabPagePO.Enabled = True

            ' Update the label with the total PO count.
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd where CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lOrderTotal.Text = "Total Orders: " + CStr(retValInt)

            sqlStmt = "SELECT COUNT(*) FROM PurchOrd where CpnyId = " + SParm(CpnyId) + " And Status in ('O', 'P')"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lOrderActive.Text = "Active: " + CStr(retValInt)


        End If

        'Disable Sales Order tab page if Sales Order Module is not set up
        If SOExists = False Then
            GroupBox4.Enabled = False
            lSalesOrder.Enabled = False
            TabPageSO.Enabled = False
        Else
            TabPageSO.Enabled = True
            '' Update the label with the total SO header count.
            sqlStmt = "SELECT COUNT(*) FROM SOHeader where CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)


            lSOTotal.Text = "Total Sales Order: " + CStr(retValInt)
            sqlStmt = "SELECT COUNT(*) FROM SOHeader where CpnyId = " + SParm(CpnyId) + " And Status in ('O','P')"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)


            lSOActive.Text = "Active: " + CStr(retValInt)


        End If

    End Sub

    Public Sub UpdateTotalLabels()

        Dim sqlStmt As String = String.Empty
        Dim retValInt As Integer = 0

        ' Update the labels for the items that are company-specific.
        If SOExists = True Then
            '' Update the label with the total SO header count.
            sqlStmt = "SELECT COUNT(*) FROM SOHeader where CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)


            lSOTotal.Text = "Total Sales Order: " + CStr(retValInt)
            sqlStmt = "SELECT COUNT(*) FROM SOHeader where CpnyId = " + SParm(CpnyId) + " And Status in ('O','P')"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)


            lSOActive.Text = "Active: " + CStr(retValInt)

        End If

        If POExists = True Then
            ' Update the label with the total PO count.
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd where CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lOrderTotal.Text = "Total Orders: " + CStr(retValInt)

            sqlStmt = "SELECT COUNT(*) FROM PurchOrd where CpnyId = " + SParm(CpnyId) + " And Status in ('O', 'P')"
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lOrderActive.Text = "Active: " + CStr(retValInt)

        End If

        If PAExists = True Then

            ' Update the label with the total item count.
            sqlStmt = "Select COUNT(*) FROM  PJProj Where CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)
            lProjTotal.Text = "Total Projects: " + CStr(retValInt)

            sqlStmt = "SELECT COUNT(*) FROM PJProj where status_pa = 'A' And CpnyId = " + SParm(CpnyId)
            Call sqlFetch_Num(retValInt, sqlStmt, SqlAppDbConn)

            lProjActive.Text = "Active: " + CStr(retValInt)

        End If

    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        ' Launch the folder dialog to browse for the event log destination.
        If (FolderBrowserDialog1.ShowDialog = DialogResult.OK) Then
            txtEventLogPath.Text = FolderBrowserDialog1.SelectedPath
            EventLogDir = txtEventLogPath.Text
            Call EnableTabPages()
        End If
    End Sub


    Private Sub NameOfServer_TextChanged(sender As Object, e As EventArgs) Handles NameOfServer.TextChanged
        SQLServerName = NameOfServer.Text
    End Sub

    Private Sub txtEventLogPath_Leave(sender As Object, e As EventArgs) Handles txtEventLogPath.Leave
        ' Set the event log output path.
        EventLogDir = txtEventLogPath.Text

        ' Only enable the tab pages if a valid value has been entered
        If (Directory.Exists(EventLogDir.Trim())) Then
            Call EnableTabPages()
        Else
            Call MessageBox.Show("Directory path is invalid.  Please enter a valid directory.", "Invalid Directory", MessageBoxButtons.OK)
        End If
    End Sub
End Class
