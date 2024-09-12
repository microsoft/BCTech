

Imports System.IO

Imports System.Text

Friend Class clsEventLog

    Private fNewFile As Boolean
    Private fLogClosed As Boolean
    Private oLogFile As FileInfo
    Private oLogText As StreamWriter
    Private sFileName As String
    Private sMsgTxt As New StringBuilder
    Private LOG_FILE As String = "Bob.log"
    Private WriteError As Boolean = False
    Private Declare Sub WaitSleep Lib "kernel32" Alias "Sleep" (ByVal dwMilliseconds As Integer)

    Private txtRichText As RichTextBox = Nothing

    Private Sub Class_Initialize_Renamed()
        LogClosed = True
        MessageText = ""
        NewFile = False
    End Sub

    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    Private Sub Class_Terminate_Renamed()
        If Not LogClosed Then
            Call WriteLog(String.Empty)
            Call CloseLog()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Class_Terminate_Renamed()
        MyBase.Finalize()
    End Sub

    Public Property FileName() As String
        Get
            FileName = sFileName
        End Get
        Set(ByVal Value As String)
            sFileName = Value
            LogFile = GetLogFile()
        End Set
    End Property

    Public Property LogClosed() As Boolean
        Get
            LogClosed = fLogClosed
        End Get
        Set(ByVal Value As Boolean)
            fLogClosed = Value
        End Set
    End Property

    Public Property LogFile() As FileInfo
        Get
            LogFile = oLogFile
        End Get
        Set(ByVal Value As FileInfo)
            oLogFile = Value
        End Set
    End Property

    Public Property LogText() As StreamWriter
        Get
            LogText = oLogText
        End Get
        Set(ByVal Value As StreamWriter)
            oLogText = Value
        End Set
    End Property

    Public Property MessageText() As String
        Get
            MessageText = sMsgTxt.ToString
        End Get
        Set(ByVal Value As String)
            If sMsgTxt.Length = 0 Then
                sMsgTxt.Append(Trim$(Value))
            Else
                sMsgTxt.AppendLine(Trim$(Value))
            End If
        End Set
    End Property

    Public Property NewFile() As Boolean
        Get
            NewFile = fNewFile
        End Get
        Set(ByVal Value As Boolean)
            fNewFile = Value
        End Set
    End Property

    Public Sub LogMessage(ByVal pMsgNo As Short, Optional ByVal pMessage As String = "")
        If WriteError = False Then
            Select Case pMsgNo
                Case 0
                    MessageText = pMessage
                Case StartProcess
                    Call OpenLog()
                Case StopProcess, EndProcess
                    Call WriteLog(pMessage)
                    Call CloseLog()
                Case Else
            End Select
        End If
    End Sub

    Public Function GetLogFile() As FileInfo
        'Dim oFileSys As Scripting.FileSystemObject
        'Dim oFolder As Scripting.Folder

        'oFileSys = New Scripting.FileSystemObject
        ' oFolder = oFileSys.GetFolder(GetEventLogPath)

        Dim FileDirInfo As New FileInfo(GetEventLogPath() & "\" & FileName)

        If (FileDirInfo.Exists()) Then
            '        If oFileSys.FileExists(oFolder.Path & "\" & FileName) Then
            NewFile = False
        Else
            NewFile = True
        End If
        GetLogFile = FileDirInfo
    End Function

    Private Function GetEventLogPath() As String

        Dim oFileSys As FileInfo
        Dim oFolder As DirectoryInfo
        Dim oSolRoot As DirectoryInfo

        oFileSys = New FileInfo(GetDirectoryLocation())
        oFolder = New DirectoryInfo(oFileSys.FullName)

        oSolRoot = oFolder
        oSolRoot = New DirectoryInfo(oSolRoot.FullName)
        GetEventLogPath = oSolRoot.FullName
    End Function

    Private Sub OpenLog()
        ' Open the StreamWriter - set to AutoFlush the stream.
        Try
            LogText = System.IO.File.AppendText(LogFile.FullName)
            LogText.AutoFlush = True

            LogClosed = False
        Catch ep As System.UnauthorizedAccessException
            'Display message saying that we can't write to the file path
            MessageBox.Show("Unable to open file: " & vbNewLine &
                            LogFile.FullName.Trim & vbNewLine & vbNewLine &
                            "Error message: " & vbNewLine &
                            ep.Message.Trim & vbNewLine & vbNewLine &
                            "Solomon.ini Eventlog Path needs to be changed." & vbNewLine &
                            "Processing will now continue with logging disabled.")
            WriteError = True
        Catch ex As Exception
            'Display message if differenct exception type
            MessageBox.Show("Unable to open file: " & vbNewLine &
                            LogFile.FullName.Trim & vbNewLine & vbNewLine &
                            "Error message: " & vbNewLine &
                            ex.Message.Trim & vbNewLine & vbNewLine &
                            "Processing will now continue with logging disabled.")
            WriteError = True
        End Try
    End Sub


    Private Sub WriteLog(strHeader As String)
        If Len(Trim(MessageText)) > 0 Then
            If Not NewFile Then
                Call LogText.Write(vbCrLf)
            End If

            Call LogText.Write(strHeader + vbCrLf)
            Call LogText.Write(New String("*", 80))
            Call LogText.Write(vbCrLf)
            Call LogText.Write("Company:       " & CpnyId + vbCrLf)
            Call LogText.Write("Date:          " & Date.Today + vbCrLf)
            Call LogText.Write("Time:          " & TimeOfDay.ToLongTimeString())
            Call LogText.Write(vbCrLf)
            Call LogText.Write(vbCrLf)
            Call LogText.Write(MessageText)
        End If
    End Sub



    Private Sub CloseLog()
        LogText.Close()
        LogClosed = True
    End Sub

    Private Function GetDirectoryLocation() As String
        Return EventLogDir.Trim()
    End Function
End Class
