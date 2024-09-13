Imports System.Data.SqlClient
Module GLSETUP

    Public Class GLSetupCls

        Private m_BaseCuryID As String
        Private m_LedgerID As String
        Private m_NbrPer As Short
        Private m_PerNbr As String
        Private m_RetEarnAcct As String
        Private m_YtdNetIncAcct As String

        Public Property BaseCuryID() As String

            Get
                Return m_BaseCuryID
            End Get

            Set(ByVal setval As String)
                m_BaseCuryID = setval
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

        Public Property RetEarnAcct() As String

            Get
                Return m_RetEarnAcct
            End Get

            Set(ByVal setval As String)
                m_RetEarnAcct = setval
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
            GLSetupBuf.LedgerID = sqlReader("LedgerID")
            GLSetupBuf.NbrPer = sqlReader("NbrPer")
            GLSetupBuf.PerNbr = sqlReader("PerNbr")
            GLSetupBuf.RetEarnAcct = sqlReader("RetEarnAcct")
            GLSetupBuf.YtdNetIncAcct = sqlReader("YtdNetIncAcct")
        Catch ex As Exception
            GLSetupBuf.BaseCuryID = String.Empty
            GLSetupBuf.LedgerID = String.Empty
            GLSetupBuf.NbrPer = 0
            GLSetupBuf.PerNbr = String.Empty
            GLSetupBuf.RetEarnAcct = String.Empty
            GLSetupBuf.YtdNetIncAcct = String.Empty

        End Try
    End Sub
End Module
