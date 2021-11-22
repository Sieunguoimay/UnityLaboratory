using UnityEditor;
using UnityEngine;

namespace Core.Unity.Editor
{
    [CustomEditor(typeof(EventListener))]
    public class EventListenerEditor : UnityEditor.Editor
    {
        private EventListener _eventListener;
        private EventListener EventListener => _eventListener ? _eventListener : (_eventListener = target as EventListener);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var eventNamesProp = serializedObject.FindProperty(nameof(EventListener.eventGroups));
            for (int i = 0; i < eventNamesProp.arraySize; i++)
            {
                DrawEventGroup(eventNamesProp.GetArrayElementAtIndex(i), i);
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                EventListener.AddEventGroup();
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                EventListener.RemoveEventGroup();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEventGroup(SerializedProperty eventGroup, int eventGroupIndex)
        {
            var ev = eventGroup.FindPropertyRelative("invoker");
            var eventNames = eventGroup.FindPropertyRelative("eventNames");

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(GUILayout.Width(100));

            EditorGUILayout.LabelField(new GUIContent("Events"), GUILayout.Width(40));

            DrawEventNames(eventNames, eventGroupIndex);

            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(ev, new GUIContent(ev.name));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEventNames(SerializedProperty eventNames, int eventGroupIndex)
        {
            for (int i = 0; i < eventNames.arraySize; i++)
            {
                EditorGUILayout.PropertyField(eventNames.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                EventListener.AddEventName(eventGroupIndex);
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                EventListener.RemoveEventName(eventGroupIndex);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}