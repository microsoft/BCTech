Import-Module "sqlps" -DisableNameChecking

# The integer values of the types as specified
# by the "Object Type" option field on the Permission table
$ObjectTypes = @{
    TableData = 0
    Table = 1
    Report = 3
    Codeunit = 5
    XMLPort = 6
    MenuSuite = 7
    Page = 8
    Query = 9
    System = 10
}

# System object IDs and names
$SystemPermissions = @{
    1350 = 'Run table'
    1610 = 'Create a new company'
    1630 = 'Rename an existing company'
    1640 = 'Delete a company'
    2510 = 'Edit, Find'
    2520 = 'Edit, Replace'
    3220 = 'View, Table Filter'
    3230 = 'View, FlowFilter'
    3410 = 'View, Sort'
    3510 = 'View, Design'
    5330 = 'Tools, Zoom'
    5410 = 'Export Data to Data File'
    5420 = 'Import Data from Data File'
    5810 = 'Tools, Security, Roles'
    5830 = 'Tools, Security, Password'
    6110 = 'Allow Action Export To Excel'
    9600 = 'SmartList Designer API'
    9605 = 'SmartList Designer Preview'
    9610 = 'SmartList Management'
    9615 = 'SmartList Import/Export'
    9620 = 'Snapshot debugging'
    9070 = 'Design, MenuSuite, Basic'
}

# Get information about an application object
function GetDetails
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Objects,
        $ObjectId,
        $App
    )
    $result = @{}
    if ($App -eq 'System')
    {
        $result.Location = 'System Application'
    }
    else
    {
        $result.Location = $App
    }
    $result.Name = $Objects[$ObjectId]
    return $result 
}

# Get information about application objects
function FindObjectDetails {
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $ObjectId,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $ObjectType,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Symbols
    )
    
    $result = @{}
    if ($ObjectId -eq 0)
    {
        $result.Location = 'System Application'
        $result.Name = '*'
        return $result
    } 
    
    foreach($App in $Symbols.Keys)
    {
        switch ($ObjectType)
        {
            $ObjectTypes.Codeunit
            {
                if ($Symbols[$App].Codeunits.ContainsKey($ObjectId))
                {  
                    return (GetDetails $Symbols[$App].Codeunits $ObjectId $App)
                }
            }
            $ObjectTypes.Page
            {
                if ($Symbols[$App].Pages.ContainsKey($ObjectId))
                {
                    return (GetDetails $Symbols[$App].Pages $ObjectId $App)
                }
            }
            $ObjectTypes.Query
            {
                if ($Symbols[$App].Queries.ContainsKey($ObjectId))
                {
                    return (GetDetails $Symbols[$App].Queries $ObjectId $App)
                }
            }
            $ObjectTypes.Report
            {
                if ($Symbols[$App].Reports.ContainsKey($ObjectId))
                {
                    return (GetDetails $Symbols[$App].Reports $ObjectId $App)
                }
            }
            $ObjectTypes.System
            {
                $result.Location = 'System Application'
                $result.Name = $SystemPermissions[$ObjectId]
                return $result
            }
            $ObjectTypes.Table
            {
                if ($Symbols[$App].Tables.ContainsKey($ObjectId))
                {
                    return (GetDetails $Symbols[$App].Tables $ObjectId $App)
                }
            }
            $ObjectTypes.TableData
            {
                if ($Symbols[$App].Tables.ContainsKey($ObjectId))
                {
                    return (GetDetails $Symbols[$App].Tables $ObjectId $App)
                }
            }
            $ObjectTypes.XMLPort
            {
                if ($Symbols[$App].XMLPorts.ContainsKey($ObjectId))
                {
                    return (GetDetails $Symbols[$App].XMLPorts $ObjectId $App)
                }
            
            }
            $ObjectTypes.MenuSuite
            {
                return ''
            }
        }
    }
    $result.Location = 'Unknown'
    $result.Name = $ObjectId
    return $result
    
}

