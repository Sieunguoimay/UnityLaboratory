using System;
using System.Linq;
using UnityEngine;

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
            [SerializeField] public DataConverter dataConverter = new DataConverter();
            [SerializeField] public PathSelector[] dstPath;
            [SerializeField] public ObjectSelector dstSelector = new ObjectSelector();
        }

        public void Bind()
        {
            if (srcSelector.selectedData == null || connectors == null) return;
            foreach (var con in connectors)
            {
                if (con.srcPath == null || con.dstPath == null || con.srcPath.Length == 0 || con.dstPath.Length == 0) continue;

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

                var newData = con.dataConverter.Convert(data);
                var newDataType = con.dataConverter.GetTargetType(dataType);

                if (requiredType == newDataType)
                {
                    lastDstSelector.SetValue(destObject, newData, newDataType);
                    Debug.Log($"{nameof(DataBinder)}:{srcLast.baseTypeWrapper.typeFullName}.{srcLast.selectedMemVarName}({data}->{newData}) ---> {lastDstSelector.baseTypeWrapper.typeFullName}.{lastDstSelector.selectedMemVarName}");
                }
                else
                {
                    Debug.LogError($"{nameof(DataBinder)} data mismatch {newDataType?.Name}!={requiredType?.Name}");
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
    }
}