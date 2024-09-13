Imports System.Data.SqlClient
Module LotSerMst
	
	Public Class LotSerMstCls
        Private m_InvtID As String
        Private m_LotSerNbr As String
        Private m_SiteID As String
        Private m_WhseLoc As String


        Public Property InvtID() As String

	    Get
                Return m_InvtID
            End Get

	    Set(ByVal setval As String)
                m_InvtID = setval
            End Set

	End Property


        Public Property LotSerNbr() As String

	    Get
                Return m_LotSerNbr
            End Get

	    Set(ByVal setval As String)
                m_LotSerNbr = setval
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

        Public Property WhseLoc() As String

	    Get
                Return m_WhseLoc
            End Get

	    Set(ByVal setval As String)
                m_WhseLoc = setval
            End Set

	End Property

	End Class
    Public bLotSerMstInfo As LotSerMstCls = New LotSerMstCls, nLotSerMstInfo As LotSerMstCls = New LotSerMstCls

    Public Sub SetLotSerMstValues(sqlReader As SqlDataReader, LotSerMstBuf As LotSerMstCls)

        Try
            LotSerMstBuf.InvtID = sqlReader("InvtId")
            LotSerMstBuf.LotSerNbr = sqlReader("LotSerNbr")
            LotSerMstBuf.SiteID = sqlReader("SiteId")
            LotSerMstBuf.WhseLoc = sqlReader("WhseLoc")
        Catch ex As Exception
            LotSerMstBuf.InvtID = String.Empty
            LotSerMstBuf.LotSerNbr = String.Empty
            LotSerMstBuf.SiteID = String.Empty
            LotSerMstBuf.WhseLoc = String.Empty

        End Try
    End Sub
End Module
