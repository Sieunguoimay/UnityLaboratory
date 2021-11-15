using UnityEditor;
using UnityEngine;

namespace Core.Unity.Editor
{
    [CustomPropertyDrawer(typeof(DataBinderTest.ColorPoint))]
    public class ColorPointDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var pos = EditorGUI.PrefixLabel(position, label);

            if (pos.height > 16f)
            {
                pos.height = 16f;
                pos.width = Screen.width;
                pos.x = position.x+20f;
                pos.y += 18f;
            }

            pos.width *= 0.75f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("position"), GUIContent.none);
            pos.x += pos.width;
            pos.width /= 3f;
            EditorGUIUtility.labelWidth = 14f;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("color"), new GUIContent("C"));
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Screen.width < 400 ? (16f + 18f) : 16f;
        }
    }
}