# Generates a line in a PermissionSet AL object
# For example, "table Customer = RIMD"
function GetPermissionLine
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Permission,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Name
    )
    $PermissionMask = GetPermissionMask $Permission
    if ($PermissionMask -eq '')
    {
        return ''
    }
    if ($Permission.'Object Type' -eq $ObjectTypes.MenuSuite)
    {
        return ''
    }

    $Type = $ObjectTypes.Keys | ? { $ObjectTypes[$_] -eq $Permission.'Object Type' }
    $Type = $Type.ToLower()
    if ($Name -match '[/\., \-&]')
    {
        $Line = "$Type `"$Name`" = $PermissionMask"
    } else 
    {
        $Line = "$Type $Name = $PermissionMask"
    }

    return $Line
}

# Convert numeric expression of permissions to a symbolic one.
# For example, 12220 will be converted to Rimd
function GetPermissionMask
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Permission
    )

    $PermissionMask = ''
        
    if ($Permission.'Read Permission' -eq 1)
    {
        $PermissionMask += 'R'
    } elseif ($Permission.'Read Permission' -eq 2)
    {
        $PermissionMask += 'r'
    }

    if ($Permission.'Insert Permission' -eq 1)
    {
        $PermissionMask += 'I'
    } elseif ($Permission.'Insert Permission' -eq 2)
    {
        $PermissionMask += 'i'
    }

    if ($Permission.'Modify Permission' -eq 1)
    {
        $PermissionMask += 'M'
    } elseif ($Permission.'Modify Permission' -eq 2)
    {
        $PermissionMask += 'm'
    }

    if ($Permission.'Delete Permission' -eq 1)
    {
        $PermissionMask += 'D'
    } elseif ($Permission.'Delete Permission' -eq 2)
    {
        $PermissionMask += 'd'
    }

    if ($Permission.'Execute Permission' -eq 1)
    {
        $PermissionMask += 'X'
    } elseif ($Permission.'Execute Permission' -eq 2)
    {
        $PermissionMask += 'x'
    }

    return $PermissionMask
}

# Write provided permission sets to *.permissionset.al files.
function WritePermissionSets
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $PermissionSets,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Destination,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $PermissionSetTableContent
    )

    if (-not (Test-Path $Destination) )
    {
        New-Item $Destination -ItemType Directory | Out-Null
    }

    foreach($SetName in $PermissionSets.Keys)
    {
        $AllTheLines = @()
        foreach($Key in ($PermissionSets[$SetName].Keys))
        {
            $AllTheLines += $PermissionSets[$SetName][$Key]
        }
            # Clean filename and write the content on file
        $File = ($SetName -replace '[\(\)/\., \-&]', '').ToLower()
        $File += ".permissionset.al"
        $Dest = Join-Path $Destination "PermissionSets"
        if (-not (Test-Path $Dest) )
        {
                New-Item $Dest -ItemType Directory | Out-Null
        }
        $File = Join-Path $Dest $File
        Write-Host "Writing File $File"
        $Content = GetFileContent $SetName $AllTheLines $Key
        $Caption = ($PermissionSetTableContent | where {$_.'Role ID' -eq $SetName}).Name
        if ($Caption.Length -gt 0)
        {
            $Access = "    Access = Public;"
            $Assignable = "    Assignable = true;"
            $Caption = "    Caption = '$Caption';"
            $Content = $Content[0..1], $Access, $Assignable, $Caption, $Content[2..$Content.Length];
        }

        if (-not (Test-Path $File) )
        {
            New-Item $File | Out-Null
        }
        Set-Content $File $Content -Encoding UTF8
    }
}

# Generate content of a PermissionSet AL object.
# The content can subsequently be written to a *.permissionset.al file.
function GetFileContent
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $SetName,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Lines,
        
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Module
    )
    
    # Sort lines and ignore quotes
    $Lines = ($Lines | Sort-Object -Property { [char[]] ($_ -replace '"', '') })
    $Content = @()
    $Id = [System.Math]::Abs(($SetName + $Module).GetHashCode() % 49999)

    $global:PSCounter += 1
    $Content += "permissionset $Id `"$SetName`""
    
    $Content += "{"

    $FirstLine = $true
    
    foreach($Line in $Lines)
    {
        if ($FirstLine) 
        {
            $Content += "    Permissions = $Line,"
            $FirstLine = $false
        } else
        {
            $Content += "                  $Line,"
        }
    }

    $LastLine = $Content[$Content.Count - 1];
    $LastLine = $LastLine.SubString(0 ,$LastLine.Length - 1) + ';'

    $Content.Set($Content.Count - 1, $LastLine)

    $Content += "}"
    return $Content
}

