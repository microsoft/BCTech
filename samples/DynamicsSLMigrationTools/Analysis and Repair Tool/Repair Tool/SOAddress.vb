Imports System.Data.SqlClient
Module SOADDRESS

    Public Class SOAddresscls

        Private m_CustId As String
        Private m_ShipToId As String
        Private m_EmailAddr As String

        Public Property CustId() As String

            Get
                Return m_CustId
            End Get

            Set(ByVal setval As String)
                m_CustId = setval
            End Set

        End Property

        Public Property ShipToId() As String

            Get
                Return m_ShipToId
            End Get

            Set(ByVal setval As String)
                m_ShipToId = setval
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


    End Class
    Public bSOAddressInfo As SOAddresscls = New SOAddresscls, nSOAddressInfo As SOAddresscls = New SOAddresscls


    Public Sub SetSOAddressValues(sqlReader As SqlDataReader, SOAddressBuf As SOAddresscls)
        Try
            SOAddressBuf.CustId = sqlReader("CustId")
            SOAddressBuf.ShipToId = sqlReader("ShipToId")
            SOAddressBuf.EmailAddr = sqlReader("EmailAddr")
        Catch ex As Exception
            SOAddressBuf.CustId = String.Empty
            SOAddressBuf.ShipToId = String.Empty
            SOAddressBuf.EmailAddr = String.Empty
        End Try
    End Sub
End Module