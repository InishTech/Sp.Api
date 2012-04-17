@ECHO OFF
SETLOCAL
set DotNetDir=%windir%\Microsoft.NET\Framework\v4.0.30319%
set MSBuildExe=%DotNetDir%\msbuild.exe

IF NOT EXIST "%DotNetDir%" GOTO :NODOTNET
IF NOT EXIST "%MSBuildExe%" GOTO :NODOTNET

set CoreMSBuildInvocation=%MSBuildExe% Sp.Api.sln /m 
IF [%1] NEQ [] %CoreMSBuildInvocation% /t:%1
IF [%1] == [] %CoreMSBuildInvocation%
IF ERRORLEVEL 1 GOTO :ERROR

echo Build completed
GOTO :EOF

:NODOTNET
ECHO Error: Could not locate MSBuild. Looked in %DotNetDir%
GOTO :EOF

:ERROR
ECHO ON
EXIT /b %ERRORLEVEL%

:EOF