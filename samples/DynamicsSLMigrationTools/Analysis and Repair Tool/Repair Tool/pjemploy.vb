Imports System.Data.SqlClient
Module PJEMPLOY

    Public Class PJEMPLOYCls

        Private m_Employee As String
        Private m_em_01 As String
        Public Property employee() As String

            Get
                Return m_Employee
            End Get

            Set(ByVal setval As String)
                m_Employee = setval
            End Set

        End Property


        Public Property em_id01() As String

            Get
                Return m_em_01
            End Get

            Set(ByVal setval As String)
                m_em_01 = setval
            End Set

        End Property



    End Class
    Public bPJEMPLOYInfo As PJEMPLOYCls = New PJEMPLOYCls, nPJEMPLOYInfo As PJEMPLOYCls = New PJEMPLOYCls


    Public Sub SetPJEmployee_Values(sqlReader As SqlDataReader, PJEmployBuff As PJEMPLOYCls)
        Try
            PJEmployBuff.employee = sqlReader("Employee")
            PJEmployBuff.em_id01 = sqlReader("em_id01")
        Catch ex As Exception
            PJEmployBuff.employee = String.Empty
            PJEmployBuff.em_id01 = String.Empty

        End Try
    End Sub
End Module
