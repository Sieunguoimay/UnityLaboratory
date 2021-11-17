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

            [SerializeField] public ObjectSelector dstSelector;

            [SerializeField] public List<PathSelector> dstPath = new List<PathSelector>();
        }

        public void AddConnector()
        {
            connectors.Add(new Connector());
        }

        public void Bind()
        {
            if (srcSelector.selectedData == null) return;

            foreach (var con in connectors)
            {
                // con.Setup();
                //
                // var data = GetLastData(srcSelector.selectedData, con.srcPath);
                // var dataType = con.srcPath.Last().selectedMemVarTypeWrapper.GetSerializedType();
                //
                // var lastDstSelector = con.dstPath.Last();
                // var destObject = GetSecondLastData(con.dstSelector.selectedData, con.dstPath);
                // var requiredType = lastDstSelector.selectedMemVarTypeWrapper.GetSerializedType();
                // if (requiredType == dataType)
                // {
                //     lastDstSelector.SetValue(destObject, data);
                //     Debug.Log($"{nameof(DataBinder)}: data {data} ---> {destObject}");
                // }
                // else
                // {
                //     Debug.LogError($"{nameof(DataBinder)} data mismatch {dataType.Name}><{requiredType.Name}");
                // }
            }
        }

        // private object GetLastData(object obj, PathSelector path)
        // {
        //     var data = obj;
        //     path.Travel(s => data = s.GetValue(data));
        //     return data;
        // }
        //
        // private object GetSecondLastData(object obj, PathSelector path)
        // {
        //     var data = obj;
        //     path.Travel(s =>
        //     {
        //         if (s.Prev != null)
        //         {
        //             data = s.Prev.GetValue(data);
        //         }
        //     });
        //     return data;
        // }

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

            DrawConnectors(DataBinder.connectors, DataBinder.srcSelector.selectedTypeWrapper.GetSerializedType());

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", MiniButtonWidth))
            {
                DataBinder.AddConnector();
            }

            if (GUILayout.Button("Bind", GUILayout.Width(80f)))
            {
                DataBinder.Bind();
            }

            // GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }


        private static void DrawConnectors(List<DataBinder.Connector> connectors, Type defaultType)
        {
            foreach (var con in connectors)
            {
                EditorGUILayout.BeginHorizontal();
                PathSelector.DrawPath(con.srcPath, defaultType, true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                PathSelector.DrawPath(con.dstPath, con.dstSelector.selectedTypeWrapper.GetSerializedType(), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                ObjectSelector.DrawObjectSelector(con.dstSelector);
                // GUILayout.FlexibleSpace();
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