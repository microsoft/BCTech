Imports System.Data.SqlClient
Module APHist
    Private m_BegBal As Double
    Private m_PtdCrAdjs00 As Double
    Private m_PtdCrAdjs01 As Double
    Private m_PtdCrAdjs02 As Double
    Private m_PtdCrAdjs03 As Double
    Private m_PtdCrAdjs04 As Double
    Private m_PtdCrAdjs05 As Double
    Private m_PtdCrAdjs06 As Double
    Private m_PtdCrAdjs07 As Double
    Private m_PtdCrAdjs08 As Double
    Private m_PtdCrAdjs09 As Double
    Private m_PtdCrAdjs10 As Double
    Private m_PtdCrAdjs11 As Double
    Private m_PtdCrAdjs12 As Double
    Private m_PtdCrAdjs13 As Double
    Private m_PtdDiscTkn00 As Double
    Private m_PtdDiscTkn01 As Double
    Private m_PtdDiscTkn02 As Double
    Private m_PtdDiscTkn03 As Double
    Private m_PtdDiscTkn04 As Double
    Private m_PtdDiscTkn05 As Double
    Private m_PtdDiscTkn06 As Double
    Private m_PtdDiscTkn07 As Double
    Private m_PtdDiscTkn08 As Double
    Private m_PtdDiscTkn09 As Double
    Private m_PtdDiscTkn10 As Double
    Private m_PtdDiscTkn11 As Double
    Private m_PtdDiscTkn12 As Double
    Private m_PtdDrAdjs00 As Double
    Private m_PtdDrAdjs01 As Double
    Private m_PtdDrAdjs02 As Double
    Private m_PtdDrAdjs03 As Double
    Private m_PtdDrAdjs04 As Double
    Private m_PtdDrAdjs05 As Double
    Private m_PtdDrAdjs06 As Double
    Private m_PtdDrAdjs07 As Double
    Private m_PtdDrAdjs08 As Double
    Private m_PtdDrAdjs09 As Double
    Private m_PtdDrAdjs10 As Double
    Private m_PtdDrAdjs11 As Double
    Private m_PtdDrAdjs12 As Double
    Private m_PtdPaymt00 As Double
    Private m_PtdPaymt01 As Double
    Private m_PtdPaymt02 As Double
    Private m_PtdPaymt03 As Double
    Private m_PtdPaymt04 As Double
    Private m_PtdPaymt05 As Double
    Private m_PtdPaymt06 As Double
    Private m_PtdPaymt07 As Double
    Private m_PtdPaymt08 As Double
    Private m_PtdPaymt09 As Double
    Private m_PtdPaymt10 As Double
    Private m_PtdPaymt11 As Double
    Private m_PtdPaymt12 As Double
    Private m_PtdPurch00 As Double
    Private m_PtdPurch01 As Double
    Private m_PtdPurch02 As Double
    Private m_PtdPurch03 As Double
    Private m_PtdPurch04 As Double
    Private m_PtdPurch05 As Double
    Private m_PtdPurch06 As Double
    Private m_PtdPurch07 As Double
    Private m_PtdPurch08 As Double
    Private m_PtdPurch09 As Double
    Private m_PtdPurch10 As Double
    Private m_PtdPurch11 As Double
    Private m_PtdPurch12 As Double


    Public Class APHistCls

        Public Property BegBal() As Double

            Get
                Return m_BegBal
            End Get

            Set(ByVal setval As Double)
                m_BegBal = setval
            End Set

        End Property

        Public Property PtdCrAdjs00 As Double

            Get
                Return m_PtdCrAdjs00
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs00 = setval
            End Set

        End Property
        Public Property PtdCrAdjs01 As Double

            Get
                Return m_PtdCrAdjs01
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs01 = setval
            End Set

        End Property
        Public Property PtdCrAdjs02 As Double

            Get
                Return m_PtdCrAdjs02
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs02 = setval
            End Set

        End Property
        Public Property PtdCrAdjs03 As Double

            Get
                Return m_PtdCrAdjs03
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs03 = setval
            End Set

        End Property
        Public Property PtdCrAdjs04 As Double

            Get
                Return m_PtdCrAdjs04
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs04 = setval
            End Set

        End Property
        Public Property PtdCrAdjs05 As Double

            Get
                Return m_PtdCrAdjs05
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs05 = setval
            End Set

        End Property
        Public Property PtdCrAdjs06 As Double

            Get
                Return m_PtdCrAdjs06
            End Get
            Set(ByVal setval As Double)
                m_PtdCrAdjs06 = setval
            End Set

        End Property
        Public Property PtdCrAdjs07 As Double

            Get
                Return m_PtdCrAdjs07
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs07 = setval
            End Set

        End Property
        Public Property PtdCrAdjs08 As Double

            Get
                Return m_PtdCrAdjs08
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs08 = setval
            End Set

        End Property
        Public Property PtdCrAdjs09 As Double

            Get
                Return m_PtdCrAdjs09
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs09 = setval
            End Set

        End Property
        Public Property PtdCrAdjs10 As Double

            Get
                Return m_PtdCrAdjs10
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs10 = setval
            End Set

        End Property
        Public Property PtdCrAdjs11 As Double

            Get
                Return m_PtdCrAdjs11
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs11 = setval
            End Set

        End Property
        Public Property PtdCrAdjs12 As Double

            Get
                Return m_PtdCrAdjs12
            End Get

            Set(ByVal setval As Double)
                m_PtdCrAdjs12 = setval
            End Set

        End Property

        Public Property PtdDiscTkn00 As Double

            Get
                Return m_PtdDiscTkn00
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn00 = setval
            End Set

        End Property
        Public Property PtdDiscTkn01 As Double

            Get
                Return m_PtdDiscTkn01
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn01 = setval
            End Set

        End Property
        Public Property PtdDiscTkn02 As Double

            Get
                Return m_PtdDiscTkn02
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn02 = setval
            End Set

        End Property
        Public Property PtdDiscTkn03 As Double

            Get
                Return m_PtdDiscTkn03
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn03 = setval
            End Set

        End Property
        Public Property PtdDiscTkn04 As Double

            Get
                Return m_PtdDiscTkn04
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn04 = setval
            End Set

        End Property
        Public Property PtdDiscTkn05 As Double

            Get
                Return m_PtdDiscTkn05
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn05 = setval
            End Set

        End Property
        Public Property PtdDiscTkn06 As Double

            Get
                Return m_PtdDiscTkn06
            End Get
            Set(ByVal setval As Double)
                m_PtdDiscTkn06 = setval
            End Set

        End Property
        Public Property PtdDiscTkn07 As Double

            Get
                Return m_PtdDiscTkn07
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn07 = setval
            End Set

        End Property
        Public Property PtdDiscTkn08 As Double

            Get
                Return m_PtdDiscTkn08
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn08 = setval
            End Set

        End Property
        Public Property PtdDiscTkn09 As Double

            Get
                Return m_PtdDiscTkn09
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn09 = setval
            End Set

        End Property
        Public Property PtdDiscTkn10 As Double

            Get
                Return m_PtdDiscTkn10
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn10 = setval
            End Set

        End Property
        Public Property PtdDiscTkn11 As Double

            Get
                Return m_PtdDiscTkn11
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn11 = setval
            End Set

        End Property
        Public Property PtdDiscTkn12 As Double

            Get
                Return m_PtdDiscTkn12
            End Get

            Set(ByVal setval As Double)
                m_PtdDiscTkn12 = setval
            End Set

        End Property
        Public Property PtdDrAdjs00 As Double

            Get
                Return m_PtdDrAdjs00
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs00 = setval
            End Set

        End Property
        Public Property PtdDrAdjs01 As Double

            Get
                Return m_PtdDrAdjs01
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs01 = setval
            End Set

        End Property
        Public Property PtdDrAdjs02 As Double

            Get
                Return m_PtdDrAdjs02
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs02 = setval
            End Set

        End Property
        Public Property PtdDrAdjs03 As Double

            Get
                Return m_PtdDrAdjs03
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs03 = setval
            End Set

        End Property
        Public Property PtdDrAdjs04 As Double

            Get
                Return m_PtdDrAdjs04
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs04 = setval
            End Set

        End Property
        Public Property PtdDrAdjs05 As Double

            Get
                Return m_PtdDrAdjs05
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs05 = setval
            End Set

        End Property
        Public Property PtdDrAdjs06 As Double

            Get
                Return m_PtdDrAdjs06
            End Get
            Set(ByVal setval As Double)
                m_PtdDrAdjs06 = setval
            End Set

        End Property
        Public Property PtdDrAdjs07 As Double

            Get
                Return m_PtdDrAdjs07
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs07 = setval
            End Set

        End Property
        Public Property PtdDrAdjs08 As Double

            Get
                Return m_PtdDrAdjs08
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs08 = setval
            End Set

        End Property
        Public Property PtdDrAdjs09 As Double

            Get
                Return m_PtdDrAdjs09
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs09 = setval
            End Set

        End Property
        Public Property PtdDrAdjs10 As Double

            Get
                Return m_PtdDrAdjs10
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs10 = setval
            End Set

        End Property
        Public Property PtdDrAdjs11 As Double

            Get
                Return m_PtdDrAdjs11
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs11 = setval
            End Set

        End Property
        Public Property PtdDrAdjs12 As Double

            Get
                Return m_PtdDrAdjs12
            End Get

            Set(ByVal setval As Double)
                m_PtdDrAdjs12 = setval
            End Set

        End Property
        Public Property PtdPaymt00 As Double

            Get
                Return m_PtdPaymt00
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt00 = setval
            End Set

        End Property
        Public Property PtdPaymt01 As Double

            Get
                Return m_PtdPaymt01
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt01 = setval
            End Set

        End Property
        Public Property PtdPaymt02 As Double

            Get
                Return m_PtdPaymt02
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt02 = setval
            End Set

        End Property
        Public Property PtdPaymt03 As Double

            Get
                Return m_PtdPaymt03
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt03 = setval
            End Set

        End Property
        Public Property PtdPaymt04 As Double

            Get
                Return m_PtdPaymt04
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt04 = setval
            End Set

        End Property
        Public Property PtdPaymt05 As Double

            Get
                Return m_PtdPaymt05
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt05 = setval
            End Set

        End Property
        Public Property PtdPaymt06 As Double

            Get
                Return m_PtdPaymt06
            End Get
            Set(ByVal setval As Double)
                m_PtdPaymt06 = setval
            End Set

        End Property
        Public Property PtdPaymt07 As Double

            Get
                Return m_PtdPaymt07
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt07 = setval
            End Set

        End Property
        Public Property PtdPaymt08 As Double

            Get
                Return m_PtdPaymt08
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt08 = setval
            End Set

        End Property
        Public Property PtdPaymt09 As Double

            Get
                Return m_PtdPaymt09
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt09 = setval
            End Set

        End Property
        Public Property PtdPaymt10 As Double

            Get
                Return m_PtdPaymt10
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt10 = setval
            End Set

        End Property
        Public Property PtdPaymt11 As Double

            Get
                Return m_PtdPaymt11
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt11 = setval
            End Set

        End Property
        Public Property PtdPaymt12 As Double

            Get
                Return m_PtdPaymt12
            End Get

            Set(ByVal setval As Double)
                m_PtdPaymt12 = setval
            End Set

        End Property

        Public Property PtdPurch00 As Double

            Get
                Return m_PtdPurch00
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch00 = setval
            End Set

        End Property
        Public Property PtdPurch01 As Double

            Get
                Return m_PtdPurch01
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch01 = setval
            End Set

        End Property
        Public Property PtdPurch02 As Double

            Get
                Return m_PtdPurch02
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch02 = setval
            End Set

        End Property
        Public Property PtdPurch03 As Double

            Get
                Return m_PtdPurch03
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch03 = setval
            End Set

        End Property
        Public Property PtdPurch04 As Double

            Get
                Return m_PtdPurch04
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch04 = setval
            End Set

        End Property
        Public Property PtdPurch05 As Double

            Get
                Return m_PtdPurch05
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch05 = setval
            End Set

        End Property
        Public Property PtdPurch06 As Double

            Get
                Return m_PtdPurch06
            End Get
            Set(ByVal setval As Double)
                m_PtdPurch06 = setval
            End Set

        End Property
        Public Property PtdPurch07 As Double

            Get
                Return m_PtdPurch07
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch07 = setval
            End Set

        End Property
        Public Property PtdPurch08 As Double

            Get
                Return m_PtdPurch08
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch08 = setval
            End Set

        End Property
        Public Property PtdPurch09 As Double

            Get
                Return m_PtdPurch09
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch09 = setval
            End Set

        End Property
        Public Property PtdPurch10 As Double

            Get
                Return m_PtdPurch10
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch10 = setval
            End Set

        End Property
        Public Property PtdPurch11 As Double

            Get
                Return m_PtdPurch11
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch11 = setval
            End Set

        End Property
        Public Property PtdPurch12 As Double

            Get
                Return m_PtdPurch12
            End Get

            Set(ByVal setval As Double)
                m_PtdPurch12 = setval
            End Set

        End Property


    End Class
    Public bAPHistInfo As APHistCls = New APHistCls, nAPHistInfo As APHistCls = New APHistCls

    Public Sub SetAPHistValues(sqlReader As SqlDataReader, APHistBuf As APHistCls)
        Try
            APHistBuf.BegBal = sqlReader("BegBal")
            APHistBuf.PtdCrAdjs00 = sqlReader("PtdCrAdjs00")
            APHistBuf.PtdCrAdjs01 = sqlReader("PtdCrAdjs01")
            APHistBuf.PtdCrAdjs02 = sqlReader("PtdCrAdjs02")
            APHistBuf.PtdCrAdjs03 = sqlReader("PtdCrAdjs03")
            APHistBuf.PtdCrAdjs04 = sqlReader("PtdCrAdjs04")
            APHistBuf.PtdCrAdjs05 = sqlReader("PtdCrAdjs05")
            APHistBuf.PtdCrAdjs06 = sqlReader("PtdCrAdjs06")
            APHistBuf.PtdCrAdjs07 = sqlReader("PtdCrAdjs07")
            APHistBuf.PtdCrAdjs08 = sqlReader("PtdCrAdjs08")
            APHistBuf.PtdCrAdjs09 = sqlReader("PtdCrAdjs09")
            APHistBuf.PtdCrAdjs10 = sqlReader("PtdCrAdjs10")
            APHistBuf.PtdCrAdjs11 = sqlReader("PtdCrAdjs11")
            APHistBuf.PtdCrAdjs12 = sqlReader("PtdCrAdjs12")
            APHistBuf.PtdDrAdjs00 = sqlReader("PtdDrAdjs00")
            APHistBuf.PtdDrAdjs01 = sqlReader("PtdDrAdjs01")
            APHistBuf.PtdDrAdjs02 = sqlReader("PtdDrAdjs02")
            APHistBuf.PtdDrAdjs03 = sqlReader("PtdDrAdjs03")
            APHistBuf.PtdDrAdjs04 = sqlReader("PtdDrAdjs04")
            APHistBuf.PtdDrAdjs05 = sqlReader("PtdDrAdjs05")
            APHistBuf.PtdDrAdjs06 = sqlReader("PtdDrAdjs06")
            APHistBuf.PtdDrAdjs07 = sqlReader("PtdDrAdjs07")
            APHistBuf.PtdDrAdjs08 = sqlReader("PtdDrAdjs08")
            APHistBuf.PtdDrAdjs09 = sqlReader("PtdDrAdjs09")
            APHistBuf.PtdDrAdjs10 = sqlReader("PtdDrAdjs10")
            APHistBuf.PtdDrAdjs11 = sqlReader("PtdDrAdjs11")
            APHistBuf.PtdDrAdjs12 = sqlReader("PtdDrAdjs12")
            APHistBuf.PtdDiscTkn00 = sqlReader("PtdDiscTkn00")
            APHistBuf.PtdDiscTkn01 = sqlReader("PtdDiscTkn01")
            APHistBuf.PtdDiscTkn02 = sqlReader("PtdDiscTkn02")
            APHistBuf.PtdDiscTkn03 = sqlReader("PtdDiscTkn03")
            APHistBuf.PtdDiscTkn04 = sqlReader("PtdDiscTkn04")
            APHistBuf.PtdDiscTkn05 = sqlReader("PtdDiscTkn05")
            APHistBuf.PtdDiscTkn06 = sqlReader("PtdDiscTkn06")
            APHistBuf.PtdDiscTkn07 = sqlReader("PtdDiscTkn07")
            APHistBuf.PtdDiscTkn08 = sqlReader("PtdDiscTkn08")
            APHistBuf.PtdDiscTkn09 = sqlReader("PtdDiscTkn09")
            APHistBuf.PtdDiscTkn10 = sqlReader("PtdDiscTkn10")
            APHistBuf.PtdDiscTkn11 = sqlReader("PtdDiscTkn11")
            APHistBuf.PtdDiscTkn12 = sqlReader("PtdDiscTkn12")

            APHistBuf.PtdPaymt00 = sqlReader("PtdPaymt00")
            APHistBuf.PtdPaymt01 = sqlReader("PtdPaymt01")
            APHistBuf.PtdPaymt02 = sqlReader("PtdPaymt02")
            APHistBuf.PtdPaymt03 = sqlReader("PtdPaymt03")
            APHistBuf.PtdPaymt04 = sqlReader("PtdPaymt04")
            APHistBuf.PtdPaymt05 = sqlReader("PtdPaymt05")
            APHistBuf.PtdPaymt06 = sqlReader("PtdPaymt06")
            APHistBuf.PtdPaymt07 = sqlReader("PtdPaymt07")
            APHistBuf.PtdPaymt08 = sqlReader("PtdPaymt08")
            APHistBuf.PtdPaymt09 = sqlReader("PtdPaymt09")
            APHistBuf.PtdPaymt10 = sqlReader("PtdPaymt10")
            APHistBuf.PtdPaymt11 = sqlReader("PtdPaymt11")
            APHistBuf.PtdPaymt12 = sqlReader("PtdPaymt12")
            APHistBuf.PtdPurch00 = sqlReader("PtdPurch00")
            APHistBuf.PtdPurch01 = sqlReader("PtdPurch01")
            APHistBuf.PtdPurch02 = sqlReader("PtdPurch02")
            APHistBuf.PtdPurch03 = sqlReader("PtdPurch03")
            APHistBuf.PtdPurch04 = sqlReader("PtdPurch04")
            APHistBuf.PtdPurch05 = sqlReader("PtdPurch05")
            APHistBuf.PtdPurch06 = sqlReader("PtdPurch06")
            APHistBuf.PtdPurch07 = sqlReader("PtdPurch07")
            APHistBuf.PtdPurch08 = sqlReader("PtdPurch08")
            APHistBuf.PtdPurch09 = sqlReader("PtdPurch09")
            APHistBuf.PtdPurch10 = sqlReader("PtdPurch10")
            APHistBuf.PtdPurch11 = sqlReader("PtdPurch11")
            APHistBuf.PtdPurch12 = sqlReader("PtdPurch12")

        Catch ex As Exception

        End Try
    End Sub
End Module