# Convert symbols downloaded from the database (in JSON format) to a PowerShell dictionary
function ProccessSymbols
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $SymbolsFolder
    )

    $RawSymbols = @{}
    
    gci $SymbolsFolder | % { $RawSymbols[$_.Name] = Get-Content $_.FullName | Out-String | ConvertFrom-Json }

    Write-Host 'Proccessing Symbols'
    $Symbols = @{}
    
    foreach($App in $RawSymbols.Keys)
    {
        $AppSymbols = @{}
        $Tables = @{}
        foreach($Table in $RawSymbols[$App].Tables)
        {
            $Tables.Add($Table.Id, $Table.Name)
        }
        $AppSymbols.Tables = $Tables
        
        $Codeunits = @{}
        foreach($Codeunit in $RawSymbols[$App].Codeunits)
        {
            $Codeunits.Add($Codeunit.Id, $Codeunit.Name)
        }
        $AppSymbols.Codeunits = $Codeunits
        
        $Pages = @{}
        foreach($Page in $RawSymbols[$App].Pages)
        {
            $Pages.Add($Page.Id, $Page.Name)
        }
        $AppSymbols.Pages = $Pages

        $Reports = @{}
        foreach($Report in $RawSymbols[$App].Reports)
        {
            $Reports.Add($Report.Id, $Report.Name)
        }
        $AppSymbols.Reports = $Reports

        $Queries = @{}
        foreach($Query in $RawSymbols[$App].Queries)
        {
            $Queries.Add($Query.Id, $Query.Name)
        }
        $AppSymbols.Queries = $Queries

        $XMLPorts = @{}
        foreach($XMLPort in $RawSymbols[$App].XMLPorts)
        {
            $XMLPorts.Add($XMLPort.Id, $XMLPort.Name)
        }
        $AppSymbols.XMLPorts = $XMLPorts

        $Symbols[$App] = $AppSymbols
    }

    return $Symbols
}

# Download symbols from the "Published Application" table.
# The symbols can be used to get application object type, name and ID.
function DownloadSymbolsFromDatabase
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $DatabaseServer,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $DatabaseName,
        
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $PackageId,
        
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $PackageSize,
        
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Path
    )

    $SymbolsBytes = (Invoke-Sqlcmd -ServerInstance $DatabaseServer -Database $DatabaseName -Query "Select [Symbols] from [Published Application] where ID = '$PackageId'" -MaxBinaryLength $PackageSize).Symbols
    
    $Stream = [System.IO.MemoryStream]::new($SymbolsBytes)
    $Stream.Position = 4 # throw away the header

    $Result = New-Object System.IO.MemoryStream
    $DeflateStream = [System.IO.Compression.DeflateStream]::new($Stream, [System.IO.Compression.CompressionMode]::Decompress)
    $DeflateStream.CopyTo($Result);
    $Bytes = $Result.ToArray()
    $Bytes = $Bytes -ne 0

    [System.IO.File]::WriteAllBytes($Path, $Bytes);
}

<#
.SYNOPSIS
    Create AL PermissionSet objects that are identical to the existing permission sets from the database.
