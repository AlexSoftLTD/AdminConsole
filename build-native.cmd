@echo off
setlocal
call "%~1\VC\Auxiliary\Build\vcvars32.bat" >nul
if errorlevel 1 exit /b 1
cd /d "%~dp0src"
rc.exe /nologo /fo "%~dp0build\App.res" App.rc
if errorlevel 1 exit /b 1
cl.exe /nologo /O2 /MT /EHsc /utf-8 /DUNICODE /D_UNICODE /D_WIN32_WINNT=0x0602 PortableStarter.cpp "%~dp0build\App.res" /Fo:"%~dp0build\PortableStarter.obj" /Fe:"%~2\Start Admin Console.exe" /link /SUBSYSTEM:WINDOWS,6.02
exit /b %errorlevel%
