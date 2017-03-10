using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.Application.CommandHandler
{
    public class GenerateTexturePreviewCommandHandler : IDomainCommandHandler<GenerateTexturePreviewCommand>
    {
        private readonly ITextureService _textureService;

        public GenerateTexturePreviewCommandHandler(ITextureService textureService)
        {
            _textureService = textureService;
        }

        public async Task HandleAsync(DomainCommandContext context, GenerateTexturePreviewCommand command)
        {
            var settings = new TextureConversionSettings
            {
                ColorSpace = command.ColorSpace,
                FileType = FileType.WIC,
                MaxHeight = command.MaxSize,
                MaxWidth = command.MaxSize,
                OutputFormat = command.OutputFormat,
                KeepMipmaps = false
            };

            var metadata = _textureService.GetMetadata(command.FilePath);
            if (metadata == null)
            {
                await context.EmitAsync(new ErrorOccuredNotification());
            }

            var texture = _textureService.Convert(command.FilePath, settings);
            if (texture?.Metadata == null)
            {
                await context.EmitAsync(new ErrorOccuredNotification());
            }

            await context.EmitAsync(new GeneratedTexturePreviewNotification(texture, metadata));
        }
    }
}