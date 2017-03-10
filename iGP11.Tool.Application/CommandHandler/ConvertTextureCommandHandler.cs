using System;
using System.IO;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.Directory;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.Application.CommandHandler
{
    public class ConvertTextureCommandHandler : IDomainCommandHandler<ConvertTextureCommand>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly ILogger _logger;
        private readonly ITextureService _textureService;

        public ConvertTextureCommandHandler(
            IDirectoryRepository directoryRepository,
            ILogger logger,
            ITextureService textureService)
        {
            _directoryRepository = directoryRepository;
            _logger = logger;
            _textureService = textureService;
        }

        public async Task HandleAsync(DomainCommandContext context, ConvertTextureCommand command)
        {
            Texture texture = null;
            try
            {
                texture = _textureService.Convert(
                    Path.Combine(command.SourceDirectoryPath, command.FileName),
                    command.Settings);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, $"texture conversion failed; exception: {exception}");
            }

            if (texture?.Metadata == null)
            {
                await context.EmitAsync(new ErrorOccuredNotification());
                return;
            }

            var directory = await _directoryRepository.LoadAsync(command.DestinationDirectoryPath);
            directory.AddFile(command.FileName, texture.Buffer);

            await _directoryRepository.SaveAsync(directory);
            await context.EmitAsync(new ActionSucceededNotification());
        }
    }
}