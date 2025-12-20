using System.ComponentModel;
using System.Reflection;

namespace NeoVoting.Domain.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            // Get the metadata for the specific enum member (e.g., MountLebanon)
            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());

            // A safety check in case the field isn't found
            if (fieldInfo == null)
            {
                return value.ToString();
            }

            // --- THE CORRECTED PART ---
            // 1. We expect that the attribute might not exist, so we store it in a
            //    nullable variable (DescriptionAttribute?).
            // 2. We use the 'as' operator for a safer cast. 'as' will return null
            //    if the cast fails, instead of throwing an exception.
            var attribute = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;

            // The null-coalescing operator provides a fallback.
            // If the attribute was found (is not null), return its Description.
            // Otherwise, fall back to the enum member's default string name.
            return attribute?.Description ?? value.ToString();
        }
    }
}