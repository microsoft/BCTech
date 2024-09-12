Imports System.Data.SqlClient
Module INSetupCls

    Public Class INSetupCls

        Private m_PerNbr As String


        Public Property PerNbr() As String

            Get
                Return m_PerNbr
            End Get

            Set(ByVal setval As String)
                m_PerNbr = setval
            End Set

        End Property


    End Class
    Public bINSetupInfo As INSetupCls = New INSetupCls, nINSetupInfo As INSetupCls = New INSetupCls

    Public Sub SetINSetupValues(sqlReader As SqlDataReader, INSetupBuf As INSetupCls)
        Try
            INSetupBuf.PerNbr = sqlReader("PerNbr")
        Catch ex As Exception
            INSetupBuf.PerNbr = String.Empty

        End Try
    End Sub
End Module
