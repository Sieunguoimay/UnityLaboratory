using System;
using UnityEditor;
using UnityEngine;

namespace Core.Unity
{
    [Serializable]
    public class DataConverter
    {
        [SerializeField] public TargetType targetType;
        [SerializeField] public string format = "{0}";

        public object Convert(object data)
        {
            if (targetType == TargetType.None) return data;
            if (targetType == TargetType.String)
            {
                if (format.Length > 0)
                {
                    return String.Format(format, data);
                }
            }

            if (targetType == TargetType.NotBool)
            {
                if (data is bool b)
                {
                    return !b;
                }

                if (data is int i)
                {
                    return i > 0;
                }
            }


            var result = System.Convert.ChangeType(data, GetTargetType());
            if (result != null)
            {
                return data;
            }
            else
            {
                Debug.LogError($"{nameof(DataConverter)} {data} is inconvertible to {GetTargetType().Name}");
                return null;
            }
        }

        public Type GetTargetType(Type defaultType = null)
        {
            switch (targetType)
            {
                case TargetType.None: return defaultType;
                case TargetType.NotBool: return typeof(bool);
                case TargetType.Int: return typeof(int);
                case TargetType.Float: return typeof(float);
                case TargetType.String: return typeof(string);
            }

            return null;
        }

        [Serializable]
        public enum TargetType
        {
            None,
            NotBool,
            Int,
            Float,
            String,
        }

#if UNITY_EDITOR
        public void Draw()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField(new GUIContent("Convert"),GUILayout.Width(50));

            targetType = (TargetType) EditorGUILayout.EnumPopup(GUIContent.none, targetType, GUILayout.Width(70));
            
            if (targetType == TargetType.String)
            {
                format = EditorGUILayout.TextField(format, GUILayout.Width(100));
            }
        }
#endif
    }
}