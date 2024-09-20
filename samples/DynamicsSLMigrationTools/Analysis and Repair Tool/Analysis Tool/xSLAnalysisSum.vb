Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Module sdoXSLANALYSISSUM

    Public Class xSLAnalysisSum

        Private m_AnalysisType As String
        Private m_Descr As String
        Private m_LupdDateTime As Date
        Private m_Module As String
        Private m_RecordID As Integer
        Private m_Result As String

        Public Property AnalysisType() As String

            Get
                Return m_AnalysisType
            End Get

            Set(ByVal setval As String)
                m_AnalysisType = setval
            End Set

        End Property

        Public Property Descr() As String

            Get
                Return m_Descr
            End Get

            Set(ByVal setval As String)
                m_Descr = setval
            End Set

        End Property


        Public Property LUpdDateTime() As Date

            Get
                Return m_LupdDateTime
            End Get

            Set(ByVal setval As Date)
                m_LupdDateTime = setval
            End Set

        End Property


        Public Property ModuleID() As String

            Get
                Return m_Module
            End Get

            Set(ByVal setval As String)
                m_Module = setval
            End Set

        End Property

        Public Property RecordID() As Integer

            Get
                Return m_RecordID
            End Get

            Set(ByVal setval As Integer)
                m_RecordID = setval
            End Set

        End Property
        Public Property Result() As String

            Get
                Return m_Result
            End Get

            Set(ByVal setval As String)
                m_Result = setval
            End Set

        End Property

    End Class

    Public bxSLAnalysisSum As xSLAnalysisSum = New xSLAnalysisSum, nxSLAnalysisSum As xSLAnalysisSum = New xSLAnalysisSum
    Public bxSLAnalysisSum1 As xSLAnalysisSum = New xSLAnalysisSum, nxSLAnalysisSum1 As xSLAnalysisSum = New xSLAnalysisSum
    Public CSR_xSLAnalysisSum As Integer

End Module 
