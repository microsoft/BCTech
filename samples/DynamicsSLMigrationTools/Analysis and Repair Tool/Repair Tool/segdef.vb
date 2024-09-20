Imports System.Data.SqlClient
Module Segdef


    Public Class SegDefCls

        Private m_Active As Short
        Private m_Crtd_DateTime As DateTime
        Private m_Crtd_Prog As String
        Private m_Crtd_User As String
        Private m_Description As String
        Private m_FieldClass As String
        Private m_FieldClassName As String
        Private m_ID As String
        Private m_LUpd_DateTime As DateTime
        Private m_LUpd_Prog As String
        Private m_LUpd_User As String
        Private m_SegNumber As String
        Private m_User1 As String
        Private m_User2 As String
        Private m_User3 As Double
        Private m_User4 As Double




        Public Property Active() As Short

            Get
                Return m_Active
            End Get

            Set(ByVal setval As Short)
                m_Active = setval
            End Set

        End Property

        Public Property Crtd_DateTime() As DateTime

            Get
                Return m_Crtd_DateTime
            End Get

            Set(ByVal setval As DateTime)
                m_Crtd_DateTime = setval
            End Set

        End Property

        Public Property Crtd_Prog() As String

            Get
                Return m_Crtd_Prog
            End Get

            Set(ByVal setval As String)
                m_Crtd_Prog = setval
            End Set

        End Property

        Public Property Crtd_User() As String

            Get
                Return m_Crtd_User
            End Get

            Set(ByVal setval As String)
                m_Crtd_User = setval
            End Set

        End Property
        Public Property Description() As String

            Get
                Return m_Description
            End Get

            Set(ByVal setval As String)
                m_Description = setval
            End Set

        End Property

        Public Property FieldClass() As String

            Get
                Return m_FieldClass
            End Get

            Set(ByVal setval As String)
                m_FieldClass = setval
            End Set

        End Property

        Public Property FieldClassName() As String

            Get
                Return m_FieldClassName
            End Get

            Set(ByVal setval As String)
                m_FieldClassName = setval
            End Set

        End Property

        Public Property ID() As String

            Get
                Return m_ID
            End Get

            Set(ByVal setval As String)
                m_ID = setval
            End Set

        End Property

        Public Property LUpd_DateTime() As DateTime

            Get
                Return m_LUpd_DateTime
            End Get

            Set(ByVal setval As DateTime)
                m_LUpd_DateTime = setval
            End Set

        End Property

        Public Property LUpd_Prog() As String

            Get
                Return m_LUpd_Prog
            End Get

            Set(ByVal setval As String)
                m_LUpd_Prog = setval
            End Set

        End Property

        Public Property LUpd_User() As String

            Get
                Return m_LUpd_User
            End Get

            Set(ByVal setval As String)
                m_LUpd_User = setval
            End Set

        End Property

        Public Property SegNumber() As String

            Get
                Return m_SegNumber
            End Get

            Set(ByVal setval As String)
                m_SegNumber = setval
            End Set

        End Property

        Public Property User1() As String

            Get
                Return m_User1
            End Get

            Set(ByVal setval As String)
                m_User1 = setval
            End Set

        End Property

        Public Property User2() As String

            Get
                Return m_User2
            End Get

            Set(ByVal setval As String)
                m_User2 = setval
            End Set

        End Property

        Public Property User3() As Double

            Get
                Return m_User3
            End Get

            Set(ByVal setval As Double)
                m_User3 = setval
            End Set

        End Property

        Public Property User4() As Double

            Get
                Return m_User4
            End Get

            Set(ByVal setval As Double)
                m_User4 = setval
            End Set

        End Property

    End Class
    Public bSegDefInfo As SegDefCls = New SegDefCls, nSegDefInfo As SegDefCls = New SegDefCls

    Public Sub SetSegDefValues(sqlReader As SqlDataReader, SegDefBuf As SegDefCls)
        Try

            SegDefBuf.Active = sqlReader("Active")
            SegDefBuf.Crtd_DateTime = sqlReader("Crtd_DateTime")
            SegDefBuf.Crtd_Prog = sqlReader("Crtd_Prog")
            SegDefBuf.Crtd_User = sqlReader("Crtd_User")
            SegDefBuf.Description = sqlReader("Description")
            SegDefBuf.FieldClass = sqlReader("FieldClass")
            SegDefBuf.FieldClassName = sqlReader("FieldClassName")
            SegDefBuf.ID = sqlReader("ID")
            SegDefBuf.LUpd_DateTime = sqlReader("LUpd_DateTime")
            SegDefBuf.LUpd_Prog = sqlReader("LUpd_Prog")
            SegDefBuf.LUpd_User = sqlReader("LUpd_User")
            SegDefBuf.SegNumber = sqlReader("SegNumber")
            SegDefBuf.User1 = sqlReader("User1")
            SegDefBuf.User2 = sqlReader("User2")
            SegDefBuf.User3 = sqlReader("User3")
            SegDefBuf.User4 = sqlReader("User4")

        Catch ex As Exception
            SegDefBuf.Active = 0
            SegDefBuf.Crtd_DateTime = MinDateValue
            SegDefBuf.Crtd_Prog = String.Empty
            SegDefBuf.Crtd_User = String.Empty
            SegDefBuf.Description = String.Empty
            SegDefBuf.FieldClass = String.Empty
            SegDefBuf.FieldClassName = String.Empty
            SegDefBuf.ID = String.Empty
            SegDefBuf.LUpd_DateTime = MinDateValue
            SegDefBuf.LUpd_Prog = String.Empty
            SegDefBuf.LUpd_User = String.Empty
            SegDefBuf.SegNumber = String.Empty
            SegDefBuf.User1 = String.Empty
            SegDefBuf.User2 = String.Empty
            SegDefBuf.User3 = 0.0
            SegDefBuf.User4 = 0.0
        End Try


    End Sub
End Module
