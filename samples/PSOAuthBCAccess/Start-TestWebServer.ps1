<#
.Synopsis
Starts powershell webserver to listen for authorization code
.Description
Starts webserver as powershell process.

A call to the server (e.g. http://localhost:8080/login) outputs an authorization code and closes the server.
    
.Inputs
None
.Outputs
None
.Example
Start-Webserver.ps1

Starts webserver with binding to http://localhost:8080/
.Notes
Version 1.0, 2020-11-18
#>

Write-Host "$(Get-Date -Format s) Starting webserver"
$HttpListener = New-Object System.Net.HttpListener
$HttpListener.Prefixes.Add("http://localhost:8080/")
$HttpListener.Start()

try
{
    Write-Host "$(Get-Date -Format s) Webserver started"
    $StopListening = $false
    while ($HttpListener.IsListening)
    {
        $HttpContext = $HttpListener.GetContext()
        $HttpRequest = $HttpContext.Request

        $RequestShortDescription = "$($HttpRequest.httpMethod) $($HttpRequest.Url.LocalPath)"
        Write-Host "Incoming request: $RequestShortDescription"

        switch ($RequestShortDescription)
        {
            "GET /login"
            {
                $AuthorizationCode = $HttpRequest.QueryString.Get("code")

                $ResponseMessage = "Login request received. You can close this page."
                Write-Host "$(Get-Date -Format s) Login request received. You can close web browser page."
                $StopListening = $true
            }

            default
            {
                $ResponseMessage = "OK."
            }
        }

        $HttpResponse = $HttpContext.Response
        $HttpResponse.Headers.Add("Content-Type","text/plain")
        $HttpResponse.StatusCode = 200

        $ResponseBuffer = [System.Text.Encoding]::UTF8.GetBytes($ResponseMessage)
        $HttpResponse.ContentLength64 = $ResponseBuffer.Length
        $HttpResponse.OutputStream.Write($ResponseBuffer,0,$ResponseBuffer.Length)
        $HttpResponse.Close()

        if ($StopListening)
        {
            Write-Host "$(Get-Date -Format s) Stopping webserver"
            break
        }
    }

    Write-Output $AuthorizationCode
}
finally
{
	$HttpListener.Stop()
    $HttpListener.Close()
    Write-Host "$(Get-Date -Format s) Webserver stopped"
}