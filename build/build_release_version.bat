echo off
echo --- building release version ---
call .\build_release.bat
echo --- create application release directory ---
if exist ".\iGP11" rmdir ".\iGP11" /s /q
mkdir ".\iGP11"
mkdir ".\iGP11\tex_override"
echo --- copying builded versions ---
robocopy "..\iGP11.Tool\bin\Release" ".\iGP11" /s /e
echo --- copying tex_override folder ---
robocopy ".\tex_override" ".\iGP11\tex_override" /s /e
echo --- copying launcher shortcut file ---
echo f | xcopy /f /y ".\iGP11.Launcher.lnk" ".\iGP11\iGP11.Launcher.lnk"
echo --- copying readme file ---
copy /Y ".\readme\readme.pdf" ".\iGP11"
echo --- done ---