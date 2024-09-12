Imports System.Data.SqlClient
Imports System.Transactions
Imports System.IO

Module PlumblineCode

    'Public variables
    Public OkToContinue As Boolean = True
    Public CurrDate As Date
    Public NbrOfErrors_COA As Integer
    Public NbrOfErrors_Cust As Integer
    Public NbrOfErrors_Vend As Integer
    Public NbrOfErrors_Inv As Integer
	Public NbrOfErrors_Proj As Integer
    Public NbrOfErrors_PO As Integer
    Public NbrOfErrors_SO As Integer
    Public NbrOfWarnings_COA As Integer
    Public NbrOfWarnings_Cust As Integer
    Public NbrOfWarnings_Vend As Integer
    Public NbrOfWarnings_Inv As Integer
	Public NbrOfWarnings_Proj As Integer
    Public NbrOfWarnings_PO As Integer
    Public NbrOfWarnings_SO As Integer
    Public DfltLedgerID As String = String.Empty
    Public CurrPeriodGL As String = String.Empty
    Public CurrPeriodAP As String = String.Empty
    Public CurrPeriodAR As String = String.Empty
    Public CurrPeriodIN As String = String.Empty
    Public CurrPeriodPA As String = String.Empty
    Public APExists As Boolean = False
    Public ARExists As Boolean = False
    Public INExists As Boolean = False
	Public PAExists As Boolean = False
    Public POExists As Boolean = False
    Public SOExists As Boolean = False
    Public Mem_AcctList As Integer
    Public Mem_CustList As Integer
    Public CSRAcctList As Integer
    Public CSRCustList As Integer
    Public Fetch_AcctList As Integer
    Public Fetch_CustList As Integer
    Public RecMaintFlg As Integer
    Public UBatchesExistGL As Boolean = False
    Public UBatchesExistAP As Boolean = False
    Public UBatchesExistAR As Boolean = False
	Public UBatchesExistIN As Boolean = False
	Public UBatchesExistPO As Boolean = False
	Public mcProductCode As String = String.Empty
    Public MultiCuryEnabled As Boolean = False
    Public BaseCuryPrec As Short = 0

    ' Used as 1st parm to Status() call
    Public Const EndProcess As Short = -2
    Public Const StartProcess As Short = -3
    Public Const StopProcess As Short = -4




    Public Const FOUND As Short = 0




    '************************
    '***** Public Subs ******
    '************************
    Public Sub GetSLModuleInfo()
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Query module setup records
        '   Determine if a module setup record exists
        '   Determine the current fiscal period for each module
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Dim countINRecCheck As Integer
        Dim countPARecCheck As Integer
        Dim countPORecCheck As Integer
        Dim countPOSetup As Integer
        Dim countSORecCheck As Integer
        Dim countSOSetup As Integer
        Dim countRegEntries As Integer
        Dim countCuryEntries As Integer

        Dim sqlStmt As String = ""
        Dim sqlReader As SqlDataReader = Nothing


        ''General Ledger
        sqlStmt = "Select BaseCuryID, LedgerId, NbrPer, PerNbr, RetEarnAcct, YtdNetIncAcct  from GLSetup"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If (sqlReader.HasRows()) Then
            Call sqlReader.Read()
            Call SetGLSetupValues(sqlReader, bGLSetupInfo)
        End If

        Call sqlReader.Close()

        CurrPeriodGL = bGLSetupInfo.PerNbr.Substring(4, 2) + "-" + bGLSetupInfo.PerNbr.Substring(0, 4)

        Form1.lCurrPeriodDisp.Text = CurrPeriodGL
        Form1.lGLPeriod.Text = CurrPeriodGL

        ' Get the currency precision for the base currency identified in GLSetup.  If no base currency is identified, then default to 2
        '       decimal places.  Otherwise, find the base currency in the currncy table.
        If (String.IsNullOrEmpty(bGLSetupInfo.BaseCuryID.Trim())) Then
            BaseCuryPrec = 2
        Else

            ' Ensure that the currncy record exists.
            sqlStmt = "Select Count(*) from Currncy where CuryId = " + SParm(bGLSetupInfo.BaseCuryID)
            Call sqlFetch_Num(countCuryEntries, sqlStmt, SqlAppDbConn)
            If (countCuryEntries = 1) Then
                sqlStmt = "Select DecPl from Currncy where CuryId = " + SParm(bGLSetupInfo.BaseCuryID)
                Call sqlFetch_Num(BaseCuryPrec, sqlStmt, SqlAppDbConn)
            Else
                BaseCuryPrec = 2
            End If
        End If


        'Accounts Payable
        sqlStmt = "Select CurrPerNbr, PerNbr From APSetup"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() = False Then
            APExists = False
        Else

            Call sqlReader.Read()
            Call SetAPSetupValues(sqlReader, bAPSetupInfo)

            If bAPSetupInfo.PerNbr.Trim <> "" Then
                'Check to see if the module is unlocked
                sqlStmt = "SELECT Count(*) FROM RegistDetail WHERE (RegItem = 'A1' AND Unlocked = 1) OR (RegItem = 'AP' AND Unlocked = 1)"
                Call sqlFetch_Num(countRegEntries, sqlStmt, SqlSysDbConn)

                If countRegEntries > 0 Then
                    'Module unlocked
                    APExists = True
                    CurrPeriodAP = bAPSetupInfo.PerNbr.Substring(4, 2) + "-" + bAPSetupInfo.PerNbr.Substring(0, 4)

                    ' Display the current period
                    Form1.lCurrAPPeriodDisp.Text = CurrPeriodAP
                    Form1.lCurrAPPeriod.Text = CurrPeriodAP

                Else
                    'Module not unlocked
                    APExists = False
                End If
            Else
                APExists = False
            End If
        End If
        Call sqlReader.Close()

        'Accounts Receivable
        sqlStmt = "SELECT PerNbr FROM ARSetup"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() = False Then
            ARExists = False
            Call sqlReader.Close()
        Else
            Call sqlReader.Read()
            Call SetARSetupValues(sqlReader, bARSetupInfo)

            If bARSetupInfo.PerNbr.Trim <> "" Then
                'Check to see if the module is unlocked
                sqlStmt = "SELECT Count(*) FROM RegistDetail WHERE (RegItem = 'A1' AND Unlocked = 1) OR (RegItem = 'AP' AND Unlocked = 1)"
                Call sqlFetch_Num(countRegEntries, sqlStmt, SqlSysDbConn)


                If countRegEntries > 0 Then
                    'Module unlocked
                    ARExists = True
                    CurrPeriodAR = bARSetupInfo.PerNbr.Substring(4, 2) + "-" + bARSetupInfo.PerNbr.Substring(0, 4)

                    ' Display the current period
                    Form1.lCurrARPerLabelDisp.Text = CurrPeriodAR
                    Form1.lCurrARPeriod.Text = CurrPeriodAR

                Else
                    ARExists = False
                End If
            Else
                ARExists = False
            End If
        End If
        Call sqlReader.Close()

        'Project
        Call sqlFetch_1(sqlReader, "SELECT PerNbr FROM PCSetup WHERE Init = 1", SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() = False Then
            PAExists = False
        Else
            'Check to see if any Projects exist
            Call sqlReader.Read()
            Call SetPCSetupValues(sqlReader, bPCSetupInfo)
            Call sqlReader.Close()
            sqlStmt = "SELECT COUNT(*) FROM PJProj"
            Call sqlFetch_Num(countPARecCheck, sqlStmt, SqlAppDbConn)
            Call sqlReader.Close()

            If countPARecCheck > 0 Then 'Project exist
                PAExists = True
                CurrPeriodPA = bPCSetupInfo.PerNbr.Substring(4, 2) + "-" + bPCSetupInfo.PerNbr.Substring(0, 4)

                ' The current period may not be populated in the PCSetup prior to setting it in the setup screen or prior to closing.
                '   If the value is empty, then extract it from pjcontrl.
                If (String.IsNullOrEmpty(bPCSetupInfo.PerNbr.Trim)) Then


                    Dim RecordParmList As New List(Of ParmList)
                    Dim ParmValues As ParmList

                    ParmValues = New ParmList
                    ParmValues.ParmName = "parm1"
                    ParmValues.ParmType = SqlDbType.VarChar
                    ParmValues.ParmValue = "PA"
                    RecordParmList.Add(ParmValues)

                    ParmValues = New ParmList
                    ParmValues.ParmName = "parm2"
                    ParmValues.ParmType = SqlDbType.VarChar
                    ParmValues.ParmValue = "CURRENT-PERIOD"
                    RecordParmList.Add(ParmValues)

                    Call sqlFetch_1(sqlReader, "pjcontrl_spk0", SqlAppDbConn, CommandType.StoredProcedure, RecordParmList)
                    If sqlReader.HasRows() Then
                        Call sqlReader.Read()
                        Call SetPJContrlValues(sqlReader, bPJContrlInfo)
                        If bPJContrlInfo.Control_Data.Trim.Length = 6 Then
                            'Set up the screen fields
                            '-----------------------------------------------
                            CurrPeriodPA = bPJContrlInfo.Control_Data.Substring(4, 2) + "-" + bPJContrlInfo.Control_Data.Substring(0, 4)
                        End If

                    End If

                End If

                ' Display the current period
                Form1.lCurrPAPeriodDisp.Text = CurrPeriodPA
                Form1.lCurrPAPeriod.Text = CurrPeriodPA

            Else 'Projects do not exist
                    PAExists = False
            End If

            Call sqlReader.Close()
        End If


        'Inventory
        Call sqlFetch_1(sqlReader, "SELECT * FROM INSetup WHERE Init = 1", SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() = False Then
            INExists = False
        Else
            Call sqlReader.Read()
            Call SetINSetupValues(sqlReader, bINSetupInfo)
            Call sqlReader.Close()
            'Check to see if any Inventory Items exist
            sqlStmt = "SELECT COUNT(*) FROM Inventory"
            Call sqlFetch_Num(countINRecCheck, sqlStmt, SqlAppDbConn)


            If countINRecCheck > 0 Then 'Items exist
                INExists = True
                CurrPeriodIN = bINSetupInfo.PerNbr.Substring(4, 2) + "-" + bINSetupInfo.PerNbr.Substring(0, 4)
                Form1.lCurrINPeriodDisp.Text = CurrPeriodIN
                Form1.lCurrINPeriod.Text = CurrPeriodIN

            Else 'Items do not exist
                INExists = False
            End If
        End If
        Call sqlReader.Close()

        'Purchasing

        sqlStmt = "SELECT COUNT(*) FROM POSetup WHERE Init = 1"
        Call sqlFetch_Num(countPOSetup, sqlStmt, SqlAppDbConn)


        If countPOSetup = 0 Then
            POExists = False
        Else
            'Check to see if any Purchase Orders exist
            sqlStmt = "SELECT COUNT(*) FROM PurchOrd WHERE POType = 'OR' AND Status IN ('O', 'P')"
            Call sqlFetch_Num(countPORecCheck, sqlStmt, SqlAppDbConn)


            If countPORecCheck > 0 Then  'POs exist
                POExists = True
            Else
                POExists = False
            End If
        End If


        'Sales Order
        sqlStmt = "SELECT COUNT(*) FROM SOSetup WHERE Init = 1"
        Call sqlFetch_Num(countSOSetup, sqlStmt, SqlAppDbConn)


        If countSOSetup = 0 Then
            SOExists = False
        Else
            'Check to see if any Sales Orders exist
            sqlStmt = "SELECT COUNT(*) FROM SOHeader WHERE SOTypeID = 'SO' AND Status IN ('O', 'P')"
            Call sqlFetch_Num(countSORecCheck, sqlStmt, SqlAppDbConn)

            If countSORecCheck > 0 Then  'Sales Orders exist
                SOExists = True
            Else
                SOExists = False
            End If
        End If


        'Multi-Currency
        Call sqlFetch_1(sqlReader, "SELECT MCActivated FROM CMSetup", SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() = False Then  'No CMSetup record found
            MultiCuryEnabled = False
        Else

            Call SetCMSetupValues(sqlReader, bCMSETUPInfo)
            Call sqlReader.Close()
            If bCMSETUPInfo.MCActivated <> 1 Then  'CMSetup record exist, but Multi-Currency is not enabled
                MultiCuryEnabled = False
            Else 'CMSetup record exists and Multi-Currency is enabled
                Call sqlFetch_1(sqlReader, "SELECT * FROM Currncy WHERE CuryId NOT IN (SELECT BaseCuryId FROM GLSetup)", SqlAppDbConn, CommandType.Text)

                If sqlReader.HasRows() = False Then  'Only Currncy record for the base currency exists
                    MultiCuryEnabled = False
                Else  'Currncy records exist for currencies other than the base currency 
                    MultiCuryEnabled = True
				End If
			End If

		End If
        Call sqlReader.Close()

    End Sub

    Public Sub UnpostedBatchCheck(ByVal pModule As String)
        ''''''''''''''''''''''''''''''''''''''''''
        'Check to see if unposted batches exist
        '   By module or all modules (%)
        ''''''''''''''''''''''''''''''''''''''''''
        Dim sqlString As String = String.Empty
        Dim retValInt As Integer


        Select Case pModule
            Case "GL"
                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'GL' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
                    UBatchesExistGL = True
                End If

            Case "AP"
                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'AP' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
                    UBatchesExistAP = True
                End If


            Case "AR"
                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'AR' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
                    UBatchesExistAR = True
                End If

            Case "IN"
                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'IN' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
					UBatchesExistIN = True
				End If

			Case "PO"
                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'PO' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
					UBatchesExistPO = True
				End If

			Case "%"
                'All Modules
                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'GL' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
                    UBatchesExistGL = True
                End If

                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'AP' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
                    UBatchesExistAP = True
                End If

                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'AR' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
                    UBatchesExistAR = True
                End If

                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'IN' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
					UBatchesExistIN = True
				End If

                sqlString = "SELECT COUNT(*) FROM Batch WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND Module = 'PO' AND Status = 'U'"
                Call sqlFetch_Num(retValInt, sqlString, SqlAppDbConn)

                If retValInt > 0 Then
					UBatchesExistPO = True
				End If

		End Select

    End Sub

    Public Sub CheckCustomerBalances(EventLog As clsEventLog)
        Dim CustId_Logged As Short
        Dim ErrorOccurred As TransactionStatus
        Dim msgstr As String
        Dim MsgStrg As String = ""
        Dim custValMsgsExist As Boolean = False

        Dim sqlReader As SqlDataReader = Nothing

        Dim sqlTran As SqlTransaction
        Dim curStatus As TransactionStatus


        Dim sqlTranConn As SqlConnection = New SqlConnection(AppDbConnStr)
        Dim sqlUpdConn As SqlConnection = New SqlConnection(AppDbConnStr)

        '    Call SetRestart(c_CustBal)

        Try

            If (sqlTranConn.State = ConnectionState.Closed) Then
                sqlTranConn.Open()
            End If

            sqlTran = TranBeg(sqlTranConn)



            msgstr = "After running this process, you should print your A/R Trial Balance report (08.260) and verify that your Accounts Receivable general ledger balance(s) agree with the document detail in the Accounts Receivable module. This process will not make any adjustments to general ledger records."

                CustId_Logged = False
            'Clean out work table

            ' Delete the entries in the work table.
            Call sqlFetch_1(sqlReader, "p08990DeleteLogs", sqlTranConn, CommandType.StoredProcedure, sqlTran)
            Call sqlReader.Close()

            curStatus = TranStatus(sqlTran)
            If (curStatus <> TransactionStatus.Active And curStatus <> TransactionStatus.Committed) Then ErrorOccurred = curStatus

            'clean up old logs
            Call sqlFetch_1(sqlReader, "p08990DeleteLogs", sqlTranConn, CommandType.StoredProcedure, sqlTran)
            Call sqlReader.Close()


            MsgStrg = "Verifying Customer Balances"

            Call sqlFetch_1(sqlReader, "p08990CheckARAcct", sqlTranConn, CommandType.StoredProcedure, sqlTran)

            curStatus = TranStatus(sqlTran)
            If (curStatus <> TransactionStatus.Active And curStatus <> TransactionStatus.Committed) Then
                ErrorOccurred = TranStatus(sqlTran)
                NbrOfErrors_Cust = NbrOfErrors_Cust + 1
            End If

            Call sqlReader.Close()

            'Update worktable for customers whose balances don't match
            Call sqlFetch_1(sqlReader, "p08990ChkCurBal", sqlTranConn, CommandType.StoredProcedure, sqlTran)


            curStatus = TranStatus(sqlTran)
            If (curStatus <> TransactionStatus.Active And curStatus <> TransactionStatus.Committed) Then
                ErrorOccurred = TranStatus(sqlTran)
                NbrOfErrors_Cust = NbrOfErrors_Cust + 1
            End If

            Call sqlReader.Close()


            Call sqlFetch_1(sqlReader, "p08990ChkFutBal", sqlTranConn, CommandType.StoredProcedure, sqlTran)


            curStatus = TranStatus(sqlTran)
            If (curStatus <> TransactionStatus.Active And curStatus <> TransactionStatus.Committed) Then
                ErrorOccurred = TranStatus(sqlTran)
                NbrOfErrors_Cust = NbrOfErrors_Cust + 1
            End If

            Call sqlReader.Close()
            'Compare calc'd balances to history balances
            Call sqlFetch_1(sqlReader, "p08990CheckHistCalc", sqlTranConn, CommandType.StoredProcedure, sqlTran)


            curStatus = TranStatus(sqlTran)
            If (curStatus <> TransactionStatus.Active And curStatus <> TransactionStatus.Committed) Then
                ErrorOccurred = TranStatus(sqlTran)
                NbrOfErrors_Cust = NbrOfErrors_Cust + 1
            End If

            Call sqlReader.Close()

            'Make note of any missing history records
            Call sqlFetch_1(sqlReader, "p08990CheckHist", sqlTranConn, CommandType.StoredProcedure, sqlTran)


            curStatus = TranStatus(sqlTran)
            If (curStatus <> TransactionStatus.Active And curStatus <> TransactionStatus.Committed) Then
                ErrorOccurred = TranStatus(sqlTran)
                NbrOfErrors_Cust = NbrOfErrors_Cust + 1
            End If

            Call sqlReader.Close()

            Call TranEnd(sqlTran)
            Call sqlTranConn.Close()



            '    End Using
        Catch ex As Exception
            Call LogMessage("Error in checking customer balances" + vbNewLine, EventLog)
            Call LogMessage("Error:  " + ex.Message, EventLog)

        End Try

        Try


            If (sqlTranConn.State = ConnectionState.Closed) Then
                sqlTranConn.Open()
            End If

            '   sqlTran = TranBeg(sqlTranConn)

            'Write information to log file
            Call sqlFetch_1(sqlReader, "p08990GetLogMsgs", sqlTranConn, CommandType.StoredProcedure)

            If sqlReader.HasRows() Then
                    Call LogMessage("", EventLog)
                    Call LogMessage(MsgStrg, EventLog)
                End If

                While sqlReader.Read()

                    Call SetWrkIChkValues(sqlReader, bWrkIChkInfo)

                    Call LogMessage("", EventLog)

                    Select Case bWrkIChkInfo.MsgId
                        Case 1
                            Call LogMessage("ERROR: Customer" + SParm(bWrkIChkInfo.Custid.Trim) + "current period" + SParm(bWrkIChkInfo.Other.Trim) + "does not match current AR period.", EventLog)
                            NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                            custValMsgsExist = True
                        Case 2
                            Call LogMessage("ERROR: Customer" + SParm(bWrkIChkInfo.Custid.Trim) + "default AR account" + SParm(bWrkIChkInfo.Other.Trim) + "is not valid.", EventLog)
                            NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                            custValMsgsExist = True
                        Case 3
                            Call LogMessage("ERROR: Customer" + SParm(bWrkIChkInfo.Custid.Trim) + "default sales account" + SParm(bWrkIChkInfo.Other.Trim) + "is not valid.", EventLog)
                            NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                            custValMsgsExist = True
                        Case 10
                        Call LogMessage("ERROR: Customer" + SParm(bWrkIChkInfo.Custid.Trim) + "current balance: " + FToA(bWrkIChkInfo.OldBal, BaseCuryPrec) + " for company" + SParm(bWrkIChkInfo.Cpnyid.Trim) + "does not agree with sum of open docs: " + FToA(bWrkIChkInfo.NewBal, BaseCuryPrec) + ".", EventLog)
                        NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                            custValMsgsExist = True
                        Case 20
                        Call LogMessage("ERROR: Customer" + SParm(bWrkIChkInfo.Custid.Trim) + "future balance: " + FToA(bWrkIChkInfo.OldBal, BaseCuryPrec) + " for company" + SParm(bWrkIChkInfo.Cpnyid.Trim) + "does not agree with sum of open future period docs: " + FToA(bWrkIChkInfo.NewBal, BaseCuryPrec) + ".", EventLog)
                        NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                            custValMsgsExist = True
                        Case 30
                        Call LogMessage("WARNING: Customer" + SParm(bWrkIChkInfo.Custid.Trim) + "YTD balance: " + FToA(bWrkIChkInfo.OldBal, BaseCuryPrec) + " for company" + SParm(bWrkIChkInfo.Cpnyid.Trim) + "differs from calculated balance: " + FToA(bWrkIChkInfo.NewBal, BaseCuryPrec) + ".", EventLog)
                        NbrOfWarnings_Cust = NbrOfWarnings_Cust + 1
                            custValMsgsExist = True
                        Case 40
                            Call LogMessage("ERROR: Customer" + SParm(bWrkIChkInfo.Custid.Trim) + "missing history record for company" + SParm(bWrkIChkInfo.Cpnyid.Trim) + ".", EventLog)
                            NbrOfErrors_Cust = NbrOfErrors_Cust + 1
                            custValMsgsExist = True
                    End Select

                End While

            '   Call TranEnd(sqlTran)
            Call sqlReader.Close()

            Call sqlTranConn.Close()


        Catch ex As Exception

            Call LogMessage("Error in checking customer balances" + vbNewLine, EventLog)
            Call LogMessage("Error:  " + ex.Message, EventLog)


        End Try



        'Display message in Event Log for suggested actions for correcting balances
        If custValMsgsExist = True Then
            Call LogMessage("", EventLog)
            MsgStrg = "Suggested actions for correcting Customer balance errors are listed below:" + vbNewLine
            MsgStrg = MsgStrg + " - Run the 'Correct Customer Balances to Total Doc Balance' option in AR Integrity Check (08.990.00)" + vbNewLine
            MsgStrg = MsgStrg + " - Run the 'Rebuild AR History from Documents' option in AR Integrity Check (08.990.00)" + vbNewLine
            MsgStrg = MsgStrg + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(MsgStrg, EventLog)
        End If

    End Sub

    Public Sub CheckVendorBalances(EventLog As clsEventLog)
        Dim AdjFound As Short
        Dim sqlstrg As String = String.Empty
        Dim SqlStrg2 As String = String.Empty
        Dim VendorCurrBal As Double
        Dim VendorFutBal As Double
        Dim HistBal As Double
        Dim MsgStrg As String = String.Empty
        Dim MsgStrg0 As String = String.Empty
        Dim ndx As Short
        Dim vendValMsgsExist As Boolean = False
        Dim sqlStmt As String = ""
        Dim sqlReader As SqlDataReader = Nothing

        Dim APBalCount As Integer = 0
        Dim sqlString As String = ""

        Dim sqlBalConn As SqlConnection = Nothing
        Dim sqlBalReader As SqlDataReader = Nothing

        Dim sqlAdjConn As SqlConnection = Nothing
        Dim sqlAdjReader As SqlDataReader = Nothing

        Dim sqlAdHistConn As SqlConnection = Nothing
        Dim sqlAdHistReader As SqlDataReader = Nothing


        Dim RecordParmList As New List(Of ParmList)
        Dim ParmValues As ParmList

        Call LogMessage("Verifying Vendor Balances", EventLog)

        sqlStmt = "SELECT APAcct, ExpAcct, PerNbr, VendId, Name FROM Vendor"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            ' Create a new connection for this reader since we need to do additional queries during processing.
            sqlBalConn = New SqlClient.SqlConnection(AppDbConnStr)
            sqlBalConn.Open()

        End If

        While sqlReader.Read()
            Call SetVendorValues(sqlReader, bVendorInfo)

            ' Get the balances sum for the vendor.  If there are no balances for the vendor, then set default values.
            Call sqlFetch_Num(APBalCount, "Select Count(*) from AP_Balances where VendID = " + SParm(bVendorInfo.VendId), sqlBalConn)
            If (APBalCount = 0) Then
                bBalanceValues.CurrBal = 0.0
                bBalanceValues.FutureBal = 0.0
                bBalanceValues.LastChkDate = Date.MinValue
                bBalanceValues.LastVODate = Date.MinValue
            Else



                RecordParmList.Clear()
                ParmValues = New ParmList
                ParmValues.ParmName = "parm1"
                ParmValues.ParmType = SqlDbType.VarChar
                ParmValues.ParmValue = bVendorInfo.VendId
                RecordParmList.Add(ParmValues)

                Call sqlFetch_1(sqlBalReader, "APBalances_Tot1", sqlBalConn, CommandType.StoredProcedure, RecordParmList)

                'Actually retrieve the SUM from the database server
                Call sqlBalReader.Read()
                Call SetVendBalances(sqlBalReader, bBalanceValues)
                sqlBalReader.Close()
            End If


            'Verify Vendor Period Number
            If bVendorInfo.PerNbr <> bAPSetupInfo.CurrPerNbr Then
                Call LogMessage("Vendor: " + bVendorInfo.VendId.Trim, EventLog)
                'Warning - period is not the current fiscal period
                Call LogMessage("Period is not the current GL fiscal period.", EventLog)
            End If

            'Verify the Accounts associated with Vendor
            Call VerifyAccount(bVendorInfo.APAcct, EventLog, sqlBalConn)
            Call VerifyAccount(bVendorInfo.ExpAcct, EventLog, sqlBalConn)

            'Select the Documents for this vendor
            'Only want all open vouchers
            sqlstrg = "Select d.CpnyId, d.DocBal, d.DocType, d.OpenDoc, d.OrigDocAmt, d.PerPost, d.RefNbr, d.Rlsed, d.Terms, d.Vendid from APDoc d where d.VendId =" + SParm(bVendorInfo.VendId.Trim) + "and d.DocClass = 'N' Order by d.VendId, d.RefNbr"
            Call sqlFetch_1(sqlBalReader, sqlstrg, sqlBalConn, CommandType.Text)

            If sqlBalReader.HasRows() Then
                VendorCurrBal = 0
                VendorFutBal = 0
                HistBal = 0

                sqlAdjConn = New SqlClient.SqlConnection(AppDbConnStr)
                sqlAdjConn.Open()

                While sqlBalReader.Read()

                    Call SetAPDocValues(sqlBalReader, bAPDocInfo)
                    If bAPDocInfo.Rlsed = LTRUE Then



                        'Based on Doc Type compute Vendor current and future Balances
                        Select Case bAPDocInfo.DocType
                            Case "VO", "AC"
                                'A vendor's balance should be the sum of the docbals from all open vouchers which are released
                                If bAPDocInfo.PerPost <= bVendorInfo.PerNbr Then
                                    VendorCurrBal = FPAdd(VendorCurrBal, bAPDocInfo.DocBal, BaseCuryPrec)

                                    SqlStrg2 = "Select * from apadjust where AdjdRefNbr =" + SParm(bAPDocInfo.RefNbr) + "And AdjdDocType =" + SParm(bAPDocInfo.DocType) + "And VendID =" + SParm(bAPDocInfo.VendId)

                                    Call sqlFetch_1(sqlAdjReader, SqlStrg2, sqlAdjConn, CommandType.Text)
                                    While sqlAdjReader.Read()
                                        Call SetAPAdjustValues(sqlAdjReader, bAPAdjustInfo)
                                        If bAPAdjustInfo.AdjgPerPost > bVendorInfo.PerNbr Then
                                            VendorCurrBal = FPAdd(VendorCurrBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                            VendorFutBal = FPSub(VendorFutBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                        End If
                                    End While
                                    Call sqlAdjReader.Close()
                                Else
                                    VendorFutBal = FPAdd(VendorFutBal, bAPDocInfo.DocBal, BaseCuryPrec)
                                    SqlStrg2 = "Select * from APAdjust where AdjdRefNbr =" + SParm(bAPDocInfo.RefNbr) + "And AdjdDocType =" + SParm(bAPDocInfo.DocType) + "And VendID =" + SParm(bAPDocInfo.VendId)
                                    Call sqlFetch_1(sqlAdjReader, SqlStrg2, sqlAdjConn, CommandType.Text)

                                    While sqlAdjReader.Read()
                                        If bAPAdjustInfo.AdjgPerPost <= bVendorInfo.PerNbr Then
                                            VendorCurrBal = FPSub(VendorCurrBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                            VendorFutBal = FPAdd(VendorFutBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                        End If
                                    End While
                                    Call sqlAdjReader.Close()
                                End If

                            Case "AD", "PP"
                                If bAPDocInfo.PerPost <= bVendorInfo.PerNbr Then
                                    VendorCurrBal = FPSub(VendorCurrBal, bAPDocInfo.DocBal, BaseCuryPrec)
                                    SqlStrg2 = "Select * from APAdjust where AdjdRefNbr =" + SParm(bAPDocInfo.RefNbr) + "And AdjdDocType =" + SParm(bAPDocInfo.DocType) + "And VendID =" + SParm(bAPDocInfo.VendId)
                                    Call sqlFetch_1(sqlAdjReader, SqlStrg2, sqlAdjConn, CommandType.Text)

                                    'If no adjustment record for PP, reverse change in calculated current balance since vendor current balance only changes when check is cut for PP
                                    If bAPDocInfo.DocType = "PP" And AdjFound <> 0 And bAPDocInfo.DocBal <> 0 Then
                                        VendorCurrBal = FPAdd(VendorCurrBal, bAPDocInfo.DocBal, BaseCuryPrec)
                                    End If

                                    While sqlAdjReader.Read()
                                        Call SetAPAdjustValues(sqlAdjReader, bAPAdjustInfo)
                                        'Add Currbal in if PPay but only add future bal if in future period.
                                        If bAPAdjustInfo.AdjgPerPost > bVendorInfo.PerNbr Or bAPDocInfo.DocType = "PP" Then
                                            VendorCurrBal = FPSub(VendorCurrBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                            If bAPAdjustInfo.AdjgPerPost > bVendorInfo.PerNbr Then
                                                VendorFutBal = FPAdd(VendorFutBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                            End If
                                        End If

                                    End While
                                    Call sqlAdjReader.Close()
                                Else
                                    VendorFutBal = FPSub(VendorFutBal, bAPDocInfo.DocBal, BaseCuryPrec)
                                    SqlStrg2 = "Select * from APAdjust where AdjdRefNbr =" + SParm(bAPDocInfo.RefNbr) + "And AdjdDocType =" + SParm(bAPDocInfo.DocType) + "And VendID =" + SParm(bAPDocInfo.VendId)
                                    Call sqlFetch_1(sqlAdjReader, SqlStrg2, sqlAdjConn, CommandType.Text)

                                    While sqlAdjReader.Read()
                                        Call SetAPAdjustValues(sqlReader, bAPAdjustInfo)

                                        If bAPAdjustInfo.AdjgPerPost <= bVendorInfo.PerNbr Then
                                            VendorCurrBal = FPAdd(VendorCurrBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                            VendorFutBal = FPSub(VendorFutBal, bAPAdjustInfo.AdjAmt + bAPAdjustInfo.AdjDiscAmt, BaseCuryPrec)
                                        End If
                                    End While
                                    Call sqlAdjReader.Close()
                                End If

                        End Select
                    End If

                End While

                sqlAdjConn.Close()


                If VendorCurrBal.ToString.Trim <> bBalanceValues.CurrBal.ToString.Trim Or VendorFutBal.ToString.Trim <> bBalanceValues.FutureBal.ToString.Trim Then
                    Call LogMessage("", EventLog)
                    MsgStrg0 = "Error: Vendor " + bVendorInfo.VendId.Trim

                    If FPRnd(VendorCurrBal, BaseCuryPrec) <> FPRnd(bBalanceValues.CurrBal, BaseCuryPrec) Then
                        MsgStrg = "Calculated Current Balance of " + FToA(VendorCurrBal, BaseCuryPrec) + " does not agree with AP_Balances.CurrBal of " + FToA(bBalanceValues.CurrBal, BaseCuryPrec)
                        NbrOfErrors_Vend = NbrOfErrors_Vend + 1
                        vendValMsgsExist = True
                    End If
                    If FPRnd(VendorFutBal, BaseCuryPrec) <> FPRnd(bBalanceValues.FutureBal, BaseCuryPrec) Then
                        MsgStrg = "Calculated Future Balance of " + FToA(VendorFutBal, BaseCuryPrec) + " does not agree with AP_Balances.FutureBal of " + FToA(bBalanceValues.FutureBal, BaseCuryPrec)
                        NbrOfErrors_Vend = NbrOfErrors_Vend + 1
                        vendValMsgsExist = True
                    End If

                    Call LogMessage(MsgStrg0.Trim + " " + MsgStrg.Trim, EventLog)
                End If

                'Validate YTD Vendor History for Current Year/Period
                ' Create the parameters for the stored procedure.
                RecordParmList.Clear()
                ParmValues = New ParmList
                ParmValues.ParmName = "parm1"
                ParmValues.ParmType = SqlDbType.VarChar
                ParmValues.ParmValue = bVendorInfo.VendId
                RecordParmList.Add(ParmValues)

                ParmValues = New ParmList
                ParmValues.ParmName = "parm2"
                ParmValues.ParmType = SqlDbType.VarChar
                ParmValues.ParmValue = bVendorInfo.PerNbr.Substring(0, 4)
                RecordParmList.Add(ParmValues)

                sqlAdHistConn = New SqlClient.SqlConnection(AppDbConnStr)
                sqlAdHistConn.Open()

                Call sqlFetch_1(sqlAdHistReader, "APHist_All", sqlAdHistConn, CommandType.StoredProcedure, RecordParmList)
                If sqlAdHistReader.HasRows() Then

                    Dim PtdCrAdjList As List(Of Double) = New List(Of Double)
                    Dim PtdDrAdjList As List(Of Double) = New List(Of Double)
                    Dim PtdDiscTknList As List(Of Double) = New List(Of Double)
                    Dim PtdPaymtList As List(Of Double) = New List(Of Double)
                    Dim PtdPurchList As List(Of Double) = New List(Of Double)

                    HistBal = 0
                    ' Retrieve the data and set up arrays for each of the indexed fields.

                    While sqlAdHistReader.Read()

                        PtdCrAdjList.Clear()
                        PtdDrAdjList.Clear()
                        PtdDiscTknList.Clear()
                        PtdPaymtList.Clear()
                        PtdPurchList.Clear()

                        Call SetAPHistValues(sqlAdHistReader, bAPHistInfo)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs00)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs01)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs02)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs03)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs04)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs05)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs06)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs07)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs08)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs09)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs10)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs11)
                        PtdCrAdjList.Add(bAPHistInfo.PtdCrAdjs12)

                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs00)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs01)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs02)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs03)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs04)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs05)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs06)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs07)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs08)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs09)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs10)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs11)
                        PtdDrAdjList.Add(bAPHistInfo.PtdDrAdjs12)

                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn00)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn01)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn02)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn03)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn04)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn05)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn06)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn07)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn08)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn09)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn10)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn11)
                        PtdDiscTknList.Add(bAPHistInfo.PtdDiscTkn12)

                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt00)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt01)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt02)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt03)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt04)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt05)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt06)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt07)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt08)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt09)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt10)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt11)
                        PtdPaymtList.Add(bAPHistInfo.PtdPaymt12)

                        PtdPurchList.Add(bAPHistInfo.PtdPurch00)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch01)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch02)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch03)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch04)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch05)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch06)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch07)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch08)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch09)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch10)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch11)
                        PtdPurchList.Add(bAPHistInfo.PtdPurch12)


                        HistBal = FPAdd(HistBal, bAPHistInfo.BegBal, BaseCuryPrec)

                        For ndx = 0 To (Val(bVendorInfo.PerNbr.Substring(4, 2)) - 1)
                            HistBal = FPAdd(HistBal, PtdPurchList(ndx), BaseCuryPrec)
                            HistBal = FPSub(HistBal, PtdPaymtList(ndx), BaseCuryPrec)
                            HistBal = FPAdd(HistBal, PtdCrAdjList(ndx), BaseCuryPrec)
                            HistBal = FPSub(HistBal, PtdDiscTknList(ndx), BaseCuryPrec)
                            HistBal = FPSub(HistBal, PtdDrAdjList(ndx), BaseCuryPrec)
                        Next


                    End While


                    Call sqlAdHistReader.Close()
                    If CStr(HistBal).Trim <> CStr(VendorCurrBal).Trim Then
                        Call LogMessage("", EventLog)
                        MsgStrg0 = "WARNING: Vendor " + bVendorInfo.VendId.Trim
                        MsgStrg = "Calculated Year to Date Balance of " + FToA(HistBal, BaseCuryPrec) + " does not agree with Calculated Current Balance of " + FToA(VendorCurrBal, BaseCuryPrec)
                        Call LogMessage(MsgStrg0.Trim + " " + MsgStrg.Trim, EventLog)
                        NbrOfWarnings_Vend = NbrOfWarnings_Vend + 1
                        vendValMsgsExist = True
                    End If

                End If
                If (sqlAdHistConn.State <> ConnectionState.Closed) Then
                    sqlAdHistConn.Close()
                End If

            End If
                sqlBalReader.Close()

        End While

        If (sqlBalConn.State <> ConnectionState.Closed) Then
            sqlBalConn.Close()
        End If

        Call sqlReader.Close()

        'Display message in Event Log for suggested actions for correcting balances
        If vendValMsgsExist = True Then
            Call LogMessage("", EventLog)
            MsgStrg = "Suggested actions for correcting Vendor balance errors are listed below:" + vbNewLine
            MsgStrg = MsgStrg + " - Run the 'Rebuild Vendor Balances from Documents' option in AP Integrity Check (03.990.00)" + vbNewLine
            MsgStrg = MsgStrg + " - Run the 'Rebuild Vendor History from Documents' option in AP Integrity Check (03.990.00)" + vbNewLine
            MsgStrg = MsgStrg + " - Contact your Microsoft Dynamics SL Partner for further assistance."
            Call LogMessage(MsgStrg, EventLog)
            Call LogMessage("", EventLog)
            Call LogMessage("", EventLog)
        End If

    End Sub

    Private Sub VerifyAccount(ByRef AcctToVerify As String, EventLog As clsEventLog, AcctConn As SqlConnection)
        Dim sqlString = "Select Count(*) from Account where Acct like " + SParm(AcctToVerify)
        Dim nbrAcct As Integer = 0

        If (AcctConn.State = ConnectionState.Closed) Then
            AcctConn.Open()
        End If
        ' Verify that the account exists in the account table.

        Call sqlFetch_Num(nbrAcct, sqlString, AcctConn)

        If nbrAcct = 0 And AcctToVerify.Trim <> "" Then
            Call LogMessage(AcctToVerify.Trim() + " is not found", EventLog)
        End If

    End Sub

    '****************************
    '***** Public Functions *****
    '****************************
    Public Function CalcCurrBalanceAcctSub(ByVal pAcct As String, ByVal pSub As String) As Double
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Gets current balance for an Account based on CpnyID, Acct, LedgerID, and FiscYr
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Dim sFiscYear As String = String.Empty
        Dim sPeriod As String = String.Empty
        Dim sqlStringStart As String = "SELECT SUM(BegBal "
        Dim sqlStringBal As String = String.Empty
        Dim sqlStringEnd As String = String.Empty
        Dim retValDbl As Double

        Dim sqlStmt As String = ""

        sFiscYear = bGLSetupInfo.PerNbr.Substring(0, 4)
        sPeriod = bGLSetupInfo.PerNbr.Substring(4, 2)

        Select Case sPeriod
            Case "01"
                sqlStringBal = "+ PtdBal00) "
            Case "02"
                sqlStringBal = "+ PtdBal00 + PtdBal01) "
            Case "03"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02) "
            Case "04"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03) "
            Case "05"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04) "
            Case "06"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05) "
            Case "07"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05 + PtdBal06) "
            Case "08"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05 + PtdBal06 + PtdBal07) "
            Case "09"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05 + PtdBal06 + PtdBal07 + PtdBal08) "
            Case "10"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05 + PtdBal06 + PtdBal07 + PtdBal08 + PtdBal09) "
            Case "11"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05 + PtdBal06 + PtdBal07 + PtdBal08 + PtdBal09 + PtdBal10) "
            Case "12"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05 + PtdBal06 + PtdBal07 + PtdBal08 + PtdBal09 + PtdBal10 + PtdBal11) "
            Case "13"
                sqlStringBal = "+ PtdBal00 + PtdBal01 + PtdBal02 + PtdBal03 + PtdBal04 + PtdBal05 + PtdBal06 + PtdBal07 + PtdBal08 + PtdBal09 + PtdBal10 + PtdBal11 + PtdBal12) "
        End Select

        sqlStringEnd = "FROM AcctHist WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND Acct =" + SParm(pAcct) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND FiscYr =" + SParm(sFiscYear) + "AND Sub LIKE" + SParm(pSub)

        'Execute SQL statement to calculate the current balance for the Account
        sqlStmt = sqlStringStart + sqlStringBal + sqlStringEnd
        Call sqlFetch_Num(retValDbl, sqlStmt, SqlAppDbConn)

        CalcCurrBalanceAcctSub = retValDbl

    End Function

    Public Function CalcPTDBalanceAcctTrans(ByVal pAcct As String, ByVal pSub As String, ByVal pPeriod As String, ByVal pType As String, ByVal pFiscYear As String) As Double
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Gets PTD balance from posted GLTran records for an Account based on CpnyID, Acct, LedgerID, and FiscYr
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        'Dim sFiscYear As String = String.Empty
        Dim sqlString As String = String.Empty
        Dim retValDbl As Double
        Dim sqlReader As SqlDataReader = Nothing

        'sFiscYear = bGLSetup.PerNbr.Substring(0, 4)

        sqlString = "SELECT SUM(DrAmt) - SUM(CrAmt) AS 'PTDAmt' FROM GLTran "
        sqlString = sqlString + "WHERE CpnyID =" + SParm(CpnyId.Trim) + "AND LedgerID =" + SParm(bGLSetupInfo.LedgerID.Trim) + "AND FiscYr =" + SParm(pFiscYear)
        sqlString = sqlString + "AND Posted = 'P' AND Acct =" + SParm(pAcct) + "AND Sub =" + SParm(pSub) + "AND PerPost =" + SParm(pFiscYear + pPeriod)

        'Execute SQL statement to calculate the PTD balance from posted transactions for the Account
        Call sqlFetch_Num(retValDbl, sqlString, SqlAppDbConn)
        ' fetchRetVal = SGroupFetch1(csrFetch, retValDbl)

        'Switch the sign if the Account Type is Liability or Income
        If (pType.Substring(1, 1).ToUpper = "L") Or (pType.Substring(1, 1).ToUpper = "I") Then
            retValDbl = (retValDbl * -1)
        End If

        CalcPTDBalanceAcctTrans = retValDbl

    End Function

    Public Function SubSegmentLookup(ByVal pSub As String, ByVal pSegment As Integer) As String
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Returns the segment value based on the subaccount and subaccount segment passed to the function
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Select Case pSegment
            Case 1
                SubSegmentLookup = pSub.Substring(0, bFlexDefInfo.SegLength00)
            Case 2
                SubSegmentLookup = pSub.Substring(bFlexDefInfo.SegLength00, bFlexDefInfo.SegLength01)
            Case 3
                SubSegmentLookup = pSub.Substring(bFlexDefInfo.SegLength00 + bFlexDefInfo.SegLength01, bFlexDefInfo.SegLength02)
            Case 4
                SubSegmentLookup = pSub.Substring(bFlexDefInfo.SegLength00 + bFlexDefInfo.SegLength01 + bFlexDefInfo.SegLength02, bFlexDefInfo.SegLength03)
            Case 5
                SubSegmentLookup = pSub.Substring(bFlexDefInfo.SegLength00 + bFlexDefInfo.SegLength01 + bFlexDefInfo.SegLength02 + bFlexDefInfo.SegLength03, bFlexDefInfo.SegLength04)
            Case 6
                SubSegmentLookup = pSub.Substring(bFlexDefInfo.SegLength00 + bFlexDefInfo.SegLength01 + bFlexDefInfo.SegLength02 + bFlexDefInfo.SegLength03 + bFlexDefInfo.SegLength04, bFlexDefInfo.SegLength05)
            Case 7
                SubSegmentLookup = pSub.Substring(bFlexDefInfo.SegLength00 + bFlexDefInfo.SegLength01 + bFlexDefInfo.SegLength02 + bFlexDefInfo.SegLength03 + bFlexDefInfo.SegLength04 + bFlexDefInfo.SegLength05, bFlexDefInfo.SegLength06)
            Case 8
                SubSegmentLookup = pSub.Substring(bFlexDefInfo.SegLength00 + bFlexDefInfo.SegLength01 + bFlexDefInfo.SegLength02 + bFlexDefInfo.SegLength03 + bFlexDefInfo.SegLength04 + bFlexDefInfo.SegLength05 + bFlexDefInfo.SegLength06, bFlexDefInfo.SegLength07)
            Case Else
                SubSegmentLookup = ""
        End Select

    End Function

    Public Function DblQuotes(ByVal input As String) As String
        '''''''''''''''''''''''''''''''''''''''''''''''''
        'Wraps the passed input string in double quotes
        '''''''''''''''''''''''''''''''''''''''''''''''''

        DblQuotes = String.Empty
        DblQuotes = """" + input + """"

    End Function

    Public Function NameFlip(ByVal NameToFlip As String) As String
        '''''''''''''''''''''''''''''''''''''''''''''''''
        'Flips name provided at appropriate place
        '''''''''''''''''''''''''''''''''''''''''''''''''

        Dim FlipPos As Short

        FlipPos = InStr(NameToFlip, "~")
        If FlipPos = 0 Then
            NameFlip = NameToFlip
        ElseIf FlipPos = 1 Then
            NameFlip = Mid(NameToFlip, 2)
        ElseIf FlipPos = Len(NameToFlip) Then
            NameFlip = Left(NameToFlip, Len(NameToFlip) - 1)
        Else
            NameFlip = Trim(Mid(NameToFlip, FlipPos + 1)) + " " + Left(NameToFlip, FlipPos - 1)
        End If

    End Function

    Public Function formatPhoneNumber(phoneNum As String, phoneLen As Integer) As String
        '''''''''''''''''''''''''''''''''''''''''''''''''
        'Formats Phone and fax numbers
        '''''''''''''''''''''''''''''''''''''''''''''''''

        Select Case phoneLen
            Case 10
                phoneNum = CLng(phoneNum).ToString("(###) ###-####")
            Case 11
                phoneNum = CLng(phoneNum).ToString("(###) ###-#### #")
            Case 12
                phoneNum = CLng(phoneNum).ToString("(###) ###-#### ##")
            Case 13
                phoneNum = CLng(phoneNum).ToString("(###) ###-#### ###")
            Case 14
                phoneNum = CLng(phoneNum).ToString("(###) ###-#### ####")
            Case 15
                phoneNum = CLng(phoneNum).ToString("(###) ###-#### #####")
            Case Else
                phoneNum = phoneNum
        End Select

        Return phoneNum
    End Function


    '**************************
    '***** Public Classes *****
    '**************************

    Public Class BalanceValuesCls
        Private m_LastChkDate As Date
        Private m_LastVODate As Date
        Private m_CurrBal As Double
        Private m_FutureBal As Double

        Public Property LastChkDate() As Date
            Get
                Return m_LastChkDate
            End Get
            Set(ByVal setval As Date)
                m_LastChkDate = setval
            End Set
        End Property

        Public Property LastVODate() As Date
            Get
                Return m_LastVODate
            End Get
            Set(ByVal setval As Date)
                m_LastVODate = setval
            End Set
        End Property

        Public Property CurrBal() As Double
            Get
                Return m_CurrBal
            End Get
            Set(ByVal setval As Double)
                m_CurrBal = setval
            End Set
        End Property


        Public Property FutureBal() As Double
            Get
                Return m_FutureBal
            End Get
            Set(ByVal setval As Double)
                m_FutureBal = setval
            End Set
        End Property

    End Class

    Public bBalanceValues As BalanceValuesCls = New BalanceValuesCls

    Public Sub SetVendBalances(sqlReader As SqlDataReader, BalanceValuesBuf As BalanceValuesCls)
        Try
            BalanceValuesBuf.LastChkDate = sqlReader.GetDateTime(0)   ' MAX(LastChkDate)
            BalanceValuesBuf.LastVODate = sqlReader.GetDateTime(1)   ' MAX(LastVODate)
            BalanceValuesBuf.CurrBal = sqlReader.GetDouble(2)  ' SUM(CurrBal)
            BalanceValuesBuf.FutureBal = sqlReader.GetDouble(3)   ' SUM(FutureBal)

        Catch ex As Exception

        End Try
    End Sub

    Public Class PerNbrCheckCls

        Private m_PerPost As String
        Public Property PerPost() As String
            Get
                Return m_PerPost
            End Get
            Set(ByVal setval As String)
                m_PerPost = setval
            End Set
        End Property

    End Class

    Public bPerNbrCheckInfo As PerNbrCheckCls = New PerNbrCheckCls, nPerNbrCheckInfo As PerNbrCheckCls = New PerNbrCheckCls

    Public Sub SetPerNbrCheckValues(sqlReader As SqlDataReader, PerNbrCheckBuf As PerNbrCheckCls)
        Try
            PerNbrCheckBuf.PerPost = sqlReader(0)
        Catch ex As Exception

        End Try
    End Sub


    Public Class SubAcctListCls

        Private m_SubAcct As String

        Public Property SubAcct() As String
            Get
                Return m_SubAcct
            End Get
            Set(ByVal setval As String)
                m_SubAcct = setval
            End Set
        End Property

    End Class

    Public bSubAcctListInfo As SubAcctListCls = New SubAcctListCls, nSubAcctListInfo As SubAcctListCls = New SubAcctListCls

    Public Sub SetSubAcctListValues(sqlReader As SqlDataReader, SubAcctListBuf As SubAcctListCls)
        Try
            SubAcctListBuf.SubAcct = sqlReader(0)
        Catch ex As Exception

        End Try
    End Sub


    Public Class SpecIDErrorListCls

        Private m_InvtId As String
        Private m_SiteId As String
        Private m_SpecificCostID As String
        Private m_Qty As Double
        Public Property InvtID() As String
            Get
                Return m_InvtId
            End Get
            Set(ByVal setval As String)
                m_InvtId = setval
            End Set
        End Property

        Public Property SiteID() As String
            Get
                Return m_SiteId
            End Get
            Set(ByVal setval As String)
                m_SiteId = setval
            End Set
        End Property

        Public Property SpecificCostID() As String
            Get
                Return m_SpecificCostID
            End Get
            Set(ByVal setval As String)
                m_SpecificCostID = setval
            End Set
        End Property

        Public Property Qty() As Double
            Get
                Return m_Qty
            End Get
            Set(ByVal setval As Double)
                m_Qty = setval
            End Set
        End Property

    End Class

    Public bSpecIDErrorList As SpecIDErrorListCls = New SpecIDErrorListCls, nSpecIDErrorList As SpecIDErrorListCls = New SpecIDErrorListCls
    Public Sub SetSpecIDErrorListValues(sqlReader As SqlDataReader, SpecIDErrorListBuf As SpecIDErrorListCls)
        Try
            SpecIDErrorListBuf.InvtID = sqlReader("InvtdID")
            SpecIDErrorListBuf.Qty = sqlReader("Qty")
            SpecIDErrorListBuf.SiteID = sqlReader("SiteID")
            SpecIDErrorListBuf.SpecificCostID = sqlReader("SpecificCostID")
        Catch ex As Exception

        End Try
    End Sub


    Public Class TermsListCls
        Private m_TermsId As String
        Public Property TermId() As String
            Get
                Return m_TermsId
            End Get
            Set(ByVal setval As String)
                m_TermsId = setval
            End Set
        End Property

    End Class

    Public bTermsListInfo As TermsListCls = New TermsListCls, nTermsListInfo As TermsListCls = New TermsListCls

    Public Sub SetTermsListValues(sqlReader As SqlDataReader, TermsListBuf As TermsListCls, FieldName As String)
        Try
            TermsListBuf.TermId = sqlReader(FieldName)
        Catch
        End Try
    End Sub


    Public Class AcctHistList

        Private m_Acct As String
        Private m_CpnyId As String
        Private m_FiscYr As String
        Private m_LedgerID As String
        Private m_SubAcct As String

        Public Property Acct() As String
            Get
                Return m_Acct
            End Get
            Set(ByVal setval As String)
                m_Acct = setval
            End Set
        End Property
        Public Property CpnyId() As String
            Get
                Return m_CpnyId
            End Get
            Set(ByVal setval As String)
                m_CpnyId = setval
            End Set
        End Property

        Public Property FiscYr() As String
            Get
                Return m_FiscYr
            End Get
            Set(ByVal setval As String)
                m_FiscYr = setval
            End Set
        End Property
        Public Property LedgerID() As String
            Get
                Return m_LedgerID
            End Get
            Set(ByVal setval As String)
                m_LedgerID = setval
            End Set
        End Property
        Public Property SubAcct() As String
            Get
                Return m_SubAcct
            End Get
            Set(ByVal setval As String)
                m_SubAcct = setval
            End Set
        End Property


    End Class

    Public bAcctHistList As AcctHistList = New AcctHistList, nAcctHistList As AcctHistList = New AcctHistList

    Public Sub SetAcctHistValues(sqlReader As System.Data.SqlClient.SqlDataReader, AcctHistRec As AcctHistList)

        If (sqlReader.HasRows) Then
            Try
                AcctHistRec.Acct = sqlReader("Acct")
                AcctHistRec.CpnyId = sqlReader("CpnyId")
                AcctHistRec.FiscYr = sqlReader("FiscYr")
                AcctHistRec.LedgerID = sqlReader("LedgerID")
                AcctHistRec.SubAcct = sqlReader("Sub")
            Catch
                AcctHistRec.Acct = String.Empty
                AcctHistRec.CpnyId = String.Empty
                AcctHistRec.FiscYr = String.Empty
                AcctHistRec.LedgerID = String.Empty
                AcctHistRec.SubAcct = String.Empty
            End Try

        End If
    End Sub

    Private m_AcctNum As String
    Private m_BatNbr As String
    Private m_LineNbr As Short
    Private m_ModuleID As String


    Public Class APBalListCls

        Private m_CpnyId As String
        Private m_VendId As String


        Public Property CpnyId() As String
            Get
                Return m_CpnyId
            End Get
            Set(ByVal setval As String)
                m_CpnyId = setval
            End Set
        End Property

        Public Property VendID() As String
            Get
                Return m_VendId
            End Get
            Set(ByVal setval As String)
                m_VendId = setval
            End Set
        End Property
    End Class

    Public bAPBalListInfo As APBalListCls = New APBalListCls, nAPBalListInfo As APBalListCls = New APBalListCls

    Public Sub SetAPBalListValues(sqlReader As SqlDataReader, APBalListBuf As APBalListCls)
        Try
            APBalListBuf.CpnyId = sqlReader("CpnyId")
            APBalListBuf.VendID = sqlReader("VendId")
        Catch ex As Exception

        End Try
    End Sub

    Public Class ARBalList

        Private m_CpnyId As String
        Private m_CustID As String

        Public Property CpnyId() As String
            Get
                Return m_CpnyId
            End Get
            Set(ByVal setval As String)
                m_CpnyId = setval
            End Set
        End Property

        Public Property CustID() As String
            Get
                Return m_CustID
            End Get
            Set(ByVal setval As String)
                m_CustID = setval
            End Set
        End Property
    End Class

    Public bARBalList As ARBalList = New ARBalList, nARBalList As ARBalList = New ARBalList

    Public Sub SetARBalListvalues(sqlReader As SqlDataReader, ARBalListBuf As ARBalList)
        Try
            ARBalListBuf.CpnyId = sqlReader("CpnyID")
            ARBalListBuf.CustID = sqlReader("CustID")
        Catch ex As Exception

        End Try
    End Sub

    Public Class OrdNbrListCls

        Private m_OrdNbr As String

        Public Property OrdNbr() As String
            Get
                Return m_OrdNbr
            End Get
            Set(ByVal setval As String)
                m_OrdNbr = setval
            End Set
        End Property


    End Class

    Public bOrdNbrListInfo As OrdNbrListCls = New OrdNbrListCls, nOrdNbrListInfo As OrdNbrListCls = New OrdNbrListCls

    Public Sub SetOrdNbrListValues(sqlReader As SqlDataReader, OrdNbrListBuf As OrdNbrListCls)
        Try
            OrdNbrListBuf.OrdNbr = sqlReader("OrdNbr")
        Catch ex As Exception

        End Try
    End Sub

    Public Class ShipperListCls

        Private m_ShipperID As String

        Public Property ShipperID() As String
            Get
                Return m_ShipperID
            End Get
            Set(ByVal setval As String)
                m_ShipperID = setval
            End Set
        End Property

    End Class

    Public bShipperListInfo As ShipperListCls = New ShipperListCls, nShipperListInfo As ShipperListCls = New ShipperListCls

    Public Sub SetShipperListValues(sqlReader As SqlDataReader, ShipperListBuf As ShipperListCls)
        Try
            ShipperListBuf.ShipperID = sqlReader("ShipperID")
        Catch ex As Exception

        End Try
    End Sub

    Public Class OrdNbrCustListCls

        Private m_OrdNbr As String
        Private m_CustID As String

        Public Property OrdNbr() As String
            Get
                Return m_OrdNbr
            End Get
            Set(ByVal setval As String)
                m_OrdNbr = setval
            End Set
        End Property
        Public Property CustID() As String
            Get
                Return m_CustID
            End Get
            Set(ByVal setval As String)
                m_CustID = setval
            End Set
        End Property

    End Class

    Public bOrdNbrCustListInfo As OrdNbrCustListCls = New OrdNbrCustListCls, nOrdNbrCustList As OrdNbrCustListCls = New OrdNbrCustListCls

    Public Sub SetOrdNbrCustListValues(sqlReader As SqlDataReader, OrdNbrCustListBuf As OrdNbrCustListCls)

        Try
            OrdNbrCustListBuf.CustID = sqlReader("CustId")
            OrdNbrCustListBuf.OrdNbr = sqlReader("OrdNbr")
        Catch ex As Exception

        End Try
    End Sub

    Public Class OrdNbrInvtListCls

        Private m_OrdNbr As String
        Private m_InvtId As String

        Public Property OrdNbr() As String
            Get
                Return m_OrdNbr
            End Get
            Set(ByVal setval As String)
                m_OrdNbr = setval
            End Set
        End Property
        Public Property InvtID() As String
            Get
                Return m_InvtId
            End Get
            Set(ByVal setval As String)
                m_InvtId = setval
            End Set
        End Property

    End Class

    Public bOrdNbrInvtListInfo As OrdNbrInvtListCls = New OrdNbrInvtListCls, nOrdNbrInvtList As OrdNbrInvtListCls = New OrdNbrInvtListCls

    Public Sub SetOrdNbrInvtListValues(sqlReader As SqlDataReader, OrdNbrInvtListBuf As OrdNbrInvtListCls)

        Try
            OrdNbrInvtListBuf.InvtID = sqlReader("InvtID")
            OrdNbrInvtListBuf.OrdNbr = sqlReader("OrdNbr")
        Catch ex As Exception

        End Try
    End Sub

    Public Class ShipperCustListCls
        Private m_ShipperID As String
        Private m_CustID As String

        Public Property ShipperID() As String
            Get
                Return m_ShipperID
            End Get
            Set(ByVal setval As String)
                m_ShipperID = setval
            End Set
        End Property
        Public Property CustID() As String
            Get
                Return m_CustID
            End Get
            Set(ByVal setval As String)
                m_CustID = setval
            End Set
        End Property


    End Class

    Public bShipperCustListInfo As ShipperCustListCls = New ShipperCustListCls, nShipperCustListInfo As ShipperCustListCls = New ShipperCustListCls

    Public Sub SetShipperCustListValues(sqlReader As SqlDataReader, ShipperCustListBuf As ShipperCustListCls)
        Try
            ShipperCustListBuf.CustID = sqlReader("CustID")
            ShipperCustListBuf.ShipperID = sqlReader("ShipperID")
        Catch ex As Exception

        End Try
    End Sub

    Public Class ShipperInvtListCls

        Private m_ShipperID As String
        Private m_InvtId As String

        Public Property ShipperID() As String
            Get
                Return m_ShipperID
            End Get
            Set(ByVal setval As String)
                m_ShipperID = setval
            End Set
        End Property
        Public Property InvtID() As String
            Get
                Return m_InvtId
            End Get
            Set(ByVal setval As String)
                m_InvtId = setval
            End Set
        End Property


    End Class

    Public bShipperInvtListInfo As ShipperInvtListCls = New ShipperInvtListCls, nShipperInvtListInfo As ShipperInvtListCls = New ShipperInvtListCls

    Public Sub SetShipperInvtListValues(sqlReader As SqlDataReader, ShipperInvtListBuf As ShipperInvtListCls)
        Try
            ShipperInvtListBuf.InvtID = sqlReader("InvtID")
            ShipperInvtListBuf.ShipperID = sqlReader("ShipperID")
        Catch ex As Exception

        End Try
    End Sub


    Public Sub ValidateSegDefValue(tblName As String, fldName As String, OpenRecs As Boolean, EventLog As clsEventLog)

        ' For each flexdef segment defined (based on the number of segments in the flexdef table), check whether the segment is valid in SegDef.
        '       If not, then log the error and 

        Dim sqlString As String = String.Empty
        Dim sqlString2 As String = String.Empty
        Dim sqlString3 As String = String.Empty
        Dim msgText As String
        Dim StartPos As Integer = 0
        Dim csrDelete As Integer = 0

        Dim baddSegDef As SegDefCls = New SegDefCls
        Dim sqlStmt As String = ""
        Dim sqlReader As SqlDataReader = Nothing

        Dim SegDefLengthArray As List(Of Short) = New List(Of Short)

        Dim sqlDefConn As SqlConnection = Nothing
        Dim sqlDefReader As SqlDataReader = Nothing
        Dim sqlErrConn As SqlConnection = Nothing
        Dim sqlErrReader As SqlDataReader = Nothing

        Dim SqlInsertStmt As String = ""
        Dim retval As Integer = 0

        Dim dbTran As SqlTransaction = Nothing


        '***********************************************************************************'
        'Get FlexDef information
        sqlStmt = "SELECT NumberSegments, SegLength00, SegLength01, SegLength02, SegLength03, SegLength04, SegLength05, SegLength06, SegLength07 FROM FlexDef WHERE FieldClassName = 'SUBACCOUNT'"

        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If (sqlReader.HasRows) Then

            Call sqlReader.Read()
            Call SetFlexDefValues(sqlReader, bFlexDefInfo)

            ' Create an array of segment lengths.
            SegDefLengthArray.Add(bFlexDefInfo.SegLength00)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength01)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength02)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength03)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength04)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength05)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength06)
            SegDefLengthArray.Add(bFlexDefInfo.SegLength07)

            sqlReader.Close()

            sqlDefConn = New SqlClient.SqlConnection(AppDbConnStr)


            sqlErrConn = New SqlClient.SqlConnection(AppDbConnStr)



            'Check Each Segment
            StartPos = 1
            For cnt As Integer = 1 To bFlexDefInfo.NumberSegments

                ' This is a little more complicated for artran and aptran tables if we only want the open docs.   Generate the 
                '       SQL to retrieve the trans for any open documents if specified.
                If (OpenRecs = True) Then
                    If (String.Compare(tblName.Trim(), "APTran", True) = 0) Then
                        sqlString = "SELECT DISTINCT(SUBSTRING(t.sub," + StartPos.ToString + "," + SegDefLengthArray(cnt - 1).ToString + ")) FROM "
                        sqlString = String.Concat(sqlString, "APTran t inner join APDoc d on t.batnbr = d.batnbr where d.OpenDoc = 1 and t.CpnyId = ", String.Concat(SParm(CpnyId)))
                    ElseIf (String.Compare(tblName.Trim(), "ARTran", True) = 0) Then
                        sqlString = "SELECT DISTINCT(SUBSTRING(t.sub," + StartPos.ToString + "," + SegDefLengthArray(cnt - 1).ToString + ")) FROM "
                        sqlString = String.Concat(sqlString, "ARTran t inner join ARDoc d on t.batnbr = d.batnbr where d.OpenDoc = 1 and t.CpnyId = ", String.Concat(SParm(CpnyId)))
                    Else ' Unhandled case - just select all records
                        sqlString = "Select DISTINCT(SUBSTRING(" + fldName + ", " + StartPos.ToString + ", " + SegDefLengthArray(cnt - 1).ToString + ")) FROM " + tblName.Trim()
                        sqlString = String.Concat(sqlString, " Where CpnyId = " + SParm(CpnyId))
                    End If
                Else

                    sqlString = "Select DISTINCT(SUBSTRING(" + fldName + "," + StartPos.ToString + "," + SegDefLengthArray(cnt - 1).ToString + ")) FROM " + tblName.Trim()
                    sqlString = String.Concat(sqlString, " Where CpnyId = " + SParm(CpnyId))
                End If


                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                While sqlReader.Read()

                    Call setSubAcctListValues(sqlReader, bSubAcctListInfo)
                    If bSubAcctListInfo.SubAcct.Trim IsNot String.Empty Then

                        If sqlDefConn.State <> ConnectionState.Open Then
                            sqlDefConn.Open()
                        End If
                        'Check to see if segment value is in the SegDef table
                        sqlString2 = "Select * FROM SegDef WHERE FieldClassName = 'SUBACCOUNT' AND SegNumber =  '" + CStr(cnt) + "' AND ID =" + SParm(bSubAcctListInfo.SubAcct.Trim)
                            Call sqlFetch_1(sqlDefReader, sqlString2, sqlDefConn, CommandType.Text)

                            If sqlDefReader.HasRows() = False Then
                                'Write record to xSLMPTSubErrors table
                                sqlString3 = "SELECT * FROM xSLMPTSubErrors WHERE SegNumber = '" + CStr(cnt) + "' AND ID =" + SParm(bSubAcctListInfo.SubAcct.Trim)
                                Call sqlFetch_1(sqlErrReader, sqlString3, sqlErrConn, CommandType.Text)
                                If sqlErrReader.HasRows() = False Then
                                    Call sqlErrReader.Close()
                                    bxSLMPTSubErrorsInfo.SegNumber = cnt
                                    bxSLMPTSubErrorsInfo.ID = bSubAcctListInfo.SubAcct.Trim


                                    If (sqlErrConn.State <> ConnectionState.Open) Then
                                        sqlErrConn.Open()
                                    End If

                                    dbTran = TranBeg(sqlErrConn)

                                    SqlInsertStmt = "Insert into xSLMPTSubErrors ([ID], [SegNumber]) Values(" + SParm(bSubAcctListInfo.SubAcct.Trim) + "," + SParm(cnt.ToString) + ")"


                                    '   Call SInsert1(CSR_SegDef, "SegDef", baddSegDef)
                                    retval = sql_1(sqlErrReader, SqlInsertStmt, sqlErrConn, OperationType.InsertOp, CommandType.Text, dbTran)
                                    If (retval = 1) Then
                                        statusExists = True
                                    End If
                                    Call TranEnd(dbTran)

                                    sqlErrConn.Close()

                                End If


                                Call sqlErrReader.Close()
                            End If
                        Call sqlDefReader.Close()
                        If (sqlDefConn.State <> ConnectionState.Closed) Then
                            Call sqlDefConn.Close()
                        End If
                    End If

                End While

                sqlReader.Close()

                StartPos = StartPos + SegDefLengthArray(cnt - 1)
            Next cnt
            Call sqlReader.Close()

        End If

        'If records exist in xSLMPTSubErrors table, write errors to the event log
        sqlString = "SELECT * FROM xSLMPTSubErrors ORDER BY SegNumber, ID"
        Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() Then
            Call LogMessage("", EventLog)
            msgText = "The following Subaccount Segment IDs are not found in the SegDef table. All Subaccount Segment IDs must be set up in"
            msgText = msgText + " Flexkey Table Maintenance (21.330.00) prior to migrating data."
            msgText = msgText + " Missing Segment IDs will be added TypeOf the SegDef table."
            Call LogMessage(msgText, EventLog)

            sqlDefConn = New SqlClient.SqlConnection(AppDbConnStr)
            sqlDefConn.Open()



        End If

        baddSegDef.Active = 0
        baddSegDef.Crtd_DateTime = Date.Now
        baddSegDef.Crtd_Prog = "SLMPT00"
        baddSegDef.Crtd_User = UserId
        baddSegDef.Description = "SL Migration Added"
        baddSegDef.FieldClass = "001"
        baddSegDef.FieldClassName = "SUBACCOUNT"
        baddSegDef.LUpd_DateTime = Date.Now
        baddSegDef.LUpd_Prog = "SLMPT00"
        baddSegDef.LUpd_User = UserId
        baddSegDef.User1 = ""
        baddSegDef.User2 = ""


        Dim cmdText As String = String.Empty
        Dim sqlSegDefReader As SqlDataReader = Nothing
        Dim sqlSegDefConn As SqlConnection = New SqlConnection(AppDbConnStr)
        sqlSegDefConn.Open()

        ' Add the segdef value to the table.
        While sqlReader.Read()

            Call SetxSLMPTSubErrorValues(sqlReader, bxSLMPTSubErrorsInfo)

            cmdText = String.Empty

            msgText = "Segment Number: " + bxSLMPTSubErrorsInfo.SegNumber.Trim + " Segment ID: " + bxSLMPTSubErrorsInfo.ID.Trim
            Call LogMessage(msgText, EventLog)
            NbrOfErrors_COA = NbrOfErrors_COA + 1

            baddSegDef.ID = bxSLMPTSubErrorsInfo.ID.Trim()
            baddSegDef.SegNumber = bxSLMPTSubErrorsInfo.SegNumber.Trim()

            Call InitializeSegDefRecord(cmdText, baddSegDef)
            cmdText = String.Concat("Insert Into SegDef (", cmdText, ")")

            Try


                If (sqlSegDefConn.State <> ConnectionState.Open) Then
                    sqlSegDefConn.Open()
                End If


                dbTran = TranBeg(sqlSegDefConn)

                retval = sql_1(sqlSegDefReader, cmdText, sqlSegDefConn, OperationType.InsertOp, CommandType.Text, dbTran)

                If (retval = 1) Then
                    statusExists = True
                End If


                Call TranEnd(dbTran)

            Catch ex As Exception
                msgText = "Failed to add Segment Number: " + bxSLMPTSubErrorsInfo.SegNumber.Trim + " Segment ID: " + bxSLMPTSubErrorsInfo.ID.Trim
                Call LogMessage(msgText, EventLog)
            End Try

        End While

        Call sqlSegDefConn.Close()
        Call sqlReader.Close()

        'Delete records in xSLMPTSubErrors table
        sqlStmt = "Delete from xSLMPTSubErrors"
        Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)



    End Sub

    Public Sub AddTerms(termsList As List(Of String), TblName As String, fldName As String)

        Dim sqlString As String
        Dim sqlReader As SqlDataReader = Nothing


        sqlString = String.Concat("Select DISTINCT(", fldName.Trim, ") from ", TblName.Trim())
        Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
        ' Add all results to the list of terms.
        While (sqlReader.Read())
            Call SetTermsListValues(sqlReader, bTermsListInfo, fldName)

            If (termsList.Contains(bTermsListInfo.TermId.Trim) = False) And (String.IsNullOrEmpty(bTermsListInfo.TermId.Trim) = False) Then
                termsList.Add(bTermsListInfo.TermId.Trim)
            End If
        End While

        Call sqlReader.Close()

    End Sub


    Public Sub LogMessage(msg As String, eventLog As clsEventLog)
        '  Add the message to the event log.
        eventLog.LogMessage(0, msg)
    End Sub


    Public Sub DisplayLog(filename As String)
        Dim NPErr As Integer

        ' Display the event log.
        If (My.Computer.FileSystem.FileExists(filename.Trim)) Then

            ' See if the file also exists in the Virtual Store....
            NPErr = Shell("Notepad.exe " + filename.Trim, AppWinStyle.NormalFocus, True)
            If NPErr > 0 Then
                Call MessageBox.Show("Unable to display log = " + filename)
            End If
        End If

    End Sub

    Public Sub VerifyTranAmt(perNumber As String, Acct As String, AcctSub As String, AcctType As String, FiscYear As String, EventLog As clsEventLog, bHistBuf As AcctHistType)
        Dim ptdTranAmt As Double
        Dim msgText As String
        Dim cmpValue As Double

        ptdTranAmt = CalcPTDBalanceAcctTrans(Acct, AcctSub, perNumber, AcctType, FiscYear)

        Select Case perNumber.Trim
            Case "01"
                cmpValue = bHistBuf.PtdBal00
            Case "02"
                cmpValue = bHistBuf.PtdBal01
            Case "03"
                cmpValue = bHistBuf.PtdBal02
            Case "04"
                cmpValue = bHistBuf.PtdBal03
            Case "05"
                cmpValue = bHistBuf.PtdBal04
            Case "06"
                cmpValue = bHistBuf.PtdBal05
            Case "07"
                cmpValue = bHistBuf.PtdBal06
            Case "08"
                cmpValue = bHistBuf.PtdBal07
            Case "09"
                cmpValue = bHistBuf.PtdBal08
            Case "10"
                cmpValue = bHistBuf.PtdBal09
            Case "11"
                cmpValue = bHistBuf.PtdBal10
            Case "12"
                cmpValue = bHistBuf.PtdBal11
            Case "13"
                cmpValue = bHistBuf.PtdBal12


        End Select
        If FPRnd(ptdTranAmt, 2) <> FPRnd(cmpValue, 2) Then
            'Write to event log
            msgText = "ERROR: Acccount: " + Acct.Trim + vbTab + "Sub: " + AcctSub.Trim + vbTab + "Period: " + FiscYear.Trim + perNumber
            msgText = msgText + vbNewLine
            msgText = msgText + "The sum of the posted transactions (" + ptdTranAmt.ToString + ") does not match the amount (" + cmpValue.ToString + ") in Account History"
            Call LogMessage("", EventLog)
            Call LogMessage(msgText, EventLog)
            NbrOfErrors_COA = NbrOfErrors_COA + 1
        End If

    End Sub


End Module
