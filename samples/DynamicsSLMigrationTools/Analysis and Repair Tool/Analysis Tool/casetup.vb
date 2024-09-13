'------------------------------------------------------------------------------
' <copyright file="casetup.sdo.vb" company="Microsoft">
'     Copyright (c) Microsoft Corporation.  All rights reserved.
' </copyright>
'------------------------------------------------------------------------------
Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Module casetup

    Public Class CASetupCls
        Private m_AcceptTransDate As Date
        Private m_CurrPerNbr As String
        Public Property AcceptTransDate() As Date

            Get
                Return m_AcceptTransDate
            End Get

            Set(ByVal setval As Date)
                m_AcceptTransDate = setval
            End Set

        End Property
        Public Property CurrPerNbr() As String

            Get
                Return m_CurrPerNbr
            End Get

            Set(ByVal setval As String)
                m_CurrPerNbr = setval
            End Set

        End Property


    End Class

    Public bCASetup As CASetupCls = New CASetupCls

    Public Sub SetCASetupValues(sqlReader As SqlDataReader, CASetupBuf As CASetupCls)
        Try
            CASetupBuf.AcceptTransDate = sqlReader("AcceptTransDate")
            CASetupBuf.CurrPerNbr = sqlReader("CurrPerNbr")
        Catch ex As Exception
            CASetupBuf.AcceptTransDate = Date.MinValue
            CASetupBuf.CurrPerNbr = String.Empty
        End Try
    End Sub

End Module
