using System;
using System.ComponentModel;

namespace customerportalapi.Entities.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// USAGE EXAMPLE: "var desc = RoleGroupTypesExtensions.GetDescription(RoleGroupTypes.StoreManagers);"
        /// </summary>
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes?.Length > 0 ?
                    ((DescriptionAttribute)attributes?[0]).Description :
                    null;
            }
            return enumerationValue.ToString();
        }

        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes?.Length > 0 ?
                attributes?[0].Description :
                null;
        }
    }
}
