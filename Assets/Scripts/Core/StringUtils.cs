using System;
using System.Text;

namespace Core.Unity
{
    public static class StringUtils
    {
        public static string Spacing(string a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Char.IsUpper(a[i]))
                {
                    a = a.Insert(i, " ");
                    i++;
                }
            }

            return a;
        }

        public static string Capitalize(string a)
        {
            var stringBuilder = new StringBuilder(a);
            if (stringBuilder.Length > 0) stringBuilder[0] = Char.ToUpper(stringBuilder[0]);
            for (int i = 0; i < stringBuilder.Length; i++)
            {
                if (stringBuilder[i] == ' ' && i < stringBuilder.Length - 1)
                {
                    stringBuilder[i + 1] = Char.ToUpper(stringBuilder[i + 1]);
                    i++;
                }
            }

            return stringBuilder.ToString();
        }
    }
}