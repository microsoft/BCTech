Imports System.Data.SqlClient



Module Itemcost

    Public Class ItemCostCls

        Private m_InvtId As String
        Private m_LayerType As String
        Private m_RcptDate As DateTime
        Private m_RcptNbr As String
        Private m_SpecificCostID As String
        Private m_SiteID As String


        Public Property InvtID() As String

            Get
                Return m_InvtId
            End Get

            Set(ByVal setval As String)
                m_InvtId = setval
            End Set

        End Property

        Public Property LayerType() As String

            Get
                Return m_LayerType
            End Get

            Set(ByVal setval As String)
                m_LayerType = setval
            End Set

        End Property
        Public Property RcptDate() As DateTime

            Get
                Return m_RcptDate
            End Get

            Set(ByVal setval As DateTime)
                m_RcptDate = setval
            End Set

        End Property

        Public Property RcptNbr() As String

            Get
                Return m_RcptNbr
            End Get

            Set(ByVal setval As String)
                m_RcptNbr = setval
            End Set

        End Property
        Public Property SiteID() As String

            Get
                Return m_SiteID
            End Get

            Set(ByVal setval As String)
                m_SiteID = setval
            End Set

        End Property

        Public Property SpecificCostID() As String

            Get
                Return m_SpecificCostID
            End Get

            Set(ByVal setval As String)
                m_SpecificCostID = setval
            End Set

        End Property



    End Class
    Public bItemCostInfo As ItemCostCls = New ItemCostCls, nItemCostInfo As ItemCostCls = New ItemCostCls

    Public Sub SetItemCostValues(sqlReader As SqlDataReader, ItemCostInfoBuf As ItemCostCls)
        Try
            ItemCostInfoBuf.InvtID = sqlReader("InvtId")
            ItemCostInfoBuf.LayerType = sqlReader("LayerType")
            ItemCostInfoBuf.RcptDate = sqlReader("RcptDate")
            ItemCostInfoBuf.RcptNbr = sqlReader("RcptNbr")
            ItemCostInfoBuf.SiteID = sqlReader("SiteId")
            ItemCostInfoBuf.SpecificCostID = sqlReader("SpecificCostID")
        Catch ex As Exception
            ItemCostInfoBuf.InvtID = String.Empty
            ItemCostInfoBuf.LayerType = String.Empty
            ItemCostInfoBuf.RcptDate = sqlReader("RcptDate")
            ItemCostInfoBuf.RcptNbr = MinDateValue
            ItemCostInfoBuf.SiteID = String.Empty
            ItemCostInfoBuf.SpecificCostID = String.Empty
        End Try
    End Sub
End Module
