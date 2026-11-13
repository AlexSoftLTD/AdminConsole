param([string]$Version = '0.1.17')
$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+([-.][a-zA-Z0-9.-]+)?$') { throw 'Invalid version' }
$root = Split-Path $PSScriptRoot
$key = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\{AE37B230-0834-4934-8793-10CFCA1BD103}_is1'
if (Test-Path $key) { throw 'AdminConsole is already installed. Use a clean test account.' }
$testDir = Join-Path $root ('build\installer-test-' + [Guid]::NewGuid().ToString('N'))
$setup = Join-Path $root "downloads\AdminConsole-Setup-v$Version.exe"
$common = @('/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/CURRENTUSER',
    '/MERGETASKS=!desktopicon,!startmenuicon,!taskbarhelp,!launchapp', ('/DIR="' + $testDir + '"'))
function Install([string]$language) {
    $process = Start-Process -FilePath $setup -ArgumentList ($common + "/LANG=$language") -WindowStyle Hidden -Wait -PassThru
    if ($process.ExitCode) { throw "Setup failed: $($process.ExitCode)" }
}
try {
    Install 'russian'
    if (!(Test-Path $key)) { throw 'Uninstall registration missing' }
    foreach ($file in @('Start Admin Console.exe', 'Admin Console APP.exe', 'README.txt', 'unins000.exe')) {
        if (!(Test-Path -LiteralPath (Join-Path $testDir $file))) { throw "Missing installed file: $file" }
    }
    $settings = Join-Path $testDir 'AdminConsole.settings'
    if ((Get-Content $settings)[1] -ne 'ru') { throw 'Russian preference not initialized' }
    [IO.File]::WriteAllText($settings, "powershell-admin`r`nru`r`n")
    Install 'english'
    $lines = Get-Content $settings
    if ($lines[0] -ne 'powershell-admin' -or $lines[1] -ne 'en') { throw 'Upgrade lost default mode or language' }
    [IO.File]::WriteAllText((Join-Path $testDir 'unrelated.txt'), 'preserve this file')
    Write-Output 'PASS: install, uninstall registration, both languages, upgrade preserves default launch'
} finally {
    $uninstaller = Join-Path $testDir 'unins000.exe'
    if (Test-Path -LiteralPath $uninstaller) {
        $process = Start-Process -FilePath $uninstaller -ArgumentList '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART' -WindowStyle Hidden -Wait -PassThru
        if ($process.ExitCode) { throw "Uninstall failed: $($process.ExitCode)" }
        for ($i = 0; $i -lt 20 -and (Test-Path -LiteralPath $uninstaller); $i++) { Start-Sleep -Milliseconds 500 }
    }
}
if ((Test-Path $key) -or (Test-Path -LiteralPath (Join-Path $testDir 'Admin Console APP.exe')) -or
    (Test-Path -LiteralPath (Join-Path $testDir 'AdminConsole.settings'))) { throw 'Uninstall left registered application files' }
if (!(Test-Path -LiteralPath (Join-Path $testDir 'unrelated.txt'))) { throw 'Uninstall removed unrelated file' }
Write-Output 'PASS: uninstall removes application, preferences and registration; unrelated file preserved'
