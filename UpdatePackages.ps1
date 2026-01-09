# PowerShell script to update NuGet packages for all solutions in SK_Course
$solutions = @(
    "D:\SK_Course\module2",
    "D:\SK_Course\module3\AzureAiInference", 
    "D:\SK_Course\module3\Hugging Face",
    "D:\SK_Course\module3\LM Studio",
    "D:\SK_Course\module3\Ollama",
    "D:\SK_Course\module3\ONNX",
    "D:\SK_Course\module3\OpenAI",
    "D:\SK_Course\module4",
    "D:\SK_Course\module5",
    "D:\SK_Course\module6",
    "D:\SK_Course\module7",
    "D:\SK_Course\module8"
)

$results = @()

foreach ($solutionDir in $solutions) {
    $solutionName = Split-Path $solutionDir -Leaf
    Write-Host "`nProcessing: $solutionName in $solutionDir" -ForegroundColor Yellow
    
    if (-not (Test-Path $solutionDir)) {
        Write-Host "Directory not found: $solutionDir" -ForegroundColor Red
        $results += [PSCustomObject]@{
            Solution = $solutionName
            Directory = $solutionDir
            Status = "fail"
            Notes = "Directory not found"
        }
        continue
    }
    
    Push-Location $solutionDir
    
    $updateResult = "succeed"
    $notes = ""
    
    try {
        # Find solution file
        $slnFile = Get-ChildItem -Filter "*.sln" | Select-Object -First 1
        if (-not $slnFile) {
            $updateResult = "fail"
            $notes = "No solution file found"
        } else {
            Write-Host "Found solution: $($slnFile.Name)" -ForegroundColor Green
            
            # Get projects in solution
            $projects = dotnet sln list 2>$null | Where-Object { $_ -like "*.csproj" }
            
            foreach ($project in $projects) {
                if ($project) {
                    Write-Host "Updating packages for: $project" -ForegroundColor Cyan
                    
                    # Update common packages to latest stable versions
                    & dotnet add $project package Microsoft.Extensions.Configuration --version 10.0.1 2>$null
                    & dotnet add $project package Microsoft.Extensions.Configuration.EnvironmentVariables --version 10.0.1 2>$null  
                    & dotnet add $project package Microsoft.Extensions.Configuration.Json --version 10.0.1 2>$null
                    & dotnet add $project package Microsoft.Extensions.Configuration.UserSecrets --version 10.0.1 2>$null
                    & dotnet add $project package Microsoft.SemanticKernel --version 1.68.0 2>$null
                    & dotnet add $project package Microsoft.SemanticKernel.Connectors.AzureOpenAI --version 1.68.0 2>$null
                    & dotnet add $project package Microsoft.SemanticKernel.Connectors.OpenAI --version 1.68.0 2>$null
                }
            }
            
            # Try to build
            Write-Host "Building solution..." -ForegroundColor Cyan
            $buildOutput = & dotnet build 2>&1
            $buildExitCode = $LASTEXITCODE
            
            if ($buildExitCode -eq 0) {
                $updateResult = "succeed"
                Write-Host "Build succeeded!" -ForegroundColor Green
            } else {
                # Check if it's just warnings
                $errorCount = ($buildOutput | Where-Object { $_ -like "*error*" -and $_ -notlike "*warning*" }).Count
                if ($errorCount -gt 0) {
                    $updateResult = "fail"
                    $notes = "Build errors found"
                    Write-Host "Build failed with errors" -ForegroundColor Red
                } else {
                    $updateResult = "succeed"
                    $notes = "Build succeeded with warnings"
                    Write-Host "Build succeeded with warnings" -ForegroundColor Yellow
                }
            }
        }
    }
    catch {
        $updateResult = "fail"
        $notes = "Exception: $($_.Exception.Message)"
        Write-Host "Exception occurred: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    $results += [PSCustomObject]@{
        Solution = $solutionName
        Directory = $solutionDir
        Status = $updateResult
        Notes = $notes
    }
    
    Pop-Location
}

Write-Host "`n=== RESULTS SUMMARY ===" -ForegroundColor Magenta
$results | Format-Table -AutoSize

Write-Host "`n=== STATUS COUNTS ===" -ForegroundColor Magenta
$results | Group-Object Status | Format-Table Count, Name -AutoSize