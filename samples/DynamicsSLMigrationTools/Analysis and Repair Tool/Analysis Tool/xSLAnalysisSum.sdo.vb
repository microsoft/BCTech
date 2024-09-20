Option Strict Off
Option Explicit On
Imports Solomon.Kernel
Module sdoXSLANALYSISSUM 
 
Public Class xSLAnalysisSum
	Inherits SolomonDataObject
		< _ 
			DataBinding(PropertyIndex:=0, StringSize:=30) _ 
		> _ 
		Public Property AnalysisType() As String 
 
			Get 
				Return Me.GetPropertyValue("AnalysisType") 
			End Get 
 
			Set (ByVal setval As String) 
				Me.SetPropertyValue("AnalysisType", setval) 
			End Set 
 
		End Property 
 
		< _ 
			DataBinding(PropertyIndex:=1, StringSize:=100) _ 
		> _ 
		Public Property Descr() As String 
 
			Get 
				Return Me.GetPropertyValue("Descr") 
			End Get 
 
			Set (ByVal setval As String) 
				Me.SetPropertyValue("Descr", setval) 
			End Set 
 
		End Property 
 
		< _ 
			DataBinding(PropertyIndex:=2) _ 
		> _ 
		Public Property LUpd_DateTime() As Integer 
 
			Get 
				Return Me.GetPropertyValue("LUpd_DateTime") 
			End Get 
 
			Set (ByVal setval As Integer) 
				Me.SetPropertyValue("LUpd_DateTime", setval) 
			End Set 
 
		End Property 
 
		< _ 
			DataBinding(PropertyIndex:=3, StringSize:=2) _ 
		> _ 
		Public Property Module_Renamed() As String 
 
			Get 
				Return Me.GetPropertyValue("Module_Renamed") 
			End Get 
 
			Set (ByVal setval As String) 
				Me.SetPropertyValue("Module_Renamed", setval) 
			End Set 
 
		End Property 
 
		< _ 
			DataBinding(PropertyIndex:=4) _ 
		> _ 
		Public Property RecordID() As Integer 
 
			Get 
				Return Me.GetPropertyValue("RecordID") 
			End Get 
 
			Set (ByVal setval As Integer) 
				Me.SetPropertyValue("RecordID", setval) 
			End Set 
 
		End Property 
 
		< _ 
			DataBinding(PropertyIndex:=5, StringSize:=60) _ 
		> _ 
		Public Property Result() As String 
 
			Get 
				Return Me.GetPropertyValue("Result") 
			End Get 
 
			Set (ByVal setval As String) 
				Me.SetPropertyValue("Result", setval) 
			End Set 
 
		End Property 
 
	End Class 
 
    Public bxSLAnalysisSum As xSLAnalysisSum = New xSLAnalysisSum, nxSLAnalysisSum As xSLAnalysisSum = New xSLAnalysisSum
    Public bxSLAnalysisSum1 As xSLAnalysisSum = New xSLAnalysisSum, nxSLAnalysisSum1 As xSLAnalysisSum = New xSLAnalysisSum
    Public CSR_xSLAnalysisSum As Integer

End Module 
