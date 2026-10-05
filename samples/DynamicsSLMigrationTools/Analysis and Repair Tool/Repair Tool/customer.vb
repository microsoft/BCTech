Imports System.Data.SqlClient
Module CUSTOMER

    Public Class Customercls

        Private m_CustId As String
        Private m_CustName As String
        Private m_EMailAddr As String
        Private m_Phone As String

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

        Public Property EMailAddr() As String

            Get
                Return m_EMailAddr
            End Get

            Set(ByVal setval As String)
                m_EMailAddr = setval
            End Set

        End Property

        Public Property Phone() As String

            Get
                Return m_Phone
            End Get

            Set(ByVal setval As String)
                m_Phone = setval
            End Set

        End Property

    End Class
    Public bCustomerInfo As Customercls = New Customercls, nCustomerInfo As Customercls = New Customercls


    Public Sub SetCustomerValues(sqlReader As SqlDataReader, CustomerBuf As Customercls)
        Try
            CustomerBuf.CustId = sqlReader("CustId")
            CustomerBuf.CustName = sqlReader("Name")
            CustomerBuf.EMailAddr = sqlReader("EMailAddr")
            CustomerBuf.Phone = sqlReader("Phone")
        Catch ex As Exception
            CustomerBuf.CustId = String.Empty
            CustomerBuf.CustName = String.Empty
            CustomerBuf.EMailAddr = String.Empty
            CustomerBuf.Phone = String.Empty
        End Try
    End Sub
End Module
