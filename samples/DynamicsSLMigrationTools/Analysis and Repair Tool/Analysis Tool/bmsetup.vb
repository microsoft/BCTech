'VBTools Converter Version: 7.0.50919.0
'------------------------------------------------------------------------------
' <copyright file="bmsetup.sdo.vb" company="Microsoft">
'     Copyright (c) Microsoft Corporation.  All rights reserved.
' </copyright>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On
Imports System.Data.SqlClient


Module sdoBMSetup

    Public Class BMSetupCls

        Private m_MatlOvhCalc As String
        Private m_MatlOvhRatePct As String
        Private m_LbrOvhRatePct As String
        Private m_SiteBOM As Short





        Public Property LbrOvhRatePct() As String

            Get
                Return m_LbrOvhRatePct
            End Get

            Set(ByVal setval As String)
                m_LbrOvhRatePct = setval
            End Set

        End Property





        Public Property MatlOvhCalc() As String

            Get
                Return m_MatlOvhCalc
            End Get

            Set(ByVal setval As String)
                m_MatlOvhCalc = setval
            End Set

        End Property




        Public Property MatlOvhRatePct() As String

            Get
                Return m_MatlOvhRatePct
            End Get

            Set(ByVal setval As String)
                m_MatlOvhRatePct = setval
            End Set

        End Property



        Public Property SiteBOM() As Short

            Get
                Return m_SiteBOM
            End Get

            Set(ByVal setval As Short)
                m_SiteBOM = setval
            End Set

        End Property


    End Class

    Public bBMSetup As BMSetupCls = New BMSetupCls, nBMSetup As BMSetupCls = New BMSetupCls

    Public Sub SetBMSetupValues(sqlReader As SqlDataReader, BMSetupBuf As BMSetupCls)
        Try
            BMSetupBuf.LbrOvhRatePct = sqlReader("LbrOvhRatePct")
            BMSetupBuf.MatlOvhCalc = sqlReader("MatlOvhCalc")
            BMSetupBuf.MatlOvhRatePct = sqlReader("MatlOvhRatePct")
            BMSetupBuf.SiteBOM = sqlReader("SiteBOM")
        Catch ex As Exception
            BMSetupBuf.LbrOvhRatePct = String.Empty
            BMSetupBuf.MatlOvhCalc = String.Empty
            BMSetupBuf.MatlOvhRatePct = String.Empty
            BMSetupBuf.SiteBOM = String.Empty
        End Try
    End Sub

End Module
