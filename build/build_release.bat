echo off
set "buildpath=%VS140COMNTOOLS%..\..\VC\vcvarsall.bat"
call "%buildpath%" x64
echo ========== cleaning ==========
if exist "..\iGP11.Tool\bin\Release" rmdir "..\iGP11.Tool\bin\Release" /s /q
mkdir "..\iGP11.Tool\bin\Release"
if exist "..\iGP11.Installer\bin\Release" rmdir "..\iGP11.Installer\bin\Release" /s /q
mkdir "..\iGP11.Installer\bin\Release"
REM echo ========== ensuring registry key ==========
REM REG ADD HKCU\SOFTWARE\Microsoft\VisualStudio\14.0_Config\MSBuild /t REG_DWORD /v EnableOutOfProcBuild /d 0 /f
echo ========== building ==========
devenv "..\iGP11.sln" /build Release /Project iGP11.Core
devenv "..\iGP11.sln" /build Release /Project iGP11
devenv "..\iGP11.sln" /build Release /Project iGP11.Direct3D11
devenv "..\iGP11.sln" /build Release /Project iGP11.External.Imaging
devenv "..\iGP11.sln" /build Release /Project iGP11.External.Injection
devenv "..\iGP11.sln" /build Release /Project iGP11.Tool
devenv "..\iGP11.sln" /build Release /Project iGP11.Installer
echo ========== building completed ==========