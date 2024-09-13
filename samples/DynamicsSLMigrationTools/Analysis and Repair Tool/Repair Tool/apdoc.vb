Imports System.Data.SqlClient

Module APDOC
    Private m_CpnyId As String
    Private m_DocBal As Double
    Private m_DocType As String
    Private m_OpenDoc As Integer
    Private m_OrigDocAmt As Double
    Private m_PerPost As String
    Private m_RefNbr As String
    Private m_Rlsed As Integer
    Private m_Terms As String
    Private m_VendID As String





    Public Class APDocCls

        Public Property CpnyID() As String

            Get
                Return m_CpnyId
            End Get

            Set(ByVal setval As String)
                m_CpnyId = setval
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

        Public Property DocType() As String

            Get
                Return m_DocType
            End Get

            Set(ByVal setval As String)
                m_DocType = setval
            End Set

        End Property


        Public Property OpenDoc() As Short

            Get
                Return m_OpenDoc
            End Get

            Set(ByVal setval As Short)
                m_OpenDoc = setval
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

        Public Property Rlsed() As Short

            Get
                Return m_Rlsed
            End Get

            Set(ByVal setval As Short)
                m_Rlsed = setval
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

        Public Property VendId() As String

            Get
                Return m_VendID
            End Get

            Set(ByVal setval As String)
                m_VendID = setval
            End Set

        End Property

    End Class
    Public bAPDocInfo As APDocCls = New APDocCls, nAPDocInfo As APDocCls = New APDocCls

    Public Sub SetAPDocValues(sqlReader As SqlDataReader, ApDocBuf As APDocCls)
        Try
            ApDocBuf.CpnyID = sqlReader("CpnyId")
            ApDocBuf.DocBal = sqlReader("DocBal")
            ApDocBuf.DocType = sqlReader("DocType")
            ApDocBuf.OpenDoc = sqlReader("OpenDoc")
            ApDocBuf.OrigDocAmt = sqlReader("OrigDocAmt")
            ApDocBuf.PerPost = sqlReader("PerPost")
            ApDocBuf.RefNbr = sqlReader("RefNbr")
            ApDocBuf.Rlsed = sqlReader("Rlsed")
            ApDocBuf.Terms = sqlReader("Terms")
            ApDocBuf.VendId = sqlReader("VendId")
        Catch
            ApDocBuf.CpnyID = String.Empty
            ApDocBuf.DocBal = 0.0
            ApDocBuf.DocType = String.Empty
            ApDocBuf.OpenDoc = 0
            ApDocBuf.OrigDocAmt = 0.0
            ApDocBuf.PerPost = String.Empty
            ApDocBuf.RefNbr = String.Empty
            ApDocBuf.Rlsed = 0
            ApDocBuf.Terms = String.Empty
            ApDocBuf.VendId = String.Empty

        End Try


    End Sub
End Module
