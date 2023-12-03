using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Dasis.Utility
{
    public static class FastMath
    {
        public static System.Random random = new System.Random();

        public static int Sign(int x)
        {
            if (x < 0)
                return -1;
            if (x > 0)
                return 1;
            return 0;
        }

        public static int Max(int x, int y)
        {
            return (x > y) ? x : y;
        }

        public static float Max(float x, float y)
        {
            return (x > y) ? x : y;
        }

        public static int Min(int x, int y)
        {
            return (x < y) ? x : y;
        }

        public static float Min(float x, float y)
        {
            return (x < y) ? x : y;
        }

        public static int Abs(int x)
        {
            if (x >= 0) return x;
            return -x;
        }

        public static float Abs(float x)
        {
            if (x >= 0) return x;
            return -x;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > value) return min;
            if (max < value) return max;
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (min > value) return min;
            if (max < value) return max;
            return value;
        }

        public static bool IsEqual(List<Vector2Int> list1, List<Vector2Int> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i]) return false;
            }
            return true;
        }

        public static void Swap(ref int element1, ref int element2)
        {
            int tmp = element1;
            element1 = element2;
            element2 = tmp;
        }

        public static void Swap(ref Vector2Int vec1, ref Vector2Int vec2)
        {
            Vector2Int tmp = vec1;
            vec1 = vec2;
            vec2 = tmp;
        }

        public static List<int> GetRandomizedOrderList(int startIndex, int endIndex)
        {
            List<int> list = new List<int>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                list.Add(i);
            }
            return Shuffle(list);
        }
             
        public static bool GachaSucceeded(float probability)
        {
            if (probability == 0) return false;
            return UnityEngine.Random.Range(0, 1f) <= probability;
        }

        public static List<int> Shuffle(List<int> list)
        {
            return list.OrderBy(val => val = random.Next()).ToList();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }

        public static List<int> ToXIntList(List<Vector2Int> list)
        {
            List<int> xIntList = new List<int>();
            foreach (var ele in list)
            {
                xIntList.Add(ele.x);
            }
            return xIntList;
        }

        public static List<int> ShuffleWithProbability(List<int> list, float probability)
        {
            int swapCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                List<int> shuffleIds = GetRandomizedOrderList(i + 1, list.Count - 1);
                foreach (int j in shuffleIds)
                {
                    if (!GachaSucceeded(probability / shuffleIds.Count)) continue;
                    swapCount++;
                    int tmp = list[i];
                    list[i] = list[j];
                    list[j] = tmp;
                }

            }
            return list;
        }

        public static float CosineSimilarity(Vector2 vec1, Vector2 vec2)
        {
            return (vec1.x * vec2.x + vec1.y * vec2.y)
                / Mathf.Sqrt((vec1.x * vec1.x + vec1.y * vec1.y) * (vec2.x * vec2.x + vec2.y * vec2.y));
        }
    }
}
