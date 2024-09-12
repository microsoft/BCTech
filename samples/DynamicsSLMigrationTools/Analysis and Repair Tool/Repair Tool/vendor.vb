Imports System.Data.SqlClient
Module Vendor

    Public Class VendorCls


        Private m_APAcct As String
        Private m_ExpAcct As String
        Private m_PerNbr As String
        Private m_VendID As String
        Private m_VendName As String


        Public Property APAcct() As String

            Get
                Return m_APAcct
            End Get

            Set(ByVal setval As String)
                m_APAcct = setval
            End Set

        End Property



        Public Property ExpAcct() As String

            Get
                Return m_ExpAcct
            End Get

            Set(ByVal setval As String)
                m_ExpAcct = setval
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


        Public Property VendId() As String

            Get
                Return m_VendID
            End Get

            Set(ByVal setval As String)
                m_VendID = setval
            End Set

        End Property

        Public Property VendName() As String

            Get
                Return m_VendName
            End Get

            Set(ByVal setval As String)
                m_VendName = setval
            End Set

        End Property


    End Class
    Public bVendorInfo As VendorCls = New VendorCls, nVendorInfo As VendorCls = New VendorCls

    Public Sub SetVendorValues(sqlReader As SqlDataReader, VendorBuf As VendorCls)
        Try


            VendorBuf.APAcct = sqlReader("APAcct")
            VendorBuf.ExpAcct = sqlReader("ExpAcct")
            VendorBuf.PerNbr = sqlReader("PerNbr")
            VendorBuf.VendId = sqlReader("VendId")
            VendorBuf.VendName = sqlReader("Name")

        Catch ex As Exception
            VendorBuf.APAcct = String.Empty
            VendorBuf.ExpAcct = String.Empty
            VendorBuf.PerNbr = String.Empty
            VendorBuf.VendId = String.Empty
            VendorBuf.VendName = String.Empty

        End Try


    End Sub
End Module
