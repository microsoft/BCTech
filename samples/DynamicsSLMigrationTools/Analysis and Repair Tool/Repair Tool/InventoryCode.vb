
Imports System.Data.SqlClient

Module InventoryCode
    '=======================================================================================
    ' This module contains code to prepare the Inventory data for migration
    '
    '=======================================================================================

    Public Sub Validate()
        '====================================================================================
        ' Validate Inventory related records
        '   - Inventory.InvtID is not blank
        '   - Inventory.InvtID is not longer than 20 characters
        '   - Inventory.StkUnit is valid
        '   - Inventory.ClassID is valid
        '   - Inventory.ValMthd is valid
        '   - Inventory.Supplr1 is valid
        '   - Site.SiteID is not blank
        '   - LocTable.Site is not blank
        '   - LocTable.WhseLoc is not blank
        '   - ItemXRef.InvtID is valid
        '   - ItemXRef.EntityID is a valid CustID or VendID
        ' Validate Quantity and Cost records
        '   - ItemSite.InvtID is not blank
        '   - ItemSite.SiteID is not blank
        '   - Location.InvtID is not blank
        '   - Location.SiteID is not blank
        '   - Location.WhseLoc is not blank
        '   - ItemCost.InvtID is not blank
        '   - ItemCost.SiteID is not blank
        '   - ItemCost.LayerType is not blank
        '   - ItemCost.RcptNbr is not blank for FIFO/LIFO items
        '   - ItemCost.SpecificCostID is not blank for Specific Identification items
        '   - LotSerMst.InvtID is not blank
        '   - LotSerMst.SiteID is not blank
        '   - LotSerMst.WhseLoc is not blank
        '   - LotSerMst.LotSerNbr is not blank
        '   - Specific Cost IDs with a Qty > 1
        '   - Verify ItemSite/ItemCost/Location/LotSerMst cost and quantities are all in balance.
        '====================================================================================
        Dim invtID20PlusExists As Boolean = False
        Dim locQtyOnHand As Boolean = False
        Dim msgText As String = String.Empty
        Dim nbrInvtIDBlank As Integer
        Dim nbrSiteIDBlank As Integer
        Dim nbrWhseLocBlank As Integer
        Dim nbrInvtIDInvalid As Integer
        Dim nbrCustIDInvalid As Integer
        Dim nbrVendIDInvalid As Integer
        Dim performQtyCostValidations As Boolean = True
        Dim sqlString As String = String.Empty
        Dim qtyItemSite As Double
        Dim qtyItemCost As Double
        Dim qtyLocation As Double
        Dim qtyLotSerMst As Double
        Dim costItemSite As Double
        Dim costItemCost As Double
        Dim qtyErrorFound As Boolean = False
        Dim costErrorFound As Boolean = False
        Dim writeInvVarianceMsg As Boolean = False
        Dim lotSer20PlusExists As Boolean = False
        Dim specID20PlusExists As Boolean = False

        Dim oEventLog As clsEventLog
        Dim sqlStmt As String = ""
        Dim sqlReader As SqlDataReader = Nothing

        Dim delTran As SqlTransaction = Nothing

        Dim fmtDate As String

        fmtDate = Date.Now.ToString
        fmtDate = fmtDate.Replace(":", "")
        fmtDate = fmtDate.Replace("/", "")
        fmtDate = fmtDate.Remove(fmtDate.Length - 3)
        fmtDate = fmtDate.Replace(" ", "-")
        fmtDate = fmtDate & Date.Now.Millisecond

        oEventLog = New clsEventLog
        oEventLog.FileName = "SL-IN-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")

        Call oEventLog.LogMessage(0, "Processing Inventory")

        '***************************************************************************************************************
        '*** Delete any Inventory records where InvtID field is blank since this is the only key field in this table ***
        '***************************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND RTRIM(InvtID) = ''"
        Call sqlFetch_Num(nbrInvtIDBlank, sqlStmt, SqlAppDbConn)

        If nbrInvtIDBlank > 0 Then
            Call LogMessage("Number of Inventory records found with a blank InvtID field: " + CStr(nbrInvtIDBlank), oEventLog)

            'Delete Inventory records with a blank InvtID field and log the number of Inventory records deleted
            Try
                sqlStmt = "DELETE FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND RTRIM(InvtID) = ''"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                Call LogMessage("Deleted " + CStr(nbrInvtIDBlank) + " Inventory record(s) With a blank InvtID field.", oEventLog)
                Call LogMessage("", oEventLog)
                Call LogMessage("", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered While deleting Inventory record(s) With a blank InvtID field", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                Exit Sub
            End Try
        End If


        '****************************************************************************
        '*** Check for Inventory records with an InvtID longer than 20 characters ***
        '****************************************************************************
        sqlStmt = "SELECT ClassId, InvtId, LotSerTrack, SerAssign, StkUnit, Supplr1, ValMthd FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND LEN(InvtID) > 20"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            msgText = "ERROR: The following items have an Inventory ID longer than 20 characters. Any Inventory ID longer than 20 characters will need to be updated to be 20 characters or less before data can be migrated."
            Call LogMessage(msgText, oEventLog)

            invtID20PlusExists = True
        End If

        While sqlReader.Read()
            Call SetInventoryValues(sqlReader, bInventoryInfo)
            'Write InvtID to event log
            Call LogMessage("Item: " + bInventoryInfo.InvtID, oEventLog)
            NbrOfErrors_Inv = NbrOfErrors_Inv + 1

        End While
        Call sqlReader.Close()

        'Display message in Event Log for suggested actions
        If invtID20PlusExists = True Then
            Call LogMessage("", oEventLog)
            msgText = "Suggested actions for updating Inventory IDs are listed below:" + vbNewLine
            msgText = msgText + " - Use the Professional Services Tools Library (PSTL) application to modify the Inventory IDs to 20 characters or less" + vbNewLine
            msgText = msgText + " - Set the Transaction Status on these items to Inactive or Delete in Inventory Items (10.250.00) to exclude these items from the migration" + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(msgText, oEventLog)
        End If


        '****************************************************************************
        '*** Check for Inventory records with an Item description longer than 50 characters ***
        '****************************************************************************
        sqlStmt = "SELECT ClassId, InvtId, LotSerTrack, SerAssign, StkUnit, Supplr1, ValMthd FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND LEN(RTrim(Descr)) > 50"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)

            msgText = "ERROR: D365 BC Item description can only be 50 characters for migration.  The following items have descriptions longer than 50 characters."
            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()
            Call SetInventoryValues(sqlReader, bInventoryInfo)
            'Write InvtID to event log
            Call LogMessage("Item: " + bInventoryInfo.InvtID, oEventLog)
            NbrOfErrors_Inv = NbrOfErrors_Inv + 1

        End While
        Call sqlReader.Close()

        '****************************************************************************
        '*** Check for Inventory records with an invalid Stocking Unit of Measure ***
        '****************************************************************************
        sqlStmt = "SELECT ClassId, InvtId, LotSerTrack, SerAssign, StkUnit, Supplr1, ValMthd FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND StkUnit NOT IN (SELECT ToUnit FROM INUnit)"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: The following items have an invalid Stocking Unit of Measure. A valid Stocking Unit of Measure must be assigned to these items prior to migrating data."
            Call LogMessage(msgText, oEventLog)
        End If

        While sqlReader.Read()

            Call SetInventoryValues(sqlReader, bInventoryInfo)
            'Write InvtID and StkUnit to event log
            Call LogMessage("Item: " + bInventoryInfo.InvtID + vbTab + "Invalid Stocking UOM: " + bInventoryInfo.StkUnit, oEventLog)
            NbrOfErrors_Inv = NbrOfErrors_Inv + 1

        End While
        Call sqlReader.Close()


        '********************************************************************
        '*** Check for Inventory records with an invalid Product Class ID ***
        '********************************************************************
        sqlStmt = "SELECT ClassId, InvtId, LotSerTrack, SerAssign, StkUnit, Supplr1, ValMthd FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND ClassID NOT IN (SELECT ClassID FROM ProductClass)"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: The following items have an invalid Product Class ID. A valid Product Class ID must be assigned to these items prior to migrating data."
            Call LogMessage(msgText, oEventLog)
        End If

        While sqlReader.Read()
            Call SetInventoryValues(sqlReader, bInventoryInfo)
            'Write InvtID and ClassID to event log
            Call LogMessage("Item: " + bInventoryInfo.InvtID + vbTab + "Invalid Product Class ID: " + bInventoryInfo.ClassId, oEventLog)
            NbrOfErrors_Inv = NbrOfErrors_Inv + 1

        End While
        Call sqlReader.Close()


        '********************************************************************
        '*** Check for Inventory records with an invalid Valuation Method ***
        '********************************************************************
        sqlStmt = "SELECT ClassId, InvtId, LotSerTrack, SerAssign, StkUnit, Supplr1, ValMthd FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND ValMthd NOT IN ('F', 'L', 'A', 'S', 'T', 'U')"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: The following items have an invalid Valuation Method. A valid Valuation Method must be assigned to these items prior to migrating data."
            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()
            Call SetInventoryValues(sqlReader, bInventoryInfo)
            'Write InvtID to event log
            Call LogMessage("Item: " + bInventoryInfo.InvtID, oEventLog)
            NbrOfErrors_Inv = NbrOfErrors_Inv + 1

        End While
        Call sqlReader.Close()


        '******************************************************************
        '*** Check for Inventory records with an invalid Primary Vendor ***
        '******************************************************************
        sqlStmt = "SELECT ClassId, InvtId, LotSerTrack, SerAssign, StkUnit, Supplr1, ValMthd FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND RTRIM(Supplr1) <> '' AND Supplr1 NOT IN (SELECT VendId FROM Vendor)"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: The following items have an invalid Primary Vendor. The current Primary Vendor must be removed or a valid Primary Vendor must be assigned to these items prior to migrating data."
            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()
            Call SetInventoryValues(sqlReader, bInventoryInfo)
            'Write InvtID and VendID to event log
            Call LogMessage("Item: " + bInventoryInfo.InvtID + vbTab + "Invalid Primary Vendor: " + bInventoryInfo.Supplr1, oEventLog)
            NbrOfErrors_Inv = NbrOfErrors_Inv + 1

        End While
        Call sqlReader.Close()


        '**********************************************************************************************************
        '*** Delete any Site records where SiteID field is blank since this is the only key field in this table ***
        '***********************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM Site WHERE RTRIM(SiteId) = '' AND CpnyID =" + SParm(CpnyId)
        Call sqlFetch_Num(nbrSiteIDBlank, sqlStmt, SqlAppDbConn)


        If nbrSiteIDBlank > 0 Then
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("Number of Site records found with a blank SiteID field: " + CStr(nbrSiteIDBlank), oEventLog)

            'Delete Site records with a blank SiteID field and log the number of Site records deleted
            Try
                sqlStmt = "DELETE FROM Site WHERE RTRIM(SiteId) = '' AND CpnyID =" + SParm(CpnyId)
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                Call LogMessage("Deleted " + CStr(nbrSiteIDBlank) + " Site record(s) with a blank SiteID field.", oEventLog)

            Catch ex As Exception
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting Site record(s) with a blank SiteID field", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                Exit Sub
            End Try
        End If


        '*****************************************************************************************************************
        '*** Delete any LocTable records where SiteID field is blank since this is one of the key field in this table ***
        '*****************************************************************************************************************
        nbrSiteIDBlank = 0

        sqlStmt = "SELECT COUNT(*) FROM LocTable WHERE RTRIM(SiteID) = ''"
        Call sqlFetch_Num(nbrSiteIDBlank, sqlStmt, SqlAppDbConn)


        If nbrSiteIDBlank > 0 Then
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("Number of LocTable records found with a blank SiteID field: " + CStr(nbrSiteIDBlank), oEventLog)

            'Delete LocTable records with a blank SiteID field and log the number of LocTable records deleted
            Try
                sqlStmt = "DELETE FROM LocTable WHERE RTRIM(SiteID) = ''"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                Call LogMessage("Deleted " + CStr(nbrSiteIDBlank) + " LocTable record(s) with a blank SiteID field.", oEventLog)


            Catch ex As Exception
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting LocTable record(s) with a blank SiteID field", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                Call LogMessage("Error Encountered: " + ex.Message.Trim, oEventLog)
                Exit Sub
            End Try
        End If


        '*****************************************************************************************************************
        '*** Delete any LocTable records where WhseLoc field is blank since this is one of the key field in this table ***
        '*****************************************************************************************************************
        sqlStmt = "SELECT COUNT(*) FROM LocTable JOIN Site ON Site.SiteId = LocTable.SiteID WHERE RTRIM(LocTable.WhseLoc) = '' AND Site.CpnyID =" + SParm(CpnyId)
        Call sqlFetch_Num(nbrWhseLocBlank, sqlStmt, SqlAppDbConn)


        If nbrWhseLocBlank > 0 Then
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("Number of LocTable records found with a blank WhseLoc field: " + CStr(nbrWhseLocBlank), oEventLog)

            'Delete LocTable records with a blank WhseLoc field and log the number of LocTable records deleted
            Try
                sqlStmt = "DELETE FROM LocTable WHERE RTRIM(WhseLoc) = '' AND SiteID IN (SELECT SiteID FROM Site WHERE CpnyID =" + SParm(CpnyId) + ")"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                If nbrWhseLocBlank = 1 Then
                    Call LogMessage("Deleted " + CStr(nbrWhseLocBlank) + " LocTable record with a blank WhseLoc field.", oEventLog)

                Else
                    Call LogMessage("Deleted " + CStr(nbrWhseLocBlank) + " LocTable records with a blank WhseLoc field.", oEventLog)

                End If

            Catch ex As Exception
                '  Call MessBox(ex.Message.Trim + vbNewLine + ex.StackTrace, MB_OK + MB_ICONEXCLAMATION, "Error Encountered")
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting LocTable record(s) with a blank WhseLoc field", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                Call LogMessage("Error Encountered: " + ex.Message.Trim, oEventLog)
                Exit Sub
            End Try
        End If


        '**********************************************************
        '*** Delete any ItemXRef records with an invalid InvtID ***
        '**********************************************************
        sqlStmt = "SELECT COUNT(*) FROM ItemXRef WHERE InvtID NOT IN (SELECT InvtID FROM Inventory)"

        Call sqlFetch_Num(nbrInvtIDInvalid, sqlStmt, SqlAppDbConn)
        If nbrInvtIDInvalid > 0 Then
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("Number of ItemXRef records found with an invalid InvtID: " + CStr(nbrInvtIDInvalid), oEventLog)

            'Delete ItemXRef records with an invalid InvtID and log the number of ItemXRef records deleted
            Try
                sqlStmt = "DELETE FROM ItemXRef WHERE InvtID NOT IN (SELECT InvtID FROM Inventory)"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)


                If nbrInvtIDInvalid = 1 Then
                    Call LogMessage("Deleted " + CStr(nbrInvtIDInvalid) + " ItemXRef record with an invalid InvtID.", oEventLog)

                Else
                    Call LogMessage("Deleted " + CStr(nbrInvtIDInvalid) + " ItemXRef records with an invalid InvtID.", oEventLog)

                End If

            Catch ex As Exception
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting ItemXRef record(s) with an invalid InvtID", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                Exit Sub
            End Try
        End If

        '**********************************************************
        '*** Delete any ItemXRef records with an invalid CustID ***
        '**********************************************************
        sqlStmt = "SELECT COUNT(*) FROM ItemXRef WHERE AltIDType = 'C' AND EntityID NOT IN (SELECT CustID FROM Customer)"
        Call sqlFetch_Num(nbrCustIDInvalid, sqlStmt, SqlAppDbConn)


        If nbrCustIDInvalid > 0 Then
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("Number of ItemXRef records found with an invalid CustID: " + CStr(nbrCustIDInvalid), oEventLog)

            'Delete ItemXRef records with an invalid CustID and log the number of ItemXRef records deleted
            Try
                sqlStmt = "DELETE FROM ItemXRef WHERE AltIDType = 'C' AND EntityID NOT IN (SELECT CustID FROM Customer)"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                If nbrCustIDInvalid = 1 Then
                    Call LogMessage("Deleted " + CStr(nbrCustIDInvalid) + " ItemXRef record with an invalid CustID.", oEventLog)

                Else
                    Call LogMessage("Deleted " + CStr(nbrCustIDInvalid) + " ItemXRef records with an invalid CustID.", oEventLog)

                End If

            Catch ex As Exception
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting ItemXRef record(s) with an invalid CustID", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                Exit Sub
            End Try
        End If


        '**********************************************************
        '*** Delete any ItemXRef records with an invalid VendID ***
        '**********************************************************
        sqlStmt = "SELECT COUNT(*) FROM ItemXRef WHERE AltIDType = 'V' AND EntityID NOT IN (SELECT VendID FROM Vendor)"
        Call sqlFetch_Num(nbrVendIDInvalid, sqlStmt, SqlAppDbConn)

        If nbrVendIDInvalid > 0 Then
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            Call LogMessage("Number of ItemXRef records found with an invalid VendID: " + CStr(nbrVendIDInvalid), oEventLog)

            'Delete ItemXRef records with an invalid VendID and log the number of ItemXRef records deleted
            Try
                sqlStmt = "DELETE FROM ItemXRef WHERE AltIDType = 'V' AND EntityID NOT IN (SELECT VendID FROM Vendor)"
                Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)


                If nbrVendIDInvalid = 1 Then
                    Call LogMessage("Deleted " + CStr(nbrVendIDInvalid) + " ItemXRef record with an invalid VendID.", oEventLog)

                Else
                    Call LogMessage("Deleted " + CStr(nbrVendIDInvalid) + " ItemXRef records with an invalid VendID.", oEventLog)

                End If

            Catch ex As Exception
                Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                Call LogMessage("", oEventLog)
                Call LogMessage("Error encountered while deleting ItemXRef record(s) with an invalid VendID", oEventLog)
                Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                Call LogMessage("", oEventLog)
                OkToContinue = False
                NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                Exit Sub
            End Try
        End If

        If performQtyCostValidations = True Then

            Dim connStr As String = ""
            Dim SqlTranConn As SqlConnection = Nothing
            Dim SqlTranReader As SqlDataReader = Nothing

            Dim SqlSumConn As SqlConnection = Nothing
            Dim SqlSumReader As SqlDataReader = Nothing


            '********************************************************
            '*** Check for ItemSite records where InvtID is blank ***
            '********************************************************
            sqlString = "SELECT InvtId, QtyOnHand, SiteId, TotCost FROM ItemSite WHERE RTRIM(ItemSite.InvtID) = ''"
            Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
            If sqlReader.HasRows() Then
                Call LogMessage("Itemsite records Exist with blank Inventory ID", oEventLog)
                Call LogMessage("", oEventLog)

                SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                SqlTranConn.Open()

            End If

            While sqlReader.Read()
                Call SetItemSiteValues(sqlReader, bItemSiteInfo)
                'Delete invalid ItemSite record
                Call LogMessage("Deleted invalid ItemSite record for InvtID:" + SParm(bItemSiteInfo.InvtID.Trim) + "and SiteID:" + SParm(bItemSiteInfo.SiteID.Trim), oEventLog)

                sqlStmt = "DELETE FROM ItemSite WHERE InvtID =" + SParm(bItemSiteInfo.InvtID) + "AND SiteID =" + SParm(bItemSiteInfo.SiteID)
                Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

            End While
            Call sqlReader.Close()
            If SqlTranReader IsNot Nothing Then
                Call SqlTranReader.Close()
                SqlTranReader = Nothing
            End If
            If SqlTranConn IsNot Nothing Then
                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If


            '********************************************************
            '*** Check for ItemSite records where SiteID is blank ***
            '********************************************************
            sqlString = "SELECT InvtId, QtyOnHand, SiteId, TotCost FROM ItemSite WHERE RTRIM(ItemSite.SiteID) = ''"

                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)

                If sqlReader.HasRows() Then
                Call LogMessage("ItemSite records exist with blank SiteID", oEventLog)
                Call LogMessage("", oEventLog)


                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

                While sqlReader.Read()


                    Call SetItemSiteValues(sqlReader, bItemSiteInfo)

                    'Delete invalid ItemSite record
                    Call LogMessage("Deleted invalid ItemSite record for InvtID:" + SParm(bItemSiteInfo.InvtID.Trim) + "and SiteID:" + SParm(bItemSiteInfo.SiteID.Trim), oEventLog)

                    sqlStmt = "DELETE FROM ItemSite WHERE InvtID =" + SParm(bItemSiteInfo.InvtID) + "AND SiteID =" + SParm(bItemSiteInfo.SiteID)
                    Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)


                End While
                '
                Call sqlReader.Close()
            If SqlTranReader IsNot Nothing Then
                Call SqlTranReader.Close()
                SqlTranReader = Nothing
            End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If


            '********************************************************
            '*** Check for Location records where InvtID is blank ***
            '********************************************************
            sqlString = "SELECT InvtId, SiteId, WhseLoc FROM Location WHERE RTRIM(Location.InvtID) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)

                If sqlReader.HasRows() Then
                Call LogMessage("Location records exist with blank Inventory ID", oEventLog)
                Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

            While sqlReader.Read()
                Call SetLocationValues(sqlReader, bLocationInfo)
                'Delete invalid Location record
                Call LogMessage("Deleted invalid Location record for InvtID:" + SParm(bLocationInfo.InvtID.Trim) + "and SiteID:" + SParm(bLocationInfo.SiteID.Trim) + "and WhseLoc:" + SParm(bLocationInfo.WhseLoc.Trim), oEventLog)


                sqlStmt = "DELETE FROM Location WHERE InvtID =" + SParm(bLocationInfo.InvtID) + "AND SiteID =" + SParm(bLocationInfo.SiteID) + "AND WhseLoc =" + SParm(bLocationInfo.WhseLoc)

                Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

            End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If
            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If



            '********************************************************
            '*** Check for Location records where SiteID is blank ***
            '********************************************************
            sqlString = "SELECT InvtId, SiteId, WhseLoc FROM Location WHERE RTRIM(Location.SiteID) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows Then
                Call LogMessage("Location records exist with blank SiteID", oEventLog)
                Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

                While sqlReader.Read()
                    Call SetLocationValues(sqlReader, bLocationInfo)
                    'Delete invalid Location record
                    Call LogMessage("Deleted invalid Location record for InvtID:" + SParm(bLocationInfo.InvtID.Trim) + "and SiteID:" + SParm(bLocationInfo.SiteID.Trim) + "and WhseLoc:" + SParm(bLocationInfo.WhseLoc.Trim), oEventLog)

                    sqlStmt = "DELETE FROM Location WHERE InvtID =" + SParm(bLocationInfo.InvtID) + "AND SiteID =" + SParm(bLocationInfo.SiteID) + "AND WhseLoc =" + SParm(bLocationInfo.WhseLoc)
                    Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

                End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If


            '*********************************************************
            '*** Check for Location records where WhseLoc is blank ***
            '*********************************************************
            sqlString = "SELECT InvtId, SiteId, WhseLoc FROM Location WHERE RTRIM(Location.WhseLoc) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows = True Then
                Call LogMessage("Location records exist with blank WhseLoc", oEventLog)
                Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

                While sqlReader.Read()
                    Call SetLocationValues(sqlReader, bLocationInfo)

                    'Delete invalid Location record
                    Call LogMessage("Deleted invalid Location record for InvtID:" + SParm(bLocationInfo.InvtID.Trim) + "and SiteID:" + SParm(bLocationInfo.SiteID.Trim) + "and WhseLoc:" + SParm(bLocationInfo.WhseLoc.Trim), oEventLog)

                    sqlStmt = "DELETE FROM Location WHERE InvtID =" + SParm(bLocationInfo.InvtID) + "AND SiteID =" + SParm(bLocationInfo.SiteID) + "AND WhseLoc =" + SParm(bLocationInfo.WhseLoc)
                    Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)
                End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If
            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If


            '********************************************************
            '*** Check for ItemCost records where InvtID is blank ***
            '********************************************************

            sqlString = "SELECT InvtId, LayerType, RcptDate, RcptNbr, SpecificCostId, SiteId FROM ItemCost WHERE RTRIM(ItemCost.InvtID) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

                While sqlReader.Read()

                    'Delete invalid ItemCost record
                    Call LogMessage("Deleted invalid ItemCost record for InvtID:" + SParm(bItemCostInfo.InvtID.Trim) + "and SiteID:" + SParm(bItemCostInfo.SiteID.Trim) + "and LayerType:" + SParm(bItemCostInfo.LayerType.Trim) + "and SpecificCostID:" + SParm(bItemCostInfo.SpecificCostID.Trim) + "and RcptNbr:" + SParm(bItemCostInfo.RcptNbr.Trim) + "and RcptDate:" + SParm(bItemCostInfo.RcptDate), oEventLog)

                    sqlStmt = sqlStmt = "DELETE FROM ItemCost WHERE InvtID =" + SParm(bItemCostInfo.InvtID) + "AND SiteID =" + SParm(bItemCostInfo.SiteID) + "AND LayerType =" + SParm(bItemCostInfo.LayerType) + "AND SpecificCostID =" + SParm(bItemCostInfo.SpecificCostID) + "AND RcptNbr =" + SParm(bItemCostInfo.RcptNbr) + "AND RcptDate =" + bItemCostInfo.RcptDate
                    Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)


                End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If

            '********************************************************
            '*** Check for ItemCost records where SiteID is blank ***
            '********************************************************
            sqlString = "SELECT InvtId, LayerType, RcptDate, RcptNbr, SpecificCostId, SiteId FROM ItemCost WHERE RTRIM(ItemCost.SiteID) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows() Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

                While sqlReader.Read()
                    Call SetItemCostValues(sqlReader, bItemCostInfo)
                    'Delete invalid ItemCost record
                    Call LogMessage("Deleted invalid ItemCost record for InvtID:" + SParm(bItemCostInfo.InvtID.Trim) + "and SiteID:" + SParm(bItemCostInfo.SiteID.Trim) + "and LayerType:" + SParm(bItemCostInfo.LayerType.Trim) + "and SpecificCostID:" + SParm(bItemCostInfo.SpecificCostID.Trim) + "and RcptNbr:" + SParm(bItemCostInfo.RcptNbr.Trim) + "and RcptDate:" + SParm(bItemCostInfo.RcptDate), oEventLog)

                    sqlStmt = "DELETE FROM ItemCost WHERE InvtID =" + SParm(bItemCostInfo.InvtID) + "AND SiteID =" + SParm(bItemCostInfo.SiteID) + "AND LayerType =" + SParm(bItemCostInfo.LayerType) + "AND SpecificCostID =" + SParm(bItemCostInfo.SpecificCostID) + "AND RcptNbr =" + SParm(bItemCostInfo.RcptNbr) + "AND RcptDate =" + bItemCostInfo.RcptDate
                    Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)


                End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If
            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If

            '***********************************************************
            '*** Check for ItemCost records where LayerType is blank ***
            '***********************************************************
            sqlString = "SELECT InvtId, LayerType, RcptDate, RcptNbr, SpecificCostId, SiteId FROM ItemCost WHERE RTRIM(ItemCost.LayerType) = ''"

                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows() Then

                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

                While sqlReader.Read()

                    Call SetItemCostValues(sqlReader, bItemCostInfo)
                    'Delete invalid ItemCost record
                    Call LogMessage("Deleted invalid ItemCost record for InvtID:" + SParm(bItemCostInfo.InvtID.Trim) + "and SiteID:" + SParm(bItemCostInfo.SiteID.Trim) + "and LayerType:" + SParm(bItemCostInfo.LayerType.Trim) + "and SpecificCostID:" + SParm(bItemCostInfo.SpecificCostID.Trim) + "and RcptNbr:" + SParm(bItemCostInfo.RcptNbr.Trim) + "and RcptDate:" + SParm(bItemCostInfo.RcptDate), oEventLog)

                    sqlStmt = "DELETE FROM ItemCost WHERE InvtID =" + SParm(bItemCostInfo.InvtID) + "AND SiteID =" + SParm(bItemCostInfo.SiteID) + "AND LayerType =" + SParm(bItemCostInfo.LayerType) + "AND SpecificCostID =" + SParm(bItemCostInfo.SpecificCostID) + "AND RcptNbr =" + SParm(bItemCostInfo.RcptNbr) + "AND RcptDate =" + bItemCostInfo.RcptDate
                    Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

                End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If



            '************************************************************************************
            '*** Check for ItemCost records where RcptNbr is blank for FIFO/LIFO valued items ***
            '************************************************************************************
            sqlString = "SELECT c.InvtId, c.LayerType, c.RcptDate, c.RcptNbr, c.SpecificCostId, c.SiteId  FROM ItemCost c JOIN Inventory v ON v.InvtID = c.InvtID WHERE v.ValMthd IN ('F', 'L') AND RTRIM(c.RcptNbr) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)


                If sqlReader.HasRows() Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()


                End If

                While sqlReader.Read()
                    Call SetItemCostValues(sqlReader, bItemCostInfo)
                    'Delete invalid ItemCost record
                    Call LogMessage("Deleted invalid ItemCost record for InvtID:" + SParm(bItemCostInfo.InvtID.Trim) + "and SiteID:" + SParm(bItemCostInfo.SiteID.Trim) + "and LayerType:" + SParm(bItemCostInfo.LayerType.Trim) + "and SpecificCostID:" + SParm(bItemCostInfo.SpecificCostID.Trim) + "and RcptNbr:" + SParm(bItemCostInfo.RcptNbr.Trim) + "and RcptDate:" + SParm(bItemCostInfo.RcptDate), oEventLog)

                    sqlStmt = "DELETE FROM ItemCost WHERE InvtID =" + SParm(bItemCostInfo.InvtID) + "AND SiteID =" + SParm(bItemCostInfo.SiteID) + "AND LayerType =" + SParm(bItemCostInfo.LayerType) + "AND SpecificCostID =" + SParm(bItemCostInfo.SpecificCostID) + "AND RcptNbr =" + SParm(bItemCostInfo.RcptNbr) + "AND RcptDate =" + bItemCostInfo.RcptDate
                    Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)

                End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If


            '*********************************************************************************************************
            '*** Check for ItemCost records where SpecificCostID is blank for Specific Identification valued items ***
            '*********************************************************************************************************
            sqlString = "SELECT c.InvtId, c.LayerType, c.RcptDate, c.RcptNbr, c.SpecificCostId, c.SiteId FROM ItemCost c JOIN Inventory v ON v.InvtID = c.InvtID WHERE v.ValMthd = 'S' AND RTRIM(c.SpecificCostID) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)

                If sqlReader.HasRows() Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()

                End If

                While sqlReader.Read()

                    Call SetItemCostValues(sqlReader, bItemCostInfo)
                    'Delete invalid ItemCost record
                    Call LogMessage("Deleted invalid ItemCost record for InvtID:" + SParm(bItemCostInfo.InvtID.Trim) + "and SiteID:" + SParm(bItemCostInfo.SiteID.Trim) + "and LayerType:" + SParm(bItemCostInfo.LayerType.Trim) + "and SpecificCostID:" + SParm(bItemCostInfo.SpecificCostID.Trim) + "and RcptNbr:" + SParm(bItemCostInfo.RcptNbr.Trim) + "and RcptDate:" + SParm(bItemCostInfo.RcptDate), oEventLog)

                    sqlStmt = "DELETE FROM ItemCost WHERE InvtID =" + SParm(bItemCostInfo.InvtID) + "AND SiteID =" + SParm(bItemCostInfo.SiteID) + "AND LayerType =" + SParm(bItemCostInfo.LayerType) + "AND SpecificCostID =" + SParm(bItemCostInfo.SpecificCostID) + "AND RcptNbr =" + SParm(bItemCostInfo.RcptNbr) + "AND RcptDate =" + bItemCostInfo.RcptDate
                    Call sql_1(SqlTranReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)


                End While

                Call sqlReader.Close()
            If SqlTranReader IsNot Nothing Then
                Call SqlTranReader.Close()
                SqlTranReader = Nothing
            End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If


            '*********************************************************
            '*** Check for LotSerMst records where InvtID is blank ***
            '*********************************************************
            sqlString = "SELECT InvtId, LotSerNbr, SiteId, WhseLoc FROM LotSerMst WHERE RTRIM(LotSerMst.InvtID) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows() Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()


                End If

                While sqlReader.Read()
                    Call SetLotSerMstValues(sqlReader, bLotSerMstInfo)
                    'Delete invalid LotSerMst record
                    Call LogMessage("Deleted invalid LotSerMst record for InvtID:" + SParm(bLotSerMstInfo.InvtID.Trim) + "and LotSerNbr:" + SParm(bLotSerMstInfo.LotSerNbr.Trim) + "and SiteID:" + SParm(bLotSerMstInfo.SiteID.Trim) + "and WhseLoc:" + SParm(bLotSerMstInfo.WhseLoc.Trim), oEventLog)

                    sqlStmt = "DELETE FROM LotSerMst WHERE InvtID =" + SParm(bLotSerMstInfo.InvtID) + "AND LotSerNbr =" + SParm(bLotSerMstInfo.LotSerNbr) + "AND SiteID =" + SParm(bLotSerMstInfo.SiteID) + "AND WhseLoc =" + SParm(bLotSerMstInfo.WhseLoc)
                Call sql_1(sqlReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

            End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If

            '************************************************************
            '*** Check for LotSerMst records where LotSerNbr is blank ***
            '************************************************************
            sqlString = "SELECT InvtId, LotSerNbr, SiteId, WhseLoc FROM LotSerMst WHERE RTRIM(LotSerMst.LotSerNbr) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows() Then

                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()


                End If

                While sqlReader.Read()
                    Call SetLotSerMstValues(sqlReader, bLotSerMstInfo)
                    'Delete invalid LotSerMst record
                    Call LogMessage("Deleted invalid LotSerMst record for InvtID:" + SParm(bLotSerMstInfo.InvtID.Trim) + "and LotSerNbr:" + SParm(bLotSerMstInfo.LotSerNbr.Trim) + "and SiteID:" + SParm(bLotSerMstInfo.SiteID.Trim) + "and WhseLoc:" + SParm(bLotSerMstInfo.WhseLoc.Trim), oEventLog)



                sqlStmt = "DELETE FROM LotSerMst WHERE InvtID =" + SParm(bLotSerMstInfo.InvtID) + "AND LotSerNbr =" + SParm(bLotSerMstInfo.LotSerNbr) + "AND SiteID =" + SParm(bLotSerMstInfo.SiteID) + "AND WhseLoc =" + SParm(bLotSerMstInfo.WhseLoc)
                Call sql_1(sqlReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

            End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If

            '*********************************************************
            '*** Check for LotSerMst records where SiteID is blank ***
            '*********************************************************
            sqlString = "SELECT InvtId, LotSerNbr, SiteId, WhseLoc FROM LotSerMst WHERE RTRIM(LotSerMst.SiteID) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows() Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()


                End If

                While sqlReader.Read()
                    Call SetLotSerMstValues(sqlReader, bLotSerMstInfo)
                    'Delete invalid LotSerMst record
                    Call LogMessage("Deleted invalid LotSerMst record for InvtID:" + SParm(bLotSerMstInfo.InvtID.Trim) + "and LotSerNbr:" + SParm(bLotSerMstInfo.LotSerNbr.Trim) + "and SiteID:" + SParm(bLotSerMstInfo.SiteID.Trim) + "and WhseLoc:" + SParm(bLotSerMstInfo.WhseLoc.Trim), oEventLog)

                    sqlStmt = "DELETE FROM LotSerMst WHERE InvtID =" + SParm(bLotSerMstInfo.InvtID) + "AND LotSerNbr =" + SParm(bLotSerMstInfo.LotSerNbr) + "AND SiteID =" + SParm(bLotSerMstInfo.SiteID) + "AND WhseLoc =" + SParm(bLotSerMstInfo.WhseLoc)
                Call sql_1(sqlReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

            End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If

            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If

            '**********************************************************
            '*** Check for LotSerMst records where WhseLoc is blank ***
            '**********************************************************
            sqlString = "SELECT InvtId, LotSerNbr, SiteId, WhseLoc FROM LotSerMst WHERE RTRIM(LotSerMst.WhseLoc) = ''"
                Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)
                If sqlReader.HasRows() Then
                    Call LogMessage("", oEventLog)
                    Call LogMessage("", oEventLog)

                    SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                    SqlTranConn.Open()


                End If

                While sqlReader.Read()
                    Call SetLotSerMstValues(sqlReader, bLotSerMstInfo)
                    'Delete invalid LotSerMst record
                    Call LogMessage("Deleted invalid LotSerMst record for InvtID:" + SParm(bLotSerMstInfo.InvtID.Trim) + "and LotSerNbr:" + SParm(bLotSerMstInfo.LotSerNbr.Trim) + "and SiteID:" + SParm(bLotSerMstInfo.SiteID.Trim) + "and WhseLoc:" + SParm(bLotSerMstInfo.WhseLoc.Trim), oEventLog)

                    sqlStmt = "DELETE FROM LotSerMst WHERE InvtID =" + SParm(bLotSerMstInfo.InvtID) + "AND LotSerNbr =" + SParm(bLotSerMstInfo.LotSerNbr) + "AND SiteID =" + SParm(bLotSerMstInfo.SiteID) + "AND WhseLoc =" + SParm(bLotSerMstInfo.WhseLoc)
                Call sql_1(sqlReader, sqlStmt, SqlTranConn, OperationType.DeleteOp, CommandType.Text)

            End While
                Call sqlReader.Close()
                If SqlTranReader IsNot Nothing Then
                    Call SqlTranReader.Close()
                    SqlTranReader = Nothing
                End If
            If SqlTranConn IsNot Nothing Then

                If (SqlTranConn.State <> ConnectionState.Closed) Then
                    SqlTranConn.Close()
                End If
                SqlTranConn = Nothing
            End If


            '************************************************
            '*** Check for Specific Cost IDs with Qty > 1 ***
            '************************************************
            Try
                    sqlString = "SELECT ItemCost.InvtID, ItemCost.SiteID, ItemCost.SpecificCostID, ItemCost.Qty FROM ItemCost JOIN Inventory on Inventory.InvtID = ItemCost.InvtID WHERE Inventory.ValMthd = 'S' AND Inventory.LotSerTrack <> 'SI' AND ItemCost.Qty > 1"
                    Call sqlFetch_1(sqlReader, sqlString, SqlAppDbConn, CommandType.Text)

                    If sqlReader.HasRows() Then
                        'Write Warning message to event log
                        Call LogMessage("", oEventLog)
                        Call LogMessage("", oEventLog)
                        msgText = "ERROR: Specific Cost IDs exist with a quantity greater than 1."
                        Call LogMessage(msgText, oEventLog)

                        msgText = "When migrating on-hand quantities and costs for Specific Cost items, each Specific Cost ID cannot have a quantity greater than 1."
                        msgText = msgText + " Use the Inventory Adjustments (10.030.00) screen to move quantities greater than 1 to their own individual Specific Cost ID."
                        msgText = msgText + vbNewLine + "Specific Cost IDs with a quantity greater than 1:"
                        Call LogMessage(msgText, oEventLog)
                    End If

                    While sqlReader.Read()

                        Call SetSpecIDErrorListValues(sqlReader, bSpecIDErrorList)
                        'Write InvtID, SiteID, SpecificCostID, and Qty to event log
                        Call LogMessage("Inventory ID: " + bSpecIDErrorList.InvtID + vbTab + "Site ID: " + bSpecIDErrorList.SiteID + vbTab + "Specific Cost ID: " + bSpecIDErrorList.SpecificCostID + vbTab + "Qty: " + bSpecIDErrorList.Qty.ToString, oEventLog)
                        NbrOfErrors_Inv = NbrOfErrors_Inv + 1

                    End While

                    Call sqlReader.Close()


                Catch ex As Exception
                    Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                    Call LogMessage("", oEventLog)
                    Call LogMessage("Error encountered while checking for Specific Cost IDs with a quantity greater than 1.", oEventLog)

                    Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                    Call LogMessage("", oEventLog)
                    OkToContinue = False
                    NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                    Exit Sub
                End Try


                '**************************************
                '*** Validate on-hand Qty and Costs ***
                '**************************************
                Try
                    'Loop through Inventory records
                    sqlStmt = "SELECT  ClassId, InvtId, LotSerTrack, SerAssign, StkUnit, Supplr1, ValMthd  FROM Inventory WHERE TranStatusCode NOT IN ('IN', 'DE') AND StkItem = 1"
                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    ' If any rows are returned, then define a second connection for processing other records.
                    If (sqlReader.HasRows) Then

                        SqlTranConn = New SqlClient.SqlConnection(AppDbConnStr)
                        SqlTranConn.Open()

                    End If
                    While sqlReader.Read()
                        Call SetInventoryValues(sqlReader, bInventoryInfo)
                        'Initialize variables
                        qtyItemSite = 0
                        qtyItemCost = 0
                        qtyLocation = 0
                        qtyLotSerMst = 0
                        costItemSite = 0
                        costItemCost = 0


                        'Loop through ItemSite records
                        sqlString = "SELECT InvtId, QtyOnHand, SiteId, TotCost FROM ItemSite WHERE InvtID =" + SParm(bInventoryInfo.InvtID.Trim)
                        Call sqlFetch_1(SqlTranReader, sqlString, SqlTranConn, CommandType.Text)

                        ' Create a second connection for additional queries.
                        If SqlTranReader.HasRows() Then

                            SqlSumConn = New SqlClient.SqlConnection(AppDbConnStr)
                            SqlSumConn.Open()


                        End If
                        While SqlTranReader.Read()
                            Call SetItemSiteValues(SqlTranReader, bItemSiteInfo)
                            'Get ItemSite.QtyOnHand and ItemSite.TotCost
                            qtyItemSite = bItemSiteInfo.QtyOnHand
                            costItemSite = bItemSiteInfo.TotCost

                            'Look up ItemCost information
                            Select Case bInventoryInfo.ValMthd
                                Case "F", "L", "S"  'FIFO, LIFO, Specific Identification
                                    'Get ItemCost.Qty
                                    sqlStmt = "SELECT SUM(Qty) FROM ItemCost WHERE InvtID =" + SParm(bItemSiteInfo.InvtID.Trim) + "AND SiteID =" + SParm(bItemSiteInfo.SiteID.Trim)
                                    Call sqlFetch_Num(qtyItemCost, sqlStmt, SqlSumConn)


                                    'Get ItemCost.TotCost
                                    sqlStmt = "SELECT SUM(TotCost) FROM ItemCost WHERE InvtID =" + SParm(bItemSiteInfo.InvtID.Trim) + "AND SiteID =" + SParm(bItemSiteInfo.SiteID.Trim)
                                    Call sqlFetch_Num(costItemCost, sqlStmt, SqlSumConn)


                                Case "A", "T", "U"  'Average, Standard, User-Specified
                                    qtyItemCost = 0
                                    costItemCost = 0
                            End Select

                            'Get Location.QtyOnHand
                            sqlStmt = "SELECT SUM(QtyOnHand) FROM Location WHERE InvtID =" + SParm(bItemSiteInfo.InvtID.Trim) + "AND SiteID =" + SParm(bItemSiteInfo.SiteID.Trim)
                            Call sqlFetch_Num(qtyLocation, sqlStmt, SqlSumConn)



                            'Look up LotSerMst information
                            Select Case bInventoryInfo.LotSerTrack
                                Case "LI", "SI"
                                    sqlStmt = "SELECT SUM(QtyOnHand) FROM LotSerMst WHERE InvtID =" + SParm(bItemSiteInfo.InvtID.Trim) + "AND SiteID =" + SParm(bItemSiteInfo.SiteID.Trim)
                                    Call sqlFetch_Num(qtyLotSerMst, sqlStmt, SqlSumConn)

                                Case Else
                                    qtyLotSerMst = 0
                            End Select

                        End While

                        If SqlTranReader IsNot Nothing Then
                            Call SqlTranReader.Close()
                            SqlTranReader = Nothing
                        End If
                        If (SqlSumReader IsNot Nothing) Then
                            Call SqlSumReader.Close()
                            SqlSumReader = Nothing
                        End If

                    If SqlTranConn IsNot Nothing Then

                        If (SqlTranConn.State <> ConnectionState.Closed) Then
                            SqlTranConn.Close()
                        End If
                    End If
                    If (SqlSumConn.State <> ConnectionState.Closed) Then
                        SqlSumConn.Close()
                    End If

                    'Check to see if ItemSite/ItemCost/Location/LotSerMst quantities balance
                    qtyErrorFound = False
                        Select Case bInventoryInfo.ValMthd
                            Case "F", "L", "S"  'FIFO, LIFO, Specific Identification
                                If bInventoryInfo.LotSerTrack = "LI" Or bInventoryInfo.LotSerTrack = "SI" Then  'Lot or Serial tracked
                                    If bInventoryInfo.SerAssign = "R" Then  'When Received
                                        If (qtyItemSite <> qtyItemCost) Or (qtyItemSite <> qtyLocation) Or (qtyItemSite <> qtyLotSerMst) Or (qtyItemCost <> qtyLocation) Or (qtyItemCost <> qtyLotSerMst) Or (qtyLocation <> qtyLotSerMst) Then
                                            qtyErrorFound = True
                                        End If
                                    Else  'When Used
                                        If (qtyItemSite <> qtyItemCost) Or (qtyItemSite <> qtyLocation) Or (qtyItemCost <> qtyLocation) Then
                                            qtyErrorFound = True
                                        End If
                                    End If
                                Else
                                    If (qtyItemSite <> qtyItemCost) Or (qtyItemSite <> qtyLocation) Or (qtyItemCost <> qtyLocation) Then
                                        qtyErrorFound = True
                                    End If
                                End If

                            Case "A", "T", "U"  'Average, Standard, User-Specified
                                If bInventoryInfo.LotSerTrack = "LI" Or bInventoryInfo.LotSerTrack = "SI" Then  'Lot or Serial tracked
                                    If bInventoryInfo.SerAssign = "R" Then  'When Received
                                        If (qtyItemSite <> qtyLocation) Or (qtyItemSite <> qtyLotSerMst) Or (qtyLocation <> qtyLotSerMst) Then
                                            qtyErrorFound = True
                                        End If
                                    Else  'When Used
                                        If qtyItemSite <> qtyLocation Then
                                            qtyErrorFound = True
                                        End If
                                    End If
                                Else
                                    If qtyItemSite <> qtyLocation Then
                                        qtyErrorFound = True
                                    End If
                                End If
                        End Select

                        'Check to see if ItemSite/ItemCost costs balance
                        costErrorFound = False
                        Select Case bInventoryInfo.ValMthd
                            Case "F", "L", "S"  'FIFO, LIFO, Specific Identification
                                If (costItemSite <> costItemCost) Then
                                    costErrorFound = True
                                End If
                        End Select

                        'If a Qty or Cost error is found, write message to the event log
                        If qtyErrorFound = True Or costErrorFound = True Then
                            Call LogMessage("", oEventLog)
                            Call LogMessage("", oEventLog)
                            writeInvVarianceMsg = True
                            Select Case bInventoryInfo.ValMthd
                                Case "F", "L", "S"  'FIFO, LIFO, Specific Identification
                                    If bInventoryInfo.LotSerTrack = "LI" Or bInventoryInfo.LotSerTrack = "SI" Then
                                        msgText = "ERROR: Variances found in master tables for InvtID:" + SParm(bItemSiteInfo.InvtID.Trim) + "SiteID:" + SParm(bItemSiteInfo.SiteID.Trim)
                                        Call LogMessage(msgText, oEventLog)

                                        msgText = vbTab + "Quantity - ItemSite: " + qtyItemSite.ToString + ", ItemCost: " + qtyItemCost.ToString + ", Location: " + qtyLocation.ToString + ", LotSerMst: " + qtyLotSerMst.ToString
                                        Call LogMessage(msgText, oEventLog)
                                        msgText = vbTab + "Cost - ItemSite: " + costItemSite.ToString + ", ItemCost: " + costItemCost.ToString
                                        Call LogMessage(msgText, oEventLog)
                                        NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                                    Else
                                        msgText = "ERROR: Variances found in master tables for InvtID:" + SParm(bItemSiteInfo.InvtID.Trim) + "SiteID:" + SParm(bItemSiteInfo.SiteID.Trim)
                                        Call LogMessage(msgText, oEventLog)

                                        msgText = vbTab + "Quantity - ItemSite: " + qtyItemSite.ToString + ", ItemCost: " + qtyItemCost.ToString + ", Location: " + qtyLocation.ToString + ", LotSerMst: N/A"
                                        Call LogMessage(msgText, oEventLog)
                                        msgText = vbTab + "Cost - ItemSite: " + costItemSite.ToString + ", ItemCost: " + costItemCost.ToString
                                        Call LogMessage(msgText, oEventLog)
                                        NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                                    End If

                                Case "A", "T", "U"  'Average, Standard, User-Specified
                                    If bInventoryInfo.LotSerTrack = "LI" Or bInventoryInfo.LotSerTrack = "SI" Then
                                        msgText = "ERROR: Variances found in master tables for InvtID:" + SParm(bItemSiteInfo.InvtID.Trim) + "SiteID:" + SParm(bItemSiteInfo.SiteID.Trim)
                                        Call LogMessage(msgText, oEventLog)

                                        msgText = vbTab + "Quantity - ItemSite: " + qtyItemSite.ToString + ", ItemCost: N/A, Location: " + qtyLocation.ToString + ", LotSerMst: " + qtyLotSerMst.ToString
                                        Call LogMessage(msgText, oEventLog)

                                        NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                                    Else
                                        msgText = "ERROR: Variances found in master tables for InvtID:" + SParm(bItemSiteInfo.InvtID.Trim) + "SiteID:" + SParm(bItemSiteInfo.SiteID.Trim)
                                        Call LogMessage(msgText, oEventLog)

                                        msgText = vbTab + "Quantity - ItemSite: " + qtyItemSite.ToString + ", ItemCost: N/A, Location: " + qtyLocation.ToString + ", LotSerMst: N/A"
                                        Call LogMessage(msgText, oEventLog)                                    'msgText = vbTab + "Cost - ItemSite: " + costItemSite.ToString + ", ItemCost: N/A"
                                        NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                                    End If

                            End Select
                        End If

                    End While
                    Call sqlReader.Close()

                    If writeInvVarianceMsg = True Then
                        'Write suggested action for correcting Inventory master table variances
                        Call LogMessage("", oEventLog)

                        msgText = "Contact your Microsoft Dynamics SL Partner for assistance with correcting Inventory master table variances."
                        Call LogMessage(msgText, oEventLog)

                        Call LogMessage("", oEventLog)

                    End If


                Catch ex As Exception
                    Call MessageBox.Show(ex.Message.Trim + vbNewLine + ex.StackTrace, "Error Encountered", MessageBoxButtons.OK)

                    Call LogMessage("", oEventLog)
                    Call LogMessage("Error encountered while validating on-hand quantities and costs.", oEventLog)
                    Call LogMessage("Error Detail: " + ex.Message.Trim + vbNewLine + ex.StackTrace, oEventLog)
                    Call LogMessage("", oEventLog)
                    OkToContinue = False
                    NbrOfErrors_Inv = NbrOfErrors_Inv + 1
                    Exit Sub
                End Try

            End If  'performQtyCostValidations = True

            Call oEventLog.LogMessage(EndProcess, "Validate Inventory")


        Call MessageBox.Show("Inventory Validation Complete", "Inventory Validation")

        ' Display the event log just created.
        Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.InvEventLogName = oEventLog.LogFile.FullName
        End If


    End Sub



End Module
