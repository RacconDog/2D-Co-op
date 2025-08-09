#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace DeveloperConsole
{
    [CustomPropertyDrawer(typeof(ColorHtmlPropertyAttribute))]
    public class ColorHtmlPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Color)
            {
                EditorGUI.LabelField(position, label.text, "Use with Color fields only.");
                return;
            }

            // Layout fields: text input (HTML) and color picker
            Rect htmlField = new Rect(position.x, position.y, position.width - 100, position.height);
            Rect colorField = new Rect(position.x + htmlField.width + 2, position.y, 98, position.height);

            // Display HTML string
            string htmlColor = "#" + ColorUtility.ToHtmlStringRGBA(property.colorValue);
            string newHtml = EditorGUI.TextField(htmlField, label, htmlColor);

            // Parse HTML input
            if (ColorUtility.TryParseHtmlString(newHtml, out Color parsedColor))
            {
                property.colorValue = parsedColor;
            }

            // Display color picker
            property.colorValue = EditorGUI.ColorField(colorField, GUIContent.none, property.colorValue, true, true, false);
        }
    }
}
#endif
