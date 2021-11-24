using UnityEditor;
using UnityEngine;

namespace Core.Unity.Editor
{
    [CustomEditor(typeof(ActionSubscriber))]
    public class ActionSubscriberEditor : UnityEditor.Editor
    {
        private ActionSubscriber _subscriber;
        private ActionSubscriber Subscriber => _subscriber ? _subscriber : (_subscriber = target as ActionSubscriber);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var sourceProp = serializedObject.FindProperty("source");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sourceProp, new GUIContent("Source"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Subscriber.ExtractData();
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawPairs(serializedObject);
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPairs(SerializedObject serializedObject)
        {
            var pairsProp = serializedObject.FindProperty("actionUnityEventPairs");
            for (int i = 0; i < pairsProp.arraySize; i++)
            {
                var pair = pairsProp.GetArrayElementAtIndex(i);
                if (pair != null)
                {
                    DrawPair(pair);
                }
            }
        }

        private void DrawPair(SerializedProperty prop)
        {
            var actionNameProp = prop.FindPropertyRelative("actionName");
            var unityEventProp = prop.FindPropertyRelative("unityEvent");
            EditorGUILayout.PropertyField(unityEventProp,new GUIContent(actionNameProp.stringValue));
        }
    }
}