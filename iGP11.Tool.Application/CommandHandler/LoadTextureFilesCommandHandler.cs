using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.Application.CommandHandler
{
    public class LoadTextureFilesCommandHandler : IDomainCommandHandler<LoadTextureFilesCommand>
    {
        private readonly ITextureService _textureService;

        public LoadTextureFilesCommandHandler(ITextureService textureService)
        {
            _textureService = textureService;
        }

        public async Task HandleAsync(DomainCommandContext context, LoadTextureFilesCommand command)
        {
            IEnumerable<TextureFile> files;
            try
            {
                files = _textureService.GetTextureFiles(command.DirectoryPath);
            }
            catch (DirectoryNotFoundException)
            {
                await context.EmitAsync(new ErrorOccuredEvent());
                return;
            }
            catch (PathTooLongException)
            {
                await context.EmitAsync(new ErrorOccuredEvent());
                return;
            }
            catch (SecurityException)
            {
                await context.EmitAsync(new ErrorOccuredEvent());
                return;
            }
            catch (UnauthorizedAccessException)
            {
                await context.EmitAsync(new ErrorOccuredEvent());
                return;
            }

            await context.EmitAsync(new TextureFilesLoadedEvent(files));
        }
    }
}