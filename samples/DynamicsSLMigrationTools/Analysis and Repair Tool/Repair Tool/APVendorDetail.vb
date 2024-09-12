Imports System.Data.SqlClient
Module APVendorDetail
    Private m_VendID As String
    Private m_RefNbr As String
    Private m_DocType As String
    Private m_PerPost As String
    Private m_InvcDate As DateTime


    Public Class APVendorDetailCls


        Public Property VendID() As String

            Get
                Return m_VendID
            End Get

            Set(ByVal setval As String)
                m_VendID = setval
            End Set

        End Property

        Public Property RefNbr() As String

            Get
                Return m_RefNbr
            End Get

            Set(ByVal setval As String)
                m_RefNbr = setval
            End Set

        End Property



        Public Property DocType() As String

            Get
                Return m_DocType
            End Get

            Set(ByVal setval As String)
                m_DocType = setval
            End Set

        End Property

        Public Property PerPost() As String

            Get
                Return m_PerPost
            End Get

            Set(ByVal setval As String)
                m_PerPost = setval
            End Set

        End Property

        Public Property InvcDate() As DateTime

            Get
                Return m_InvcDate
            End Get

            Set(ByVal setval As DateTime)
                m_InvcDate = setval
            End Set

        End Property

    End Class

    Public bAPVendorDetailInfo As APVendorDetailCls = New APVendorDetailCls, nAPVendorDetailInfo As APVendorDetailCls = New APVendorDetailCls

    Public Sub SetAPVendorDetailValues(sqlReader As SqlDataReader, APVendorDetailBuf As APVendorDetailCls)
        Try
            APVendorDetailBuf.DocType = sqlReader("DocType")
            APVendorDetailBuf.PerPost = sqlReader("PerPost")
            APVendorDetailBuf.RefNbr = sqlReader("RefNbr")
            APVendorDetailBuf.VendID = sqlReader("VendID")
            APVendorDetailBuf.InvcDate = sqlReader("InvcDate")
        Catch ex As Exception
            APVendorDetailBuf.DocType = String.Empty
            APVendorDetailBuf.PerPost = String.Empty
            APVendorDetailBuf.RefNbr = String.Empty
            APVendorDetailBuf.VendID = String.Empty
            APVendorDetailBuf.InvcDate = MinDateValue

        End Try
    End Sub

End Module
