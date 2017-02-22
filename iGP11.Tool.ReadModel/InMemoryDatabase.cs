﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Tool.ReadModel.Api.Model;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.InjectionSettings;
using iGP11.Tool.Shared.Model.TextureManagementSettings;

namespace iGP11.Tool.ReadModel
{
    [DataContract]
    public class InMemoryDatabase
    {
        [DataMember(Name = "applicationSettings", IsRequired = true)]
        public ConstantSettings ConstantSettings { get; set; }

        [DataMember(Name = "injectionSettings", IsRequired = true)]
        public List<InjectionSettings> InjectionSettings { get; private set; } = new List<InjectionSettings>();

        [DataMember(Name = "injectionStatuses", IsRequired = true)]
        public ConcurrentDictionary<string, InjectionStatus> InjectionStatuses { get; private set; } = new ConcurrentDictionary<string, InjectionStatus>();

        [DataMember(Name = "lastEditedInjectionSettingsId", IsRequired = true)]
        public Guid? LastEditedInjectionSettingsId { get; set; }

        [DataMember(Name = "pluginState", IsRequired = true)]
        public ProxySettings PluginState { get; set; }

        [DataMember(Name = "proxyActivationStatuses", IsRequired = true)]
        public ConcurrentDictionary<string, ActivationStatus> ProxyActivationStatuses { get; private set; } = new ConcurrentDictionary<string, ActivationStatus>();

        [DataMember(Name = "proxySettings", IsRequired = true)]
        public ProxySettings ProxySettings { get; set; }

        [DataMember(Name = "textureManagementSettings", IsRequired = true)]
        public TextureManagementSettings TextureManagementSettings { get; set; }

        [DataMember(Name = "usageStatistics", IsRequired = true)]
        public UsageStatistics UsageStatistics { get; set; }
    }
}