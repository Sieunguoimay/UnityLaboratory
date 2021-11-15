using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.Unity
{
    public class DataBinder : MonoBehaviour
    {
        [SerializeField] public UnityEngine.Object source;
        [SerializeField] public List<Connector> connectors;

        public Type SelectedType => SelectedTypeIndex < 0 ? null : _srcTypes[SelectedTypeIndex];
        private readonly List<Type> _srcTypes = new List<Type>();
        [NonSerialized] public int SelectedTypeIndex;
        [NonSerialized] public string[] SrcTypesOptions;

        [Serializable]
        public class Connector
        {
            public List<FieldOrPropertySelector> srcPath = new List<FieldOrPropertySelector>();

            public UnityEngine.Object dst;

            public List<FieldOrPropertySelector> dstPath = new List<FieldOrPropertySelector>();
        }

        [Serializable]
        public class FieldOrPropertySelector
        {
            public int selectedIndex = -1;
            public string[] options;
            public Type Type;

            public Type GetSelectedType()
            {
                if (selectedIndex == -1) return null;

                var ps = Type.GetProperties();
                var fs = Type.GetFields();
                if (selectedIndex < ps.Length)
                {
                    return ps[selectedIndex].PropertyType;
                }
                else
                {
                    return fs[selectedIndex - ps.Length].FieldType;
                }
            }

            public FieldOrPropertySelector(Type type)
            {
                if (type == null)
                {
                    options = new string[0];
                    return;
                }

                Type = type;
                options = type.GetProperties().Select(p => p.Name).ToArray();
                options = options.Concat(type.GetFields().Select(p => p.Name)).ToArray();
            }
        }

        public void AddConnector()
        {
            connectors.Add(new Connector());
        }

        [ContextMenu("Test")]
        public void CreateSrcTypeOptions()
        {
            if (source == null) return;
            _srcTypes.Clear();
            SelectedTypeIndex = -1;

            if (source is Component src1)
            {
                _srcTypes.Add(src1.GetType());
                SrcTypesOptions = new string[1];
                SrcTypesOptions[0] = src1.GetType().Name;
            }

            if (source is GameObject src)
            {
                var srcTypeNames = new List<string>();
                var components = src.GetComponents<Component>();
                foreach (var c in components)
                {
                    _srcTypes.Add(c.GetType());
                    srcTypeNames.Add(c.GetType().Name);
                    Debug.Log(c.GetType().Name);
                }

                SrcTypesOptions = srcTypeNames.ToArray();
            }

            if (SrcTypesOptions.Length > 0)
            {
                SelectedTypeIndex = 0;
            }
        }
    }

    [CustomEditor(typeof(DataBinder))]
    [CanEditMultipleObjects]
    public class DataBinderEditor : Editor
    {
        private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20f);
        private static readonly GUILayoutOption MedButtonWidth = GUILayout.Width(100f);

        private DataBinder _dataBinder;
        private DataBinder DataBinder => _dataBinder ? _dataBinder : (_dataBinder = target as DataBinder);

        private void OnValidate()
        {
            Debug.Log("Refresh");
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DataBinder.source)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DataBinder.connectors)),true);
            serializedObject.ApplyModifiedProperties();
            return;
            
            EditorGUILayout.BeginHorizontal();
            
            var src = EditorGUILayout.ObjectField(nameof(DataBinder.source), DataBinder.source, typeof(UnityEngine.Object));
            if (DataBinder.source == null || DataBinder.source != src || DataBinder.SrcTypesOptions == null)
            {
                DataBinder.source = src;
                DataBinder.CreateSrcTypeOptions();
            }

            DataBinder.SelectedTypeIndex = EditorGUILayout.Popup(DataBinder.SelectedTypeIndex, DataBinder.SrcTypesOptions, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();


            DrawConnectors(DataBinder.connectors, DataBinder.SelectedType);

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", MiniButtonWidth))
            {
                DataBinder.AddConnector();
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawConnectors(List<DataBinder.Connector> connectors, Type defaultType)
        {
            foreach (var con in connectors)
            {
                EditorGUILayout.BeginHorizontal();


                DrawPath(con.srcPath, defaultType);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("X", MiniButtonWidth))
                {
                    connectors.Remove(con);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                
                var dst = EditorGUILayout.ObjectField(nameof(con.dst), con.dst, typeof(UnityEngine.Object));
                if (con.dst == null || con.dst != dst)
                {
                    con.dst = dst;
                }

                EditorGUILayout.BeginHorizontal();

                var dstDefaultType = con.dst?.GetType();
                if (dstDefaultType != null)
                {
                    DrawPath(con.dstPath, dstDefaultType);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawPath(List<DataBinder.FieldOrPropertySelector> path, Type defaultType)
        {
            if (GUILayout.Button("-", MiniButtonWidth))
            {
                if (path.Count > 0)
                {
                    path.RemoveAt(path.Count - 1);
                    return;
                }
            }

            if (GUILayout.Button("+", MiniButtonWidth))
            {
                var t = defaultType;
                if (path.Count > 0)
                {
                    t = path[path.Count - 1]?.GetSelectedType() ?? null;
                }

                path.Add(new DataBinder.FieldOrPropertySelector(t));
            }

            for (int i = 0; i < path.Count; i++)
            {
                var s = path[i];
                var newIndex = EditorGUILayout.Popup(s.selectedIndex, s.options, GUILayout.Width(100));

                if (i == 0 && s.Type != defaultType)
                {
                    s.selectedIndex = -2;
                    newIndex = -1;
                    s = path[i] = new DataBinder.FieldOrPropertySelector(defaultType);

                    if (path.Count > 1)
                    {
                        for (int j = 1; j < path.Count; j++)
                        {
                            path[j] = new DataBinder.FieldOrPropertySelector(null);
                        }
                    }
                }

                if (newIndex != s.selectedIndex)
                {
                    s.selectedIndex = newIndex;

                    if (i < path.Count - 1)
                    {
                        path[i + 1] = new DataBinder.FieldOrPropertySelector(s.GetSelectedType());
                    }

                    if (i < path.Count - 2)
                    {
                        for (int j = i + 2; j < path.Count; j++)
                        {
                            path[j] = new DataBinder.FieldOrPropertySelector(null);
                        }
                    }
                }
            }
        }
    }
}