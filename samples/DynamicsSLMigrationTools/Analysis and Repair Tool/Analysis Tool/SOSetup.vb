'------------------------------------------------------------------------------
' <copyright file="SOSetup.sdo.vb" company="Microsoft">
'     Copyright (c) Microsoft Corporation.  All rights reserved.
' </copyright>
'------------------------------------------------------------------------------
'VBTools Converter Version: 7.0.50813.0
Option Strict Off
Option Explicit On
Imports System.Data.SqlClient


Module SOSetup_sdo

    Public Class SOSetupCls
        Private m_AllowDiscPrice As Short
        Private m_ChainDiscEnabled As Short
        Private m_ConsolInv As Short
        Private m_CreditCheck As Short
        Private m_DiscBySite As Short
        Private m_PostBookings As Short
        Private m_TotalOrdDisc As Short

        Public Property AllowDiscPrice() As Short

            Get
                Return m_AllowDiscPrice
            End Get

            Set(ByVal setval As Short)
                m_AllowDiscPrice = setval
            End Set

        End Property


        Public Property ChainDiscEnabled() As Short

            Get
                Return m_ChainDiscEnabled
            End Get

            Set(ByVal setval As Short)
                m_ChainDiscEnabled = setval
            End Set

        End Property

        Public Property ConsolInv() As Short

            Get
                Return m_ConsolInv
            End Get

            Set(ByVal setval As Short)
                m_ConsolInv = setval
            End Set

        End Property


        Public Property CreditCheck() As Short

            Get
                Return m_CreditCheck
            End Get

            Set(ByVal setval As Short)
                m_CreditCheck = setval
            End Set

        End Property

        Public Property DiscBySite() As Short

            Get
                Return m_DiscBySite
            End Get

            Set(ByVal setval As Short)
                m_DiscBySite = setval
            End Set

        End Property



        Public Property PostBookings() As Short

            Get
                Return m_PostBookings
            End Get

            Set(ByVal setval As Short)
                m_PostBookings = setval
            End Set

        End Property


        Public Property TotalOrdDisc() As Short

            Get
                Return m_TotalOrdDisc
            End Get

            Set(ByVal setval As Short)
                m_TotalOrdDisc = setval
            End Set

        End Property


    End Class

    Public bSOSetup As SOSetupCls = New SOSetupCls, nSOSetup As SOSetupCls = New SOSetupCls

    Public Sub SetSOSetupValues(sqlReader As SqlDataReader, bSOSetupBuf As SOSetupCls)
        Try
            bSOSetup.AllowDiscPrice = sqlReader("AllowDiscPrice")
            bSOSetup.ChainDiscEnabled = sqlReader("ChainDiscEnabled")
            bSOSetup.ConsolInv = sqlReader("ConsolInv")
            bSOSetup.CreditCheck = sqlReader("CreditCheck")
            bSOSetup.DiscBySite = sqlReader("DiscBySite")
            bSOSetup.PostBookings = sqlReader("PostBookings")
            bSOSetup.TotalOrdDisc = sqlReader("TotalOrdDisc")

        Catch ex As Exception
            bSOSetup.AllowDiscPrice = 0
            bSOSetup.ChainDiscEnabled = 0
            bSOSetup.ConsolInv = 0
            bSOSetup.CreditCheck = 0
            bSOSetup.DiscBySite = 0
            bSOSetup.PostBookings = 0
            bSOSetup.TotalOrdDisc = 0
        End Try
    End Sub
End Module
