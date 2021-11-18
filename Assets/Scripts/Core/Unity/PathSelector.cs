using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Unity
{
    [Serializable]
    public class PathSelector
    {
        public SerializableTypeWrapper baseTypeWrapper = new SerializableTypeWrapper();

        public SerializableTypeWrapper selectedMemVarTypeWrapper = new SerializableTypeWrapper();
        public string selectedMemVarName;

        public bool isProperty;
        public bool isField;
        public bool isToRead;

        const BindingFlags PropertyFlag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        const BindingFlags FieldFlag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        const BindingFlags MethodFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public object GetValue(object obj)
        {
            if (isProperty)
            {
                return baseTypeWrapper.GetSerializedType().GetProperty(selectedMemVarName, PropertyFlag)?.GetValue(obj);
            }
            else if (isField)
            {
                return baseTypeWrapper.GetSerializedType().GetField(selectedMemVarName, FieldFlag)?.GetValue(obj);
            }
            else
            {
                var methInfo = baseTypeWrapper.GetSerializedType().GetMethods(MethodFlag).FirstOrDefault(m => m.Name.Equals(selectedMemVarName) && m.ReturnType != null);
                if (methInfo != null)
                {
                    return methInfo?.Invoke(obj, null);
                }
                else
                {
                    Debug.LogError($"{nameof(PathSelector)}{selectedMemVarName} method return type is void");
                    return null;
                }
            }
        }

        public void SetValue(object target, object data)
        {
            if (isProperty)
            {
                baseTypeWrapper.GetSerializedType().GetProperty(selectedMemVarName, PropertyFlag)?.SetValue(target, data);
            }
            else if (isField)
            {
                baseTypeWrapper.GetSerializedType().GetField(selectedMemVarName, FieldFlag)?.SetValue(target, data);
            }
            else
            {
                var methInfo = baseTypeWrapper.GetSerializedType().GetMethods(MethodFlag).FirstOrDefault(m => m.Name.Equals(selectedMemVarName) && m.GetParameters().Length == 1);
                if (methInfo != null)
                {
                    methInfo?.Invoke(target, new[] {data});
                }
                else
                {
                    Debug.LogError($"{nameof(PathSelector)}.{selectedMemVarName} no matching method found");
                }
            }
        }


        public Type GetMemVarType(bool toRead = true)
        {
            if (isProperty)
            {
                var propInfo = baseTypeWrapper.GetSerializedType().GetProperty(selectedMemVarName, PropertyFlag);
                return propInfo?.PropertyType;
            }
            else if (isField)
            {
                var fieldInfo = baseTypeWrapper.GetSerializedType().GetField(selectedMemVarName, FieldFlag);
                return fieldInfo?.FieldType;
            }
            else
            {
                if (toRead)
                {
                    var methInfo = baseTypeWrapper.GetSerializedType().GetMethods(MethodFlag).FirstOrDefault(m => m.Name.Equals(selectedMemVarName) && m.ReturnType != null);
                    return methInfo?.ReturnType;
                }
                else
                {
                    var methInfo = baseTypeWrapper.GetSerializedType().GetMethods(MethodFlag).FirstOrDefault(m => m.Name.Equals(selectedMemVarName) && m.GetParameters().Length == 1);
                    if (methInfo != null)
                        return methInfo.GetParameters()[0].ParameterType;
                    return null;
                }
            }
        }
#if UNITY_EDITOR
        private string[] _options = new string[0];
        private PropertyInfo[] _propertyInfos;
        private FieldInfo[] _fieldInfos;
        private MethodInfo[] _methodInfos;
        private int _selectedIndex = -1;

        public void Reset()
        {
            _selectedIndex = -1;

            _options = new string[0];
            _propertyInfos = null;
            _fieldInfos = null;
            _methodInfos = null;

            selectedMemVarName = null;
            baseTypeWrapper = new SerializableTypeWrapper();
            selectedMemVarTypeWrapper = new SerializableTypeWrapper();
        }

        public void SetBaseType(Type baseType, bool toRead)
        {
            _selectedIndex = -1;

            if (baseType == null) return;

            if (baseTypeWrapper.GetSerializedType() != baseType)
            {
                UpdateSerializedBaseType(baseType.FullName);
                selectedMemVarName = null;
                isToRead = toRead;
            }

            _propertyInfos = baseType.GetProperties(PropertyFlag);
            _fieldInfos = baseType.GetFields(FieldFlag);
            _methodInfos = baseType.GetMethods(MethodFlag);
            if (isToRead)
            {
                _methodInfos = _methodInfos.Where(t => (t.GetParameters().Length == 0) && (t.ReturnType != null)).ToArray();
            }
            else
            {
                _methodInfos = _methodInfos.Where(t => t.GetParameters().Length == 1).ToArray();
            }

            _options = _propertyInfos.Select(p => p.Name).ToArray();
            _options = _options.Concat(_fieldInfos.Select(p => p.Name)).ToArray();
            _options = _options.Concat(_methodInfos.Select(p => p.Name)).ToArray();
        }

        public void UpdateSerializedBaseType(string baseTypeFullName)
        {
            baseTypeWrapper = new SerializableTypeWrapper()
            {
                typeFullName = baseTypeFullName
            };
        }

        public void UpdateSerializedMemVar()
        {
            if (_selectedIndex < 0 || _selectedIndex >= _options.Length) return;
            selectedMemVarName = _options[_selectedIndex];
            isProperty = IsPropertyFromInspector();
            isField = IsFieldFromInspector();

            selectedMemVarTypeWrapper = new SerializableTypeWrapper()
            {
                typeFullName = GetMemVarType()?.FullName
            };
        }


        private bool IsPropertyFromInspector() => (_selectedIndex < _propertyInfos.Length);
        private bool IsFieldFromInspector() => (_selectedIndex < _fieldInfos.Length) && !IsPropertyFromInspector();

        public bool Draw()
        {
            if (_selectedIndex == -1 && selectedMemVarName != null)
            {
                InitializeWithSerializedData();
            }

            var index = EditorGUILayout.Popup(_selectedIndex, _options, GUILayout.Width(100));

            if (index != _selectedIndex)
            {
                _selectedIndex = index;

                UpdateSerializedMemVar();

                return true;
            }

            return false;
        }

        public static PathSelector CreatePathSelector(PathSelector[] pathContainer, Type defaultType, bool toRead)
        {
            var ps = new PathSelector();
            if (pathContainer != null && pathContainer.Length > 0)
            {
                var basePs = pathContainer[pathContainer.Length - 1];
                ps.SetBaseType(basePs.GetMemVarType(true), toRead);
            }
            else
            {
                ps.SetBaseType(defaultType, toRead);
            }

            return ps;
        }

        public static void DrawPath(PathSelector[] pathContainer, Type defaultType, bool toRead)
        {
            if (pathContainer == null) return;
            if (pathContainer.Length > 0)
            {
                if (defaultType != pathContainer[0].baseTypeWrapper.GetSerializedType())
                {
                    pathContainer[0].SetBaseType(defaultType, toRead);
                    for (var j = 1; j < pathContainer.Length; j++)
                    {
                        pathContainer[j].Reset();
                    }
                }
            }

            for (int i = 0; i < pathContainer.Length; i++)
            {
                var path = pathContainer[i];

                if (path?.Draw() ?? false)
                {
                    for (var j = i + 1; j < pathContainer.Length; j++)
                    {
                        if (j == i + 1)
                            pathContainer[j].SetBaseType(pathContainer[j - 1].selectedMemVarTypeWrapper.GetSerializedType(), toRead);
                        else
                            pathContainer[j].Reset();
                    }
                }
            }
        }

        private void InitializeWithSerializedData()
        {
            SetBaseType(baseTypeWrapper.GetSerializedType(), isToRead);
            if (_options == null || _options.Length == 0) return;
            _selectedIndex = _options.ToList().IndexOf(selectedMemVarName);
        }

#endif
    }
}