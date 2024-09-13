Imports System.Data.SqlClient

Module FLEXDEFCls

    Public Class FlexDefCls

        Private m_NumberSegments As Short
        Private m_SegLength00 As Short
        Private m_SegLength01 As Short
        Private m_SegLength02 As Short
        Private m_SegLength03 As Short
        Private m_SegLength04 As Short
        Private m_SegLength05 As Short
        Private m_SegLength06 As Short
        Private m_SegLength07 As Short

        Public Property NumberSegments As Short

            Get
                Return m_NumberSegments
            End Get

            Set(ByVal setval As Short)
                m_NumberSegments = setval
            End Set

        End Property

        Public Property SegLength00 As Short

            Get
                Return m_SegLength00
            End Get

            Set(ByVal setval As Short)
                m_SegLength00 = setval
            End Set

        End Property

        Public Property SegLength01 As Short

            Get
                Return m_SegLength01
            End Get

            Set(ByVal setval As Short)
                m_SegLength01 = setval
            End Set

        End Property

        Public Property SegLength02 As Short

            Get
                Return m_SegLength02
            End Get

            Set(ByVal setval As Short)
                m_SegLength02 = setval
            End Set

        End Property

        Public Property SegLength03 As Short

            Get
                Return m_SegLength03
            End Get

            Set(ByVal setval As Short)
                m_SegLength03 = setval
            End Set

        End Property

        Public Property SegLength04 As Short

            Get
                Return m_SegLength04
            End Get

            Set(ByVal setval As Short)
                m_SegLength04 = setval
            End Set

        End Property

        Public Property SegLength05 As Short

            Get
                Return m_SegLength05
            End Get

            Set(ByVal setval As Short)
                m_SegLength05 = setval
            End Set

        End Property

        Public Property SegLength06 As Short

            Get
                Return m_SegLength06
            End Get

            Set(ByVal setval As Short)
                m_SegLength06 = setval
            End Set

        End Property

        Public Property SegLength07 As Short

            Get
                Return m_SegLength07
            End Get

            Set(ByVal setval As Short)
                m_SegLength07 = setval
            End Set

        End Property


    End Class
    Public bFlexDefInfo As FlexDefCls = New FlexDefCls, nFlexDefInfo As FlexDefCls = New FlexDefCls

    Public Sub SetFlexDefValues(sqlReader As SqlDataReader, FlexDefBuf As FlexDefCls)
        Try
            FlexDefBuf.NumberSegments = sqlReader("NumberSegments")
            FlexDefBuf.SegLength00 = sqlReader("SegLength00")
            FlexDefBuf.SegLength01 = sqlReader("SegLength01")
            FlexDefBuf.SegLength02 = sqlReader("SegLength02")
            FlexDefBuf.SegLength03 = sqlReader("SegLength03")
            FlexDefBuf.SegLength04 = sqlReader("SegLength04")
            FlexDefBuf.SegLength05 = sqlReader("SegLength05")
            FlexDefBuf.SegLength06 = sqlReader("SegLength06")
            FlexDefBuf.SegLength07 = sqlReader("SegLength07")
        Catch ex As Exception
            FlexDefBuf.NumberSegments = 0
            FlexDefBuf.SegLength00 = 0
            FlexDefBuf.SegLength01 = 0
            FlexDefBuf.SegLength02 = 0
            FlexDefBuf.SegLength03 = 0
            FlexDefBuf.SegLength04 = 0
            FlexDefBuf.SegLength05 = 0
            FlexDefBuf.SegLength06 = 0
            FlexDefBuf.SegLength07 = 0

        End Try
    End Sub
End Module
