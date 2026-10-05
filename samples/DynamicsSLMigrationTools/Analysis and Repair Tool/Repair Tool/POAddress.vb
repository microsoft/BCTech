Imports System.Data.SqlClient
Module POADDRESS

    Public Class POAddresscls

        Private m_VendId As String
        Private m_OrdFromId As String
        Private m_EmailAddr As String
        Private m_Phone As String

        Public Property VendId() As String

            Get
                Return m_VendId
            End Get

            Set(ByVal setval As String)
                m_VendId = setval
            End Set

        End Property

        Public Property OrdFromId() As String

            Get
                Return m_OrdFromId
            End Get

            Set(ByVal setval As String)
                m_OrdFromId = setval
            End Set

        End Property

        Public Property EmailAddr() As String

            Get
                Return m_EmailAddr
            End Get

            Set(ByVal setval As String)
                m_EmailAddr = setval
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
    Public bPOAddressInfo As POAddresscls = New POAddresscls, nPOAddressInfo As POAddresscls = New POAddresscls

    Public Sub SetPOAddressValues(sqlReader As SqlDataReader, POAddressBuf As POAddresscls)
        Try
            POAddressBuf.VendId = sqlReader("VendId")
            POAddressBuf.OrdFromId = sqlReader("OrdFromId")
            POAddressBuf.EmailAddr = sqlReader("EmailAddr")
            POAddressBuf.Phone = sqlReader("Phone")

        Catch
            POAddressBuf.VendId = String.Empty
            POAddressBuf.OrdFromId = String.Empty
            POAddressBuf.EmailAddr = String.Empty
            POAddressBuf.Phone = String.Empty

        End Try

    End Sub
End Module