'------------------------------------------------------------------------------
' <copyright file="gltran.sdo.vb" company="Microsoft">
'     Copyright (c) Microsoft Corporation.  All rights reserved.
' </copyright>
'------------------------------------------------------------------------------
Imports System.Data.SqlClient

Module sGLTRAN

    Public Class GLTranList

        Private m_AcctNum As String
        Private m_BatNbr As String
        Private m_CpnyId As String
        Private m_FiscYr As String
        Private m_LineNbr As Short
        Private m_ModuleID As String
        Private m_PerPost As String

        Public Property AcctNum() As String
            Get
                Return m_AcctNum
            End Get
            Set(ByVal setval As String)
                m_AcctNum = setval
            End Set
        End Property
        Public Property BatNbr() As String
            Get
                Return m_BatNbr
            End Get
            Set(ByVal setval As String)
                m_BatNbr = setval
            End Set
        End Property
        Public Property CpnyId() As String
            Get
                Return m_CpnyId
            End Get
            Set(ByVal setval As String)
                m_CpnyId = setval
            End Set
        End Property
        Public Property FiscYr() As String
            Get
                Return m_FiscYr
            End Get
            Set(ByVal setval As String)
                m_FiscYr = setval
            End Set
        End Property
        Public Property LineNbr() As Short
            Get
                Return m_LineNbr
            End Get
            Set(ByVal setval As Short)
                m_LineNbr = setval
            End Set
        End Property

        Public Property ModuleID() As String
            Get
                Return m_ModuleID
            End Get
            Set(ByVal setval As String)
                m_ModuleID = setval
            End Set
        End Property
        Public Property PerPost() As String
            Get
                Return m_PerPost
            End Get
            Set(ByVal setval As String)
                m_PerPost = setval
            End Set
        End Property
    End Class

    Public bGLTranList As GLTranList = New GLTranList, nGLTranList As GLTranList = New GLTranList


    Public Sub SetGLTranValues(sqlReader As SqlDataReader, GLTranInfo As GLTranList)
        Try

            GLTranInfo.AcctNum = sqlReader("Acct")
            GLTranInfo.BatNbr = sqlReader("BatNbr")
            GLTranInfo.CpnyId = sqlReader("CpnyId")
            GLTranInfo.FiscYr = sqlReader("FiscYr")
            GLTranInfo.LineNbr = sqlReader("LineNbr")
            GLTranInfo.ModuleID = sqlReader("Module")
            GLTranInfo.PerPost = sqlReader("PerPost")
        Catch ex As Exception
            GLTranInfo.AcctNum = String.Empty
            GLTranInfo.BatNbr = String.Empty
            GLTranInfo.CpnyId = String.Empty
            GLTranInfo.FiscYr = String.Empty
            GLTranInfo.LineNbr = 0
            GLTranInfo.ModuleID = String.Empty
            GLTranInfo.PerPost = String.Empty

        End Try


    End Sub
End Module
