Imports System.Data.SqlClient

Module SITE

    Public Class Sitecls

        Private m_SiteId As String
        Private m_Phone As String

        Public Property SiteId() As String

            Get
                Return m_SiteId
            End Get

            Set(ByVal setval As String)
                m_SiteId = setval
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

    Public bSiteInfo As Sitecls = New Sitecls, nSiteInfo As Sitecls = New Sitecls

    Public Sub SetSiteValues(sqlReader As SqlDataReader, SiteBuf As Sitecls)
        Try
            SiteBuf.SiteId = sqlReader("SiteId")
            SiteBuf.Phone = sqlReader("Phone")
        Catch ex As Exception
            SiteBuf.SiteId = String.Empty
            SiteBuf.Phone = String.Empty
        End Try
    End Sub

End Module