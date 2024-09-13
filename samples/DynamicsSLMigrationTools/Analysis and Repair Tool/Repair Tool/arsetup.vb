Imports System.Data.SqlClient
Module ARSETUP

    Public Class ARSetupCls

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
    Public bARSetupInfo As ARSetupCls = New ARSetupCls, nARSetupInfo As ARSetupCls = New ARSetupCls

    Public Sub SetARSetupValues(sqlReader As SqlDataReader, ARSetupBuf As ARSetupCls)
        Try
            ARSetupBuf.PerNbr = sqlReader("PerNbr")
        Catch
            ARSetupBuf.PerNbr = String.Empty
        End Try

    End Sub



End Module
