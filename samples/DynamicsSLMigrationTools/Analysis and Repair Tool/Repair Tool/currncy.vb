Imports System.Data.SqlClient
Module CURRNCY
	
	Public Class CURRNCYCls
    < _
    DataBinding(PropertyIndex:=0, StringSize:=3) _
    > _
    Public Property BitmapId() As String

        Get
            Return Me.GetPropertyValue("BitmapId")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("BitmapId", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=1) _
    > _
    Public Property Crtd_DateTime() As Integer

        Get
            Return Me.GetPropertyValue("Crtd_DateTime")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("Crtd_DateTime", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=2, StringSize:=8) _
    > _
    Public Property Crtd_Prog() As String

        Get
            Return Me.GetPropertyValue("Crtd_Prog")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("Crtd_Prog", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=3, StringSize:=10) _
    > _
    Public Property Crtd_User() As String

        Get
            Return Me.GetPropertyValue("Crtd_User")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("Crtd_User", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=4, StringSize:=15) _
    > _
    Public Property CuryCapt() As String

        Get
            Return Me.GetPropertyValue("CuryCapt")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("CuryCapt", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=5, StringSize:=4) _
    > _
    Public Property curyid() As String

        Get
            Return Me.GetPropertyValue("curyid")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("curyid", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=6, StringSize:=4) _
    > _
    Public Property CurySym() As String

        Get
            Return Me.GetPropertyValue("CurySym")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("CurySym", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=7) _
    > _
    Public Property DecPl() As Short

        Get
            Return Me.GetPropertyValue("DecPl")
        End Get

        Set(ByVal setval As Short)
            Me.SetPropertyValue("DecPl", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=8, StringSize:=30) _
    > _
    Public Property Descr() As String

        Get
            Return Me.GetPropertyValue("Descr")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("Descr", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=9) _
    > _
    Public Property LUpd_DateTime() As Integer

        Get
            Return Me.GetPropertyValue("LUpd_DateTime")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("LUpd_DateTime", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=10, StringSize:=8) _
    > _
    Public Property LUpd_Prog() As String

        Get
            Return Me.GetPropertyValue("LUpd_Prog")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("LUpd_Prog", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=11, StringSize:=10) _
    > _
    Public Property LUpd_User() As String

        Get
            Return Me.GetPropertyValue("LUpd_User")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("LUpd_User", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=12) _
    > _
    Public Property NoteId() As Integer

        Get
            Return Me.GetPropertyValue("NoteId")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("NoteId", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=13, StringSize:=10) _
    > _
    Public Property RealGainAcct() As String

        Get
            Return Me.GetPropertyValue("RealGainAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RealGainAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=14, StringSize:=24) _
    > _
    Public Property RealGainSub() As String

        Get
            Return Me.GetPropertyValue("RealGainSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RealGainSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=15, StringSize:=10) _
    > _
    Public Property RealLossAcct() As String

        Get
            Return Me.GetPropertyValue("RealLossAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RealLossAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=16, StringSize:=24) _
    > _
    Public Property RealLossSub() As String

        Get
            Return Me.GetPropertyValue("RealLossSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RealLossSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=17, StringSize:=10) _
    > _
    Public Property RvalGainAcct() As String

        Get
            Return Me.GetPropertyValue("RvalGainAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RvalGainAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=18, StringSize:=24) _
    > _
    Public Property RvalGainSub() As String

        Get
            Return Me.GetPropertyValue("RvalGainSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RvalGainSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=19, StringSize:=10) _
    > _
    Public Property RvalLossAcct() As String

        Get
            Return Me.GetPropertyValue("RvalLossAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RvalLossAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=20, StringSize:=24) _
    > _
    Public Property RvalLossSub() As String

        Get
            Return Me.GetPropertyValue("RvalLossSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("RvalLossSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=21, StringSize:=30) _
    > _
    Public Property S4Future01() As String

        Get
            Return Me.GetPropertyValue("S4Future01")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("S4Future01", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=22, StringSize:=30) _
    > _
    Public Property S4Future02() As String

        Get
            Return Me.GetPropertyValue("S4Future02")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("S4Future02", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=23) _
    > _
    Public Property S4Future03() As Double

        Get
            Return Me.GetPropertyValue("S4Future03")
        End Get

        Set(ByVal setval As Double)
            Me.SetPropertyValue("S4Future03", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=24) _
    > _
    Public Property S4Future04() As Double

        Get
            Return Me.GetPropertyValue("S4Future04")
        End Get

        Set(ByVal setval As Double)
            Me.SetPropertyValue("S4Future04", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=25) _
    > _
    Public Property S4Future05() As Double

        Get
            Return Me.GetPropertyValue("S4Future05")
        End Get

        Set(ByVal setval As Double)
            Me.SetPropertyValue("S4Future05", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=26) _
    > _
    Public Property S4Future06() As Double

        Get
            Return Me.GetPropertyValue("S4Future06")
        End Get

        Set(ByVal setval As Double)
            Me.SetPropertyValue("S4Future06", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=27) _
    > _
    Public Property S4Future07() As Integer

        Get
            Return Me.GetPropertyValue("S4Future07")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("S4Future07", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=28) _
    > _
    Public Property S4Future08() As Integer

        Get
            Return Me.GetPropertyValue("S4Future08")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("S4Future08", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=29) _
    > _
    Public Property S4Future09() As Integer

        Get
            Return Me.GetPropertyValue("S4Future09")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("S4Future09", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=30) _
    > _
    Public Property S4Future10() As Integer

        Get
            Return Me.GetPropertyValue("S4Future10")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("S4Future10", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=31, StringSize:=10) _
    > _
    Public Property S4Future11() As String

        Get
            Return Me.GetPropertyValue("S4Future11")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("S4Future11", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=32, StringSize:=10) _
    > _
    Public Property S4Future12() As String

        Get
            Return Me.GetPropertyValue("S4Future12")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("S4Future12", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=33, StringSize:=1) _
    > _
    Public Property Status() As String

        Get
            Return Me.GetPropertyValue("Status")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("Status", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=34, StringSize:=10) _
    > _
    Public Property TrslGainAcct() As String

        Get
            Return Me.GetPropertyValue("TrslGainAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("TrslGainAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=35, StringSize:=24) _
    > _
    Public Property TrslGainSub() As String

        Get
            Return Me.GetPropertyValue("TrslGainSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("TrslGainSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=36, StringSize:=10) _
    > _
    Public Property TrslLossAcct() As String

        Get
            Return Me.GetPropertyValue("TrslLossAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("TrslLossAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=37, StringSize:=24) _
    > _
    Public Property TrslLossSub() As String

        Get
            Return Me.GetPropertyValue("TrslLossSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("TrslLossSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=38, StringSize:=10) _
    > _
    Public Property UnrlGainAcct() As String

        Get
            Return Me.GetPropertyValue("UnrlGainAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("UnrlGainAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=39, StringSize:=24) _
    > _
    Public Property UnrlGainSub() As String

        Get
            Return Me.GetPropertyValue("UnrlGainSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("UnrlGainSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=40, StringSize:=10) _
    > _
    Public Property UnrlLossAcct() As String

        Get
            Return Me.GetPropertyValue("UnrlLossAcct")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("UnrlLossAcct", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=41, StringSize:=24) _
    > _
    Public Property UnrlLossSub() As String

        Get
            Return Me.GetPropertyValue("UnrlLossSub")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("UnrlLossSub", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=42, StringSize:=30) _
    > _
    Public Property User1() As String

        Get
            Return Me.GetPropertyValue("User1")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("User1", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=43, StringSize:=30) _
    > _
    Public Property User2() As String

        Get
            Return Me.GetPropertyValue("User2")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("User2", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=44) _
    > _
    Public Property User3() As Double

        Get
            Return Me.GetPropertyValue("User3")
        End Get

        Set(ByVal setval As Double)
            Me.SetPropertyValue("User3", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=45) _
    > _
    Public Property User4() As Double

        Get
            Return Me.GetPropertyValue("User4")
        End Get

        Set(ByVal setval As Double)
            Me.SetPropertyValue("User4", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=46, StringSize:=10) _
    > _
    Public Property User5() As String

        Get
            Return Me.GetPropertyValue("User5")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("User5", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=47, StringSize:=10) _
    > _
    Public Property User6() As String

        Get
            Return Me.GetPropertyValue("User6")
        End Get

        Set(ByVal setval As String)
            Me.SetPropertyValue("User6", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=48) _
    > _
    Public Property User7() As Integer

        Get
            Return Me.GetPropertyValue("User7")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("User7", setval)
        End Set

    End Property

    < _
    DataBinding(PropertyIndex:=49) _
    > _
    Public Property User8() As Integer

        Get
            Return Me.GetPropertyValue("User8")
        End Get

        Set(ByVal setval As Integer)
            Me.SetPropertyValue("User8", setval)
        End Set

    End Property

	End Class
Public bCURRNCY As CURRNCY = New CURRNCY, nCURRNCY As CURRNCY = New CURRNCY
End Module
