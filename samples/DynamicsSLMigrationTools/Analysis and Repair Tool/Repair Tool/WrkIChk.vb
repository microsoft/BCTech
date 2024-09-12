
Imports System.Data.SqlClient
Module WRKICHK

    Public Class WrkIChkCls

        Private m_Custid As String
        Private m_Other As String
        Private m_OldBal As Double
        Private m_CpnyId As String
        Private m_NewBal As Double
        Private m_MsgId As String


        Public Property Custid() As String

            Get
                Return m_Custid
            End Get

            Set(ByVal setval As String)
                m_Custid = setval
            End Set

        End Property

        Public Property Cpnyid() As String

            Get
                Return m_CpnyId
            End Get

            Set(ByVal setval As String)
                m_CpnyId = setval
            End Set

        End Property

        Public Property MsgId() As String

            Get
                Return m_MsgId
            End Get

            Set(ByVal setval As String)
                m_MsgId = setval
            End Set

        End Property

        Public Property OldBal() As Double

            Get
                Return m_OldBal
            End Get

            Set(ByVal setval As Double)
                m_OldBal = setval
            End Set

        End Property

        Public Property NewBal() As Double

            Get
                Return m_NewBal
            End Get

            Set(ByVal setval As Double)
                m_NewBal = setval
            End Set

        End Property

        Public Property Other() As String

            Get
                Return m_Other
            End Get

            Set(ByVal setval As String)
                m_Other = setval
            End Set

        End Property

    End Class

    Public bWrkIChkInfo As WrkIChkCls = New WrkIChkCls, nWrkIChkInfo As WrkIChkCls = New WrkIChkCls

    Public Sub SetWrkIChkValues(sqlReader As SqlDataReader, WrkIChkBuf As WrkIChkCls)
        Try
            WrkIChkBuf.Cpnyid = sqlReader("CpnyId")
            WrkIChkBuf.Custid = sqlReader("CustId")
            WrkIChkBuf.MsgId = sqlReader("MsgId")
            WrkIChkBuf.NewBal = sqlReader("NewBal")
            WrkIChkBuf.OldBal = sqlReader("OldBal")
            WrkIChkBuf.Other = sqlReader("Other")
        Catch ex As Exception
            WrkIChkBuf.Cpnyid = String.Empty
            WrkIChkBuf.Custid = String.Empty
            WrkIChkBuf.MsgId = String.Empty
            WrkIChkBuf.NewBal = 0.0
            WrkIChkBuf.OldBal = 0.0
            WrkIChkBuf.Other = String.Empty

        End Try
    End Sub

End Module 
