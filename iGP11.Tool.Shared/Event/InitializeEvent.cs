using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.ApplicationSettings;
using iGP11.Tool.Shared.Model.InjectionSettings;
using iGP11.Tool.Shared.Model.TextureManagementSettings;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class InitializeEvent
    {
        public InitializeEvent(
            ApplicationSettings applicationSettings,
            IEnumerable<InjectionSettings> injectionSettings,
            Guid? lastEditedInjectionSettingsId,
            TextureManagementSettings textureManagementSettings,
            UsageStatistics usageStatistics)
        {
            ApplicationSettings = applicationSettings;
            InjectionSettings = injectionSettings;
            LastEditedInjectionSettingsId = lastEditedInjectionSettingsId;
            TextureManagementSettings = textureManagementSettings;
            UsageStatistics = usageStatistics;
        }

        [DataMember(Name = "applicationSettings", IsRequired = true)]
        public ApplicationSettings ApplicationSettings { get; private set; }

        [DataMember(Name = "injectionSettings", IsRequired = true)]
        public IEnumerable<InjectionSettings> InjectionSettings { get; private set; }

        [DataMember(Name = "lastEditedInjectionSettingsId", IsRequired = true)]
        public Guid? LastEditedInjectionSettingsId { get; private set; }

        [DataMember(Name = "textureManagementSettings", IsRequired = true)]
        public TextureManagementSettings TextureManagementSettings { get; private set; }

        [DataMember(Name = "usageStatistics", IsRequired = true)]
        public UsageStatistics UsageStatistics { get; private set; }
    }
}