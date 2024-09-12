Imports System.Data.SqlClient
Module PJPROJ

    Public Class PJPROJCls
        Private m_Project As String
        Private m_Status_pa As String

        Public Property Project() As String

            Get
                Return m_Project
            End Get

            Set(ByVal setval As String)
                m_Project = setval
            End Set

        End Property


        Public Property Status_pa() As String

            Get
                Return m_Status_pa
            End Get

            Set(ByVal setval As String)
                m_Status_pa = setval
            End Set

        End Property



    End Class
    Public bPJPROJInfo As PJPROJCls = New PJPROJCls, nPJPROJInfo As PJPROJCls = New PJPROJCls

    Public Sub SetPJProjValues(sqlReader As SqlDataReader, PJProjBuf As PJPROJCls)
        Try
            PJProjBuf.Project = sqlReader("project")
            PJProjBuf.Status_pa = sqlReader("status_pa")
        Catch ex As Exception
            PJProjBuf.Project = String.Empty
            PJProjBuf.Status_pa = String.Empty

        End Try
    End Sub
End Module
