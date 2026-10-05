Imports System.Data.SqlClient
Module Vendor

    Public Class VendorCls


        Private m_APAcct As String
        Private m_ExpAcct As String
        Private m_PerNbr As String
        Private m_VendID As String
        Private m_VendName As String
        Private m_EMailAddr As String
        Private m_Phone As String


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

        Public Property EMailAddr() As String

            Get
                Return m_EMailAddr
            End Get

            Set(ByVal setval As String)
                m_EMailAddr = setval
            End Set

        End Property

        Public Property Phone() As String

            Get
                Return m_Phone
            End Get

            Set(ByVal setval As String)
                m_Phone = setval
            End Set

        End Property


    End Class
    Public bVendorInfo As VendorCls = New VendorCls, nVendorInfo As VendorCls = New VendorCls

    Private Function ReaderHasColumn(sqlReader As SqlDataReader, columnName As String) As Boolean
        Dim i As Integer

        For i = 0 To sqlReader.FieldCount - 1
            If String.Compare(sqlReader.GetName(i), columnName, True) = 0 Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Sub SetVendorValues(sqlReader As SqlDataReader, VendorBuf As VendorCls)
        Try
            If ReaderHasColumn(sqlReader, "APAcct") AndAlso Not IsDBNull(sqlReader("APAcct")) Then
                VendorBuf.APAcct = sqlReader("APAcct").ToString()
            Else
                VendorBuf.APAcct = String.Empty
            End If

            If ReaderHasColumn(sqlReader, "ExpAcct") AndAlso Not IsDBNull(sqlReader("ExpAcct")) Then
                VendorBuf.ExpAcct = sqlReader("ExpAcct").ToString()
            Else
                VendorBuf.ExpAcct = String.Empty
            End If

            If ReaderHasColumn(sqlReader, "PerNbr") AndAlso Not IsDBNull(sqlReader("PerNbr")) Then
                VendorBuf.PerNbr = sqlReader("PerNbr").ToString()
            Else
                VendorBuf.PerNbr = String.Empty
            End If

            If ReaderHasColumn(sqlReader, "VendId") AndAlso Not IsDBNull(sqlReader("VendId")) Then
                VendorBuf.VendId = sqlReader("VendId").ToString()
            Else
                VendorBuf.VendId = String.Empty
            End If

            If ReaderHasColumn(sqlReader, "Name") AndAlso Not IsDBNull(sqlReader("Name")) Then
                VendorBuf.VendName = sqlReader("Name").ToString()
            Else
                VendorBuf.VendName = String.Empty
            End If

            If ReaderHasColumn(sqlReader, "EMailAddr") AndAlso Not IsDBNull(sqlReader("EMailAddr")) Then
                VendorBuf.EMailAddr = sqlReader("EMailAddr").ToString()
            Else
                VendorBuf.EMailAddr = String.Empty
            End If

            If ReaderHasColumn(sqlReader, "Phone") AndAlso Not IsDBNull(sqlReader("Phone")) Then
                VendorBuf.Phone = sqlReader("Phone").ToString()
            Else
                VendorBuf.Phone = String.Empty
            End If

        Catch ex As Exception
            VendorBuf.APAcct = String.Empty
            VendorBuf.ExpAcct = String.Empty
            VendorBuf.PerNbr = String.Empty
            VendorBuf.VendId = String.Empty
            VendorBuf.VendName = String.Empty
            VendorBuf.EMailAddr = String.Empty
            VendorBuf.Phone = String.Empty

        End Try


    End Sub
End Module
