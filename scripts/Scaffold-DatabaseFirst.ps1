param(
    [string]$ContextName = "DatabaseFirstContext",
    [string]$ContextDir = "Scaffolding/DatabaseFirst/Context",
    [string]$OutputDir = "Scaffolding/DatabaseFirst/Models/Generated",
    [string]$Namespace = "EFCoreDemo.Scaffolding.DatabaseFirst.Models.Generated",
    [string]$ContextNamespace = "EFCoreDemo.Scaffolding.DatabaseFirst.Context",
    [string[]]$Schemas = @(),
    [string[]]$Tables = @(),
    [switch]$UseDatabaseNames,
    [switch]$DataAnnotations,
    [switch]$Force,
    [bool]$NoOnConfiguring = $true
)

$ErrorActionPreference = "Stop"

function Get-ConnectionString {
    $environmentCandidates = @(
        $env:EFCOREDEMO_CONNECTION_STRING,
        $env:PG_CONNECTION_STRING
    )

    foreach ($candidate in $environmentCandidates) {
        if (-not [string]::IsNullOrWhiteSpace($candidate)) {
            return $candidate.Trim()
        }
    }

    $workspaceRoot = Split-Path -Parent $PSScriptRoot
    $candidateFiles = @(
        (Join-Path $workspaceRoot "EFCoreDemo\appsettings.local.json"),
        (Join-Path $workspaceRoot "EFCoreDemo\appsettings.example.json"),
        (Join-Path $workspaceRoot "appsettings.local.json"),
        (Join-Path $workspaceRoot "appsettings.example.json")
    )

    foreach ($file in $candidateFiles) {
        if (-not (Test-Path $file)) {
            continue
        }

        $json = Get-Content $file -Raw | ConvertFrom-Json

        if ($null -ne $json.ConnectionString -and -not [string]::IsNullOrWhiteSpace($json.ConnectionString)) {
            return $json.ConnectionString.Trim()
        }

        if ($null -ne $json.ConnectionStrings -and
            $null -ne $json.ConnectionStrings.Default -and
            -not [string]::IsNullOrWhiteSpace($json.ConnectionStrings.Default)) {
            return $json.ConnectionStrings.Default.Trim()
        }
    }

    throw "No connection string was found. Set EFCOREDEMO_CONNECTION_STRING, PG_CONNECTION_STRING, EFCoreDemo/appsettings.local.json, or EFCoreDemo/appsettings.example.json."
}

$workspaceRoot = Split-Path -Parent $PSScriptRoot
$connectionString = Get-ConnectionString

$arguments = @(
    "ef",
    "dbcontext",
    "scaffold",
    $connectionString,
    "Npgsql.EntityFrameworkCore.PostgreSQL",
    "--project",
    "EFCoreDemo",
    "--startup-project",
    "EFCoreDemo",
    "--context",
    $ContextName,
    "--context-dir",
    $ContextDir,
    "--output-dir",
    $OutputDir,
    "--namespace",
    $Namespace,
    "--context-namespace",
    $ContextNamespace
)

if ($NoOnConfiguring) {
    $arguments += "--no-onconfiguring"
}

if ($UseDatabaseNames.IsPresent) {
    $arguments += "--use-database-names"
}

if ($DataAnnotations.IsPresent) {
    $arguments += "--data-annotations"
}

if ($Force.IsPresent) {
    $arguments += "--force"
}

foreach ($schema in $Schemas) {
    $arguments += "--schema"
    $arguments += $schema
}

foreach ($table in $Tables) {
    $arguments += "--table"
    $arguments += $table
}

Write-Host "Scaffolding database-first models into EFCoreDemo..." -ForegroundColor Cyan
Write-Host "Context name: $ContextName"
Write-Host "Context dir : $ContextDir"
Write-Host "Models dir  : $OutputDir"

if ($Schemas.Count -gt 0) {
    Write-Host "Schemas     : $($Schemas -join ', ')"
}

if ($Tables.Count -gt 0) {
    Write-Host "Tables      : $($Tables -join ', ')"
}

Push-Location $workspaceRoot
try {
    & dotnet @arguments
    exit $LASTEXITCODE
}
finally {
    Pop-Location
}
