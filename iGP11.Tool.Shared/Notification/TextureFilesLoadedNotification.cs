using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class TextureFilesLoadedNotification
    {
        public TextureFilesLoadedNotification(IEnumerable<TextureFile> files)
        {
            Files = files;
        }

        [DataMember(Name = "files", IsRequired = true)]
        public IEnumerable<TextureFile> Files { get; private set; }
    }
}