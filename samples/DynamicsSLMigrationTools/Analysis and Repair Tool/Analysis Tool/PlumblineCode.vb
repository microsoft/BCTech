Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Imports System.IO

Module PlumblineCode
    'Public Cursors
    Public CSR_GLSetup As Integer
    Public CSR_APSetup As Integer
    Public CSR_ARSetup As Integer
    Public CSR_FlexDef As Integer
    Public CSR_INSetup As Integer
    Public CSR_POSetup As Integer
    Public CSR_RQSetup As Integer
    Public CSR_SOSetup As Integer
    Public CSR_PJContrl As Integer
    Public CSR_CMSetup As Integer
    Public CSR_CASetup As Integer
    Public CSR_BMSetup As Integer
    Public CSR_SMProServSetup As Integer
    Public CSR_SegDef As Integer
    Public CSR_xSLMPTSubErrors As Integer

    'Public Variables
    Public RecID As Long
    Public CurrDate As Date
    Public CurrDateStr As String = String.Empty
    Public LastYrDate As Date
    Public LastYrDateStr As String = String.Empty
    Public OkToContinue As Boolean = True
    Public MultiCpnyAppDB As Boolean = False
    Public Mem_CpnyIDApp As Integer
    Public CSRCpnyIDApp As Integer
    Public Fetch_CpnyIDApp As Integer
    Public SQLStr_CpnyIDApp As String = String.Empty
    Public Mem_CpnyIDSys As Integer
    Public CSRCpnyIDSys As Integer
    Public Fetch_CpnyIDSys As Integer
    Public SQLStr_CpnyIDSys As String = String.Empty
    Public RecMaintFlg As Integer
    Public GLRetention As String = String.Empty

    Public BaseCuryPrec As Short = 0

    Public ProdName As String = String.Empty

    Public oEventLog As clsEventLog
    Public DBNameSystem As String = String.Empty


    Public Const EndProcess As Short = -2
    Public Const StartProcess As Short = -3
    Public Const StopProcess As Short = -4



    'Registration Information
    Private ENCODED As String = "[\ObSaTWdXj3k" + Convert.ToString(Chr(138)) + "GD=F>HEJ@IBKL?-;A,/0C:]mnopqrstuvwxyz{|}~" + Convert.ToString(Chr(127)) + Convert.ToString(Chr(128)) + Convert.ToString(Chr(129)) + Convert.ToString(Chr(130)) + Convert.ToString(Chr(131)) + Convert.ToString(Chr(132)) + Convert.ToString(Chr(133)) + Convert.ToString(Chr(134)) + "Z`U    Y4fPQc^    h "
    Private READABLE As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqr" + "s" + "t" + "u" + "v" + "w" + "x" + "y" + "z" + ".,+!#$%&'()-/:;<=>@" + Convert.ToString(Chr(34))

    Public Class VersionSLCls

        Private m_VersionNbr As String
        Public Property VersionNbr() As String
            Get
                Return m_VersionNbr
            End Get
            Set(ByVal setval As String)
                m_VersionNbr = setval
            End Set
        End Property

    End Class

    Public bVersionSL As VersionSLCls = New VersionSLCls

    Public Class MCDatabases
        Private m_MCDatabaseName As String

        Public Property MCDatabaseName() As String
            Get
                Return m_MCDatabaseName
            End Get
            Set(ByVal setval As String)
                Me.m_MCDatabaseName = setval
            End Set
        End Property

    End Class

    Public bMCDatabases As MCDatabases = New MCDatabases, nMCDatabases As MCDatabases = New MCDatabases

    Public Class CpnyIDListApp

        Private m_CompanyID As String
        Private m_CompanyName As String

        Public Property CompanyID() As String
            Get
                Return m_CompanyID
            End Get
            Set(ByVal setval As String)
                m_CompanyID = setval
            End Set
        End Property
        Public Property CompanyName() As String
            Get
                Return m_CompanyName
            End Get
            Set(ByVal setval As String)
                m_CompanyName = setval
            End Set
        End Property


    End Class

    Public bCpnyIDListApp As CpnyIDListApp = New CpnyIDListApp, nCpnyIDListApp As CpnyIDListApp = New CpnyIDListApp
    Public bCpnyIDListAppTemp As CpnyIDListApp = New CpnyIDListApp, nCpnyIDListAppTemp As CpnyIDListApp = New CpnyIDListApp

    Public Class UnboundList30
        Private m_ListID As String
        Public Property ListID() As String
            Get
                Return m_ListID
            End Get
            Set(ByVal setval As String)
                m_ListID = setval
            End Set
        End Property

    End Class

    Public bUnboundList30 As UnboundList30 = New UnboundList30
    Public Sub SetUnboundList30Values(sqlReader As SqlDataReader, bUnboundListBuf As UnboundList30)
        Try
            bUnboundListBuf.ListID = sqlReader(0)
        Catch
            bUnboundListBuf.ListID = String.Empty
        End Try

    End Sub

    Public Sub InitUnboundList30Values(bUnboundListBuf As UnboundList30)
        bUnboundListBuf.ListID = String.Empty
    End Sub


    Public Class SubAcctList

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

    Public bSubAcctList As SubAcctList = New SubAcctList
    Public Sub SetSubAcctListValues(sqlReader As SqlDataReader, SubAcctListBuf As SubAcctList)
        Try
            SubAcctListBuf.SubAcct = sqlReader(0)
        Catch ex As Exception
            SubAcctListBuf.SubAcct = String.Empty
        End Try
    End Sub

    Public Class ProjTaskList

        Private m_ProjectID As String
        Private m_TaskID As String
        Public Property ProjectID() As String
            Get
                Return m_ProjectID
            End Get
            Set(ByVal setval As String)
                m_ProjectID = setval
            End Set
        End Property


        Public Property TaskID() As String
            Get
                Return m_TaskID
            End Get
            Set(ByVal setval As String)
                m_TaskID = setval
            End Set
        End Property

    End Class

    Public bProjTaskListInfo As ProjTaskList = New ProjTaskList, nProjTaskList As ProjTaskList = New ProjTaskList

    Public Sub SetProjTaskListValues(sqlReader As SqlDataReader, bProjTaskListBuf As ProjTaskList)
        Try
            bProjTaskListBuf.ProjectID = sqlReader("Project")
            bProjTaskListBuf.TaskID = sqlReader("pjt_entity")
        Catch ex As Exception
            bProjTaskListBuf.ProjectID = String.Empty
            bProjTaskListBuf.TaskID = String.Empty

        End Try
    End Sub

    Public Class SpecCostIDList
        Private m_InvtId As String
        Private m_SpecificCostID As String

        Public Property InvtID() As String
            Get
                Return m_InvtId
            End Get
            Set(ByVal setval As String)
                m_InvtId = setval
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

    End Class

    Public bSpecCostIDList As SpecCostIDList = New SpecCostIDList, nSpecCostIDList As SpecCostIDList = New SpecCostIDList

    Public Sub SetSpecificCostIDListValues(sqlReader As SqlDataReader, SpecCostIDListBuf As SpecCostIDList)
        Try
            SpecCostIDListBuf.InvtID = sqlReader("InvtId")
            SpecCostIDListBuf.SpecificCostID = sqlReader("SpecificCostID")
        Catch ex As Exception
            SpecCostIDListBuf.InvtID = String.Empty
            SpecCostIDListBuf.SpecificCostID = String.Empty
        End Try
    End Sub

    Public Class SQLInfoCls

        Dim m_ProductVersion As String
        Dim m_MachineName As String
        Dim m_ServerName As String
        Dim m_Edition As String

        Public Property ProductVersion() As String
            Get
                Return m_ProductVersion
            End Get
            Set(ByVal setval As String)
                m_ProductVersion = setval
            End Set
        End Property
        Public Property MachineName() As String
            Get
                Return m_MachineName
            End Get
            Set(ByVal setval As String)
                m_MachineName = setval
            End Set
        End Property
        Public Property ServerName() As String
            Get
                Return m_ServerName
            End Get
            Set(ByVal setval As String)
                m_ServerName = setval
            End Set
        End Property
        Public Property Edition() As String
            Get
                Return m_Edition
            End Get
            Set(ByVal setval As String)
                m_Edition = setval
            End Set
        End Property


    End Class

    Public bSQLInfo As SQLInfoCls = New SQLInfoCls, nModuleList As SQLInfoCls = New SQLInfoCls

    Public Sub SetSQLInfoValues(sqlReader As SqlDataReader, SqlInfoBuf As SQLInfoCls)
        Try
            SqlInfoBuf.ProductVersion = sqlReader(0)
            SqlInfoBuf.MachineName = sqlReader(1)
            SqlInfoBuf.ServerName = sqlReader(2)
            SqlInfoBuf.Edition = sqlReader(3)
        Catch ex As Exception
            SqlInfoBuf.ProductVersion = String.Empty
            SqlInfoBuf.MachineName = String.Empty
            SqlInfoBuf.ServerName = String.Empty
            SqlInfoBuf.Edition = String.Empty
        End Try
    End Sub
    Public Function DecodeCustId(ByVal input As Byte()) As String
        DecodeCustId = String.Empty

        Try
            Dim SolCustID As String = String.Empty
            For i As Integer = 9 To 0 Step -1
                'get the char
                Dim tempChar As Byte = input(i)

                'find it in the encoded list
                Dim tempPos As Integer = ENCODED.IndexOf(Chr(tempChar))
                'get the readable value from the same position and append to end
                Dim wrkChar As Char = READABLE(tempPos).ToString()
                SolCustID = String.Concat(SolCustID, wrkChar)
            Next

            DecodeCustId = SolCustID
        Catch ex As Exception

        End Try

    End Function

    Public Function DecodeCustName(ByVal input As Byte()) As String
        DecodeCustName = String.Empty

        Try
            Dim SolCustName As String = String.Empty
            For i As Integer = 29 To 0 Step -1
                Dim tempChar As Byte = input(i)
                Dim tempPos As Integer = ENCODED.IndexOf(Chr(tempChar))
                Dim wrkChar As Char = READABLE(tempPos).ToString()
                SolCustName = String.Concat(SolCustName, wrkChar)
            Next

            DecodeCustName = SolCustName
        Catch ex As Exception

        End Try

    End Function

    Public Function DblQuotes(ByVal input As String) As String
        DblQuotes = String.Empty

        DblQuotes = """" + input + """"

    End Function



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
            sqlDefConn.Open()

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

                    Call SetSubAcctListValues(sqlReader, bSubAcctList)
                    If bSubAcctList.SubAcct.Trim IsNot String.Empty Then
                        'Check to see if segment value is in the SegDef table
                        sqlString2 = "Select * FROM SegDef WHERE FieldClassName = 'SUBACCOUNT' AND SegNumber =  '" + CStr(cnt) + "' AND ID =" + SParm(bSubAcctList.SubAcct.Trim)
                        Call sqlFetch_1(sqlDefReader, sqlString2, sqlDefConn, CommandType.Text)

                        If sqlDefReader.HasRows() = False Then
                            'Write record to xSLMPTSubErrors table
                            sqlString3 = "SELECT * FROM xSLMPTSubErrors WHERE SegNumber = '" + CStr(cnt) + "' AND ID =" + SParm(bSubAcctList.SubAcct.Trim)
                            Call sqlFetch_1(sqlErrReader, sqlString3, sqlErrConn, CommandType.Text)
                            If sqlErrReader.HasRows() = False Then
                                Call sqlErrReader.Close()
                                bxSLMPTSubErrors.SegNumber = cnt
                                bxSLMPTSubErrors.ID = bSubAcctList.SubAcct.Trim


                                If (sqlErrConn.State <> ConnectionState.Open) Then
                                    sqlErrConn.Open()
                                End If

                                dbTran = TranBeg(sqlErrConn)

                                SqlInsertStmt = "Insert into xSLMPTSubErrors ([ID], [SegNumber]) Values(" + SParm(bSubAcctList.SubAcct.Trim) + "," + SParm(cnt.ToString) + ")"


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
            Call oEventLog.LogMessage(0, "")
            msgText = "The following Subaccount Segment IDs are not found in the SegDef table. All Subaccount Segment IDs must be set up in"
            msgText = msgText + " Flexkey Table Maintenance (21.330.00) prior to migrating data."
            msgText = msgText + " Missing Segment IDs will be added TypeOf the SegDef table."
            Call oEventLog.LogMessage(0, "")

            sqlDefConn = New SqlClient.SqlConnection(AppDbConnStr)
            sqlDefConn.Open()



        End If



        'Delete records in xSLMPTSubErrors table
        sqlStmt = "Delete from xSLMPTSubErrors"
        Call sql_1(sqlReader, sqlStmt, SqlAppDbConn, OperationType.DeleteOp, CommandType.Text)



    End Sub

End Module
