using System;

namespace KrisG.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; set; }

        public DefaultValueAttribute(object value)
        {
            Value = value;
        }
    }
}