using System;
using System.ComponentModel;

namespace customerportalapi.Entities.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// USAGE EXAMPLE: "var desc = DocumentStatusTypes.Ready.GetDescription();"
        /// </summary>
        public static string GetDescription<T>(this T enumerationValue) where T : struct, IConvertible
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

        public static int ToInt<T>(this T soure) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            return (int)(IConvertible)soure;
        }

        public static int Count<T>(this T soure) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            return Enum.GetNames(typeof(T)).Length;
        }
    }
}
