using UnityEngine;
using System.Collections.Generic;

namespace Dasis.Utility
{
    public static class Stringify
    {
        public static string ToString<T>(List<T> list)
        {
            string content = string.Empty;
            foreach (var ele in list) content += $"{ele}, ";
            return content;
        }

        public static string ToString(List<GameObject> list)
        {
            string content = string.Empty;
            foreach (var ele in list) content += $"{ele.name}, ";
            return content;
        }

        public static List<string> ToStringList(List<GameObject> list)
        {
            List<string> stringList = new List<string>();
            foreach (var ele in list)
            {
                stringList.Add(ele.name);
            }
            return stringList;
        }
    }
}
