'VBTools Converter Version: 7.0.50901.0
Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Module Registration

    Class RegistrationCls

        Private m_CustomerID As Byte()
        Private m_CustomerName As Byte()

        Public Property CustomerId() As Byte()

            Get
                Return m_CustomerID
            End Get

            Set(ByVal setval As Byte())
                m_CustomerID = setval
            End Set

        End Property

        Public Property CustomerName() As Byte()

            Get
                Return m_CustomerName
            End Get

            Set(ByVal setval As Byte())
                m_CustomerName = setval
            End Set

        End Property



    End Class
    Public bRegistrationInfo As RegistrationCls = New RegistrationCls, nRegistrationInfo As RegistrationCls = New RegistrationCls

    Public Sub SetRegistrationValues(sqlReader As SqlDataReader, RegistBuf As RegistrationCls)
        Try
            RegistBuf.CustomerId = sqlReader("CustomerId")
            RegistBuf.CustomerName = sqlReader("CustomerName")
        Catch ex As Exception
        End Try
    End Sub
End Module
