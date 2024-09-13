Imports System.Data.SqlClient
Module INSetupCls

    Public Class INSetupCls

        Private m_BMIEnabled As Short
        Private m_CPSOnOff As Short
        Private m_MultWhse As Short
        Private m_NegQty As Short
        Private m_PerNbr As String

        Public Property BMIEnabled() As Short

            Get
                Return m_BMIEnabled
            End Get

            Set(ByVal setval As Short)
                m_BMIEnabled = setval
            End Set

        End Property
        Public Property CPSOnOff() As Short

            Get
                Return m_CPSOnOff
            End Get

            Set(ByVal setval As Short)
                m_CPSOnOff = setval
            End Set

        End Property
        Public Property MultWhse() As Short

            Get
                Return m_MultWhse
            End Get

            Set(ByVal setval As Short)
                m_MultWhse = setval
            End Set

        End Property
        Public Property NegQty() As Short

            Get
                Return m_NegQty
            End Get

            Set(ByVal setval As Short)
                m_NegQty = setval
            End Set

        End Property

        Public Property PerNbr() As String

            Get
                Return m_PerNbr
            End Get

            Set(ByVal setval As String)
                m_PerNbr = setval
            End Set

        End Property


    End Class
    Public bINSetupInfo As INSetupCls = New INSetupCls, nINSetupInfo As INSetupCls = New INSetupCls

    Public Sub SetINSetupValues(sqlReader As SqlDataReader, INSetupBuf As INSetupCls)
        Try
            INSetupBuf.BMIEnabled = sqlReader("BMIEnabled")
            INSetupBuf.CPSOnOff = sqlReader("CPSOnOff")
            INSetupBuf.MultWhse = sqlReader("MultWhse")
            INSetupBuf.NegQty = sqlReader("NegQty")
            INSetupBuf.PerNbr = sqlReader("PerNbr")
        Catch ex As Exception
            INSetupBuf.BMIEnabled = 0
            INSetupBuf.CPSOnOff = 0
            INSetupBuf.MultWhse = 0
            INSetupBuf.NegQty = 0
            INSetupBuf.PerNbr = String.Empty
        End Try
    End Sub
End Module
