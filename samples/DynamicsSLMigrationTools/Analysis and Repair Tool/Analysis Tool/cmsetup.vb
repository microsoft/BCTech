Imports System.Data.SqlClient
Module CMSETUP
    Public Class CMSETUPCls

        Private m_APRtTpDflt As String
        Private m_APRateDate As String
        Private m_ARRtTpDflt As String
        Private m_CARtTpDflt As String
        Private m_GLRtTpDflt As String
        Private m_MCActivated As Short
        Private m_OERateDate As String

        Public Property APRtTpDflt() As String

            Get
                Return m_APRtTpDflt
            End Get

            Set(ByVal setval As String)
                m_APRtTpDflt = setval
            End Set

        End Property
        Public Property ARRtTpDflt() As String

            Get
                Return m_ARRtTpDflt
            End Get

            Set(ByVal setval As String)
                m_ARRtTpDflt = setval
            End Set

        End Property
        Public Property APRateDate() As String

            Get
                Return m_APRateDate
            End Get

            Set(ByVal setval As String)
                m_APRateDate = setval
            End Set

        End Property
        Public Property CARtTpDflt() As String

            Get
                Return m_CARtTpDflt
            End Get

            Set(ByVal setval As String)
                m_CARtTpDflt = setval
            End Set

        End Property
        Public Property GLRtTpDflt() As String

            Get
                Return m_GLRtTpDflt
            End Get

            Set(ByVal setval As String)
                m_GLRtTpDflt = setval
            End Set

        End Property

        Public Property MCActivated() As Short

            Get
                Return m_MCActivated
            End Get

            Set(ByVal setval As Short)
                m_MCActivated = setval
            End Set

        End Property
        Public Property OERateDate() As String

            Get
                Return m_OERateDate
            End Get

            Set(ByVal setval As String)
                m_OERateDate = setval
            End Set

        End Property


    End Class
    Public bCMSETUPInfo As CMSETUPCls = New CMSETUPCls, nCMSETUPInfo As CMSETUPCls = New CMSETUPCls

    Public Sub SetCMSetupValues(sqlReader As SqlDataReader, CMSetupBuf As CMSETUPCls)
        Try
            CMSetupBuf.APRateDate = sqlReader("APRateDate")
            CMSetupBuf.APRtTpDflt = sqlReader("APRtTpDflt")
            CMSetupBuf.ARRtTpDflt = sqlReader("ARRtTpDflt")
            CMSetupBuf.CARtTpDflt = sqlReader("CARtTpDflt")
            CMSetupBuf.GLRtTpDflt = sqlReader("GLRtTpDflt")
            CMSetupBuf.OERateDate = sqlReader("OERateDate")
            CMSetupBuf.MCActivated = sqlReader("MCActivated")
        Catch ex As Exception
            CMSetupBuf.APRateDate = String.Empty
            CMSetupBuf.APRtTpDflt = String.Empty
            CMSetupBuf.ARRtTpDflt = String.Empty
            CMSetupBuf.CARtTpDflt = String.Empty
            CMSetupBuf.GLRtTpDflt = String.Empty
            CMSetupBuf.OERateDate = String.Empty
            CMSetupBuf.MCActivated = 0
        End Try
    End Sub

    Public Sub InitCMSetupValues(CMSetupBuf As CMSETUPCls)
        CMSetupBuf.APRateDate = String.Empty
        CMSetupBuf.APRtTpDflt = String.Empty
        CMSetupBuf.ARRtTpDflt = String.Empty
        CMSetupBuf.CARtTpDflt = String.Empty
        CMSetupBuf.GLRtTpDflt = String.Empty
        CMSetupBuf.OERateDate = String.Empty
        CMSetupBuf.MCActivated = 0
    End Sub
End Module
