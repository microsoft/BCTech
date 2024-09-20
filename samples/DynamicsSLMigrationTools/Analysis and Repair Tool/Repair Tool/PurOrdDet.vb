Imports System.Data.SqlClient
Module PurOrdDet
	
	Public Class PurOrdDetCls
        Private m_InvtId As String
        Private m_LineRef As String
        Private m_PONbr As String


        Public Property InvtID() As String

            Get
                Return m_InvtId
            End Get

            Set(ByVal setval As String)
                m_InvtId = setval
            End Set

        End Property
        Public Property LineRef() As String

            Get
                Return m_LineRef
            End Get

            Set(ByVal setval As String)
                m_LineRef = setval
            End Set

        End Property
        Public Property PONbr() As String

            Get
                Return m_PONbr
            End Get

            Set(ByVal setval As String)
                m_PONbr = setval
            End Set

        End Property



    End Class
    Public bPurOrdDetInfo As PurOrdDetCls = New PurOrdDetCls, nPurOrdDet As PurOrdDetCls = New PurOrdDetCls

    Public Sub SetPurOrdDetValues(sqlReader As SqlDataReader, PurOrdDetBuf As PurOrdDetCls)
        Try
            PurOrdDetBuf.InvtID = sqlReader("InvtId")
            PurOrdDetBuf.LineRef = sqlReader("LineRef")
            PurOrdDetBuf.PONbr = sqlReader("PONbr")
        Catch ex As Exception
            PurOrdDetBuf.InvtID = String.Empty
            PurOrdDetBuf.LineRef = String.Empty
            PurOrdDetBuf.PONbr = String.Empty

        End Try
    End Sub
End Module
