Imports System.Data.SqlClient
Module Inventory

    Public Class InventoryCls

        Private m_ClassId As String
        Private m_InvtID As String
        Private m_LotSerTrack As String
        Private m_SerAssign As String
        Private m_StkUnit As String
        Private m_Supplr1 As String
        Private m_ValMthd As String


        Public Property ClassId() As String

            Get
                Return m_ClassId
            End Get

            Set(ByVal setval As String)
                m_ClassId = setval
            End Set

        End Property

        Public Property InvtID() As String

            Get
                Return m_InvtID
            End Get

            Set(ByVal setval As String)
                m_InvtID = setval
            End Set

        End Property
        Public Property LotSerTrack() As String

            Get
                Return m_LotSerTrack
            End Get

            Set(ByVal setval As String)
                m_LotSerTrack = setval
            End Set

        End Property
        Public Property SerAssign() As String

            Get
                Return m_SerAssign
            End Get

            Set(ByVal setval As String)
                m_SerAssign = setval
            End Set

        End Property


        Public Property StkUnit() As String

            Get
                Return m_StkUnit
            End Get

            Set(ByVal setval As String)
                m_StkUnit = setval
            End Set

        End Property

        Public Property Supplr1() As String

            Get
                Return m_Supplr1
            End Get

            Set(ByVal setval As String)
                m_Supplr1 = setval
            End Set

        End Property


        Public Property ValMthd() As String

            Get
                Return m_ValMthd
            End Get

            Set(ByVal setval As String)
                m_ValMthd = setval
            End Set

        End Property



    End Class
    Public bInventoryInfo As InventoryCls = New InventoryCls, nInventoryInfo As InventoryCls = New InventoryCls

    Public Sub SetInventoryValues(sqlReader As SqlDataReader, InventoryBuf As InventoryCls)
        Try
            InventoryBuf.ClassId = sqlReader("ClassId")
            InventoryBuf.InvtID = sqlReader("InvtId")
            InventoryBuf.LotSerTrack = sqlReader("LotSerTrack")
            InventoryBuf.SerAssign = sqlReader("SerAssign")
            InventoryBuf.StkUnit = sqlReader("StkUnit")
            InventoryBuf.Supplr1 = sqlReader("Supplr1")
            InventoryBuf.ValMthd = sqlReader("ValMthd")

        Catch ex As Exception
            InventoryBuf.ClassId = String.Empty
            InventoryBuf.InvtID = String.Empty
            InventoryBuf.LotSerTrack = String.Empty
            InventoryBuf.SerAssign = String.Empty
            InventoryBuf.StkUnit = String.Empty
            InventoryBuf.Supplr1 = String.Empty
            InventoryBuf.ValMthd = String.Empty
        End Try
    End Sub
End Module
