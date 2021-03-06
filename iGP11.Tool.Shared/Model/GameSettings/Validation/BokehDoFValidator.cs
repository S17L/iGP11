using System;
using System.Collections.Generic;

using iGP11.Library;
using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings.Validation
{
    [AttributeUsage(AttributeTargets.All)]
    public class BokehDoFValidator : Attribute,
                                     IValidator<BokehDoF>
    {
        public IEnumerable<Localizable> Validate(
            IComponentContext<BokehDoF> context,
            IValidationContext validationContext,
            BokehDoF bokehDoF)
        {
            if (bokehDoF.DepthMaximum <= bokehDoF.DepthMinimum)
            {
                yield return new Localizable(
                    "ValueLessThan",
                    context.GetComponentName(entity => entity.DepthMaximum),
                    context.GetComponentName(entity => entity.DepthMinimum));
            }
        }
    }
}