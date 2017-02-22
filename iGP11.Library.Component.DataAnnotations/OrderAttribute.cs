using System;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class OrderAttribute : Attribute
    {
        public OrderAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}