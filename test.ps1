param([string]$Version='0.1.17')
$ErrorActionPreference='Stop'
$root=$PSScriptRoot
$build=Join-Path $root 'build'
$out=Join-Path $root ('dist\AdminConsolePortable-v'+$Version)
if (!(Test-Path (Join-Path $out 'Start Admin Console.exe'))) { throw 'Run build.ps1 first' }
$process=Start-Process -FilePath (Join-Path $out 'Start Admin Console.exe') -ArgumentList '--check-runtime' -Wait -PassThru
if ($process.ExitCode -ne 0) { throw 'Native runtime check failed' }
$framework=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319'
$licenseResource=@(
    ('/resource:"'+(Join-Path $root 'licenses\LICENSE.ru.txt')+'",AdminConsole.License.ru'),
    ('/resource:"'+(Join-Path $root 'licenses\LICENSE.en.txt')+'",AdminConsole.License.en')
)
$refs=@('System.Windows.Forms.dll','System.Drawing.dll',(Join-Path $framework 'WPF\PresentationFramework.dll'),(Join-Path $framework 'WPF\PresentationCore.dll'),(Join-Path $framework 'WPF\WindowsBase.dll'),(Join-Path $framework 'System.Xaml.dll'))
$common=@('/nologo','/target:exe',('/win32icon:"'+(Join-Path $root 'src\assets\AdminConsole.ico')+'"'),('/win32manifest:"'+(Join-Path $root 'src\AdminConsoleDpi.manifest')+'"'))
$common += $licenseResource
$common += $refs | ForEach-Object { '/reference:"'+$_+'"' }
$common += @('AdminConsole.cs','DpiForm.cs','StarterIntegration.cs') | ForEach-Object { '"'+(Join-Path $root ('src\'+$_))+'"' }
foreach ($probe in @('HomeProbe','DpiProbe','StarterProbe','DefaultProbe','AboutProbe')) {
    $exe=Join-Path $build ($probe+'-'+[Guid]::NewGuid().ToString('N')+'.exe')
    $rsp=Join-Path $build 'test.rsp'
    @('/main:'+$probe; '/out:"'+$exe+'"'; $common; '"'+(Join-Path $root ('tests\'+$probe+'.cs'))+'"') | Set-Content $rsp -Encoding UTF8
    & (Join-Path $framework 'csc.exe') ('@'+$rsp)
    if ($LASTEXITCODE -ne 0) { throw 'Test build failed' }
    & $exe (Join-Path $build 'previews')
    if ($LASTEXITCODE -ne 0) { throw ($probe+' failed') }
}
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip=[IO.Compression.ZipFile]::OpenRead((Join-Path $root ('dist\AdminConsolePortable-v'+$Version+'.zip')))
try {
    $names=@($zip.Entries | ForEach-Object { $_.FullName } | Sort-Object)
    if (($names -join ',') -ne 'Admin Console APP.exe,Create Shortcuts.cmd,LICENSE.txt,README.txt,Start Admin Console.exe') { throw 'Unexpected release contents' }
} finally { $zip.Dispose() }
Write-Output 'PASS: runtime, shell home directory, DPI, taskbar properties, two EXEs and README in ZIP'
