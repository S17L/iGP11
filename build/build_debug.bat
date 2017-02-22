echo off
set "buildpath=%VS140COMNTOOLS%..\..\VC\vcvarsall.bat"
call "%buildpath%" x64
echo ========== cleaning ==========
if exist "..\iGP11.Tool\bin\Debug" rmdir "..\iGP11.Tool\bin\Debug" /s /q
mkdir "..\iGP11.Tool\bin\Debug"
if exist "..\iGP11.Installer\bin\Debug" rmdir "..\iGP11.Installer\bin\Debug" /s /q
mkdir "..\iGP11.Installer\bin\Debug"
echo ========== building ==========
devenv "..\iGP11.sln" /build Debug /Project iGP11.Core
devenv "..\iGP11.sln" /build Debug /Project iGP11
devenv "..\iGP11.sln" /build Debug /Project iGP11.Direct3D11
devenv "..\iGP11.sln" /build Debug /Project iGP11.External.Imaging
devenv "..\iGP11.sln" /build Debug /Project iGP11.External.Injection
devenv "..\iGP11.sln" /build Debug /Project iGP11.Tool
devenv "..\iGP11.sln" /build Debug /Project iGP11.Installer
echo ========== building completed ==========