Imports System.Data.SqlClient
Module PCSETUP
	
	Public Class PCSetupCls

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
    Public bPCSetupInfo As PCSetupCls = New PCSetupCls, nPCSetupInfo As PCSetupCls = New PCSetupCls

    Public Sub SetPCSetupValues(sqlReader As SqlDataReader, PCSetupInfoBuf As PCSetupCls)
        Try
            PCSetupInfoBuf.PerNbr = sqlReader("PerNbr")
        Catch ex As Exception
            PCSetupInfoBuf.PerNbr = String.Empty

        End Try
    End Sub
End Module
