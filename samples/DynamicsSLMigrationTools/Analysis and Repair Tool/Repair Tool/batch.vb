Imports System.Data.SqlClient
Module BATCH

    Public Class BatchCls

        Private m_Batnbr As String
        Private m_EditScrnNbr As String
        Private m_PerPost As String

        Public Property BatNbr() As String

            Get
                Return m_Batnbr
            End Get

            Set(ByVal setval As String)
                m_Batnbr = setval
            End Set

        End Property

        Public Property EditScrnNbr() As String

            Get
                Return m_EditScrnNbr
            End Get

            Set(ByVal setval As String)
                m_EditScrnNbr = setval
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

    End Class
    Public bBatchInfo As BatchCls = New BatchCls, nBatchInfo As BatchCls = New BatchCls

    Public Sub SetBatchValues(sqlReader As SqlDataReader, BatchBuf As BatchCls)
        Try
            BatchBuf.BatNbr = sqlReader("BatNbr")
            BatchBuf.EditScrnNbr = sqlReader("EditScrnNbr")
            BatchBuf.PerPost = sqlReader("PerPost")
        Catch ex As Exception
            BatchBuf.BatNbr = String.Empty
            BatchBuf.EditScrnNbr = String.Empty
            BatchBuf.PerPost = String.Empty
        End Try
    End Sub
End Module
