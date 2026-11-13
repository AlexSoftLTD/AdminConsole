param([string]$Version='0.1.17')
$ErrorActionPreference='Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+([-.][a-zA-Z0-9.-]+)?$') { throw 'Invalid version' }
$root=$PSScriptRoot
$out=Join-Path $root ('dist\AdminConsolePortable-v'+$Version)
$temp=Join-Path $root 'build'
New-Item -ItemType Directory -Force $out,$temp | Out-Null
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (!(Test-Path $vswhere)) { throw 'Install Visual Studio Build Tools with Desktop development with C++ and Windows SDK.' }
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
if (!$vs) { throw 'MSVC tools not found' }
& cmd.exe /d /c (Join-Path $root 'build-native.cmd') $vs $out
if ($LASTEXITCODE -ne 0) { throw 'Native build failed' }
$framework=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319'
$compiler=Join-Path $framework 'csc.exe'
$licenseResource=@(
    ('/resource:"'+(Join-Path $root 'licenses\LICENSE.ru.txt')+'",AdminConsole.License.ru'),
    ('/resource:"'+(Join-Path $root 'licenses\LICENSE.en.txt')+'",AdminConsole.License.en')
)
$refs=@('System.Windows.Forms.dll','System.Drawing.dll',(Join-Path $framework 'WPF\PresentationFramework.dll'),(Join-Path $framework 'WPF\PresentationCore.dll'),(Join-Path $framework 'WPF\WindowsBase.dll'),(Join-Path $framework 'System.Xaml.dll'))
$options=@('/nologo','/target:winexe','/optimize+',('/out:"'+(Join-Path $out 'Admin Console APP.exe')+'"'),('/win32icon:"'+(Join-Path $root 'src\assets\AdminConsole.ico')+'"'),('/win32manifest:"'+(Join-Path $root 'src\AdminConsoleDpi.manifest')+'"'))
$options += $licenseResource
$options += $refs | ForEach-Object { '/reference:"'+$_+'"' }
$options += @('AdminConsole.cs','DpiForm.cs','StarterIntegration.cs') | ForEach-Object { '"'+(Join-Path $root ('src\'+$_))+'"' }
$rsp=Join-Path $temp 'core.rsp'
$options | Set-Content $rsp -Encoding UTF8
& $compiler ('@'+$rsp)
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed' }
Copy-Item -LiteralPath (Join-Path $root 'README-download.txt') -Destination (Join-Path $out 'README.txt') -Force
Copy-Item -LiteralPath (Join-Path $root 'Create Shortcuts.cmd') -Destination $out -Force
Copy-Item -LiteralPath (Join-Path $root 'LICENSE.txt') -Destination $out -Force
$files=@((Join-Path $out 'Start Admin Console.exe'),(Join-Path $out 'Admin Console APP.exe'),(Join-Path $out 'README.txt'),(Join-Path $out 'LICENSE.txt'),(Join-Path $out 'Create Shortcuts.cmd'))
$zip=Join-Path $root ('dist\AdminConsolePortable-v'+$Version+'.zip')
Compress-Archive -LiteralPath $files -DestinationPath $zip -Force
Get-FileHash -Algorithm SHA256 $zip | ForEach-Object { $_.Hash.ToLowerInvariant()+'  '+[IO.Path]::GetFileName($_.Path) } | Set-Content ($zip+'.sha256') -Encoding ASCII
Write-Output $zip
