using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
    public string gender;
    public int height;
    public int weight;

    [Button]
    public void Run()
    {
        var result = FuzzyCalculate(gender, height, weight);
        PrintResult(result);
    }

    private static void PrintResult(Dictionary<string, double> result)
    {
        foreach (var kvp in result)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }

    private static Dictionary<string, double> FuzzyCalculate(string gender, int height, int weight)
    {
        var genderMembership = new Dictionary<string, double> { { "male", 1 }, { "female", 0 } };
        var heightMembership = new Dictionary<string, double> { { "shortHeight", 0 }, { "mediumHeight", 0.0 }, { "tallHeight", 0.0 } };
        var weightMembership = new Dictionary<string, double> { { "lowWeight", 0 }, { "mediumWeight", 0.0 }, { "highWeight", 0.0 } };

        // Mờ hoá
        if (height < 110)
            heightMembership["shortHeight"] = 1;
        else if (110 <= height && height < 130)
        {
            heightMembership["mediumHeight"] = (height - 110) / 20.0;
            heightMembership["tallHeight"] = (130 - height) / 20.0;
        }
        else
            heightMembership["tallHeight"] = 1;

        if (weight < 15)
            weightMembership["lowWeight"] = 1;
        else if (15 <= weight && weight < 25)
        {
            weightMembership["mediumWeight"] = (weight - 15) / 10.0;
            weightMembership["highWeight"] = (25 - weight) / 10.0;
        }
        else
            weightMembership["highWeight"] = 1;

        // Suy diễn mờ
        double fatMembership = Math.Max(weightMembership["highWeight"], genderMembership[gender]);
        double sugarMembership = Math.Max(heightMembership["tallHeight"], genderMembership[gender]);
        double proteinMembership = Math.Min(heightMembership["mediumHeight"], weightMembership["mediumWeight"]);
        double fiberMembership = Math.Min(weightMembership["mediumWeight"], genderMembership[gender]);

        // Giải mờ
        var result = new Dictionary<string, double>
        {
            { "fat", 20 + 10 * fatMembership },
            { "sugar", 30 + 10 * sugarMembership },
            { "protein", 15 + 5 * proteinMembership },
            { "fiber", 8 + 2 * fiberMembership },
            { "calorie", (15 + 5 * proteinMembership) * 4 + (30 + 10 * sugarMembership) * 4 + (20 + 10 * fatMembership) * 9 }
        };

        return result;
    }
}