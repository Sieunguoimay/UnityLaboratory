using System;
using System.Collections.Generic;
using System.Linq;
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
    //
    // [CustomPropertyDrawer(typeof(ObjectSelector))]
    // public class ObjectSelectorDrawer : PropertyDrawer
    // {
    //     private ObjectSelector objectSelector;
    //
    //     public ObjectSelector GetSelector(SerializedProperty property)
    //     {
    //         if (objectSelector == null)
    //         {
    //             var baseObject = property.serializedObject.targetObject;
    //             var baseObjectType = baseObject.GetType();
    //             var objectSelectorFieldInfo = baseObjectType.GetFields().FirstOrDefault(f => f.FieldType == typeof(ObjectSelector));
    //             if (objectSelectorFieldInfo != null)
    //             {
    //                 objectSelector = objectSelectorFieldInfo.GetValue(baseObject) as ObjectSelector;
    //             }
    //         }
    //
    //         return objectSelector;
    //     }
    //
    //
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         EditorGUI.BeginProperty(position, label, property);
    //         var sourceProp = property.FindPropertyRelative("source");
    //         EditorGUI.BeginChangeCheck();
    //         position.width = 100;
    //         EditorGUI.PropertyField(position, sourceProp);
    //         if (EditorGUI.EndChangeCheck())
    //         {
    //             InvokeMethod(GetSelector(property));
    //         }
    //
    //         position.x += 100;
    //         var SrcTypesOptionsProp = property.FindPropertyRelative("SrcTypesOptions");
    //
    //         EditorGUI.PropertyField(position,SrcTypesOptionsProp,)
    //         EditorGUI.EndProperty();
    //         // var src = EditorGUILayout.ObjectField(GUIContent.none, Selector.source, typeof(UnityEngine.Object), true,
    //         //     GUILayout.Width(150));
    //         //
    //         // if (Selector.source == null || Selector.source != src || Selector.SrcTypesOptions == null)
    //         // {
    //         //     Selector.source = src;
    //         //     Selector.CreateSrcTypeOptionsFromInspector();
    //         // }
    //         //
    //         // if (Selector.SrcTypesOptions == null) return;
    //         // var index = EditorGUILayout.Popup(Selector.selectedTypeIndex, Selector.SrcTypesOptions, GUILayout.Width(50));
    //         // if (Selector.selectedTypeIndex != index)
    //         // {
    //         //     Selector.selectedTypeIndex = index;
    //         //     Selector.UpdateSerializable(index);
    //         // }
    //     }
    //
    //     private void InvokeMethod(ObjectSelector selector)
    //     {
    //         var methInfo = typeof(ObjectSelector).GetMethod("CreateSrcTypeOptionsFromInspector");
    //         methInfo?.Invoke(selector, null);
    //     }
    // }
}