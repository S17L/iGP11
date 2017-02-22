using System;
using System.Collections.Generic;

using iGP11.Library;
using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.ApplicationSettings.Validation
{
    [AttributeUsage(AttributeTargets.All)]
    public class ApplicationSettingsValidator : Attribute,
                                                IValidator<ApplicationSettings>
    {
        public IEnumerable<Localizable> Validate(
            IComponentContext<ApplicationSettings> context,
            IValidationContext validationContext,
            ApplicationSettings settings)
        {
            if (settings.ApplicationCommunicationPort == settings.ProxyCommunicationPort)
            {
                yield return new Localizable(
                    "ValueEqual",
                    context.GetName(entity => entity.ApplicationCommunicationPort),
                    context.GetName(entity => entity.ProxyCommunicationPort));
            }
        }
    }
}