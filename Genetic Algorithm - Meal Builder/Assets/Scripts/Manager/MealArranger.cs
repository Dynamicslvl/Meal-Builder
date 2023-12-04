using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dasis.Utility;

public static class MealArranger
{
    private static List<Vector2Int> dishes;
    private static List<int> orders;
    private static readonly List<int> bestOrders = new();
    private static bool[] visited;
    private static float minCaloDifference;
    private static float averageCalo;
    private static int count = 0;

    public static int NumberOfMeals => GeneticAlgorithm.Instance.numberOfDays;

    public static void ReorderChromosome(List<Vector2Int> chromosome)
    {
        dishes = new List<Vector2Int>();
        dishes.AddRange(chromosome);
        averageCalo = GetAverageCalo(dishes);
        orders = new List<int>();
        visited = new bool[3 * NumberOfMeals];
        minCaloDifference = float.PositiveInfinity;
        count = 0;
        FindBestOrders();
        PrintOrders(bestOrders);
    }

    private static float GetAverageCalo(List<Vector2Int> dishes)
    {
        float totalCalo = 0;
        foreach (var dish in dishes)
        {
            totalCalo += DataHolder.GetNutrients(dish.x).calo * dish.y;
        }
        return totalCalo / NumberOfMeals;
    }

    private static float GetCaloDifference(List<int> orders)
    {
        float difference = 0;
        for (int i = 0; i < NumberOfMeals; i++)
        {
            float totalCalo = 0;
            for (int j = 0; j < 3; j++)
            {
                Vector2Int dish = dishes[orders[j * NumberOfMeals + i]];
                totalCalo += DataHolder.GetNutrients(dish.x).calo * dish.y;
            }
            difference += FastMath.Abs(totalCalo - averageCalo);
        }
        return difference;
    }

    private static void PrintOrders(List<int> orders)
    {
        string result = string.Empty;
        for (int i = 0; i < NumberOfMeals; i++)
        {
            result += $"Ngày {i + 1}: ";
            for (int j = 0; j < 3; j++)
            {
                FoodData food = DataHolder.GetFood(dishes[orders[j * NumberOfMeals + i]].x);
                result += $"{food.name} ({dishes[orders[j * NumberOfMeals + i]].y} {food.unit}), ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    private static void FindBestOrders()
    {
        if (orders.Count == dishes.Count)
        {
            count++;
            float caloDifference = GetCaloDifference(orders);
            if (caloDifference < minCaloDifference)
            {
                minCaloDifference = caloDifference;
                bestOrders.Clear();
                bestOrders.AddRange(orders);
            }
            return;
        }

        int range = orders.Count / NumberOfMeals;
        for (int i = range * NumberOfMeals; i < (range + 1) * NumberOfMeals; i++)
        {
            if (visited[i]) continue;
            visited[i] = true;
            orders.Add(i);
            FindBestOrders();
            orders.RemoveAt(orders.Count - 1);
            visited[i] = false;
        }
    }
}
