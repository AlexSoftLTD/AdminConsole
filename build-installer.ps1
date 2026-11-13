param(
    [string]$Version = '0.1.17',
    [string]$CompilerPath
)
$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+([-.][a-zA-Z0-9.-]+)?$') { throw 'Invalid version' }
if (!$CompilerPath) {
    $candidates = @(
        (Join-Path $PSScriptRoot 'build\tools\InnoSetup\ISCC.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe')
    )
    $CompilerPath = $candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
}
if (!$CompilerPath -or !(Test-Path -LiteralPath $CompilerPath)) { throw 'Install Inno Setup 6 or pass -CompilerPath.' }
$zip = Join-Path $PSScriptRoot "downloads\AdminConsolePortable-v$Version.zip"
$expected = ((Get-Content -LiteralPath ($zip + '.sha256') -Raw).Trim() -split '\s+')[0]
if ((Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash -ne $expected) { throw 'Portable ZIP checksum mismatch' }
$payload = Join-Path $PSScriptRoot "build\installer-payload-v$Version"
New-Item -ItemType Directory -Path $payload -Force | Out-Null
Expand-Archive -LiteralPath $zip -DestinationPath $payload -Force
& $CompilerPath "/DAppVersion=$Version" "/DPayloadDir=$payload" (Join-Path $PSScriptRoot 'installer\AdminConsole.iss')
if ($LASTEXITCODE) { throw 'Installer build failed' }
$output = Join-Path $PSScriptRoot "downloads\AdminConsole-Setup-v$Version.exe"
$hash = (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash.ToLowerInvariant()
[IO.File]::WriteAllText($output + '.sha256', $hash + '  ' + [IO.Path]::GetFileName($output) + "`n")
Write-Output $output
