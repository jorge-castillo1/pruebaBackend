using System;
using System.Reflection;

namespace customerportalapi.Entities.Enums
{
    public class EnumGuidMapperAttribute : Attribute
    {
        public virtual Guid Value { get; private set; }

        public EnumGuidMapperAttribute(string value)
        {
            Value = Guid.Parse(value);
        }

        public static Guid? GetValue(RoleGroupTypes value)
        {
            var type = typeof(RoleGroupTypes);
            if (!type.IsEnum) throw new InvalidOperationException();
            FieldInfo enumValue = type.GetField(value.ToString());
            if (enumValue == null) return null;
            EnumGuidMapperAttribute attribute = Attribute.GetCustomAttribute(enumValue, typeof(EnumGuidMapperAttribute)) as EnumGuidMapperAttribute;
            if (attribute == null) return null;
            return attribute.Value;
        }

        public static RoleGroupTypes GetEnum(Guid value)
        {
            var type = typeof(RoleGroupTypes);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(EnumGuidMapperAttribute)) as EnumGuidMapperAttribute;
                if (attribute != null)
                {
                    if (attribute.Value == value)
                        return (RoleGroupTypes)field.GetValue(null);
                }
            }
            return default(RoleGroupTypes);
        }
    }
}