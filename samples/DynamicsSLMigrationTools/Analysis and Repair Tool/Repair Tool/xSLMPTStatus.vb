Module xSLMPTStatus


    Public Class strxSLMPTStatus

        Private m_COAErrors As Integer
        Private m_COAEventLogName As String
        Private m_COA_LValidated As DateTime
        Private m_COA_ValCmpltd As Integer
        Private m_COA_Warnings As Integer
        Private m_CpnyID As String
        Private m_Crtd_DateTime As Date
        Private m_Crtd_User As String
        Private m_Cust_Errors As Integer
        Private m_CustEventLogName As String
        Private m_Cust_LValidated As DateTime
        Private m_Cust_ValCmpltd As Integer
        Private m_Cust_Warnings As Integer
        Private m_CustAddrOpt As Integer
        Private m_ExpFilePath As String
        Private m_FiscYr_Beg As String
        Private m_FiscYr_End As String
        Private m_HistoryDate As DateTime
        Private m_Inv_Errors As Integer
        Private m_InvEventLogName As String
        Private m_Inv_LValidated As DateTime
        Private m_Inv_ValCmpltd As Integer
        Private m_Inv_Warnings As Integer
        Private m_LUpd_DateTime As DateTime
        Private m_LUpd_User As String
        Private m_Method As String
        Private m_PO_Errors As Integer
        Private m_POEventLogName As String
        Private m_PO_LValidated As DateTime
        Private m_PO_ValCmpltd As Integer
        Private m_PO_Warnings As Integer
        Private m_POFreightChrgItem As String
        Private m_Proj_Errors As Integer
        Private m_ProjEventLogName As String
        Private m_Proj_LValidated As DateTime
        Private m_Proj_StatusActive As Integer
        Private m_Proj_StatusInactive As Integer
        Private m_Proj_StatusPlan As Integer
        Private m_Proj_ValCmpltd As Integer
        Private m_Proj_Warnings As Integer
        Private m_S4Future01 As String
        Private m_S4Future02 As String
        Private m_S4Future03 As Double
        Private m_S4Future04 As Double
        Private m_S4Future05 As Double
        Private m_S4Future06 As Double
        Private m_S4Future07 As DateTime
        Private m_S4Future08 As DateTime
        Private m_S4Future09 As Integer
        Private m_S4Future10 As Integer
        Private m_S4Future11 As String
        Private m_S4Future12 As String
        Private m_SO_Errors As Integer
        Private m_SOEventLogName As String
        Private m_SO_LValidated As DateTime
        Private m_SO_ValCmpltd As Integer
        Private m_SO_Warnings As Integer
        Private m_Task_StatusActive As Integer
        Private m_Task_StatusInactive As Integer
        Private m_Task_StatusPlan As Integer
        Private m_Vend_Errors As Integer
        Private m_VendEventLogName As String
        Private m_Vend_LValidated As DateTime
        Private m_Vend_ValCmpltd As Integer
        Private m_Vend_Warnings As Integer
        Private m_VendAddrOpt As Integer


        Public Property COAErrors() As Integer

            Get
                Return m_COAErrors
            End Get

            Set(ByVal setval As Integer)
                m_COAErrors = setval
            End Set

        End Property
        Public Property COAEventLogName() As String

            Get
                Return m_COAEventLogName
            End Get

            Set(ByVal setval As String)
                m_COAEventLogName = setval
            End Set

        End Property

        Public Property COA_LValidated() As DateTime

            Get
                Return m_COA_LValidated
            End Get

            Set(ByVal setval As DateTime)
                m_COA_LValidated = setval
            End Set

        End Property

        Public Property COA_ValCmpltd() As Integer

            Get
                Return m_COA_ValCmpltd
            End Get

            Set(ByVal setval As Integer)
                m_COA_ValCmpltd = setval
            End Set

        End Property
        Public Property COA_Warnings() As Integer

            Get
                Return m_COA_Warnings
            End Get

            Set(ByVal setval As Integer)
                m_COA_Warnings = setval
            End Set

        End Property

        Public Property CpnyID() As String

            Get
                Return m_CpnyID
            End Get

            Set(ByVal setval As String)
                m_CpnyID = setval
            End Set

        End Property

        Public Property Crtd_DateTime() As DateTime

            Get
                Return m_Crtd_DateTime
            End Get

            Set(ByVal setval As DateTime)
                m_Crtd_DateTime = setval
            End Set

        End Property

        Public Property Crtd_User() As String

            Get
                Return m_Crtd_User
            End Get

            Set(ByVal setval As String)
                m_Crtd_User = setval
            End Set

        End Property

        Public Property Cust_Errors() As Integer

            Get
                Return m_Cust_Errors
            End Get

            Set(ByVal setval As Integer)
                m_Cust_Errors = setval
            End Set

        End Property

        Public Property CustEventLogName() As String

            Get
                Return m_CustEventLogName
            End Get

            Set(ByVal setval As String)
                m_CustEventLogName = setval
            End Set

        End Property
        Public Property Cust_LValidated() As DateTime

            Get
                Return m_Cust_LValidated
            End Get

            Set(ByVal setval As DateTime)
                m_Cust_LValidated = setval
            End Set

        End Property

        Public Property Cust_ValCmpltd() As Integer

            Get
                Return m_Cust_ValCmpltd
            End Get

            Set(ByVal setval As Integer)
                m_Cust_ValCmpltd = setval
            End Set

        End Property

        Public Property Cust_Warnings() As Integer

            Get
                Return m_Cust_Warnings
            End Get

            Set(ByVal setval As Integer)
                m_Cust_Warnings = setval
            End Set

        End Property

        Public Property CustAddrOpt() As Integer

            Get
                Return m_CustAddrOpt
            End Get

            Set(ByVal setval As Integer)
                m_CustAddrOpt = setval
            End Set

        End Property
        Public Property ExpFilePath() As String

            Get
                Return m_ExpFilePath
            End Get

            Set(ByVal setval As String)
                m_ExpFilePath = setval
            End Set

        End Property
        Public Property FiscYr_Beg() As String

            Get
                Return m_FiscYr_Beg
            End Get

            Set(ByVal setval As String)
                m_FiscYr_Beg = setval
            End Set

        End Property

        Public Property FiscYr_End() As String

            Get
                Return m_FiscYr_End
            End Get

            Set(ByVal setval As String)
                m_FiscYr_End = setval
            End Set

        End Property

        Public Property HistoryDate() As DateTime

            Get
                Return m_HistoryDate
            End Get

            Set(ByVal setval As DateTime)
                m_HistoryDate = setval
            End Set

        End Property

        Public Property Inv_Errors() As Integer

            Get
                Return m_Inv_Errors
            End Get

            Set(ByVal setval As Integer)
                m_Inv_Errors = setval
            End Set

        End Property
        Public Property InvEventLogName() As String

            Get
                Return m_InvEventLogName
            End Get

            Set(ByVal setval As String)
                m_InvEventLogName = setval
            End Set

        End Property

        Public Property Inv_LValidated() As DateTime

            Get
                Return m_Inv_LValidated
            End Get

            Set(ByVal setval As DateTime)
                m_Inv_LValidated = setval
            End Set

        End Property

        Public Property Inv_ValCmpltd() As Integer

            Get
                Return m_Inv_ValCmpltd
            End Get

            Set(ByVal setval As Integer)
                m_Inv_ValCmpltd = setval
            End Set

        End Property

        Public Property Inv_Warnings() As Integer

            Get
                Return m_Inv_Warnings
            End Get

            Set(ByVal setval As Integer)
                m_Inv_Warnings = setval
            End Set

        End Property

        Public Property LUpd_DateTime() As DateTime

            Get
                Return m_LUpd_DateTime
            End Get

            Set(ByVal setval As DateTime)
                m_LUpd_DateTime = setval
            End Set

        End Property

        Public Property LUpd_User() As String

            Get
                Return m_LUpd_User
            End Get

            Set(ByVal setval As String)
                m_LUpd_User = setval
            End Set

        End Property
        Public Property Method() As String

            Get
                Return m_Method
            End Get

            Set(ByVal setval As String)
                m_Method = setval
            End Set

        End Property
        Public Property PO_Errors() As Integer

            Get
                Return m_PO_Errors
            End Get

            Set(ByVal setval As Integer)
                m_PO_Errors = setval
            End Set

        End Property
        Public Property POEventLogName() As String

            Get
                Return m_POEventLogName
            End Get

            Set(ByVal setval As String)
                m_POEventLogName = setval
            End Set

        End Property
        Public Property PO_LValidated() As DateTime

            Get
                Return m_PO_LValidated
            End Get

            Set(ByVal setval As DateTime)
                m_PO_LValidated = setval
            End Set

        End Property
        Public Property PO_ValCmpltd() As Integer

            Get
                Return m_PO_ValCmpltd
            End Get

            Set(ByVal setval As Integer)
                m_PO_ValCmpltd = setval
            End Set

        End Property

        Public Property PO_Warnings() As Integer

            Get
                Return m_PO_Warnings
            End Get

            Set(ByVal setval As Integer)
                m_PO_Warnings = setval
            End Set

        End Property
        Public Property POFreightChrgItem() As String

            Get
                Return m_POFreightChrgItem
            End Get

            Set(ByVal setval As String)
                m_POFreightChrgItem = setval
            End Set

        End Property
        Public Property Proj_Errors() As Integer

            Get
                Return m_Proj_Errors
            End Get

            Set(ByVal setval As Integer)
                m_Proj_Errors = setval
            End Set

        End Property
        Public Property ProjEventLogName() As String

            Get
                Return m_ProjEventLogName
            End Get

            Set(ByVal setval As String)
                m_ProjEventLogName = setval
            End Set

        End Property
        Public Property Proj_LValidated() As DateTime

            Get
                Return m_Proj_LValidated
            End Get

            Set(ByVal setval As DateTime)
                m_Proj_LValidated = setval
            End Set

        End Property
        Public Property Proj_StatusActive() As Integer

            Get
                Return m_Proj_StatusActive
            End Get

            Set(ByVal setval As Integer)
                m_Proj_StatusActive = setval
            End Set

        End Property
        Public Property Proj_StatusInactive() As Integer

            Get
                Return m_Proj_StatusInactive
            End Get

            Set(ByVal setval As Integer)
                m_Proj_StatusInactive = setval
            End Set

        End Property
        Public Property Proj_StatusPlan() As Integer

            Get
                Return m_Proj_StatusPlan
            End Get

            Set(ByVal setval As Integer)
                m_Proj_StatusPlan = setval
            End Set

        End Property
        Public Property Proj_ValCmpltd() As Integer

            Get
                Return m_Proj_ValCmpltd
            End Get

            Set(ByVal setval As Integer)
                m_Proj_ValCmpltd = setval
            End Set

        End Property
        Public Property Proj_Warnings() As Integer

            Get
                Return m_Proj_Warnings
            End Get

            Set(ByVal setval As Integer)
                m_Proj_Warnings = setval
            End Set

        End Property
        Public Property S4Future01() As String

            Get
                Return m_S4Future01
            End Get

            Set(ByVal setval As String)
                m_S4Future01 = setval
            End Set

        End Property

        Public Property S4Future02() As String

            Get
                Return m_S4Future02
            End Get

            Set(ByVal setval As String)
                m_S4Future02 = setval
            End Set

        End Property
        Public Property S4Future03() As Double

            Get
                Return m_S4Future03
            End Get

            Set(ByVal setval As Double)
                m_S4Future03 = setval
            End Set

        End Property
        Public Property S4Future04() As Double

            Get
                Return m_S4Future04
            End Get

            Set(ByVal setval As Double)
                m_S4Future04 = setval
            End Set

        End Property
        Public Property S4Future05() As Double

            Get
                Return m_S4Future05
            End Get

            Set(ByVal setval As Double)
                m_S4Future05 = setval
            End Set

        End Property
        Public Property S4Future06() As Double

            Get
                Return m_S4Future06
            End Get

            Set(ByVal setval As Double)
                m_S4Future06 = setval
            End Set

        End Property
        Public Property S4Future07() As DateTime

            Get
                Return m_S4Future07
            End Get

            Set(ByVal setval As DateTime)
                m_S4Future07 = setval
            End Set

        End Property
        Public Property S4Future08() As DateTime

            Get
                Return m_S4Future08
            End Get

            Set(ByVal setval As DateTime)
                m_S4Future08 = setval
            End Set

        End Property
        Public Property S4Future09() As Integer

            Get
                Return m_S4Future09
            End Get

            Set(ByVal setval As Integer)
                m_S4Future09 = setval
            End Set

        End Property
        Public Property S4Future10() As Integer

            Get
                Return m_S4Future10
            End Get

            Set(ByVal setval As Integer)
                m_S4Future10 = setval
            End Set

        End Property
        Public Property S4Future11() As String

            Get
                Return m_S4Future11
            End Get

            Set(ByVal setval As String)
                m_S4Future11 = setval
            End Set

        End Property
        Public Property S4Future12() As String

            Get
                Return m_S4Future12
            End Get

            Set(ByVal setval As String)
                m_S4Future12 = setval
            End Set

        End Property
        Public Property SO_Errors() As Integer

            Get
                Return m_SO_Errors
            End Get

            Set(ByVal setval As Integer)
                m_SO_Errors = setval
            End Set

        End Property
        Public Property SOEventLogName() As String

            Get
                Return m_SOEventLogName
            End Get

            Set(ByVal setval As String)
                m_SOEventLogName = setval
            End Set

        End Property
        Public Property SO_LValidated() As DateTime

            Get
                Return m_SO_LValidated
            End Get

            Set(ByVal setval As DateTime)
                m_SO_LValidated = setval
            End Set

        End Property

        Public Property SO_ValCmpltd() As Integer

            Get
                Return m_SO_ValCmpltd
            End Get

            Set(ByVal setval As Integer)
                m_SO_ValCmpltd = setval
            End Set

        End Property
        Public Property SO_Warnings() As Integer

            Get
                Return m_SO_Warnings
            End Get

            Set(ByVal setval As Integer)
                m_SO_Warnings = setval
            End Set

        End Property
        Public Property Task_StatusActive() As Integer

            Get
                Return m_Task_StatusActive
            End Get

            Set(ByVal setval As Integer)
                m_Task_StatusActive = setval
            End Set

        End Property
        Public Property Task_StatusInactive() As Integer

            Get
                Return m_Task_StatusInactive
            End Get

            Set(ByVal setval As Integer)
                m_Task_StatusInactive = setval
            End Set

        End Property
        Public Property Task_StatusPlan() As Integer

            Get
                Return m_Task_StatusPlan
            End Get

            Set(ByVal setval As Integer)
                m_Task_StatusPlan = setval
            End Set

        End Property
        Public Property Vend_Errors() As Integer

            Get
                Return m_Vend_Errors
            End Get

            Set(ByVal setval As Integer)
                m_Vend_Errors = setval
            End Set

        End Property
        Public Property VendEventLogName() As String

            Get
                Return m_VendEventLogName
            End Get

            Set(ByVal setval As String)
                m_VendEventLogName = setval
            End Set

        End Property
        Public Property Vend_LValidated() As DateTime

            Get
                Return m_Vend_LValidated
            End Get

            Set(ByVal setval As DateTime)
                m_Vend_LValidated = setval
            End Set

        End Property
        Public Property Vend_ValCmpltd() As Integer

            Get
                Return m_Vend_ValCmpltd
            End Get

            Set(ByVal setval As Integer)
                m_Vend_ValCmpltd = setval
            End Set

        End Property
        Public Property Vend_Warnings() As Integer

            Get
                Return m_Vend_Warnings
            End Get

            Set(ByVal setval As Integer)
                m_Vend_Warnings = setval
            End Set

        End Property
        Public Property VendAddrOpt() As Integer

            Get
                Return m_VendAddrOpt
            End Get

            Set(ByVal setval As Integer)
                m_VendAddrOpt = setval
            End Set

        End Property
    End Class
    Public bSLMPTStatus As strxSLMPTStatus = New strxSLMPTStatus



    Public Sub SetStatusValues(sqlReader As System.Data.SqlClient.SqlDataReader, SLMPTStatusBuf As strxSLMPTStatus)
        If (sqlReader.HasRows = True) Then
            sqlReader.Read()
            SLMPTStatusBuf.COAErrors = sqlReader("COA_Errors")
            SLMPTStatusBuf.COAEventLogName = sqlReader("COAEventLogName")
            SLMPTStatusBuf.COA_LValidated = sqlReader("COA_LValidated")
            SLMPTStatusBuf.COA_ValCmpltd = sqlReader("COA_ValCmpltd")
            SLMPTStatusBuf.COA_Warnings = sqlReader("COA_Warnings")
            SLMPTStatusBuf.CpnyID = sqlReader("CpnyID")
            SLMPTStatusBuf.Crtd_DateTime = sqlReader("Crtd_DateTime")
            SLMPTStatusBuf.Crtd_User = sqlReader("Crtd_User")
            SLMPTStatusBuf.Cust_Errors = sqlReader("Cust_Errors")
            SLMPTStatusBuf.CustEventLogName = sqlReader("CustEventLogName")
            SLMPTStatusBuf.Cust_LValidated = sqlReader("Cust_LValidated")
            SLMPTStatusBuf.Cust_ValCmpltd = sqlReader("Cust_ValCmpltd")
            SLMPTStatusBuf.Cust_Warnings = sqlReader("Cust_Warnings")
            SLMPTStatusBuf.CustAddrOpt = sqlReader("CustAddrOpt")
            SLMPTStatusBuf.ExpFilePath = sqlReader("ExpFilePath")
            SLMPTStatusBuf.FiscYr_Beg = sqlReader("FiscYr_Beg")
            SLMPTStatusBuf.FiscYr_End = sqlReader("FiscYr_End")
            SLMPTStatusBuf.HistoryDate = sqlReader("HistoryDate")
            SLMPTStatusBuf.Inv_Errors = sqlReader("Inv_Errors")
            SLMPTStatusBuf.InvEventLogName = sqlReader("InvEventLogName")
            SLMPTStatusBuf.Inv_LValidated = sqlReader("Inv_LValidated")
            SLMPTStatusBuf.Inv_ValCmpltd = sqlReader("Inv_ValCmpltd")
            SLMPTStatusBuf.Inv_Warnings = sqlReader("Inv_Warnings")
            SLMPTStatusBuf.LUpd_DateTime = sqlReader("LUpd_DateTime")
            SLMPTStatusBuf.LUpd_User = sqlReader("LUpd_User")
            SLMPTStatusBuf.Method = sqlReader("Method")
            SLMPTStatusBuf.PO_Errors = sqlReader("PO_Errors")
            SLMPTStatusBuf.POEventLogName = sqlReader("POEventLogName")
            SLMPTStatusBuf.PO_LValidated = sqlReader("PO_LValidated")
            SLMPTStatusBuf.PO_ValCmpltd = sqlReader("PO_ValCmpltd")
            SLMPTStatusBuf.PO_Warnings = sqlReader("PO_Warnings")
            SLMPTStatusBuf.POFreightChrgItem = sqlReader("POFreightChrgItem")
            SLMPTStatusBuf.Proj_Errors = sqlReader("Proj_Errors")
            SLMPTStatusBuf.ProjEventLogName = sqlReader("ProjEventLogName")
            SLMPTStatusBuf.Proj_LValidated = sqlReader("Proj_LValidated")
            SLMPTStatusBuf.Proj_StatusActive = sqlReader("Proj_StatusActive")
            SLMPTStatusBuf.Proj_StatusInactive = sqlReader("Proj_StatusInactive")
            SLMPTStatusBuf.Proj_StatusPlan = sqlReader("Proj_StatusPlan")
            SLMPTStatusBuf.Proj_ValCmpltd = sqlReader("Proj_ValCmpltd")
            SLMPTStatusBuf.Proj_Warnings = sqlReader("Proj_Warnings")
            SLMPTStatusBuf.SO_Errors = sqlReader("SO_Errors")
            SLMPTStatusBuf.SOEventLogName = sqlReader("SOEventLogName")
            SLMPTStatusBuf.SO_LValidated = sqlReader("SO_LValidated")
            SLMPTStatusBuf.SO_ValCmpltd = sqlReader("SO_ValCmpltd")
            SLMPTStatusBuf.SO_Warnings = sqlReader("SO_Warnings")
            SLMPTStatusBuf.Task_StatusActive = sqlReader("Task_StatusActive")
            SLMPTStatusBuf.Task_StatusInactive = sqlReader("Task_StatusInactive")
            SLMPTStatusBuf.Task_StatusPlan = sqlReader("Task_StatusPlan")
            SLMPTStatusBuf.Vend_Errors = sqlReader("Vend_Errors")
            SLMPTStatusBuf.VendEventLogName = sqlReader("VendEventLogName")
            SLMPTStatusBuf.Vend_LValidated = sqlReader("Vend_LValidated")
            SLMPTStatusBuf.Vend_ValCmpltd = sqlReader("Vend_ValCmpltd")
            SLMPTStatusBuf.Vend_Warnings = sqlReader("Vend_Warnings")
            SLMPTStatusBuf.VendAddrOpt = sqlReader("VendAddrOpt")

        End If
    End Sub
    Public Sub InitStatusValues(SLMPTStatusBuf As strxSLMPTStatus)
        SLMPTStatusBuf.COAErrors = 0
        SLMPTStatusBuf.COAEventLogName = String.Empty
        SLMPTStatusBuf.COA_LValidated = MinDateValue
        SLMPTStatusBuf.COA_ValCmpltd = 0
        SLMPTStatusBuf.COA_Warnings = 0
        SLMPTStatusBuf.CpnyID = String.Empty
        SLMPTStatusBuf.Crtd_DateTime = Date.MinValue
        SLMPTStatusBuf.Crtd_User = String.Empty
        SLMPTStatusBuf.Cust_Errors = 0
        SLMPTStatusBuf.CustEventLogName = String.Empty
        SLMPTStatusBuf.Cust_LValidated = MinDateValue
        SLMPTStatusBuf.Cust_ValCmpltd = 0
        SLMPTStatusBuf.Cust_Warnings = 0
        SLMPTStatusBuf.CustAddrOpt = 0
        SLMPTStatusBuf.ExpFilePath = String.Empty
        SLMPTStatusBuf.FiscYr_Beg = String.Empty
        SLMPTStatusBuf.FiscYr_End = String.Empty
        SLMPTStatusBuf.HistoryDate = Date.MinValue
        SLMPTStatusBuf.Inv_Errors = 0
        SLMPTStatusBuf.InvEventLogName = String.Empty
        SLMPTStatusBuf.Inv_LValidated = MinDateValue
        SLMPTStatusBuf.Inv_ValCmpltd = 0
        SLMPTStatusBuf.Inv_Warnings = 0
        SLMPTStatusBuf.LUpd_DateTime = Date.MinValue
        SLMPTStatusBuf.LUpd_User = String.Empty
        SLMPTStatusBuf.Method = String.Empty
        SLMPTStatusBuf.PO_Errors = 0
        SLMPTStatusBuf.POEventLogName = String.Empty
        SLMPTStatusBuf.PO_LValidated = MinDateValue
        SLMPTStatusBuf.PO_ValCmpltd = 0
        SLMPTStatusBuf.PO_Warnings = 0
        SLMPTStatusBuf.POFreightChrgItem = String.Empty
        SLMPTStatusBuf.Proj_Errors = 0
        SLMPTStatusBuf.ProjEventLogName = String.Empty
        SLMPTStatusBuf.Proj_LValidated = MinDateValue
        SLMPTStatusBuf.Proj_StatusActive = 0
        SLMPTStatusBuf.Proj_StatusInactive = 0
        SLMPTStatusBuf.Proj_StatusPlan = 0
        SLMPTStatusBuf.Proj_ValCmpltd = 0
        SLMPTStatusBuf.Proj_Warnings = 0
        SLMPTStatusBuf.SO_Errors = 0
        SLMPTStatusBuf.SOEventLogName = String.Empty
        SLMPTStatusBuf.SO_LValidated = MinDateValue
        SLMPTStatusBuf.SO_ValCmpltd = 0
        SLMPTStatusBuf.SO_Warnings = 0
        SLMPTStatusBuf.Task_StatusActive = 0
        SLMPTStatusBuf.Task_StatusInactive = 0
        SLMPTStatusBuf.Task_StatusPlan = 0
        SLMPTStatusBuf.Vend_Errors = 0
        SLMPTStatusBuf.VendEventLogName = String.Empty
        SLMPTStatusBuf.Vend_LValidated = MinDateValue
        SLMPTStatusBuf.Vend_ValCmpltd = 0
        SLMPTStatusBuf.Vend_Warnings = 0
        SLMPTStatusBuf.VendAddrOpt = 0


    End Sub
End Module
