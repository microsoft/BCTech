

Module AcctHistBuf

    Public Class AcctHistType

        Private m_Acct As String
        Private m_PtdBal00 As Double
        Private m_PtdBal01 As Double
        Private m_PtdBal02 As Double
        Private m_PtdBal03 As Double
        Private m_PtdBal04 As Double
        Private m_PtdBal05 As Double
        Private m_PtdBal06 As Double
        Private m_PtdBal07 As Double
        Private m_PtdBal08 As Double
        Private m_PtdBal09 As Double
        Private m_PtdBal10 As Double
        Private m_PtdBal11 As Double
        Private m_PtdBal12 As Double
        Private m_PtdBal13 As Double

        Private m_AcctSub As String
        Public Property Acct() As String

            Get
                Return m_Acct
            End Get

            Set(ByVal setval As String)
                m_Acct = setval
            End Set

        End Property

        Public Property PtdBal00 As Double

            Get
                Return m_PtdBal00
            End Get

            Set(ByVal value As Double)
                m_PtdBal00 = value
            End Set

        End Property

        Public Property PtdBal01 As Double

            Get
                Return m_PtdBal01
            End Get

            Set(ByVal value As Double)
                m_PtdBal01 = value
            End Set

        End Property
        Public Property PtdBal02 As Double

            Get
                Return m_PtdBal02
            End Get

            Set(ByVal value As Double)
                m_PtdBal02 = value
            End Set

        End Property
        Public Property PtdBal03 As Double

            Get
                Return m_PtdBal03
            End Get

            Set(ByVal value As Double)
                m_PtdBal03 = value
            End Set

        End Property
        Public Property PtdBal04 As Double

            Get
                Return m_PtdBal04
            End Get

            Set(ByVal value As Double)
                m_PtdBal04 = value
            End Set

        End Property
        Public Property PtdBal05 As Double

            Get
                Return m_PtdBal05
            End Get

            Set(ByVal value As Double)
                m_PtdBal05 = value
            End Set

        End Property
        Public Property PtdBal06 As Double

            Get
                Return m_PtdBal06
            End Get

            Set(ByVal value As Double)
                m_PtdBal06 = value
            End Set

        End Property
        Public Property PtdBal07 As Double

            Get
                Return m_PtdBal07
            End Get

            Set(ByVal value As Double)
                m_PtdBal07 = value
            End Set

        End Property
        Public Property PtdBal08 As Double

            Get
                Return m_PtdBal08
            End Get

            Set(ByVal value As Double)
                m_PtdBal08 = value
            End Set

        End Property
        Public Property PtdBal09 As Double

            Get
                Return m_PtdBal09
            End Get

            Set(ByVal value As Double)
                m_PtdBal09 = value
            End Set

        End Property
        Public Property PtdBal10 As Double

            Get
                Return m_PtdBal10
            End Get

            Set(ByVal value As Double)
                m_PtdBal10 = value
            End Set

        End Property
        Public Property PtdBal11 As Double

            Get
                Return m_PtdBal11
            End Get

            Set(ByVal value As Double)
                m_PtdBal11 = value
            End Set

        End Property
        Public Property PtdBal12 As Double

            Get
                Return m_PtdBal12
            End Get

            Set(ByVal value As Double)
                m_PtdBal12 = value
            End Set

        End Property


        Public Property AcctSub() As String

            Get
                Return m_AcctSub
            End Get

            Set(ByVal setval As String)
                m_AcctSub = setval
            End Set

        End Property



    End Class
    Public bAcctHistBuf As AcctHistType = New AcctHistType, nAcctHistBuf As AcctHistType = New AcctHistType

    Public Sub SetAcctHistBufValues(sqlReader As System.Data.SqlClient.SqlDataReader, ByRef AcctHistBuf As AcctHistType)

        Try


            AcctHistBuf.Acct = sqlReader("Acct")
            AcctHistBuf.AcctSub = sqlReader("Sub")
            AcctHistBuf.PtdBal00 = sqlReader("PTDBal00")
            AcctHistBuf.PtdBal01 = sqlReader("PTDBal01")
            AcctHistBuf.PtdBal02 = sqlReader("PTDBal02")
            AcctHistBuf.PtdBal03 = sqlReader("PTDBal03")
            AcctHistBuf.PtdBal04 = sqlReader("PTDBal04")
            AcctHistBuf.PtdBal05 = sqlReader("PTDBal05")
            AcctHistBuf.PtdBal06 = sqlReader("PTDBal06")
            AcctHistBuf.PtdBal07 = sqlReader("PTDBal07")
            AcctHistBuf.PtdBal08 = sqlReader("PTDBal08")
            AcctHistBuf.PtdBal09 = sqlReader("PTDBal09")
            AcctHistBuf.PtdBal10 = sqlReader("PTDBal10")
            AcctHistBuf.PtdBal11 = sqlReader("PTDBal11")
            AcctHistBuf.PtdBal12 = sqlReader("PTDBal12")

        Catch ex As Exception
            AcctHistBuf.Acct = String.Empty
            AcctHistBuf.AcctSub = String.Empty
            AcctHistBuf.PtdBal00 = 0.0
            AcctHistBuf.PtdBal01 = 0.0
            AcctHistBuf.PtdBal02 = 0.0
            AcctHistBuf.PtdBal03 = 0.0
            AcctHistBuf.PtdBal04 = 0.0
            AcctHistBuf.PtdBal05 = 0.0
            AcctHistBuf.PtdBal06 = 0.0
            AcctHistBuf.PtdBal07 = 0.0
            AcctHistBuf.PtdBal08 = 0.0
            AcctHistBuf.PtdBal09 = 0.0
            AcctHistBuf.PtdBal10 = 0.0
            AcctHistBuf.PtdBal11 = 0.0
            AcctHistBuf.PtdBal12 = 0.0
        End Try



    End Sub

End Module

