Imports System.Data.SqlClient
Module CUSTOMER

    Public Class Customercls

        Private m_CustId As String
        Private m_CustName As String

        Public Property CustId() As String

            Get
                Return m_CustId
            End Get

            Set(ByVal setval As String)
                m_CustId = setval
            End Set

        End Property

        Public Property CustName() As String

            Get
                Return m_CustName
            End Get

            Set(ByVal setval As String)
                m_CustName = setval
            End Set

        End Property


    End Class
    Public bCustomerInfo As Customercls = New Customercls, nCustomerInfo As Customercls = New Customercls


    Public Sub SetCustomerValues(sqlReader As SqlDataReader, CustomerBuf As Customercls)
        Try
            CustomerBuf.CustId = sqlReader("CustId")
            CustomerBuf.CustName = sqlReader("Name")
        Catch ex As Exception
            CustomerBuf.CustId = String.Empty
            CustomerBuf.CustName = String.Empty
        End Try
    End Sub
End Module
