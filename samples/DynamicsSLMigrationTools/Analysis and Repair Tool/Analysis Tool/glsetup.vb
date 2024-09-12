Imports System.Data.SqlClient
Module GLSETUP

    Public Class GLSetupCls

        Private m_BaseCuryID As String
        Private m_Central_Cash_Cntl As Short
        Private m_CpnyId As String
        Private m_COAOrder As String
        Private m_LedgerID As String
        Private m_MCActive As Short
        Private m_Mult_Cpny_DB As Short
        Private m_NbrPer As Short
        Private m_PerNbr As String
        Private m_PerRetHist As Short
        Private m_PerRetModTran As Short
        Private m_PerRetTran As Short
        Private m_RetEarnAcct As String
        Private m_ValidateAcctSub As Short
        Private m_YtdNetIncAcct As String

        Public Property BaseCuryID() As String

            Get
                Return m_BaseCuryID
            End Get

            Set(ByVal setval As String)
                m_BaseCuryID = setval
            End Set

        End Property
        Public Property Central_Cash_Cntl() As Short

            Get
                Return m_Central_Cash_Cntl
            End Get

            Set(ByVal setval As Short)
                m_Central_Cash_Cntl = setval
            End Set

        End Property
        Public Property COAOrder() As String

            Get
                Return m_COAOrder
            End Get

            Set(ByVal setval As String)
                m_COAOrder = setval
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
        Public Property LedgerID() As String

            Get
                Return m_LedgerID
            End Get

            Set(ByVal setval As String)
                m_LedgerID = setval
            End Set

        End Property

        Public Property MCActive() As Short

            Get
                Return m_MCActive
            End Get

            Set(ByVal setval As Short)
                m_MCActive = setval
            End Set

        End Property
        Public Property Mult_Cpny_Db() As Short

            Get
                Return m_Mult_Cpny_DB
            End Get

            Set(ByVal setval As Short)
                m_Mult_Cpny_DB = setval
            End Set

        End Property
        Public Property NbrPer() As Short

            Get
                Return m_NbrPer
            End Get

            Set(ByVal setval As Short)
                m_NbrPer = setval
            End Set

        End Property

        Public Property PerNbr() As String

            Get
                Return m_PerNbr
            End Get

            Set(ByVal setval As String)
                m_PerNbr = setval
            End Set

        End Property
        Public Property PerRetHist() As Short

            Get
                Return m_PerRetHist
            End Get

            Set(ByVal setval As Short)
                m_PerRetHist = setval
            End Set

        End Property
        Public Property PerRetModTran() As Short

            Get
                Return m_PerRetModTran
            End Get

            Set(ByVal setval As Short)
                m_PerRetModTran = setval
            End Set

        End Property
        Public Property PerRetTran() As Short

            Get
                Return m_PerRetTran
            End Get

            Set(ByVal setval As Short)
                m_PerRetTran = setval
            End Set

        End Property

        Public Property RetEarnAcct() As String

            Get
                Return m_RetEarnAcct
            End Get

            Set(ByVal setval As String)
                m_RetEarnAcct = setval
            End Set

        End Property

        Public Property ValidateAcctSub() As Short

            Get
                Return m_ValidateAcctSub
            End Get

            Set(ByVal setval As Short)
                m_ValidateAcctSub = setval
            End Set

        End Property


        Public Property YtdNetIncAcct() As String

            Get
                Return m_YtdNetIncAcct
            End Get

            Set(ByVal setval As String)
                m_YtdNetIncAcct = setval
            End Set

        End Property


    End Class
    Public bGLSetupInfo As GLSetupCls = New GLSetupCls, nGLSetupInfo As GLSetupCls = New GLSetupCls

    Public Sub SetGLSetupValues(sqlReader As SqlDataReader, GLSetupBuf As GLSetupCls)
        Try
            GLSetupBuf.BaseCuryID = sqlReader("BaseCuryID")
            GLSetupBuf.Central_Cash_Cntl = sqlReader("Central_Cash_Cntl")
            GLSetupBuf.COAOrder = sqlReader("COAOrder")
            GLSetupBuf.CpnyId = sqlReader("CpnyId")
            GLSetupBuf.LedgerID = sqlReader("LedgerID")
            GLSetupBuf.MCActive = sqlReader("MCActive")
            GLSetupBuf.Mult_Cpny_Db = sqlReader("Mult_Cpny_Db")
            GLSetupBuf.NbrPer = sqlReader("NbrPer")
            GLSetupBuf.PerNbr = sqlReader("PerNbr")
            GLSetupBuf.PerRetHist = sqlReader("PerRetHist")
            GLSetupBuf.PerRetModTran = sqlReader("PerRetModTran")
            GLSetupBuf.PerRetTran = sqlReader("PerRetTran")
            GLSetupBuf.RetEarnAcct = sqlReader("RetEarnAcct")
            GLSetupBuf.ValidateAcctSub = sqlReader("ValidateAcctSub")
            GLSetupBuf.YtdNetIncAcct = sqlReader("YtdNetIncAcct")



        Catch ex As Exception
            GLSetupBuf.BaseCuryID = String.Empty
            GLSetupBuf.Central_Cash_Cntl = 0
            GLSetupBuf.COAOrder = String.Empty
            GLSetupBuf.LedgerID = String.Empty
            GLSetupBuf.MCActive = 0
            GLSetupBuf.Mult_Cpny_Db = 0
            GLSetupBuf.NbrPer = 0
            GLSetupBuf.PerNbr = String.Empty
            GLSetupBuf.PerRetHist = 0
            GLSetupBuf.PerRetModTran = 0
            GLSetupBuf.PerRetTran = 0
            GLSetupBuf.RetEarnAcct = String.Empty
            GLSetupBuf.ValidateAcctSub = 0
            GLSetupBuf.YtdNetIncAcct = String.Empty

        End Try
    End Sub

    Public Sub InitGLSetupValues(GLSetupBuf As GLSetupCls)
        GLSetupBuf.BaseCuryID = String.Empty
        GLSetupBuf.Central_Cash_Cntl = 0
        GLSetupBuf.COAOrder = String.Empty
        GLSetupBuf.LedgerID = String.Empty
        GLSetupBuf.MCActive = 0
        GLSetupBuf.Mult_Cpny_Db = 0
        GLSetupBuf.NbrPer = 0
        GLSetupBuf.PerNbr = String.Empty
        GLSetupBuf.PerRetHist = 0
        GLSetupBuf.PerRetModTran = 0
        GLSetupBuf.PerRetTran = 0
        GLSetupBuf.RetEarnAcct = String.Empty
        GLSetupBuf.ValidateAcctSub = 0
        GLSetupBuf.YtdNetIncAcct = String.Empty
    End Sub

End Module
