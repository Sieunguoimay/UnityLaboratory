using System;
using UnityEditor;
using UnityEngine;

namespace Core.Unity.Editor
{
    [CustomEditor(typeof(EventSignal))]
    [CanEditMultipleObjects]
    public class EventSignalEditor : UnityEditor.Editor
    {
        private EventSignal _eventSignal;
        private EventSignal EventSignal => _eventSignal ? _eventSignal : (_eventSignal = target as EventSignal);

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawEventGroups(serializedObject);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                EventSignal.AddNewEventGroup();
            }

            if (GUILayout.Button("-"))
            {
                EventSignal.RemoveEventGroup();
            }

            if (GUILayout.Button("+"))
            {
                EventSignal.AddFilterGroup();
            }

            if (GUILayout.Button("-"))
            {
                EventSignal.RemoveFilterGroup();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawEventGroups(SerializedObject serializedObject)
        {
            serializedObject.Update();
            var eventGroupsSerializedProp = serializedObject.FindProperty("eventGroups");
            var filterGroupsSerializedProp = serializedObject.FindProperty("filterGroups");
            DrawEditableFilterGroups(filterGroupsSerializedProp);

            for (int i = 0; i < eventGroupsSerializedProp.arraySize; i++)
            {
                var prop = eventGroupsSerializedProp.GetArrayElementAtIndex(i);
                DrawEventGroup(prop, filterGroupsSerializedProp);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEventGroup(SerializedProperty eventGroupSerializedProp, SerializedProperty filterGroupsSerializedProp)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            var filter = eventGroupSerializedProp.FindPropertyRelative("filter");
            // EditorGUILayout.PropertyField(filter, GUIContent.none);
            DrawFilterGroups(filterGroupsSerializedProp, filter.intValue);
            EditorGUILayout.EndVertical();
            EditorGUILayout.PropertyField(eventGroupSerializedProp.FindPropertyRelative("onSignal"));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFilterGroups(SerializedProperty filterGroupsSerializedProp, int filter)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < filterGroupsSerializedProp.arraySize; i++)
            {
                // EditorGUILayout.BeginHorizontal();
                var filterGroupProp = filterGroupsSerializedProp.GetArrayElementAtIndex(i);
                var selected = EventSignal.CheckFilterByIndex(i, filter);
                var result = EditorGUILayout.Toggle(selected, GUILayout.Width(16));
                EditorGUILayout.LabelField(filterGroupProp.stringValue, GUILayout.Width(50));
                // EditorGUILayout.EndHorizontal();

                if ((i + 1) % 3 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEditableFilterGroups(SerializedProperty filterGroupsSerializedProp)
        {
            int w = 50;
            int nPerLine = Math.Max(Screen.width / (w+10), 1);
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < filterGroupsSerializedProp.arraySize; i++)
            {
                var filterGroupProp = filterGroupsSerializedProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(filterGroupProp, GUIContent.none, GUILayout.Width(w));
                if ((i + 1) % nPerLine == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}