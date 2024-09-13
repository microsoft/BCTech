Imports System.Data.SqlClient
Module PJCONTRL

    Public Class PJContrlCls

        Private m_Control_Data As String


        Public Property Control_Data() As String

            Get
                Return m_Control_Data
            End Get

            Set(ByVal setval As String)
                m_Control_Data = setval
            End Set

        End Property



    End Class
    Public bPJContrlInfo As PJContrlCls = New PJContrlCls, nPContrlInfo As PJContrlCls = New PJContrlCls

    Public Sub SetPJContrlValues(sqlReader As SqlDataReader, PJControlInfoBuf As PJContrlCls)
        Try
            bPJContrlInfo.Control_Data = sqlReader("Control_Data")
        Catch ex As Exception
            bPJContrlInfo.Control_Data = String.Empty
        End Try
    End Sub

End Module
