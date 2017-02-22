set ConfigurationName=%1
set ConfigurationName=%ConfigurationName:"=%
set SolutionDir=%2
set SolutionDir=%SolutionDir:"=%
set TargetDir=%3
set TargetDir=%TargetDir:"=%
set "AlternativeConfigurationName=%ConfigurationName%"
if "%ConfigurationName%" == "Release - no manifest" (set "AlternativeConfigurationName=Release")
copy /Y "%SolutionDir%x64\%AlternativeConfigurationName%\iGP11.dll" "%TargetDir%"
copy /Y "%SolutionDir%x64\%AlternativeConfigurationName%\iGP11.Direct3D11.dll" "%TargetDir%"
copy /Y "%SolutionDir%x64\%AlternativeConfigurationName%\iGP11.External.Imaging.dll" "%TargetDir%"
copy /Y "%SolutionDir%x64\%AlternativeConfigurationName%\iGP11.External.Injection.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library\bin\%AlternativeConfigurationName%\iGP11.Library.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Component\bin\%AlternativeConfigurationName%\iGP11.Library.Component.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Component.DataAnnotations\bin\%AlternativeConfigurationName%\iGP11.Library.Component.DataAnnotations.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.DDD\bin\%AlternativeConfigurationName%\iGP11.Library.DDD.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.EventPublisher\bin\%AlternativeConfigurationName%\iGP11.Library.EventPublisher.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.File\bin\%AlternativeConfigurationName%\iGP11.Library.File.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Hub\bin\%AlternativeConfigurationName%\iGP11.Library.Hub.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Hub.Client\bin\%AlternativeConfigurationName%\iGP11.Library.Hub.Client.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Hub.Shared\bin\%AlternativeConfigurationName%\iGP11.Library.Hub.Shared.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Hub.Transport\bin\%AlternativeConfigurationName%\iGP11.Library.Hub.Transport.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Network\bin\%AlternativeConfigurationName%\iGP11.Library.Network.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Library.Scheduler\bin\%AlternativeConfigurationName%\iGP11.Library.Scheduler.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Application\bin\%AlternativeConfigurationName%\iGP11.Tool.Application.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Application.Api\bin\%AlternativeConfigurationName%\iGP11.Tool.Application.Api.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Bootstrapper\bin\%AlternativeConfigurationName%\iGP11.Tool.Bootstrapper.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Domain\bin\%AlternativeConfigurationName%\iGP11.Tool.Domain.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Infrastructure.Communication\bin\%AlternativeConfigurationName%\iGP11.Tool.Infrastructure.Communication.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Infrastructure.Database\bin\%AlternativeConfigurationName%\iGP11.Tool.Infrastructure.Database.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Infrastructure.External\bin\%AlternativeConfigurationName%\iGP11.Tool.Infrastructure.External.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Localization\bin\%AlternativeConfigurationName%\iGP11.Tool.Localization.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.ReadModel\bin\%AlternativeConfigurationName%\iGP11.Tool.ReadModel.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.ReadModel.Api\bin\%AlternativeConfigurationName%\iGP11.Tool.ReadModel.Api.dll" "%TargetDir%"
copy /Y "%SolutionDir%iGP11.Tool.Shared\bin\%AlternativeConfigurationName%\iGP11.Tool.Shared.dll" "%TargetDir%"