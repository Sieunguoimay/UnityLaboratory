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
        public int selectedIndex = -1;
        public string[] options;
        public string baseTypeFullName;

        private PropertyInfo[] _propertyInfos;
        private FieldInfo[] _fieldInfos;

        public Type GetSelectedType()
        {
            if (selectedIndex == -1) return null;

            if (IsProperty())
            {
                return _propertyInfos[selectedIndex].PropertyType;
            }

            return _fieldInfos[selectedIndex - _propertyInfos.Length].FieldType;
        }

        public void SetValue(object target, object data)
        {
            if (IsProperty())
            {
                GetPropertyInfo().SetValue(target, data);
            }
            else
            {
                GetFieldInfo().SetValue(target, data);
            }
        }

        public bool IsProperty() => (selectedIndex < _propertyInfos.Length);

        public PropertyInfo GetPropertyInfo()
        {
            return IsProperty() ? _propertyInfos[selectedIndex] : null;
        }

        public FieldInfo GetFieldInfo()
        {
            return IsProperty() ? null : _fieldInfos[selectedIndex - _propertyInfos.Length];
        }

        public PathSelector(Type baseType)
        {
            if (baseType == null)
            {
                options = new string[0];
                return;
            }

            baseTypeFullName = baseType.FullName;

            const BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                      BindingFlags.Static;

            _propertyInfos = baseType.GetProperties(flag);
            _fieldInfos = baseType.GetFields(flag);
            options = _propertyInfos.Select(p => p.Name).ToArray();
            options = options.Concat(_fieldInfos.Select(p => p.Name)).ToArray();
        }

#if UNITY_EDITOR
        public static void DrawPath(List<PathSelector> path, Type defaultType)
        {
            if (GUILayout.Button("-", GUILayout.Width(20f)))
            {
                if (path.Count > 0)
                {
                    path.RemoveAt(path.Count - 1);
                    return;
                }
            }

            if (GUILayout.Button("+", GUILayout.Width(20f)))
            {
                var t = defaultType;
                if (path.Count > 0)
                {
                    t = path[path.Count - 1]?.GetSelectedType() ?? null;
                }

                path.Add(new PathSelector(t));
            }

            for (int i = 0; i < path.Count; i++)
            {
                var s = path[i];
                var newIndex = EditorGUILayout.Popup(s.selectedIndex, s.options, GUILayout.Width(100));

                if (i == 0 && !(s.baseTypeFullName?.Equals(defaultType?.FullName ?? s.baseTypeFullName) ??
                                (defaultType?.FullName == null)))
                {
                    s.selectedIndex = -2;
                    newIndex = -1;
                    s = path[i] = new PathSelector(defaultType);

                    if (path.Count > 1)
                    {
                        for (int j = 1; j < path.Count; j++)
                        {
                            path[j] = new PathSelector(null);
                        }
                    }
                }

                if (newIndex != s.selectedIndex)
                {
                    s.selectedIndex = newIndex;

                    if (i < path.Count - 1)
                    {
                        path[i + 1] = new PathSelector(s.GetSelectedType());
                    }

                    if (i < path.Count - 2)
                    {
                        for (int j = i + 2; j < path.Count; j++)
                        {
                            path[j] = new PathSelector(null);
                        }
                    }
                }
            }
        }
#endif
    }
}