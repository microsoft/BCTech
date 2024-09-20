Option Strict Off
Option Explicit On
Imports System.Data.SqlClient

Module ProjectCode
    '=======================================================================================
    ' This module contains code to prepare the Project data for migration
    '
    '=======================================================================================
    Dim fetchPJPent As Integer
    Dim msgText As String = String.Empty
    Dim taskID20PlusExists As Boolean = False
	Dim fetchPJEmploy As Integer
	Dim fetchPJProj As Integer
	Dim statusClause As String = String.Empty
	Dim statusClauseTask As String = String.Empty



	Public Sub Validate()
        '====================================================================================
        'Validate Project related records
        '	- Verify Project has a Customer assigned
        '   - PJPent.pjt_entity is not longer than 20 characters
        '   - Verify Resource has a valid Vendor ID if populated
        '====================================================================================
        Dim oEventLog As clsEventLog
        Dim sqlStmt As String = ""
        Dim sqlReader As SqlDataReader = Nothing


        Dim fmtDate As String

        fmtDate = Date.Now.ToString
        fmtDate = fmtDate.Replace(":", "")
        fmtDate = fmtDate.Replace("/", "")
        fmtDate = fmtDate.Remove(fmtDate.Length - 3)
        fmtDate = fmtDate.Replace(" ", "-")
        fmtDate = fmtDate & Date.Now.Millisecond

        oEventLog = New clsEventLog
        oEventLog.FileName = "SL-PJ-" & "-" & fmtDate & "-" & Trim(UserId) & ".log"

        Call oEventLog.LogMessage(StartProcess, "")
        Call oEventLog.LogMessage(0, "")

        Call oEventLog.LogMessage(0, "Processing Projects")

        '**********************************************************
        '*** Verify Projects have the Customer field populated  ***
        '**********************************************************
        sqlStmt = "SELECT Project, Status_PA FROM PJProj WHERE RTRIM(customer) = '' " + statusClause
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)
        If sqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            msgText = "WARNING: Only Projects with the Customer field populated in Project Maintenance (PA.PRJ.00) will be exported from Dynamics SL. "
            msgText = msgText + "This is due to Dynamics 365 Business Central requiring the Bill-to Customer field on a Job to be populated when importing Job Task Lines. "
            msgText = msgText + "If you would like non-Customer projects to be migrated from Dynamics SL, it is suggested that a new 'internal' Customer ID be created to be used on Projects without a Customer assigned. "
            msgText = msgText + "This new Customer ID can be used to populate the Customer field on these projects, allowing the project and any associated tasks to be exported from Dynamics SL."
            msgText = msgText + vbNewLine + vbNewLine + "The Projects below will not be exported since a Customer has not been assigned:"
            Call LogMessage(msgText, oEventLog)
        End If

        While sqlReader.Read()

            Call SetPJProjValues(sqlReader, bPJPROJInfo)
            'Write Projects and Status to the event log
            Call LogMessage("Project: " + bPJPROJInfo.Project + vbTab + "Project Status: " + bPJPROJInfo.Status_pa.Trim, oEventLog)
            NbrOfWarnings_Proj = NbrOfWarnings_Proj + 1

        End While
        Call sqlReader.Close()

        '***********************************************************************
        '*** Check for Task records with a Task ID longer than 20 characters ***
        '***********************************************************************
        sqlStmt = "SELECT p.Project, p.PJT_entity, p.Status_PA FROM PJPENT p JOIN PJPROJ j ON j.project = p.project WHERE j.status_pa IN ('A', 'I', 'M') AND LEN(p.pjt_entity) > 20"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)

        If sqlReader.HasRows() Then
            'Write Error message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "ERROR: The following Task IDs are longer than 20 characters. Any Task ID longer than 20 characters will need to be updated to be 20 characters or less before data can be migrated."
            Call LogMessage(msgText, oEventLog)
            taskID20PlusExists = True
        End If

        While sqlReader.Read()

            Call SetPJPENTValues(sqlReader, bPJPENTInfo)
            'Write Project and Task ID to event log
            Call LogMessage("Project: " + bPJPENTInfo.Project + vbTab + "Task ID: " + bPJPENTInfo.PJT_entity + vbTab + "Task Status: " + bPJPENTInfo.Status_PA.Trim, oEventLog)
            NbrOfErrors_Proj = NbrOfErrors_Proj + 1

        End While
        Call sqlReader.Close()

        'Display message in Event Log for suggested actions
        If taskID20PlusExists = True Then
            Call LogMessage("", oEventLog)
            msgText = "Suggested actions for updating Task IDs are listed below:" + vbNewLine
            msgText = msgText + " - Use the Professional Services Tools Library (PSTL) application to modify the Task IDs to 20 characters or less" + vbNewLine
            msgText = msgText + " - Set the PA Status field on these tasks to Inactive, Purge, or Terminate on the Task tab in Project Maintenance (PA.PRJ.00) to exclude these tasks from the migration" + vbNewLine
            msgText = msgText + " - Contact your Microsoft Dynamics SL Partner for further assistance"
            Call LogMessage(msgText, oEventLog)
        End If


        '**********************************************************
        '*** Verify Resource has a valid Vendor ID if populated ***
        '**********************************************************
        sqlStmt = "SELECT employee, em_id01 FROM PJEMPLOY WHERE emp_status = 'A' AND MSPType = '' AND em_id01 <> '' AND RTRIM(em_id01) NOT IN (SELECT RTRIM(VendId) FROM Vendor)"
        Call sqlFetch_1(sqlReader, sqlStmt, SqlAppDbConn, CommandType.Text)


        If sqlReader.HasRows() Then
            'Write Warning message to event log
            Call LogMessage("", oEventLog)
            Call LogMessage("", oEventLog)
            msgText = "WARNING: The following Employees have an invalid Vendor ID assigned in Employee and Resource Maintenance (PA.EMP.00). Invalid Vendor IDs will not be assigned to Resources in Dynamics 365 Business Central."
            Call LogMessage(msgText, oEventLog)

        End If

        While sqlReader.Read()

            Call SetPJEmployee_Values(sqlReader, bPJEMPLOYInfo)
            'Write Employee and invalid Vendor ID to event log
            Call LogMessage("Employee: " + bPJEMPLOYInfo.employee.Trim + vbTab + "Invalid Vendor ID: " + bPJEMPLOYInfo.em_id01.Trim, oEventLog)
            NbrOfWarnings_Proj = NbrOfWarnings_Proj + 1

        End While
        Call sqlReader.Close()

        Call oEventLog.LogMessage(EndProcess, "Validate Project")


        Call MessageBox.Show("Project Validation Complete", "Project Validation")

        ' Display the event log just created.
        Call DisplayLog(oEventLog.LogFile.FullName.Trim())

        ' Store the filename in the table.
        If (My.Computer.FileSystem.FileExists(oEventLog.LogFile.FullName.Trim())) Then
            bSLMPTStatus.ProjEventLogName = oEventLog.LogFile.FullName
        End If


    End Sub


End Module
