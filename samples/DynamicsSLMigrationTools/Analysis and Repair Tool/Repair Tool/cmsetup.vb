Imports System.Data.SqlClient
Module CMSETUP
    Public Class CMSETUPCls

        Private m_MCActivated As Short


        Public Property MCActivated() As Short

            Get
                Return m_MCActivated
            End Get

            Set(ByVal setval As Short)
                m_MCActivated = setval
            End Set

        End Property


    End Class
    Public bCMSETUPInfo As CMSETUPCls = New CMSETUPCls, nCMSETUPInfo As CMSETUPCls = New CMSETUPCls

    Public Sub SetCMSetupValues(sqlReader As SqlDataReader, CMSetupBuf As CMSETUPCls)
        Try
            CMSetupBuf.MCActivated = sqlReader("MCActivated")
        Catch ex As Exception
            CMSetupBuf.MCActivated = 0

        End Try
    End Sub
End Module
