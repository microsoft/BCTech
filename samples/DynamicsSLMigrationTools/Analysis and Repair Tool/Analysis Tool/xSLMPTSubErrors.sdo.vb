'------------------------------------------------------------------------------ 
' <copyright file="xslmptsuberrors.sdo.vb" company="Microsoft"> 
'     Copyright (c) Microsoft Corporation.  All rights reserved. 
' </copyright> 
'------------------------------------------------------------------------------ 
 
Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Module sdoXSLMPTSUBERRORS 
 
Public Class xSLMPTSubErrors
        Private m_ID As String
        Private m_SegNumber As String

        Public Property ID() As String 
 
			Get
                Return m_ID
            End Get 
 
			Set (ByVal setval As String)
                m_ID = setval
            End Set 
 
		End Property

        Public Property SegNumber() As String 
 
			Get
                Return m_SegNumber
            End Get 
 
			Set (ByVal setval As String)
                m_SegNumber = setval
            End Set 
 
		End Property 
 
	End Class 
 
    Public bxSLMPTSubErrors As xSLMPTSubErrors = New xSLMPTSubErrors, nxSLMPTSubErrors As xSLMPTSubErrors = New xSLMPTSubErrors


    Public Sub SetxSLMPTSubValues(sqlReader As SqlDataReader, SLMPTSubBuf As xSLMPTSubErrors)
        Try
            SLMPTSubBuf.ID = sqlReader("ID")
            SLMPTSubBuf.SegNumber = sqlReader("SegNumber")
        Catch ex As Exception
            SLMPTSubBuf.ID = String.Empty
            SLMPTSubBuf.SegNumber = String.Empty
        End Try
    End Sub
End Module 
