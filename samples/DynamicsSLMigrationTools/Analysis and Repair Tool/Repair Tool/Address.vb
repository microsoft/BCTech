Imports System.Data.SqlClient

Module ADDRESS

    Public Class Addresscls

        Private m_AddrId As String
        Private m_Phone As String

        Public Property AddrId() As String

            Get
                Return m_AddrId
            End Get

            Set(ByVal setval As String)
                m_AddrId = setval
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

    Public bAddressInfo As Addresscls = New Addresscls, nAddressInfo As Addresscls = New Addresscls

    Public Sub SetAddressValues(sqlReader As SqlDataReader, AddressBuf As Addresscls)
        Try
            AddressBuf.AddrId = sqlReader("AddrId")
            AddressBuf.Phone = sqlReader("Phone")
        Catch ex As Exception
            AddressBuf.AddrId = String.Empty
            AddressBuf.Phone = String.Empty
        End Try
    End Sub

End Module