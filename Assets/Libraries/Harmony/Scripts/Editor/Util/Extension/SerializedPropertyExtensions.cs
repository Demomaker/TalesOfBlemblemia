using System;
using System.Reflection;
using UnityEditor;

namespace Harmony
{
    public static class SerializedPropertyExtensions
    {
        public static Enum GetEnumValue(this SerializedProperty property)
        {
            var propertyPathParts = property.propertyPath.Split('.');
            object targetObject = property.serializedObject.targetObject;

            foreach (var path in propertyPathParts)
            {
                // ReSharper disable once PossibleNullReferenceException
                targetObject = targetObject.GetType().GetField(path, BindingFlags.NonPublic |
                                                                     BindingFlags.Public |
                                                                     BindingFlags.Instance).GetValue(targetObject);
            }

            return targetObject as Enum;
        }

        public static void SetEnumValue(this SerializedProperty property, Enum value)
        {
            property.intValue = (int) Convert.ChangeType(value, property.GetEnumValue().GetType());
        }

        public static bool NeedRefresh(this SerializedProperty property)
        {
            //If this throws an Exception, the property is invalid.
            try
            {
                var ignore = property.name;
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}