set ConfigurationName=%1
set ConfigurationName=%ConfigurationName:"=%
set TargetDir=%2
set TargetDir=%TargetDir:"=%
if "%ConfigurationName%" == "Release" rd /s /q "%TargetDir%"
if "%ConfigurationName%" == "Release - no manifest" rd /s /q "%TargetDir%"
exit 0