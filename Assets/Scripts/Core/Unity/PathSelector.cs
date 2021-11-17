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

        public object GetValue(object data)
        {
            if (isProperty)
            {
                return baseTypeWrapper.GetPropertyValue(data, selectedMemVarName);
            }
            else if (isField)
            {
                return baseTypeWrapper.GetFieldValue(data, selectedMemVarName);
            }
            else
            {
                return null;
            }
        }

        public void SetValue(object target, object data)
        {
            if (isProperty)
            {
                baseTypeWrapper.SetPropertyValue(target, selectedMemVarName, data);
            }
            else if (isField)
            {
                baseTypeWrapper.SetFieldValue(target, selectedMemVarName, data);
            }
            else
            {
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

            BindingFlags propertyFlag = BindingFlags.Public | BindingFlags.Instance;
            BindingFlags fieldFlag = BindingFlags.Public | BindingFlags.Instance;
            BindingFlags methodFlag = BindingFlags.Public | BindingFlags.Instance;

            if (toRead)
            {
                propertyFlag |= BindingFlags.GetProperty;
                fieldFlag |= BindingFlags.GetField;
                methodFlag |= BindingFlags.DeclaredOnly;
            }
            else
            {
                propertyFlag |= BindingFlags.SetProperty;
                fieldFlag |= BindingFlags.SetField;
                methodFlag |= BindingFlags.DeclaredOnly;
            }

            if (baseTypeWrapper.GetSerializedType() != baseType)
            {
                UpdateSerializedBaseType(baseType.FullName);
                selectedMemVarName = null;
                isToRead = toRead;
            }

            _propertyInfos = baseType.GetProperties(propertyFlag);
            _fieldInfos = baseType.GetFields(fieldFlag);
            _methodInfos = baseType.GetMethods(methodFlag);
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

            selectedMemVarTypeWrapper = new SerializableTypeWrapper()
            {
                typeFullName = GetMemVarTypeFromInspector().FullName
            };
            selectedMemVarName = _options[_selectedIndex];
            isProperty = IsPropertyFromInspector();
            isField = IsFieldFromInspector();
        }

        public Type GetMemVarTypeFromInspector()
        {
            if (_selectedIndex == -1 || _propertyInfos == null || _fieldInfos == null || _methodInfos == null) return null;

            if (IsPropertyFromInspector())
            {
                return _propertyInfos[_selectedIndex].PropertyType;
            }
            else if (IsFieldFromInspector())
            {
                return _fieldInfos[_selectedIndex - _propertyInfos.Length].FieldType;
            }
            else
            {
                return _methodInfos[_selectedIndex - _propertyInfos.Length - _fieldInfos.Length].ReturnType;
            }
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

        public static void DrawPath(List<PathSelector> pathContainer, Type defaultType, bool toRead)
        {
            if (GUILayout.Button("-", GUILayout.Width(20f)))
            {
                if (pathContainer.Count > 0)
                {
                    pathContainer.RemoveAt(pathContainer.Count - 1);
                }
            }

            if (GUILayout.Button("+", GUILayout.Width(20f)))
            {
                var ps = new PathSelector();
                if (pathContainer.Count > 0)
                {
                    ps.SetBaseType(pathContainer[pathContainer.Count - 1].selectedMemVarTypeWrapper.GetSerializedType(), toRead);
                }
                else
                {
                    ps.SetBaseType(defaultType, toRead);
                }

                pathContainer.Add(ps);
            }

            if (pathContainer.Count > 0)
            {
                if (defaultType != pathContainer[0].baseTypeWrapper.GetSerializedType())
                {
                    pathContainer[0].SetBaseType(defaultType, toRead);
                    for (var j = 1; j < pathContainer.Count; j++)
                    {
                        pathContainer[j].Reset();
                    }
                }
            }

            for (int i = 0; i < pathContainer.Count; i++)
            {
                var path = pathContainer[i];

                if (path?.Draw() ?? false)
                {
                    for (var j = i + 1; j < pathContainer.Count; j++)
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