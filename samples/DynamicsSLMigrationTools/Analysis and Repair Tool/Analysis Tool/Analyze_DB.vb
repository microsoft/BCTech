Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Imports System.IO

Module Analyze_DB

    '================================================================================
    ' This module contains code to analyze Database objects
    '================================================================================

    Public Sub Analyze_DB()

        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "DB"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing

        Try

            '=== Objects ===

            Form1.UpdateAnalysisToolStatusBar("Analyzing Custom Objects")

            Call oEventLog.LogMessage(0, "CUSTOM OBJECTS")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Custom Objects")

            '============
            ' TABLES
            '============
            If SLTableList.Trim() <> "" Then

                sAnalysisType = "Tables"

                Form1.UpdateAnalysisToolStatusBar("Analyzing Custom Objects - Tables")

                RecID = RecID + 1

                ' Get count of custom tables - application DB
                'sDescr = "Number of custom tables in application DB (" + AppDBName.Trim + "):"
                sDescr = "Number of custom sysobjects tables: "
                sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE XType = 'U' AND Name NOT IN " + SLTableList
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                ' List of custom tables - application DB
                If retValInt1 > 0 Then

                    sqlStmt = "SELECT name, crdate FROM sysobjects WHERE XType = 'U' AND Name NOT IN " + SLTableList + " ORDER BY Name"

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    If (sqlReader.HasRows()) Then

                        While sqlReader.Read()

                            RecID = RecID + 1

                            sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Date Created: " + sqlReader(1).ToShortDateString().Trim(), 100)
                            sResult = ""
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        End While

                    End If

                End If
                Call sqlReader.Close()

            End If

            'Call oEventLog.LogMessage(0, "")
            'Call oEventLog.LogMessage(0, "")

            '============
            ' TRIGGERS
            '============

            '=== Triggers ===

            Form1.UpdateAnalysisToolStatusBar("Analyzing Custom Objects - Triggers")

            '  Call oEventLog.LogMessage(0, "TRIGGERS")

            Call oEventLog.LogMessage(0, "")
            ' Call oEventLog.LogMessage(0, "Analyzing Triggers")

            sAnalysisType = "Triggers"

            RecID = RecID + 1

            ' Get count of standard triggers - system DB
            sDescr = "Number of standard Triggers in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE type = 'TR' AND NAME IN ('trg_MC_Company_Upd_GLSetupSync', 'trg_MC_InterCompany_Insert_Delete_Acct_Sub')"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Get list of standard triggers - system DB
            If retValInt1 > 0 Then

                sqlStmt = "SELECT o.Name [TriggerName], object_name(parent_obj) [TableName], crdate, CASE WHEN is_disabled = 0 THEN 'Enabled' ELSE 'Disabled' END AS Status"
                sqlStmt += " FROM sysobjects o JOIN sys.triggers t ON t.object_id = o.id"
                sqlStmt += " WHERE o.type = 'TR' AND o.NAME IN ('trg_MC_Company_Upd_GLSetupSync', 'trg_MC_InterCompany_Insert_Delete_Acct_Sub') ORDER BY o.Name"


                Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()

                        RecID = RecID + 1

                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Date Created: " + sqlReader(2).ToShortDateString().Trim() + " | Status: " + sqlReader(3).trim, 100)
                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)
                        sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)

                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If
            Call sqlReader.Close()

            RecID = RecID + 1
            Call oEventLog.LogMessage(0, "")

            ' Get count of custom triggers - system DB
            sDescr = "Number of custom Triggers in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE type = 'TR' AND NAME NOT IN ('trg_MC_Company_Upd_GLSetupSync', 'trg_MC_InterCompany_Insert_Delete_Acct_Sub')"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Get list of custom triggers - system DB
            If retValInt1 > 0 Then

                sqlStmt = "SELECT o.Name [TriggerName], object_name(parent_obj) [TableName], crdate, CASE WHEN is_disabled = 0 THEN 'Enabled' ELSE 'Disabled' END AS Status"
                sqlStmt += " FROM sysobjects o JOIN sys.triggers t ON t.object_id = o.id"
                sqlStmt += " WHERE o.type = 'TR' AND o.NAME NOT IN ('trg_MC_Company_Upd_GLSetupSync', 'trg_MC_InterCompany_Insert_Delete_Acct_Sub') ORDER BY o.Name"

                Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()

                        RecID = RecID + 1

                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Date Created: " + sqlReader(2).ToShortDateString().Trim() + " | Status: " + sqlReader(3).trim, 100)
                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)
                        sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)

                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If
            Call sqlReader.Close()

            RecID = RecID + 1
            Call oEventLog.LogMessage(0, "")

            ' Get count of standard triggers - application DB
            sDescr = "Number of standard Triggers in application DB (" + AppDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE type = 'TR' AND NAME IN ('EDPurchOrd_Insert','TrnsfrDocStatusUpdate','UpdateSOShipLotWithShipLine','ADG_TR_CustNameXref_Add','ADG_TR_CustNameXref_Delete','ADG_TR_InvtDescrXref_Add','ADG_TR_InvtDescrXref_Delete','INPrjAllocHistory_Insert','INPrjAllocTranHist_Insert','INPrjAllocLotHist_Insert','Delegation_Message_Trigger')"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Get list of standard triggers - application DB
            If retValInt1 > 0 Then

                sqlStmt = "SELECT o.Name [TriggerName], object_name(parent_obj) [TableName], crdate, CASE WHEN is_disabled = 0 THEN 'Enabled' ELSE 'Disabled' END AS Status"
                sqlStmt += " FROM sysobjects o JOIN sys.triggers t ON t.object_id = o.id"
                sqlStmt += " WHERE o.type = 'TR' AND o.Name IN ('EDPurchOrd_Insert','TrnsfrDocStatusUpdate','UpdateSOShipLotWithShipLine','ADG_TR_CustNameXref_Add','ADG_TR_CustNameXref_Delete','ADG_TR_InvtDescrXref_Add','ADG_TR_InvtDescrXref_Delete','INPrjAllocHistory_Insert','INPrjAllocTranHist_Insert','INPrjAllocLotHist_Insert','Delegation_Message_Trigger')"
                sqlStmt += " ORDER BY o.Name"


                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()

                        RecID = RecID + 1

                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Date Created: " + sqlReader(2).ToShortDateString().Trim() + " | Status: " + sqlReader(3).trim, 100)
                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)
                        sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)

                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If
            Call sqlReader.Close()

            RecID = RecID + 1
            Call oEventLog.LogMessage(0, "")

            ' Get count of custom triggers - application DB
            sDescr = "Number of custom Triggers in application DB (" + AppDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE type = 'TR' AND NAME NOT IN ('EDPurchOrd_Insert','TrnsfrDocStatusUpdate','UpdateSOShipLotWithShipLine','ADG_TR_CustNameXref_Add','ADG_TR_CustNameXref_Delete','ADG_TR_InvtDescrXref_Add','ADG_TR_InvtDescrXref_Delete','INPrjAllocHistory_Insert','INPrjAllocTranHist_Insert','INPrjAllocLotHist_Insert','Delegation_Message_Trigger')"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Get list of custom triggers - application DB
            If retValInt1 > 0 Then

                sqlStmt = "SELECT o.Name [TriggerName], object_name(parent_obj) [TableName], crdate, CASE WHEN is_disabled = 0 THEN 'Enabled' ELSE 'Disabled' END AS Status"
                sqlStmt += " FROM sysobjects o JOIN sys.triggers t ON t.object_id = o.id"
                sqlStmt += " WHERE o.type = 'TR' AND o.Name NOT IN ('EDPurchOrd_Insert','TrnsfrDocStatusUpdate','UpdateSOShipLotWithShipLine','ADG_TR_CustNameXref_Add','ADG_TR_CustNameXref_Delete','ADG_TR_InvtDescrXref_Add','ADG_TR_InvtDescrXref_Delete','INPrjAllocHistory_Insert','INPrjAllocTranHist_Insert','INPrjAllocLotHist_Insert','Delegation_Message_Trigger')"
                sqlStmt += " ORDER BY o.Name"

                Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()

                        RecID = RecID + 1

                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Date Created: " + sqlReader(2).ToShortDateString().Trim() + " | Status: " + sqlReader(3).trim, 100)
                        'sDescr = Strings.Left(" - Name: " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)
                        sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Table: " + sqlReader(1).Trim() + " | Status: " + sqlReader(3).trim, 100)

                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If
            Call sqlReader.Close()

            '***** End of Triggers section *****

            Call oEventLog.LogMessage(0, "")


            '============
            ' FUNCTIONS
            '============
            If SLFunctionsList.Trim() <> "" Then

                sAnalysisType = "Functions"

                Form1.UpdateAnalysisToolStatusBar("Analyzing Custom Objects - Functions")

                RecID = RecID + 1

                ' Get count of custom tables - application DB
                ' sDescr = "Number of custom functions in application DB (" + AppDBName.Trim + "):"
                sDescr = "Number of custom sysobjects functions: "
                sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE XType IN " + SLFunctionsXType.Trim() + " AND Name NOT IN " + SLFunctionsList.Trim()
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                ' List of custom tables - application DB
                If retValInt1 > 0 Then

                    sqlStmt = "SELECT name, crdate FROM sysobjects WHERE XType IN " + SLFunctionsXType.Trim() + " AND Name NOT IN " + SLFunctionsList.Trim() + " ORDER BY Name"
                    'Call oEventLog.LogMessage(0, "")

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    If (sqlReader.HasRows()) Then

                        While sqlReader.Read()

                            RecID = RecID + 1

                            sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Date Created: " + sqlReader(1).ToShortDateString().Trim(), 100)
                            sResult = ""
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        End While

                    End If

                End If
                Call sqlReader.Close()

            End If
            Call oEventLog.LogMessage(0, "")

            '=========
            ' VIEWS
            '=========
            If SLViewsList.Trim() <> "" Then

                sAnalysisType = "Views"

                Form1.UpdateAnalysisToolStatusBar("Analyzing Custom Objects - Views")

                RecID = RecID + 1

                ' Get count of custom views - application DB
                sDescr = "Number of custom sysobjects views: "
                sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE NAME NOT LIKE 'vs_%' AND XType IN " + SLViewsXType.Trim() + " AND Name NOT IN " + SLViewsList.Trim()
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                ' List of custom tables - application DB
                If retValInt1 > 0 Then

                    sqlStmt = "SELECT name, crdate FROM sysobjects WHERE NAME NOT LIKE 'vs_%' AND XType IN " + SLViewsXType.Trim() + " AND Name NOT IN " + SLViewsList.Trim() + " ORDER BY Name"
                    'Call oEventLog.LogMessage(0, "")

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    If (sqlReader.HasRows()) Then

                        While sqlReader.Read()

                            RecID = RecID + 1

                            sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Date Created: " + sqlReader(1).ToShortDateString().Trim(), 100)
                            sResult = ""
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        End While

                    End If

                End If
                Call sqlReader.Close()

            End If
            Call oEventLog.LogMessage(0, "")

            '=============
            ' PROCEDURES
            '=============
            Dim List0 As String
            Dim List1 As String

            For i As Integer = Asc("A") To Asc("Z")

                sAnalysisType = "Procedures"
                Form1.UpdateAnalysisToolStatusBar("Analyzing Custom Objects - Procedures (" + Chr(i) + ")")

                ' create variable names to get public strings from PlumblineCode module
                List0 = "SLProcs_" + Chr(i) + "_List0"
                List1 = "SLProcs_" + Chr(i) + "_List1"

                ' get values of the variables from PlumblineCode module
                List0 = GetVariableValueFromModule(List0)
                List1 = GetVariableValueFromModule(List1)

                RecID = RecID + 1

                ' Get count of custom tables - application DB
                sDescr = "Number of custom sysobjects procedures (" + Chr(i) + "): "
                'sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE XType IN " + SLProcsXType.Trim() + " AND Name NOT IN " + SLProcs_A_List0.Trim() + " AND Name NOT IN " + SLProcs_A_List1.Trim() + " AND Name LIKE 'A%'"

                If List0.Trim = "" And List1.Trim = "" Then
                    sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE XType IN " + SLProcsXType.Trim() + " AND Name LIKE '" + Chr(i) + "%'"
                Else

                    If List1.Trim = "" Then
                        sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE XType IN " + SLProcsXType.Trim() + " AND Name NOT IN " + List0.Trim() + " AND Name LIKE '" + Chr(i) + "%'"
                    Else
                        sqlStmt = "SELECT COUNT(*) FROM sysobjects WHERE XType IN " + SLProcsXType.Trim() + " AND Name NOT IN " + List0.Trim() + " AND Name NOT IN " + List1.Trim() + " AND Name LIKE '" + Chr(i) + "%'"
                    End If
                End If

                Call sqlFetch_Num(retValInt1, sqlStmt, SqlAppDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                ' List of procedure - application DB
                If retValInt1 > 0 Then

                    If List0.Trim = "" And List1.Trim = "" Then
                        sqlStmt = "SELECT name, crdate FROM sysobjects WHERE XType IN " + SLProcsXType.Trim() + " AND Name LIKE '" + Chr(i) + "%' ORDER BY Name"
                    Else
                        If List1.Trim = "" Then
                            sqlStmt = "SELECT name, crdate FROM sysobjects WHERE XType IN " + SLProcsXType.Trim() + " AND Name NOT IN " + List0.Trim() + " AND Name LIKE '" + Chr(i) + "%' ORDER BY Name"
                        Else
                            sqlStmt = "SELECT name, crdate FROM sysobjects WHERE XType IN " + SLProcsXType.Trim() + " AND Name NOT IN " + List0.Trim() + " AND Name NOT IN " + List1.Trim() + " AND Name LIKE '" + Chr(i) + "%' ORDER BY Name"
                        End If
                    End If

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

                    If (sqlReader.HasRows()) Then

                        While sqlReader.Read()

                            RecID = RecID + 1

                            sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " | Date Created: " + sqlReader(1).ToShortDateString().Trim(), 100)
                            sResult = ""
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        End While

                    End If
                    Call sqlReader.Close()

                End If
                Call oEventLog.LogMessage(0, "")

            Next
            Call sqlReader.Close()

            '***** End of Objects section *****

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

        Catch ex As Exception
            Form1.UpdateAnalysisToolStatusBar("Error encountered while analyzing Objects")
            Call MessageBox.Show("Error Encountered " + ex.Message + vbNewLine + ex.StackTrace, "Error Encountered - Objects")
            OkToContinue = False
        End Try

    End Sub

End Module
