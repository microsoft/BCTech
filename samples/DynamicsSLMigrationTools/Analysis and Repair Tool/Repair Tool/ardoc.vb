
Imports System.Data.SqlClient
Module ARDOC

    Public Class ARDocCls
        Private m_CustId As String
        Private m_DocBal As Double
        Private m_DocDate As DateTime
        Private m_DocType As String
        Private m_OrigDocAmt As Double
        Private m_PerPost As String
        Private m_RefNbr As String
        Private m_Terms As String

        Public Property CustId() As String

            Get
                Return m_CustId
            End Get

            Set(ByVal setval As String)
                m_CustId = setval
            End Set

        End Property

        Public Property DocBal() As Double

            Get
                Return m_DocBal
            End Get

            Set(ByVal setval As Double)
                m_DocBal = setval
            End Set

        End Property
        Public Property DocDate() As DateTime

            Get
                Return m_DocDate
            End Get

            Set(ByVal setval As DateTime)
                m_DocDate = setval
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


        Public Property OrigDocAmt() As Double

            Get
                Return m_OrigDocAmt
            End Get

            Set(ByVal setval As Double)
                m_OrigDocAmt = setval
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

        Public Property RefNbr() As String

            Get
                Return m_RefNbr
            End Get

            Set(ByVal setval As String)
                m_RefNbr = setval
            End Set

        End Property

        Public Property Terms() As String

            Get
                Return m_Terms
            End Get

            Set(ByVal setval As String)
                m_Terms = setval
            End Set

        End Property

    End Class
    Public bARDocInfo As ARDocCls = New ARDocCls, nARDocInfo As ARDocCls = New ARDocCls

    Public Sub SetARDocValues(sqlReader As SqlDataReader, ARDocBuf As ARDocCls)
        Try
            ARDocBuf.CustId = sqlReader("CustId")
            ARDocBuf.DocBal = sqlReader("DocBal")
            ARDocBuf.DocDate = sqlReader("DocDate")
            ARDocBuf.DocType = sqlReader("DocType")
            ARDocBuf.OrigDocAmt = sqlReader("OrigDocAmt")
            ARDocBuf.PerPost = sqlReader("PerPost")
            ARDocBuf.RefNbr = sqlReader("RefNbr")
            ARDocBuf.Terms = sqlReader("Terms")

        Catch ex As Exception
            ARDocBuf.CustId = String.Empty
            ARDocBuf.DocBal = 0.0
            ARDocBuf.DocDate = MinDateValue
            ARDocBuf.DocType = String.Empty
            ARDocBuf.OrigDocAmt = 0.0
            ARDocBuf.PerPost = String.Empty
            ARDocBuf.RefNbr = String.Empty
            ARDocBuf.Terms = String.Empty

        End Try
    End Sub
End Module
