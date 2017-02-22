using iGP11.Library;
using iGP11.Tool.Domain.Configuration;
using iGP11.Tool.Domain.Exceptions;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    public class InjectionSettingsFactory
    {
        public InjectionSettings Create(Direct3D11ProfileType profileType)
        {
            var jsonTemplate = Configurations.ResourceManager.GetString(profileType.GetResourceKey());
            if (jsonTemplate == null)
            {
                throw new ProfileTemplateNotFoundException("profile template not found");
            }

            return jsonTemplate.Deserialize<InjectionSettings>();
        }
    }
}