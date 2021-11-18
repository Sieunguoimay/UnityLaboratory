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
        [SerializeField] public Connector[] connectors;

        [SerializeField] public ObjectSelector srcSelector;

        [Serializable]
        public class Connector
        {
            [SerializeField] public PathSelector[] srcPath;

            [SerializeField] public ObjectSelector dstSelector = new ObjectSelector();

            [SerializeField] public PathSelector[] dstPath;
        }

        public void Bind()
        {
            if (srcSelector.selectedData == null || connectors == null) return;
            foreach (var con in connectors)
            {
                if (con.srcPath == null || con.dstPath == null) continue;

                var srcLast = con.srcPath.Last();
                var data = GetLastData(srcSelector.selectedData, con.srcPath);
                var dataType = srcLast.GetMemVarType(true);

                var lastDstSelector = con.dstPath.Last();
                var l = con.dstPath.ToList();
                var destObject = GetLastData(con.dstSelector.selectedData, l.GetRange(0, l.Count - 1).ToArray());
                var requiredType = lastDstSelector.GetMemVarType(false);

                if (requiredType == null || dataType == null)
                {
                    Debug.Log($"{nameof(DataBinder)}: data types are null");
                    continue;
                }

                if (requiredType == dataType)
                {
                    lastDstSelector.SetValue(destObject, data);
                    Debug.Log($"{nameof(DataBinder)}:{srcLast.baseTypeWrapper.typeFullName}.{srcLast.selectedMemVarName}({data}) ---> {lastDstSelector.baseTypeWrapper.typeFullName}.{lastDstSelector.selectedMemVarName}");
                }
                else
                {
                    Debug.LogError($"{nameof(DataBinder)} data mismatch {dataType.Name}!={requiredType.Name}");
                }
            }
        }

        private object GetLastData(object baseData, PathSelector[] path)
        {
            var currentData = baseData;
            try
            {
                foreach (var ps in path)
                {
                    currentData = ps.GetValue(currentData);
                }
            }
            catch (Exception exception)
            {
                Debug.Log(nameof(DataBinder) + ": GetLastData: " + exception.Message);
            }

            return currentData;
        }

        [ContextMenu("Test")]
        public void CreateSrcTypeOptions() => srcSelector.CreateSrcTypeOptionsFromInspector();
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
            if (DataBinder.connectors != null)
            {
                DrawConnectors(DataBinder, DataBinder.connectors, DataBinder.srcSelector.selectedTypeWrapper.GetSerializedType());
            }

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
                if (GUILayout.Button("x", new[] {MiniButtonWidth}))
                {
                    ArrayUtils.Remove(ref dataBinder.connectors, con);
                    break;
                }

                ObjectSelector.DrawObjectSelector(con.dstSelector);
                // GUILayout.FlexibleSpace();


                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
        }
    }
}