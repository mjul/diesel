Push-Location -Path Diesel
& NuGet pack Diesel.csproj -IncludeReferencedProjects
Pop-Location
Write-Host -ForegroundColor Cyan "Ready. Publish with:" 
Write-Host -ForegroundColor Yellow "  NuGet push <package>" 
