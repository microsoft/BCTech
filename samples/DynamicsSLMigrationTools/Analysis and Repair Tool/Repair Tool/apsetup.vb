Imports System.Data.SqlClient
Module APSETUP

    Public Class APSetupCls

        Private m_CurrPernbr As String
        Private m_PerNbr As String

        Public Property CurrPerNbr() As String

            Get
                Return m_CurrPernbr
            End Get

            Set(ByVal setval As String)
                m_CurrPernbr = setval
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

    End Class
    Public bAPSetupInfo As APSetupCls = New APSetupCls, nAPSetupInfo As APSetupCls = New APSetupCls

    Public Sub SetAPSetupValues(sqlReader As SqlDataReader, APSetupBuf As APSetupCls)

        Try
            APSetupBuf.CurrPerNbr = sqlReader("CurrPerNbr")
            APSetupBuf.PerNbr = sqlReader("PerNbr")
        Catch ex As Exception
            APSetupBuf.CurrPerNbr = String.Empty
            APSetupBuf.PerNbr = String.Empty

        End Try
    End Sub
End Module
