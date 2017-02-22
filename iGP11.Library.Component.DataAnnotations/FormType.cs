using System;

namespace iGP11.Library.Component.DataAnnotations
{
    [Flags]
    public enum FormType
    {
        None = 0x00,
        New = 0x01,
        Edit = 0x02
    }
}