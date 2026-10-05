Imports System.Data.SqlClient
Module PJADDR

    Public Class PJAddrcls

        Private m_addr_key_cd As String
        Private m_addr_key As String
        Private m_addr_type_cd As String
        Private m_email As String
        Private m_phone As String

        Public Property addr_key_cd() As String

            Get
                Return m_addr_key_cd
            End Get

            Set(ByVal setval As String)
                m_addr_key_cd = setval
            End Set

        End Property

        Public Property addr_key() As String

            Get
                Return m_addr_key
            End Get

            Set(ByVal setval As String)
                m_addr_key = setval
            End Set

        End Property

        Public Property addr_type_cd() As String

            Get
                Return m_addr_type_cd
            End Get

            Set(ByVal setval As String)
                m_addr_type_cd = setval
            End Set

        End Property

        Public Property email() As String

            Get
                Return m_email
            End Get

            Set(ByVal setval As String)
                m_email = setval
            End Set

        End Property

        Public Property phone() As String

            Get
                Return m_phone
            End Get

            Set(ByVal setval As String)
                m_phone = setval
            End Set

        End Property



    End Class
    Public bPJAddrInfo As PJAddrcls = New PJAddrcls, nPJAddrInfo As PJAddrcls = New PJAddrcls

    Public Sub SetPJAddrValues(sqlReader As SqlDataReader, PJAddrBuf As PJAddrcls)
        Try
            PJAddrBuf.addr_key_cd = sqlReader("addr_key_cd")
            PJAddrBuf.addr_key = sqlReader("addr_key")
            PJAddrBuf.addr_type_cd = sqlReader("addr_type_cd")
            PJAddrBuf.email = sqlReader("email")
            PJAddrBuf.phone = sqlReader("phone")

        Catch
            PJAddrBuf.addr_key_cd = String.Empty
            PJAddrBuf.addr_key = String.Empty
            PJAddrBuf.addr_type_cd = String.Empty
            PJAddrBuf.email = String.Empty
            PJAddrBuf.phone = String.Empty

        End Try

    End Sub
End Module