.DESCRIPTION
    Reads all the permission sets from the specified database and creates PermissionSet AL objects with identical names and permissions
.EXAMPLE
    Convert-PermissionSets -DatabaseServer localhost -DatabaseName W1 -Destination ".\NewPermissionSets" 
.PARAMETER DatabaseServer
    Database server from which permission sets will be read.
.PARAMETER DatabaseName
    Database name from which permission sets will be read.
.PARAMETER Destination
    The output directory.
#>
function Convert-PermissionSets
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $DatabaseServer,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $DatabaseName,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Destination
    )

    Write-Host "Downloading Symbols from $DatabaseServer $DatabaseName"
    $Apps = Invoke-Sqlcmd -ServerInstance $DatabaseServer -Database $DatabaseName -Query 'Select ID, Name, Datalength(Symbols) as Size from [Published Application]'
    $ApplicationAppID = 'C1335042-3002-4257-BF8A-75C898CCB1B8'

    $Apps | % { $_.Name = $_.Name.Replace('_Exclude_', '').TrimEnd('_')}
    $Apps = $Apps | Where-Object { $_.ID -ne $ApplicationAppID}
    $SymbolsFolder = Join-Path $env:Temp 'Symbols'
    if (-not (Test-Path $SymbolsFolder))
    {
        New-Item $SymbolsFolder -ItemType Directory | out-Null
    }

    foreach($App in $Apps)
    {
        DownloadSymbolsFromDatabase -DatabaseServer $DatabaseServer -DatabaseName $DatabaseName -PackageId $App.ID -PackageSize $App.Size -Path (Join-Path $SymbolsFolder ($App.Name))
    }
    
    $Symbols = ProccessSymbols $SymbolsFolder

    Write-Host "Quering permissions from $DatabaseServer $DatabaseName"
    $Permissions = Invoke-Sqlcmd -ServerInstance $DatabaseServer -Database $DatabaseName -Query 'Select [Role ID],[Object Type],[Object ID],[Read Permission],[Insert Permission],[Modify Permission],[Delete Permission],[Execute Permission],[Security Filter] from Permission UNION Select [Role ID],[Object Type],[Object ID],[Read Permission],[Insert Permission],[Modify Permission],[Delete Permission],[Execute Permission],[Security Filter] from [Tenant Permission] order by [Role ID]'
    $PermissionSets = @{}

    foreach($Permission in $Permissions)
    {
        $Details = FindObjectDetails $Permission.'Object ID' $Permission.'Object Type' $Symbols
        
        $Line = GetPermissionLine $Permission $Details.Name

        if ($Line -eq '')
        {
            continue
        }

        if ($PermissionSets.ContainsKey($Permission.'Role ID'))
        {
            if ($PermissionSets[$Permission.'Role ID'].ContainsKey($Details.Location)) 
            {
                $Lines = $PermissionSets[$Permission.'Role ID'][$Details.Location]
                $Lines += $Line
                $PermissionSets[$Permission.'Role ID'][$Details.Location] = $Lines
            } else
            {
                $Lines = @()
                $Lines += $Line
                $PermissionSets[$Permission.'Role ID'].Add($Details.Location, $Lines)
            }

        } else
        {   
            $Set = $Permission.'Role ID'
            Write-Host "Processing Permission Set $Set"
            $Lines = @()
            $Lines += $Line
            $App = @{}
            $App.Add($Details.Location, $Lines)
            $PermissionSets.Add($Permission.'Role ID', $App)
        }
    }

    $PermissionSetTableContent = Invoke-Sqlcmd -ServerInstance $DatabaseServer -Database $DatabaseName -Query 'Select [Role ID], Name from [Permission Set] UNION Select [Role ID], Name from [Tenant Permission Set]'
    WritePermissionSets $PermissionSets $Destination $PermissionSetTableContent
}

Export-ModuleMember -Function "Convert-PermissionSets"
