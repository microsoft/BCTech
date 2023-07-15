# need Powershell 7 to use Test-Json

# function GetBCPTTestResultSummary {
    Param(
        [Parameter(Mandatory=$true)]
        [string] $testResultsFile,
        [xml] $baseline,        
        [xml] $thresholds,
        [int] $includeFailures
    )

    $summarySb = [System.Text.StringBuilder]::new()  

    $summarySb.AppendLine('Starting...') | Out-Null    

    if (!(Test-Path $testResultsFile)) 
    {
        Write-Warning "$testResultsFile file not found"
        exit 1
    }
    $testResults = (Get-Content "$testResultsFile") -join " "

    try
    {
        $json = ConvertFrom-Json $testResults -ErrorAction Stop;
        Write-Host "Provided file has been correctly parsed to JSON";    
    }
    catch
    {
        Write-Host "Provided file is not a valid JSON";
        exit 1
    }

    $suites = @{}
    foreach ($rec in $json)
    {
        $id = $rec.id
        $bcptCode = $rec.bcptCode
        $lineNumber = $rec.lineNumber
        $tag = $rec.tag
        $version = $rec.version
        $entryNumber = $rec.entryNumber
        $startTime = $rec.startTime
        $endTime = $rec.endTime
        $codeunitID = $rec.codeunitID
        $codeunitName = $rec.codeunitName
        $sessionId = $rec.sessionId
        $operation = $rec.operation
        $message = $rec.message
        $durationMin = $rec.durationMin
        $numberOfSQLStmts = $rec.numberOfSQLStmts
        $status = $rec.status

        if(-not $suites[$bcptCode])
        {
            $codeunit = @{}
            $codeunit.codeunitID = $codeunitID
            $codeunit.codeunitName = $codeunitName

            $codeunits = @{}
            $codeunits.add( $codeunitID, $codeunit )
            $suites.add( $bcptCode, $codeunits) 
        }
        $codeunits = $suites[$bcptCode]
        if(-not $codeunits[$codeunitID])
        {
            $codeunit = @{}
            $codeunit.codeunitID = $codeunitID
            $codeunit.codeunitName = $codeunitName
            $codeunits.add( $codeunitID, $codeunit )
        }
        $codeunit=$codeunits[$codeunitID]

        $measurement = @{}
        $measurement.sessionId = $sessionId
        $measurement.durationMin = $durationMin
        $measurement.numberOfSQLStmts = $numberOfSQLStmts
        $measurement.status = $status

        $operations = $codeunit.operations
        if( $operations.count -eq 0) 
        {
            $operations = @{}
            $codeunit.operations = $operations
        }
        $operations = $codeunit.operations

        if(-not $operations[$operation])
        {
            $measurements = @{}

            $measurements[$entryNumber] = $measurement
            $operations.add( $operation, $measurements )
        }
        else
        {
            $measurements = $operations[$operation]
            $measurements[$entryNumber] = $measurement
        }
    }

    ## pretty-printing the results
    $globalNumberOfErrors = 0
    $globalNumberOfSuccess = 0
    $numberOfSuites = $suites.count
    $summarySb.AppendLine( "Found test results for $numberOfSuites suites." ) | Out-Null    
    $summarySb.AppendLine( "------------------------------" ) | Out-Null    
    foreach($suiteKey in $suites.keys)
    {
        $suiteNumberOfErrors = 0
        $suiteNumberOfSuccess = 0
        $summarySb.AppendLine( "Suite: $suiteKey" ) | Out-Null
        $codeunits = $suites[$suiteKey]
        $codeunitCount = $codeunits.count
        $summarySb.AppendLine( "Found $codeunitCount codeunits" )

        foreach($codeunitKey in $codeunits.keys)
        {
            $codeunit = $codeunits[$codeunitKey]
            $codeunitID=$codeunit['codeunitID']
            $codeunitName = $codeunit['codeunitName']
            $numberOfOperations = $codeunit.operations.count
            $summarySb.AppendLine( "  Codeunit: $codeunitID, Codeunitname: $codeunitName" ) | Out-Null
            $summarySb.AppendLine( "  $numberOfOperations operations" ) | Out-Null
            $summarySb.AppendLine( "    ------------------------------------" ) | Out-Null
            foreach($key in $codeunit.operations.keys)
            {
                $measurements = $codeunit.operations[$key]                
                $measurementsCount = $measurements.count
                $summarySb.AppendLine( "    Operation: $key, Measurements: $measurementsCount" ) | Out-Null
                $durationMinSum = 0
                $numberOfSQLStmtsSum = 0
                $numberOfErrors = 0
                $numberOfSuccess = 0
                foreach($measurementKey in $measurements.keys)
                {
                    $measurement = $measurements[$measurementKey]
                    $sessionId=$measurement.sessionId
                    $durationMin=$measurement.durationMin
                    $durationMinSum += $durationMin
                    $numberOfSQLStmts=$measurement.numberOfSQLStmts
                    $numberOfSQLStmtsSum += $numberOfSQLStmts
                    $status=$measurement.status
                    if($status -eq "Success")
                    {
                        $numberOfSuccess += 1 
                        $suiteNumberOfSuccess += 1 
                        $globalNumberOfSuccess += 1 
                    }
                    else
                    {
                        $numberOfErrors += 1
                        $suiteNumberOfErrors += 1
                        $globalNumberOfErrors += 1
                    }
                    # verbose
                    #$summarySb.AppendLine( "      Measurement: sessionId=$sessionId, durationMin=$durationMin, numberOfSQLStmts=$numberOfSQLStmts, status=$status" ) | Out-Null
                }
                $durationMinAvg = $durationMinSum / $measurementsCount
                $numberOfSQLStmtsAvg = $numberOfSQLStmtsSum / $measurementsCount
                $summarySb.AppendLine( "    Avg duration (in min)=$durationMinAvg" ) | Out-Null
                $summarySb.AppendLine( "    Avg SQL statements=$numberOfSQLStmtsAvg" ) | Out-Null                
                $summarySb.AppendLine( "    Number of errors=$numberOfErrors" ) | Out-Null                                
                $summarySb.AppendLine( "    Number of successes=$numberOfSuccess" ) | Out-Null
                $summarySb.AppendLine( "    ------------------------------------" ) | Out-Null
            }
        }
        $summarySb.AppendLine( "Suite number of success: $suiteNumberOfSuccess" ) | Out-Null    
        $summarySb.AppendLine( "Suite number of errors: $suiteNumberOfErrors" ) | Out-Null    
    }    
    $summarySb.AppendLine( "------------------------------" ) | Out-Null    
    $summarySb.AppendLine( "Global number of success: $globalNumberOfSuccess" ) | Out-Null    
    $summarySb.AppendLine( "Global number of errors: $globalNumberOfErrors" ) | Out-Null    


    $json | Export-Csv -Path "$testResultsFile.csv" -Delimiter ';' -NoTypeInformation

    $summarySb.AppendLine('Finished parsing results') | Out-Null    
    $summarySb.ToString()

   
   
   
    # $totalTests = 0
    # $totalTime = 0.0
    # $totalFailed = 0
    # $totalSkipped = 0
    # $failuresIncluded = 0
    # $summarySb = [System.Text.StringBuilder]::new()
    # ## regressionsSb ?
    # $failuresSb = [System.Text.StringBuilder]::new()
    # if ($testResults.testsuites) {
    #     $appNames = @($testResults.testsuites.testsuite | ForEach-Object { $_.Properties.property | Where-Object { $_.Name -eq "appName" } | ForEach-Object { $_.Value } } | Select-Object -Unique)
    #     if (-not $appNames) {
    #         $appNames = @($testResults.testsuites.testsuite | ForEach-Object { $_.Properties.property | Where-Object { $_.Name -eq "extensionId" } | ForEach-Object { $_.Value } } | Select-Object -Unique)
    #     }
    #     $testResults.testsuites.testsuite | ForEach-Object {
    #         $totalTests += $_.Tests
    #         $totalTime += [decimal]::Parse($_.time, [System.Globalization.CultureInfo]::InvariantCulture)
    #         $totalFailed += $_.failures
    #         $totalSkipped += $_.skipped
    #     }
    #     Write-Host "$($appNames.Count) TestApps, $totalTests tests, $totalFailed failed, $totalSkipped skipped, $totalTime seconds"
    #     $summarySb.Append('|Test app|Tests|Passed|Failed|Skipped|Time|\n|:---|---:|---:|---:|---:|---:|\n') | Out-Null
    #     $appNames | ForEach-Object {
    #         $appName = $_
    #         $appTests = 0
    #         $appTime = 0.0
    #         $appFailed = 0
    #         $appSkipped = 0
    #         $suites = $testResults.testsuites.testsuite | where-Object { $_.Properties.property | Where-Object { $_.Value -eq $appName } }
    #         $suites | ForEach-Object {
    #             $appTests += [int]$_.tests
    #             $appFailed += [int]$_.failures
    #             $appSkipped += [int]$_.skipped
    #             $appTime += [decimal]::Parse($_.time, [System.Globalization.CultureInfo]::InvariantCulture)
    #         }
    #         $appPassed = $appTests-$appFailed-$appSkipped
    #         Write-Host "- $appName, $appTests tests, $appPassed passed, $appFailed failed, $appSkipped skipped, $appTime seconds"
    #         $summarySb.Append("|$appName|$appTests|") | Out-Null
    #         if ($appPassed -gt 0) {
    #             $summarySb.Append("$($appPassed):white_check_mark:") | Out-Null
    #         }
    #         $summarySb.Append("|") | Out-Null
    #         if ($appFailed -gt 0) {
    #             $summarySb.Append("$($appFailed):x:") | Out-Null
    #         }
    #         $summarySb.Append("|") | Out-Null
    #         if ($appSkipped -gt 0) {
    #             $summarySb.Append("$($appSkipped):white_circle:") | Out-Null
    #         }
    #         $summarySb.Append("|$($appTime)s|\n") | Out-Null
    #         if ($appFailed -gt 0) {
    #             $failuresSb.Append("<details><summary><i>$appName, $appTests tests, $appPassed passed, $appFailed failed, $appSkipped skipped, $appTime seconds</i></summary>\n") | Out-Null
    #             $suites | ForEach-Object {
    #                 Write-Host "  - $($_.name), $($_.tests) tests, $($_.failures) failed, $($_.skipped) skipped, $($_.time) seconds"
    #                 if ($_.failures -gt 0 -and $failuresSb.Length -lt 32000) {
    #                     $failuresSb.Append("<details><summary><i>$($_.name), $($_.tests) tests, $($_.failures) failed, $($_.skipped) skipped, $($_.time) seconds</i></summary>") | Out-Null
    #                     $_.testcase | ForEach-Object {
    #                         if ($_.ChildNodes.Count -gt 0) {
    #                             Write-Host "    - $($_.name), Failure, $($_.time) seconds"
    #                             $failuresSb.Append("<details><summary><i>$($_.name), Failure</i></summary>") | Out-Null
    #                             $_.ChildNodes | ForEach-Object {
    #                                 Write-Host "      - Error: $($_.message)"
    #                                 Write-Host "        Stacktrace:"
    #                                 Write-Host "        $($_."#text".Trim().Replace("`n","`n        "))"
    #                                 $failuresSb.Append("<i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Error: $($_.message)</i><br/>") | Out-Null
    #                                 $failuresSb.Append("<i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Stack trace</i><br/>") | Out-Null
    #                                 $failuresSb.Append("<i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$($_."#text".Trim().Replace("`n","<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"))</i><br/>") | Out-Null
    #                             }
    #                             $failuresSb.Append("</details>") | Out-Null
    #                         }
    #                     }
    #                     $failuresSb.Append("</details>") | Out-Null
    #                     $failuresIncluded++
    #                 }
    #             }
    #             $failuresSb.Append("</details>") | Out-Null
    #         }
    #     }
    # }
    # if ($totalFailed -gt 0) {
    #     if ($totalFailed -gt $failuresIncluded) {
    #         $failuresSb.Insert(0,"<details><summary><i>$totalFailed failing tests (showing the first $failuresIncluded here, download test results to see all)</i></summary>") | Out-Null
    #     }
    #     else {
    #         $failuresSb.Insert(0,"<details><summary><i>$totalFailed failing tests</i></summary>") | Out-Null
    #     }
    #     $failuresSb.Append("</details>") | Out-Null
    #     if (($summarySb.Length + $failuresSb.Length) -lt 65000) {
    #         $summarySb.Append("\n\n$($failuresSb.ToString())") | Out-Null
    #     }
    #     else {
    #         $summarySb.Append("\n\n<i>$totalFailed failing tests. Download test results to see all</i>") | Out-Null
    #     }
    # }
    # else {
    #     $summarySb.Append("\n\n<i>No test failures</i>") | Out-Null
    # }
    # if ($summarySb.Length -lt 65500) {
    #     $summarySb.ToString()
    # }
    # else {
    #     "<i>$totalFailed failing tests. Download test results to see all</i>"
    # }
# }