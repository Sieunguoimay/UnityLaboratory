using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Unity
{
    public class DataBinder : MonoBehaviour
    {
        [SerializeField] public List<Connector> connectors = new List<Connector>();

        [SerializeField] public ObjectSelector srcSelector = new ObjectSelector();

        [Serializable]
        public class Connector
        {
            [SerializeField] public List<PathSelector> srcPath = new List<PathSelector>();

            public ObjectSelector dstSelector;

            [SerializeField] public List<PathSelector> dstPath = new List<PathSelector>();
        }

        public void AddConnector()
        {
            connectors.Add(new Connector());
        }

        public void Bind()
        {
            if (srcSelector.SelectedData == null) return;

            foreach (var con in connectors)
            {
                var data = GetData(srcSelector.SelectedData, con.srcPath);
                var dataType = con.srcPath[con.srcPath.Count - 1].GetSelectedType();

                var destObject = GetData(con.dstSelector.SelectedData, con.dstPath.GetRange(0, con.dstPath.Count - 1));
                var lastSelector = con.dstPath[con.dstPath.Count - 1];
                var requiredType = lastSelector.GetSelectedType();
                if (requiredType == dataType)
                {
                    lastSelector.SetValue(destObject, data);
                    Debug.Log($"{nameof(DataBinder)}: data {data} ---> {destObject}");
                }
                else
                {
                    Debug.LogError($"{nameof(DataBinder)} data mismatch {dataType.Name}><{requiredType.Name}");
                }
            }
        }

        private object GetData(object obj, List<PathSelector> path)
        {
            var data = obj;
            foreach (var s in path)
            {
                if (s.IsProperty())
                {
                    data = s.GetPropertyInfo().GetValue(data);
                }
                else
                {
                    data = s.GetFieldInfo().GetValue(data);
                }
            }

            return data;
        }

        [ContextMenu("Test")]
        public void CreateSrcTypeOptions() => srcSelector.CreateSrcTypeOptions();
    }

    [CustomEditor(typeof(DataBinder))]
    [CanEditMultipleObjects]
    public class DataBinderEditor : Editor
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
            ObjectSelector.DrawObjectSelector(DataBinder.srcSelector);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            DrawConnectors(DataBinder.connectors, DataBinder.srcSelector.SelectedType);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Bind",GUILayout.Width(80f)))
            {
                DataBinder.Bind();
            }

            // GUILayout.FlexibleSpace();
            
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
                PathSelector.DrawPath(con.srcPath, defaultType);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                PathSelector.DrawPath(con.dstPath, con.dstSelector.SelectedType);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                ObjectSelector.DrawObjectSelector(con.dstSelector);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", MiniButtonWidth))
                {
                    connectors.Remove(con);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
        }
    }
}