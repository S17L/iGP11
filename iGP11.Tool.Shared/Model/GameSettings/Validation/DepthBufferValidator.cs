using System;
using System.Collections.Generic;

using iGP11.Library;
using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings.Validation
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
                    context.GetComponentName(entity => entity.LinearZFar),
                    context.GetComponentName(entity => entity.LinearZNear));
            }

            if (depthBuffer.DepthMaximum <= depthBuffer.DepthMinimum)
            {
                yield return new Localizable(
                    "ValueLessThan",
                    context.GetComponentName(entity => entity.DepthMaximum),
                    context.GetComponentName(entity => entity.DepthMinimum));
            }
        }
    }
}