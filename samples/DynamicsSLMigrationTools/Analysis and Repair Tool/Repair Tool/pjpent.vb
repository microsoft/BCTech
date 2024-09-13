Imports System.Data.SqlClient
Module PJPENT
    Private m_PJT_Entity As String
    Private m_Project As String
    Private m_Status_Pa As String


    Public Class PJPENTCls

        Public Property PJT_entity() As String

            Get
                Return m_PJT_Entity
            End Get

            Set(ByVal setval As String)
                m_PJT_Entity = setval
            End Set

        End Property


        Public Property Project() As String

            Get
                Return m_Project
            End Get

            Set(ByVal setval As String)
                m_Project = setval
            End Set

        End Property

        Public Property Status_PA() As String

            Get
                Return m_Status_Pa
            End Get

            Set(ByVal setval As String)
                m_Status_Pa = setval
            End Set

        End Property




    End Class
    Public bPJPENTInfo As PJPENTCls = New PJPENTCls, nPJPENT As PJPENTCls = New PJPENTCls

    Public Sub SetPJPENTValues(sqlReader As SqlDataReader, PJPEntBuf As PJPENTCls)
        Try

            PJPEntBuf.PJT_entity = sqlReader("PJT_Entity")
            PJPEntBuf.Project = sqlReader("Project")
            PJPEntBuf.Status_PA = sqlReader("Status_PA")
        Catch ex As Exception
            PJPEntBuf.PJT_entity = sqlReader("PJT_Entity")
            PJPEntBuf.Project = sqlReader("Project")
            PJPEntBuf.Status_PA = sqlReader("Status_PA")

        End Try
    End Sub
End Module
