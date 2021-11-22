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

        private bool _filterGroupsToggle = true;
        private bool _eventGroupsToggle = true;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawEventGroups(serializedObject);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                EventSignal.AddNewEventGroup();
            }

            if (GUILayout.Button("-", GUILayout.Width(50)))
            {
                EventSignal.RemoveEventGroup();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawEventGroups(SerializedObject serializedObject)
        {
            serializedObject.Update();
            var eventGroupsSerializedProp = serializedObject.FindProperty("eventGroups");
            var filterGroupsSerializedProp = serializedObject.FindProperty("filterGroups");
            _filterGroupsToggle = EditorGUILayout.Foldout(_filterGroupsToggle, "Edit Filters");
            if (_filterGroupsToggle)
            {
                DrawEditableFilterGroups(filterGroupsSerializedProp);
                EditorGUILayout.Space(5);
            }

            _eventGroupsToggle = EditorGUILayout.Foldout(_eventGroupsToggle, "Show Filters");

            for (int i = 0; i < eventGroupsSerializedProp.arraySize; i++)
            {
                var prop = eventGroupsSerializedProp.GetArrayElementAtIndex(i);
                DrawEventGroup(prop, filterGroupsSerializedProp);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEventGroup(SerializedProperty eventGroupSerializedProp, SerializedProperty filterGroupsSerializedProp)
        {
            if (_eventGroupsToggle)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                // EditorGUILayout.PropertyField(filter, GUIContent.none);
                var filter = eventGroupSerializedProp.FindPropertyRelative("filter");
                DrawFilterGroups(filterGroupsSerializedProp, filter);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(eventGroupSerializedProp.FindPropertyRelative("onSignal"));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.PropertyField(eventGroupSerializedProp.FindPropertyRelative("onSignal"));
            }
        }

        private void DrawFilterGroups(SerializedProperty filterGroupsSerializedProp, SerializedProperty filter)
        {
            EditorGUILayout.BeginHorizontal();
            int w = 96;
            int nPerLine = Math.Max(Screen.width / ((w + 10) * 2), 1);
            for (int i = 0; i < filterGroupsSerializedProp.arraySize; i++)
            {
                var filterGroupProp = filterGroupsSerializedProp.GetArrayElementAtIndex(i);
                var selected = EventSignal.CheckFilterByIndex(i, filter.intValue);
                var result = EditorGUILayout.Toggle(selected, GUILayout.Width(16));
                var pow = Mathf.CeilToInt(Mathf.Pow(2, i));
                if (result)
                {
                    filter.intValue |= pow;
                }
                else
                {
                    filter.intValue &= ~pow;
                }

                EditorGUILayout.LabelField(filterGroupProp.stringValue, GUILayout.Width(80));

                if ((i + 1) % nPerLine == 0)
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
            int nPerLine = Math.Max(Screen.width / (w + 5), 1);
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < filterGroupsSerializedProp.arraySize; i++)
            {
                var filterGroupProp = filterGroupsSerializedProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(filterGroupProp, GUIContent.none, GUILayout.Width(w));
                if (GUILayout.Button("x", GUILayout.Width(16)))
                {
                    EventSignal.RemoveFilterGroup(i);
                    break;
                }

                GUILayout.Space(5);
                if ((i + 1) % nPerLine == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }

            if (GUILayout.Button("+", GUILayout.Width(16)))
            {
                EventSignal.AddFilterGroup();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}