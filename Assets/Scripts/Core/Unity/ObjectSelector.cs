using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Unity
{
    [Serializable]
    public class ObjectSelector
    {
        public UnityEngine.Object source;
        public object SelectedData => SelectedTypeIndex < 0 ? null : _srcData[SelectedTypeIndex];
        public Type SelectedType => SelectedTypeIndex < 0 ? null : _srcTypes[SelectedTypeIndex];
        private readonly List<Type> _srcTypes = new List<Type>();
        private readonly List<object> _srcData = new List<object>();
        [NonSerialized] public int SelectedTypeIndex = -1;
        [NonSerialized] public string[] SrcTypesOptions;

        public void CreateSrcTypeOptions()
        {
            if (source == null) return;

            _srcTypes.Clear();
            SelectedTypeIndex = -1;

            if (source is GameObject src)
            {
                var srcTypeNames = new List<string>();
                var components = src.GetComponents<Component>();
                foreach (var c in components)
                {
                    _srcTypes.Add(c.GetType());
                    _srcData.Add(c);
                    srcTypeNames.Add(c.GetType().Name);
                    Debug.Log(c.GetType().Name);
                }

                SrcTypesOptions = srcTypeNames.ToArray();
            }
            else
            {
                _srcTypes.Add(source.GetType());
                _srcData.Add(source);
                SrcTypesOptions = new string[1];
                SrcTypesOptions[0] = source.GetType().Name;
            }

            if (SrcTypesOptions.Length > 0)
            {
                SelectedTypeIndex = 0;
            }
        }
#if UNITY_EDITOR

        public static void DrawObjectSelector(ObjectSelector selector)
        {
            var src = EditorGUILayout.ObjectField(GUIContent.none, selector.source, typeof(UnityEngine.Object), true,
                GUILayout.Width(150));

            if (selector.source == null || selector.source != src || selector.SrcTypesOptions == null)
            {
                selector.source = src;
                selector.CreateSrcTypeOptions();
            }

            if (selector.SrcTypesOptions == null) return;
            selector.SelectedTypeIndex = EditorGUILayout.Popup(selector.SelectedTypeIndex,
                selector.SrcTypesOptions, GUILayout.Width(50));
        }
#endif
    }
}