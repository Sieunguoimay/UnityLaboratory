using System;

namespace Core.Unity
{
    [Serializable]
    public class SerializableTypeWrapper
    {
        public string typeFullName;

        private Type _type;

        public Type GetSerializedType()
        {
            if (_type == null && typeFullName != null && !typeFullName.Equals(""))
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var a in assemblies)
                {
                    var t = a.GetType(typeFullName);
                    if (t != null)
                    {
                        _type = t;
                        break;
                    }
                }
            }

            return _type;
        }

        public object GetPropertyValue(object data, string name)
        {
            var baseType = GetSerializedType();
            return baseType.GetProperty(name)?.GetValue(data);
        }

        public object GetFieldValue(object data, string name)
        {
            var baseType = GetSerializedType();
            return baseType.GetField(name)?.GetValue(data);
        }
        public void SetPropertyValue(object data, string name, object value)
        {
            var baseType = GetSerializedType();
            baseType.GetProperty(name)?.SetValue(data, value);
        }

        public void SetFieldValue(object data, string name, object value)
        {
            var baseType = GetSerializedType();
            baseType.GetField(name)?.SetValue(data, value);
        }
    }
}