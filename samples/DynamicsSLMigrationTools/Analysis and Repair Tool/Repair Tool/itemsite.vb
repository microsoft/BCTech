Imports System.Data.SqlClient
Module itemsite

    Public Class ItemSiteCls
        Private m_InvtId As String
        Private m_QtyOnHand As Double
        Private m_SiteId As String
        Private m_TotCost As Double
        Public Property InvtID() As String

            Get
                Return m_InvtId
            End Get

            Set(ByVal setval As String)
                m_InvtId = setval
            End Set

        End Property
        Public Property QtyOnHand() As Double

            Get
                Return m_QtyOnHand
            End Get

            Set(ByVal setval As Double)
                m_QtyOnHand = setval
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
        Public Property TotCost() As Double

            Get
                Return m_TotCost
            End Get

            Set(ByVal setval As Double)
                m_TotCost = setval
            End Set

        End Property

    End Class
    Public bItemSiteInfo As ItemSiteCls = New ItemSiteCls, nItemSiteInfo As ItemSiteCls = New ItemSiteCls

    Public Sub SetItemSiteValues(sqlReader As SqlDataReader, ItemSiteBuf As ItemSiteCls)
        Try
            ItemSiteBuf.InvtID = sqlReader("InvtId")
            ItemSiteBuf.QtyOnHand = sqlReader("QtyOnHand")
            ItemSiteBuf.SiteID = sqlReader("SiteId")
            ItemSiteBuf.TotCost = sqlReader("TotCost")

        Catch ex As Exception
            ItemSiteBuf.InvtID = String.Empty
            ItemSiteBuf.QtyOnHand = 0.0
            ItemSiteBuf.SiteID = String.Empty
            ItemSiteBuf.TotCost = 0.0

        End Try
    End Sub

End Module