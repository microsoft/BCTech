'
Imports System.Data.SqlClient
Module Location

    Public Class LocationCls
        Private m_InvtId As String
        Private m_SiteId As String
        Private m_WhseLoc As String

        Public Property InvtID() As String

            Get
                Return m_InvtId
            End Get

            Set(ByVal setval As String)
                m_InvtId = setval
            End Set

        End Property


        Public Property SiteID() As String

            Get
                Return m_SiteId
            End Get

            Set(ByVal setval As String)
                m_SiteId = setval
            End Set

        End Property



        Public Property WhseLoc() As String

            Get
                Return m_WhseLoc
            End Get

            Set(ByVal setval As String)
                m_WhseLoc = setval
            End Set

        End Property

    End Class
    Public bLocationInfo As LocationCls = New LocationCls, nLocationInfo As LocationCls = New LocationCls

    Public Sub SetLocationValues(sqlReader As SqlDataReader, LocationBuf As LocationCls)
        Try
            LocationBuf.InvtID = sqlReader("InvtId")
            LocationBuf.SiteID = sqlReader("SiteId")
            LocationBuf.WhseLoc = sqlReader("WhseLoc")
        Catch ex As Exception
            LocationBuf.InvtID = String.Empty
            LocationBuf.SiteID = sqlReader("SiteId")
            LocationBuf.WhseLoc = sqlReader("WhseLoc")

        End Try
    End Sub
End Module
