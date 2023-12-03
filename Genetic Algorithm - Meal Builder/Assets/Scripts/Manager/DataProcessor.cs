using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Dasis.Data;
using Dasis.Utility;

public class DataProcessor : MonoBehaviour
{
    [FilePath]
    public string dataFilePath;

    [Space]
    public float proteinThreshold = 0.15f;
    public float cellulozaThreshold = 0.045f;

    [Button]
    public void ProcessData()
    {
        LoadData();
        NormalizeFoods();
        ClassifyFoods();
    }

    public void LoadData()
    {
        if (!Loader.IsFileExist(dataFilePath)) {
            Debug.LogWarning("File not exist!");
            return;
        }

        DataHolder.Foods.dataList = new List<FoodData>();
        string data = Loader.ReadTextFile(dataFilePath);
        string[] line = data.Split("\n");
        for (int i = 0; i < line.Length; i++)
        {
            FoodData food = new();
            string[] properties = line[i].Split(",");
            food.name = properties[0];
            (food.amount, food.unit) = GetAmountAndUnit(properties[1]);
            food.nutrients.calo = float.Parse(properties[2].Replace(" ", ""));
            food.nutrients.lipit = float.Parse(properties[3].Replace(" ", ""));
            food.nutrients.glucid = float.Parse(properties[4].Replace(" ", ""));
            food.nutrients.protein = float.Parse(properties[5].Replace(" ", ""));
            food.nutrients.celluloza = float.Parse(properties[6].Replace(" ", ""));
            DataHolder.Foods.dataList.Add(food);
        }
    }

    private (int, string) GetAmountAndUnit(string data)
    {
        string[] properties = data.Split(" ");
        int amount = int.Parse(properties[1]);
        string unit = string.Empty;
        for (int i = 2; i < properties.Length; i++)
        {
            unit += properties[i];
            if (i < properties.Length - 1)
                unit += " ";
        }
        return (amount, unit);
    }

    public void NormalizeFoods()
    {
        for (int i = 0; i < DataHolder.Foods.dataList.Count; i++)
        {
            FoodData food = DataHolder.Foods.dataList[i];
            food.index = i + 1;
            if (food.amount > 1)
            {
                food.nutrients /= food.amount;
                food.amount = 1;
            }
            food.nutrients.calo = food.nutrients.ExpectedCalo;
        }
        UnityEditor.EditorUtility.SetDirty(DataHolder.Foods);
    }

    public void ClassifyFoods()
    {
        DataHolder.GlucidFoods.dataList = new List<FoodData>();
        DataHolder.ProteinFoods.dataList = new List<FoodData>();
        DataHolder.CellulozaFoods.dataList = new List<FoodData>();
        foreach (var food in DataHolder.Foods.dataList)
        {
            if (food.nutrients.celluloza / food.nutrients.Total >= cellulozaThreshold)
            {
                food.type = FoodData.Type.Celluloza;
                DataHolder.CellulozaFoods.dataList.Add(food);
                continue;
            }
            if (food.nutrients.protein / food.nutrients.Total >= proteinThreshold)
            {
                food.type = FoodData.Type.Protein;
                DataHolder.ProteinFoods.dataList.Add(food);
                continue;
            }
            food.type = FoodData.Type.Glucid;
            DataHolder.GlucidFoods.dataList.Add(food);
        }
        UnityEditor.EditorUtility.SetDirty(DataHolder.GlucidFoods);
        UnityEditor.EditorUtility.SetDirty(DataHolder.ProteinFoods);
        UnityEditor.EditorUtility.SetDirty(DataHolder.CellulozaFoods);
    }
}
