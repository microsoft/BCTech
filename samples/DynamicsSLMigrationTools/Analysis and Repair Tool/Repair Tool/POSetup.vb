Imports System.Data.SqlClient
Module POSETUP

    Public Class POSetupcls

        Private m_SetupID As String
        Private m_BillEmail As String
        Private m_ShipEmail As String

        Public Property SetupID() As String

            Get
                Return m_SetupID
            End Get

            Set(ByVal setval As String)
                m_SetupID = setval
            End Set

        End Property

        Public Property BillEmail() As String

            Get
                Return m_BillEmail
            End Get

            Set(ByVal setval As String)
                m_BillEmail = setval
            End Set

        End Property

        Public Property ShipEmail() As String

            Get
                Return m_ShipEmail
            End Get

            Set(ByVal setval As String)
                m_ShipEmail = setval
            End Set

        End Property



    End Class
    Public bPOSetupInfo As POSetupcls = New POSetupcls, nPOSetupInfo As POSetupcls = New POSetupcls

    Public Sub SetPOSetupValues(sqlReader As SqlDataReader, POSetupBuf As POSetupcls)
        Try
            POSetupBuf.SetupID = sqlReader("SetupID")
            POSetupBuf.BillEmail = sqlReader("BillEmail")
            POSetupBuf.ShipEmail = sqlReader("ShipEmail")

        Catch
            POSetupBuf.SetupID = String.Empty
            POSetupBuf.BillEmail = String.Empty
            POSetupBuf.ShipEmail = String.Empty

        End Try

    End Sub
End Module