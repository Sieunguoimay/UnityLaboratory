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
        [SerializeField] public UnityEngine.Object source;
        [SerializeField] public object selectedData;

        [SerializeField] public SerializableTypeWrapper selectedTypeWrapper = new SerializableTypeWrapper();

#if UNITY_EDITOR

        private readonly List<Type> _srcTypes = new List<Type>();
        private readonly List<object> _srcData = new List<object>();
        [NonSerialized] public string[] SrcTypesOptions;

        public int selectedTypeIndex = -1;

        public void UpdateSerializable(int index)
        {
            if (index < 0) return;
            selectedData = _srcData[index];

            selectedTypeWrapper = new SerializableTypeWrapper()
            {
                typeFullName = _srcTypes[index].FullName
            };
        }

        public void CreateSrcTypeOptionsFromInspector()
        {
            if (source == null) return;

            _srcTypes.Clear();
            _srcData.Clear();
            
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

            if (SrcTypesOptions.Length > 0 && selectedTypeIndex < 0)
            {
                selectedTypeIndex = 0;
            }

            UpdateSerializable(selectedTypeIndex);
        }

        public static void DrawObjectSelector(ObjectSelector selector)
        {
            var src = EditorGUILayout.ObjectField(GUIContent.none, selector.source, typeof(UnityEngine.Object), true,
                GUILayout.Width(150));

            if (selector.source == null || selector.source != src || selector.SrcTypesOptions == null)
            {
                selector.source = src;
                selector.CreateSrcTypeOptionsFromInspector();
            }

            if (selector.SrcTypesOptions == null) return;
            var index = EditorGUILayout.Popup(selector.selectedTypeIndex, selector.SrcTypesOptions, GUILayout.Width(50));
            if (selector.selectedTypeIndex != index)
            {
                selector.selectedTypeIndex = index;
                selector.UpdateSerializable(index);
            }
        }
#endif
    }
}