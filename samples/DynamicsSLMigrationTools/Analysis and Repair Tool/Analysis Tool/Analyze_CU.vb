Option Strict Off
Option Explicit On
Imports System.Data.SqlClient
Imports System.IO

Module Analyze_CU
    '================================================================================
    ' This module contains code to analyze Customization type information
    '================================================================================

    Public Sub Analyze_CU()
        Dim sqlStringExec As String = String.Empty
        Dim sqlStringStart As String = "INSERT INTO xSLAnalysisSum VALUES("
        Dim sqlStringValues As String = String.Empty
        Dim sqlStringEnd As String = ", NULL)"
        Dim sAnalysisType As String = String.Empty
        Dim sDescr As String = String.Empty
        Dim sModule As String = "CU"
        Dim sResult As String = String.Empty
        Dim retValInt1 As Integer

        Dim sqlStmt As String = String.Empty
        Dim sqlReader As SqlDataReader = Nothing
        Dim sqlReader1 As SqlDataReader = Nothing

        Dim CurrLoopMd As String = String.Empty
        Dim fileNameWithoutExtension As String = String.Empty
        Dim lcFullScreenName As String = String.Empty
        Dim rptFiles As String()  '= String.Empty
        Dim SLUsrRptsPath As String = String.Empty

        Try
            '=== Customizations ===
            Form1.UpdateAnalysisToolStatusBar("Analyzing Customizations")

            Call oEventLog.LogMessage(0, "CUSTOMIZATIONS")
            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "Analyzing Customizations")

            sAnalysisType = "Customizations"

            RecID = RecID + 1

            ' display customization legend
            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "Sequence/Level Legend:")
            Call oEventLog.LogMessage(0, "100 = Supplemental Product. The Customization will only load for ALL users")
            Call oEventLog.LogMessage(0, "200 = Language Specific. The Customization will only load for ALL users")
            Call oEventLog.LogMessage(0, "300 = All Users. The Customization will only load for ALL users")
            Call oEventLog.LogMessage(0, "350 = Group. The Customization will only load for users whose Customization Group Is the same as the Group specified in the EntityID field")
            Call oEventLog.LogMessage(0, "400 = One User. The Customization will only load for the user specified in the EntityID field")
            Call oEventLog.LogMessage(0, "500 = Self. The Customization will only load for the creator")
            Call oEventLog.LogMessage(0, "")

            ' Get count of VBA customizations 
            sDescr = "Number of VBA Customizations in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM (SELECT DISTINCT c.ScreenId, s.Name, s.Module, c.Sequence,c.EntityId FROM CustomVBA c LEFT JOIN Screen s ON c.ScreenId = s.Number) AS SubQuery;"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Get list distinct VBA customizations 
            If retValInt1 > 0 Then

                sqlStmt = "SELECT DISTINCT ISNULL(c.ScreenId,s.Number) [Screen], ISNULL(s.Name,'??'), ISNULL(s.Module,'??'), c.Sequence, c.EntityId, c.Description"
                sqlStmt += " FROM CustomVBA c left join Screen s on c.ScreenId = s.Number"
                sqlStmt += " ORDER BY c.Sequence, [Screen], c.EntityId"

                'sqlStmt += " Level = CASE When c.Sequence = '100' then 'Supplemental' When c.Sequence = '200' then 'Language' When c.Sequence = '250' then 'Locale' When c.Sequence = '300' then 'All Users' When c.Sequence = '400' then 'One User' When c.Sequence = '500' then 'Self' else CONVERT(varchar(5), c.Sequence) end, c.EntityId"
                'sqlStmt += " ORDER BY 'Level', [Screen], c.EntityId"

                Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()

                        lcFullScreenName = sqlReader(1).Trim + " (" + sqlReader(0).Substring(0, 2) + "." + sqlReader(0).Substring(2, 3) + "." + sqlReader(0).Substring(5, 2) + ")"

                        If sqlReader(1).trim <> "??" Then
                            lcFullScreenName = sqlReader(1).Trim + " (" + sqlReader(0).Substring(0, 2) + "." + sqlReader(0).Substring(2, 3) + "." + sqlReader(0).Substring(5, 2) + ")"
                        Else
                            lcFullScreenName = "Unknown Screen (" + sqlReader(0).Trim + ")"
                        End If

                        RecID = RecID + 1
                        If sqlReader(4).trim <> "" Then
                            'sDescr = Strings.Left(" - " + lcFullScreenName.Trim() + " | Md: " + sqlReader(2).Trim() + " | Level: " + sqlReader(3).ToString.Trim() + " | Descr: " + sqlReader(5).ToString.Trim() + " | Entity: " + sqlReader(4).Trim(), 100)
                            sDescr = Strings.Left(" - " + lcFullScreenName.Trim() + " | " + sqlReader(3).ToString.Trim() + " | Descr: " + sqlReader(5).ToString.Trim() + " | Entity: " + sqlReader(4).Trim(), 100)
                        Else
                            'sDescr = Strings.Left(" - " + lcFullScreenName.Trim() + " | Md: " + sqlReader(2).Trim() + " | Level: " + sqlReader(3).ToString.Trim() + " | Descr: " + sqlReader(5).ToString.Trim(), 100)
                            sDescr = Strings.Left(" - " + lcFullScreenName.Trim() + " | " + sqlReader(3).ToString.Trim() + " | Descr: " + sqlReader(5).ToString.Trim(), 100)
                        End If
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If
            Call sqlReader.Close()

            Call oEventLog.LogMessage(0, "")

            RecID = RecID + 1

            ' Get count of BSL customizations 
            sDescr = "Number of BSL Customizations in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM (SELECT DISTINCT c.ScreenId, s.Name, s.Module, c.Sequence,c.EntityId FROM Custom2 c LEFT JOIN Screen s ON c.ScreenId = s.Number) AS SubQuery;"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Get list distinct BSL customizations 
            If retValInt1 > 0 Then

                sqlStmt = "SELECT DISTINCT ISNULL(c.ScreenId,s.Number) [Screen], ISNULL(s.Name,'??'), ISNULL(s.Module,'??'), c.Sequence, c.EntityId, c.Description"
                sqlStmt += " FROM Custom2 c left join Screen s on c.ScreenId = s.Number"
                sqlStmt += " ORDER BY c.Sequence, [Screen], c.EntityId"


                Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()

                        lcFullScreenName = sqlReader(1).Trim + " (" + sqlReader(0).Substring(0, 2) + "." + sqlReader(0).Substring(2, 3) + "." + sqlReader(0).Substring(5, 2) + ")"

                        RecID = RecID + 1
                        If sqlReader(4).trim <> "" Then
                            sDescr = Strings.Left(" - " + lcFullScreenName.Trim() + " | Md: " + sqlReader(2).Trim() + " | Level: " + sqlReader(3).Trim() + " | Entity: " + sqlReader(4).Trim(), 100)
                        Else
                            sDescr = Strings.Left(" - " + lcFullScreenName.Trim() + " | Md: " + sqlReader(2).Trim() + " | Level: " + sqlReader(3).Trim(), 100)
                        End If
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If
            Call sqlReader.Close()
            '***** End of Customizations section *****

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

            '=== Templates ===
            Form1.UpdateAnalysisToolStatusBar("Analyzing Templates")

            Call oEventLog.LogMessage(0, "TEMPLATES")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Templates")

            sAnalysisType = "Templates"

            RecID = RecID + 1

            ' Get count of templates
            sDescr = "Number of Templates in system DB (" + SysDBName.Trim + "):"
            sqlStmt = "SELECT COUNT(*) FROM Template t WHERE t.AvailableToPublic = 0"
            Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
            sResult = retValInt1
            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            ' Get list of templates
            If retValInt1 > 0 Then

                sqlStmt = "SELECT ISNULL(s.Number, t.ScrnNbr), ISNULL(s.Name,'??'), ISNULL(s.Module,'??'),"
                sqlStmt += " ScreenType = CASE WHEN s.ScreenType = 'S' THEN 'Screen' WHEN s.ScreenType = 'R' THEN 'Report' ELSE '??' END, t.TemplateId, t.Descr"
                sqlStmt += " FROM Template T LEFT OUTER JOIN Screen s ON LEFT(s.Number, 5) = t.ScrnNbr"
                sqlStmt += " WHERE t.AvailableToPublic = 0 ORDER BY t.ScrnNbr"

                Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                If (sqlReader.HasRows()) Then
                    While sqlReader.Read()

                        lcFullScreenName = sqlReader(1).Trim + " (" + sqlReader(0).Substring(0, 2) + "." + sqlReader(0).Substring(2, 3) + "." + sqlReader(0).Substring(5, 2) + ")"

                        RecID = RecID + 1
                        If sqlReader(1).trim <> "??" Then
                            lcFullScreenName = sqlReader(1).Trim + " (" + sqlReader(0).Substring(0, 2) + "." + sqlReader(0).Substring(2, 3) + "." + sqlReader(0).Substring(5, 2) + ")"
                        Else
                            lcFullScreenName = "Unknown Screen (" + sqlReader(0).Trim + ")"
                        End If
                        sDescr = Strings.Left(" - " + sqlReader(4).Trim() + " | " + lcFullScreenName.Trim() + " | Module: " + sqlReader(2).Trim() + " | Type: " + sqlReader(3).Trim(), 100) '+ " | Descr: " + sqlReader(5).Trim(), 100)

                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)
                    End While
                End If

            End If
            Call sqlReader.Close()
            '***** End of Template section *****

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")


            '=== List of Custom and 3rd Party Modules ===
            Form1.UpdateAnalysisToolStatusBar("Analyzing Custom and 3rd Party Modules")

            Call oEventLog.LogMessage(0, "CUSTOM & THIRD PARTY MODULES")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Custom and 3rd Party Modules")

            sAnalysisType = "Modules"

            If (SLModulesList.Trim() <> "") Then

                RecID = RecID + 1

                ' Get count of custom Custom and 3rd Party Modules - system DB
                sDescr = "Number of custom and third party modules:" ' in system DB (" + SysDBName.Trim + "):"
                sqlStmt = "SELECT COUNT(*) FROM Modules WHERE ModuleId NOT IN " + SLModulesList.Trim()
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                ' record list of custom Custom and 3rd Party Modules - system DB
                If retValInt1 > 0 Then

                    sqlStmt = "SELECT ModuleId, ModuleName FROM Modules WHERE ModuleId NOT IN " + SLModulesList.Trim() + " ORDER BY ModuleId"

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                    If (sqlReader.HasRows()) Then

                        While sqlReader.Read()

                            RecID = RecID + 1

                            sDescr = Strings.Left(" - " + sqlReader(0).Trim() + " " + sqlReader(1).Trim(), 100)
                            sResult = ""
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        End While

                    End If

                End If
                Call sqlReader.Close()

            End If
            '***** End of Custom and 3rd Party Modules section *****

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

            '=== List of Custom and 3rd Party Screens ===
            Form1.UpdateAnalysisToolStatusBar("Analyzing Custom and 3rd Party Screens/Reports")

            Call oEventLog.LogMessage(0, "CUSTOM AND THIRD PARTY SCREENS/REPORTS")
            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "Analyzing Custom and 3rd Party Screens & Reports")

            sAnalysisType = "Screens"

            If SLSscreenList.Trim <> "" Then

                RecID = RecID + 1

                ' Get count of custom Custom and 3rd Party screens - system DB
                sDescr = "Number of custom and third party screens and reports:" 'in system DB (" + SysDBName.Trim + "):"
                sqlStmt = "SELECT COUNT(*) FROM Screen WHERE Number NOT IN " + SLSscreenList.Trim()
                Call sqlFetch_Num(retValInt1, sqlStmt, SqlSysDbConn)
                sResult = retValInt1
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                ' record list of custom Custom and 3rd Party screens - system DB
                If retValInt1 > 0 Then

                    sqlStmt = "SELECT Number, Name, Module, ScreenType = CASE WHEN ScreenType = 'S' THEN 'Screen' WHEN ScreenType = 'R' THEN 'Report' WHEN ScreenType = 'Q' THEN 'SRS Report' WHEN ScreenType = 'V' THEN 'Query' ELSE '??' END  FROM Screen WHERE Number NOT IN " + SLSscreenList.Trim() + " ORDER BY Module, Number"

                    Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                    If (sqlReader.HasRows()) Then

                        While sqlReader.Read()

                            ' display module header if module changes
                            If CurrLoopMd <> sqlReader(2).Trim Then 'LastLoopMd Or LastLoopMd.Trim() = "" Then

                                CurrLoopMd = sqlReader(2).Trim

                                sqlStmt = "SELECT ModuleId, ModuleName FROM Modules WHERE ModuleId = '" + CurrLoopMd.Trim() + "' ORDER BY ModuleId"

                                Call sqlFetch_1(sqlReader1, sqlStmt, SqlSysDbConn1, CommandType.Text)

                                If (sqlReader1.HasRows()) Then

                                    If sqlReader1.Read() Then

                                        RecID = RecID + 1
                                        Call oEventLog.LogMessage(0, "")

                                        RecID = RecID + 1
                                        sDescr = Strings.Left(" - " + sqlReader1(0).Trim() + " " + sqlReader1(1).Trim(), 100)
                                        sResult = ""
                                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                                    End If

                                End If
                                Call sqlReader1.Close()

                            End If

                            RecID = RecID + 1

                            ' sDescr = Strings.Left(" - " + sqlReader(1).Trim + " (" + sqlReader(0).Substring(0, 2) + "." + sqlReader(0).Substring(2, 3) + "." + sqlReader(0).Substring(5, 2) + ") | Md: " + sqlReader(2).Trim + ") | Type: " + sqlReader(3).Trim, 100)
                            sDescr = Strings.Left(" - " + sqlReader(1).Trim + " (" + sqlReader(0).Substring(0, 2) + "." + sqlReader(0).Substring(2, 3) + "." + sqlReader(0).Substring(5, 2) + ") | Type: " + sqlReader(3).Trim, 100)
                            sResult = ""
                            sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                            sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                            Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        End While

                    End If

                End If
                Call sqlReader.Close()

            End If
            '***** End of Custom and 3rd Party Screens section *****

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

            '=== Reports ===
            Form1.UpdateAnalysisToolStatusBar("Analyzing Reports")

            Call oEventLog.LogMessage(0, "CUSTOM REPORTS")
            Call oEventLog.LogMessage(0, "")

            Call oEventLog.LogMessage(0, "Analyzing Custom Report Formats")

            sAnalysisType = "Reports"

            If (Form1.cUsrRptsPath.Text.Trim() <> "") Then

                'REPORTS IN RPTCONTROL TABLE

                ' add folder \Usr_Rpts to the Dynamics path
                SLUsrRptsPath = Path.Combine(Form1.cUsrRptsPath.Text.Trim(), "")

                ' if slusrptspath is not found then exit
                If Directory.Exists(SLUsrRptsPath) Then

                    ' get list of .rpt files in the Usr_Rpts folder
                    rptFiles = Directory.GetFiles(SLUsrRptsPath, "*.rpt")

                    If rptFiles.Length > 0 Then

                        RecID = RecID + 1
                        sDescr = "Custom Report Formats known in RptFormat:"
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        For Each rptFile In rptFiles

                            ' get file name without path and extension
                            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(rptFile)

                            sqlStmt = "SELECT ReportNbr, FileName, FormatName, S.Number, S.Name FROM RptFormat R JOIN Screen S ON S.Number = ReportNbr+'00' WHERE S.ScreenType = 'R' AND FileName = " + SParm(fileNameWithoutExtension)

                            Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                            ' record list of custom Report Formats in RptFormat - system DB
                            If (sqlReader.HasRows()) Then

                                While sqlReader.Read

                                    RecID = RecID + 1

                                    sDescr = Strings.Left(" - " + sqlReader(1).Trim() + " [" + sqlReader(2).Trim() + "] | Rpt: " + sqlReader(4).Trim + " (" + sqlReader(3).Substring(0, 2) + "." + sqlReader(3).Substring(2, 3) + "." + sqlReader(3).Substring(5, 2) + ")", 100)
                                    sResult = ""

                                    sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                                    sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                                    Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                                End While

                            End If
                            Call sqlReader.Close()

                        Next

                    Else

                        RecID = RecID + 1
                        sDescr = "No .rpt files found in Usr_Rpts folder"
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                    End If

                    'REPORTS NOT IN RPTCONTROL TABLE

                    ' get list of .rpt files in the Usr_Rpts folder
                    rptFiles = Directory.GetFiles(SLUsrRptsPath, "*.rpt")

                    If rptFiles.Length > 0 Then

                        Call oEventLog.LogMessage(0, "")
                        RecID = RecID + 1
                        sDescr = "Custom Report Formats NOT known in RptFormat:"
                        sResult = ""
                        sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                        sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                        Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                        For Each rptFile In rptFiles

                            ' get file name without path and extension
                            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(rptFile)

                            sqlStmt = "SELECT FileName FROM RptFormat R JOIN Screen S ON S.Number = ReportNbr+'00' WHERE S.ScreenType = 'R' AND FileName = " + SParm(fileNameWithoutExtension)

                            Call sqlFetch_1(sqlReader, sqlStmt, SqlSysDbConn, CommandType.Text)

                            ' record list of custom Report Formats NOT in RptFormat - system DB
                            If Not (sqlReader.HasRows()) Then

                                RecID = RecID + 1

                                sDescr = Strings.Left(" - " + fileNameWithoutExtension, 100)
                                sResult = ""

                                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                            End If
                            Call sqlReader.Close()

                        Next

                    End If

                Else

                    RecID = RecID + 1
                    'sDescr = Strings.Left("Usr_Rpts folder not found", 100)
                    sDescr = Strings.Left("path " + SLUsrRptsPath + " not found. Analysis not performed.", 100)
                    sResult = ""
                    sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                    sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                    Call AddStatusInfo(sqlStringExec, sDescr, sResult)

                End If

            Else

                RecID = RecID + 1
                sDescr = Strings.Left("Usr_Rpts path not provided. Analysis not performed.", 100)
                sResult = ""
                sqlStringValues = SParm(sAnalysisType) + "," + SParm(sDescr) + "," + SParm(CurrDateStr) + "," + SParm(sModule) + "," + CStr(RecID) + "," + SParm(sResult)
                sqlStringExec = sqlStringStart + sqlStringValues + sqlStringEnd
                Call AddStatusInfo(sqlStringExec, sDescr, sResult)

            End If
            '***** End of Reports section *****

            Call oEventLog.LogMessage(0, "")
            Call oEventLog.LogMessage(0, "")

        Catch ex As Exception
            Call oEventLog.LogMessage(1, "Analyze_CU: " & ex.Message)
            Call oEventLog.LogMessage(1, ex.StackTrace)
        Finally
            If Not sqlReader Is Nothing Then
                Try
                    sqlReader.Close()
                Catch
                End Try
            End If
        End Try

    End Sub

End Module