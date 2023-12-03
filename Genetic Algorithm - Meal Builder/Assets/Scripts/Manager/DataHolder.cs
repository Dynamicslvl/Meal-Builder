using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dasis.DesignPattern;

public class DataHolder : Singleton<DataHolder>
{
    public FoodDataList foods;
    public FoodDataList glucidFoods;
    public FoodDataList proteinFoods;
    public FoodDataList cellulozaFoods;

    public static FoodDataList Foods
    {
        get { return Instance.foods; }
        set { Instance.foods = value; }
    }

    public static FoodDataList GlucidFoods
    {
        get { return Instance.glucidFoods; }
        set { Instance.glucidFoods = value; }
    }

    public static FoodDataList ProteinFoods
    {
        get { return Instance.proteinFoods; }
        set { Instance.proteinFoods = value; }
    }

    public static FoodDataList CellulozaFoods
    {
        get { return Instance.cellulozaFoods; }
        set { Instance.cellulozaFoods = value; }
    }

    public static Nutrients GetNutrients(int foodIndex)
    {
        return Foods.dataList[foodIndex - 1].nutrients;
    }

    public static FoodData GetFood(int foodIndex)
    {
        return Foods.dataList[foodIndex - 1];
    }

    public static FoodDataList GetFoods(FoodType foodType)
    {
        return foodType switch
        {
            FoodType.Protein => ProteinFoods,
            FoodType.Celluloza => CellulozaFoods,
            _ => GlucidFoods,
        };
    }
}

public enum FoodType
{
    Glucid,
    Protein,
    Celluloza,
}
