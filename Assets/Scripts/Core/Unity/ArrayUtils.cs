using System;
using UnityEngine;

namespace Core.Unity
{
    public static class ArrayUtils
    {
        public static void Add<T>(ref T[] array, T newItem)
        {
            var n = array?.Length ?? 0;
            var newArray = new T[n + 1];
            for (int i = 0; i < n; i++)
            {
                newArray[i] = array[i];
            }

            newArray[n] = newItem;
            array = newArray;
        }

        public static void Remove<T>(ref T[] array,T item)
        {
            var newArray = new T[array.Length - 1];
            int skip = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                {
                    //skipp
                    skip++;
                }
                else
                {
                    newArray[i - skip] = array[i];
                }
            }

            array = newArray;
        }
    }
}