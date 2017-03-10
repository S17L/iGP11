using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using iGP11.Library;
using iGP11.Library.Component.DataAnnotations;
using iGP11.Library.DDD;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class Game : AggregateRoot<AggregateId>
    {
        private IList<GameProfile> _profiles;

        public Game(
            AggregateId id,
            string name,
            string filePath,
            IEnumerable<GameProfile> profiles)
            : base(id)
        {
            Name = name;
            FilePath = filePath;
            _profiles = profiles?.ToList() ?? new List<GameProfile>();
        }

        [DataMember(Name = "name", IsRequired = true)]
        [Editable]
        public string Name { get; private set; }

        [DataMember(Name = "filePath")]
        [Editable]
        [FilePath]
        public string FilePath { get; private set; }

        [DataMember(Name = "profileId")]
        [Editable]
        public AggregateId ProfileId { get; private set; }

        [DataMember(Name = "profiles", IsRequired = true)]
        [Editable]
        public IList<GameProfile> Profiles
        {
            get { return _profiles; }
            private set { _profiles = value?.ToList() ?? new List<GameProfile>(); }
        }

        public void ChangeFilePath(string filePath)
        {
            FilePath = filePath;
        }

        public void ChangeName(string name)
        {
            Name = name;
        }

        public GameProfile AddGameProfile(
            AggregateId gameProfileId,
            string name,
            string proxyDirectoryPath,
            string logsDirectoryPath,
            PluginType pluginType,
            Direct3D11Settings direct3D11Settings)
        {
            var profile = new GameProfile(
                gameProfileId,
                Id,
                name,
                proxyDirectoryPath,
                logsDirectoryPath,
                pluginType,
                direct3D11Settings);

            _profiles.Remove(entity => entity.Id == gameProfileId);
            _profiles.Add(profile);
            ProfileId = profile.Id;

            return profile.Clone();
        }

        public void RemoveGameProfile(AggregateId gameProfileId)
        {
            if (_profiles.Count <= 1)
            {
                throw new DomainOperationException("game must have at least one game profile");
            }

            _profiles.Remove(entity => entity.Id == gameProfileId);
            if (ProfileId == gameProfileId)
            {
                ProfileId = _profiles.First().Id;
            }
        }
    }
}
