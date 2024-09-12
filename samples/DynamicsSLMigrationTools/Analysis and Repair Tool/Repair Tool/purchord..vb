Imports System.Data.SqlClient
Module PURCHORD

    Public Class PurchOrdCls

        Private m_PONbr As String
        Private m_POType As String
        Private m_Status As String
        Private m_VendId As String

        Public Property PONbr() As String

            Get
                Return m_PONbr
            End Get

            Set(ByVal setval As String)
                m_PONbr = setval
            End Set

        End Property
        Public Property POType() As String

            Get
                Return m_POType
            End Get

            Set(ByVal setval As String)
                m_POType = setval
            End Set

        End Property

        Public Property Status() As String

            Get
                Return m_Status
            End Get

            Set(ByVal setval As String)
                m_Status = setval
            End Set

        End Property

        Public Property VendID() As String

            Get
                Return m_VendId
            End Get

            Set(ByVal setval As String)
                m_VendId = setval
            End Set

        End Property



    End Class
    Public bPurchOrdInfo As PurchOrdCls = New PurchOrdCls, nPurchOrdInfo As PurchOrdCls = New PurchOrdCls

    Public Sub SetPurchOrdValues(sqlReader As SqlDataReader, PurchOrdBuf As PurchOrdCls)
        Try
            PurchOrdBuf.PONbr = sqlReader("PONbr")
            PurchOrdBuf.POType = sqlReader("POType")
            PurchOrdBuf.Status = sqlReader("Status")
            PurchOrdBuf.VendID = sqlReader("VendId")

        Catch
            PurchOrdBuf.PONbr = String.Empty
            PurchOrdBuf.POType = String.Empty
            PurchOrdBuf.Status = String.Empty
            PurchOrdBuf.VendID = String.Empty

        End Try

    End Sub
End Module
