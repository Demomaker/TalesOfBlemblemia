using UnityEditor;
using UnityEngine;

namespace Harmony
{
    [CustomPropertyDrawer(typeof(FlagsFieldAttribute))]
    public class FlagsPropertyDrawer : BasePropertyDrawer<FlagsFieldAttribute>
    {
        protected override void Draw(FlagsFieldAttribute fieldAttribute, SerializedProperty property, Rect position)
        {
            var currentValue = property.GetEnumValue();
            var newValue = EditorGUI.EnumFlagsField(
                position,
                fieldAttribute.Label ?? property.displayName,
                currentValue
            );

            property.SetEnumValue(newValue);
        }
    }
}