Imports System.Data.SqlClient
Module XSLMPTSUBERRORS

    Private m_ID As String
    Private m_SegNumber As String

    Public Class xSLMPTSubErrorsCls
        Public Property ID() As String

            Get
                Return m_ID
            End Get

            Set(ByVal setval As String)
                m_ID = setval
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

    End Class

    Public bxSLMPTSubErrorsInfo As xSLMPTSubErrorsCls = New xSLMPTSubErrorsCls, nxSLMPTSubErrorsInfo As xSLMPTSubErrorsCls = New xSLMPTSubErrorsCls

    Public Sub SetxSLMPTSubErrorValues(sqlReader As SqlDataReader, xSLMPTSubErrorsBuf As xSLMPTSubErrorsCls)
        Try
            xSLMPTSubErrorsBuf.ID = sqlReader("ID")
            xSLMPTSubErrorsBuf.SegNumber = sqlReader("SegNumber")
        Catch ex As Exception
            xSLMPTSubErrorsBuf.ID = String.Empty
            xSLMPTSubErrorsBuf.SegNumber = String.Empty
        End Try
    End Sub

End Module 
