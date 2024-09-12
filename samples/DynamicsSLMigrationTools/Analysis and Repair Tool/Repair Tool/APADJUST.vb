Imports System.Data.SqlClient

Module APAdjust

    Public Class APAdjustCls

        Private m_AdjAmt As Double
        Private m_AdjgPerPost As String
        Private m_AdjDiscAmt As Double

        Public Property AdjAmt() As Double

            Get
                Return m_AdjAmt
            End Get

            Set(ByVal setval As Double)
                m_AdjAmt = setval
            End Set

        End Property


        Public Property AdjDiscAmt() As Double

            Get
                Return m_AdjDiscAmt
            End Get

            Set(ByVal setval As Double)
                m_AdjDiscAmt = setval
            End Set

        End Property


        Public Property AdjgPerPost() As String

            Get
                Return m_AdjgPerPost
            End Get

            Set(ByVal setval As String)
                m_AdjgPerPost = setval
            End Set

        End Property



    End Class
    Public bAPAdjustInfo As APAdjustCls = New APAdjustCls, nAPAdjustInfo As APAdjustCls = New APAdjustCls

    Public Sub SetAPAdjustValues(sqlReader As SqlDataReader, APAdjustBuf As APAdjustCls)
        Try
            APAdjustBuf.AdjAmt = sqlReader("AdjAmt")
            APAdjustBuf.AdjDiscAmt = sqlReader("AdjDiscAmt")
            APAdjustBuf.AdjgPerPost = sqlReader("AdjgPerPost")
        Catch ex As Exception
            APAdjustBuf.AdjAmt = 0.0
            APAdjustBuf.AdjDiscAmt = 0.0
            APAdjustBuf.AdjgPerPost = String.Empty


        End Try
    End Sub
End Module
