Imports System.Data.SqlClient
Module ARSETUP

    Public Class ARSetupCls

        Private m_PerNbr As String
        Private m_PerRetHist As Short
        Private m_PerRetTran As Short

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

        Public Property PerRetTran() As Short

            Get
                Return m_PerRetTran
            End Get

            Set(ByVal setval As Short)
                m_PerRetTran = setval
            End Set

        End Property


    End Class
    Public bARSetupInfo As ARSetupCls = New ARSetupCls, nARSetupInfo As ARSetupCls = New ARSetupCls

    Public Sub SetARSetupValues(sqlReader As SqlDataReader, ARSetupBuf As ARSetupCls)
        Try
            ARSetupBuf.PerNbr = sqlReader("PerNbr")
            ARSetupBuf.PerRetHist = sqlReader("PerRetHist")
            ARSetupBuf.PerRetTran = sqlReader("PerRetTran")
        Catch
            ARSetupBuf.PerNbr = String.Empty
            ARSetupBuf.PerRetHist = 0
            ARSetupBuf.PerRetTran = 0
        End Try

    End Sub



End Module
