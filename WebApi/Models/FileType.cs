using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public enum FileType
    {
        [EnumDescription("jpg")]
        jpg,
        [EnumDescription("png")]
        png,
        [EnumDescription("jpeg")]
        jpeg

    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        private string description;
        public string Description { get { return description; } }

        public EnumDescriptionAttribute(string description)
            : base()
        {
            this.description = description;
        }
    }
    public static class EnumHelper
    {
        public static string GetDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }
            string description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes =
                (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }
    }

}