Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic

Public Module HelperFunctions
    ''' <summary>
    ''' Validates an email address string and returns True when valid.
    ''' </summary>
    ''' <param name="email">Input email string to validate.</param>
    Public Function IsValidEmail(email As String) As Boolean
        If String.IsNullOrWhiteSpace(email) Then
            Return False
        End If

        Try
            Dim addr As New MailAddress(email.Trim())
            ' Ensure the parsed address exactly matches the input (prevents "Display Name <address@domain>" cases)
            Return String.Equals(addr.Address, email.Trim(), StringComparison.Ordinal)
        Catch ex As FormatException
            Return False
        End Try
    End Function

    Public Function IsValidPhoneNumber(phone As String) As Boolean
        If String.IsNullOrWhiteSpace(phone) Then Return False

        ' Remove common separators
        Dim normalized = Regex.Replace(phone, "[\s\-\(\)\.]", "")

        ' Valid examples: 5551234567, +15551234567, 15551234567
        Return Regex.IsMatch(normalized, "^(?:\+?1)?\d{10}$")
    End Function

End Module
