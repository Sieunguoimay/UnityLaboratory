using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Unity.Editor
{
    [CustomEditor(typeof(DataBinder))]
    [CanEditMultipleObjects]
    public class DataBinderEditor : UnityEditor.Editor
    {
        private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20f);

        private DataBinder _dataBinder;
        private DataBinder DataBinder => _dataBinder ? _dataBinder : (_dataBinder = target as DataBinder);

        private void OnValidate()
        {
            Debug.Log("Refresh");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Data"), GUILayout.Width(30));
            ObjectSelector.DrawObjectSelector(DataBinder.srcSelector);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add " + nameof(EventListener)))
            {
                var el = DataBinder.gameObject.AddComponent<EventListener>();
                el.AddEventGroup();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(el.eventGroups[0].invoker, DataBinder.Bind);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (DataBinder.connectors != null)
            {
                DrawConnectors(DataBinder, DataBinder.connectors, DataBinder.srcSelector.selectedTypeWrapper.GetSerializedType());
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", MiniButtonWidth))
            {
                ArrayUtils.Add(ref DataBinder.connectors, new DataBinder.Connector());
            }

            if (GUILayout.Button("Bind", GUILayout.Width(80f)))
            {
                DataBinder.Bind();
            }

            // GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }


        private static void DrawConnectors(DataBinder dataBinder, DataBinder.Connector[] connectors, Type srcDefaultType)
        {
            foreach (var con in connectors)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                var dstDefaultType = con.dstSelector?.selectedTypeWrapper.GetSerializedType();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20f)))
                {
                    if (con.srcPath != null && con.srcPath.Length > 0)
                    {
                        ArrayUtils.Remove(ref con.srcPath, con.srcPath.Last());
                    }
                }

                if (GUILayout.Button("+", GUILayout.Width(20f)))
                {
                    if (con.srcPath == null) con.srcPath = new PathSelector[0];
                    ArrayUtils.Add(ref con.srcPath, PathSelector.CreatePathSelector(con.srcPath, srcDefaultType, true));
                }

                PathSelector.DrawPath(con.srcPath, srcDefaultType, true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20f)))
                {
                    if (con.dstPath != null && con.dstPath.Length > 0)
                    {
                        ArrayUtils.Remove(ref con.dstPath, con.dstPath.Last());
                    }
                }

                if (GUILayout.Button("+", GUILayout.Width(20f)))
                {
                    ArrayUtils.Add(ref con.dstPath, PathSelector.CreatePathSelector(con.dstPath, dstDefaultType, false));
                }

                PathSelector.DrawPath(con.dstPath, dstDefaultType, false);
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                var guiColor = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("x", new[] {MiniButtonWidth}))
                {
                    ArrayUtils.Remove(ref dataBinder.connectors, con);
                    break;
                }

                GUI.color = guiColor;

                ObjectSelector.DrawObjectSelector(con.dstSelector);
                con.dataConverter.Draw();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }
    }
}