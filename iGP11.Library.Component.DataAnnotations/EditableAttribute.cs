using System;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class EditableAttribute : Attribute
    {
        public EditableAttribute(FormType type = FormType.New | FormType.Edit)
        {
            FormType = type;
        }

        public FormType FormType { get; }
    }
}