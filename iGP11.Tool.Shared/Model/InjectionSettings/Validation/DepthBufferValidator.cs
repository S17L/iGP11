using System;
using System.Collections.Generic;

using iGP11.Library;
using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.InjectionSettings.Validation
{
    [AttributeUsage(AttributeTargets.All)]
    public class DepthBufferValidator : Attribute,
                                        IValidator<DepthBuffer>
    {
        public IEnumerable<Localizable> Validate(
            IComponentContext<DepthBuffer> context,
            IValidationContext validationContext,
            DepthBuffer depthBuffer)
        {
            if (depthBuffer.LinearZFar <= depthBuffer.LinearZNear)
            {
                yield return new Localizable(
                    "ValueLessThan",
                    context.GetName(entity => entity.LinearZFar),
                    context.GetName(entity => entity.LinearZNear));
            }

            if (depthBuffer.DepthMaximum <= depthBuffer.DepthMinimum)
            {
                yield return new Localizable(
                    "ValueLessThan",
                    context.GetName(entity => entity.DepthMaximum),
                    context.GetName(entity => entity.DepthMinimum));
            }
        }
    }
}