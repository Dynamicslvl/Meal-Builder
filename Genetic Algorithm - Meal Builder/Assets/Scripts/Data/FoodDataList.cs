using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Dasis.Utility;

[CreateAssetMenu(fileName ="FoodDataList", menuName ="Data/Food Data List")]
public class FoodDataList : ScriptableObject
{
    [Searchable]
    public List<FoodData> dataList;
}

[System.Serializable]
public class FoodData
{
    public enum Type
    {
        Glucid,
        Protein,
        Celluloza,
    }

    public int index;
    public Type type;

    [HorizontalGroup("0")]
    public string name;

    [HorizontalGroup("1")]
    public int amount;
    [HorizontalGroup("1")]
    public string unit;

    public Nutrients nutrients = new();
}

[System.Serializable]
public class Nutrients
{
    public float calo;
    public float lipit;
    public float glucid;
    public float protein;
    public float celluloza;

    public float Total => lipit + glucid + protein + celluloza;
    public float ExpectedCalo => lipit * 9 + protein * 4 + glucid * 4;

    public void SetNutrient(int nutrientIndex, float value)
    {
        switch (nutrientIndex)
        {
            case 1:
                calo = value;
                break;
            case 2:
                lipit = value;
                break;
            case 3:
                glucid = value;
                break;
            case 4:
                protein = value;
                break;
            case 5:
                celluloza = value;
                break;
        }
    }

    public float GetNutrient(int nutrientIndex)
    {
        return nutrientIndex switch
        {
            2 => lipit,
            3 => glucid,
            4 => protein,
            5 => celluloza,
            _ => calo,
        };
    }

    public static Nutrients RandomNutrientsPerDay()
    {
        Nutrients nutrients = new();
        nutrients.lipit = Random.Range(30, 50);
        nutrients.glucid = Random.Range(190, 210);
        nutrients.protein = Random.Range(20, 30);
        nutrients.calo = nutrients.lipit * 9 + nutrients.glucid * 4 + nutrients.protein * 4;
        nutrients.celluloza = Random.Range(10, 13);
        return nutrients;
    }

    public static Nutrients operator +(Nutrients me, Nutrients other)
    {
        Nutrients nutrients = new();
        nutrients.calo = me.calo + other.calo;
        nutrients.lipit = me.lipit + other.lipit;
        nutrients.glucid = me.glucid + other.glucid;
        nutrients.protein = me.protein + other.protein;
        nutrients.celluloza = me.celluloza + other.celluloza;
        return nutrients;
    }

    public static Nutrients operator -(Nutrients me, Nutrients other)
    {
        Nutrients nutrients = new();
        nutrients.calo = me.calo - other.calo;
        nutrients.lipit = me.lipit - other.lipit;
        nutrients.glucid = me.glucid - other.glucid;
        nutrients.protein = me.protein - other.protein;
        nutrients.celluloza = me.celluloza - other.celluloza;
        return nutrients;
    }

    public static Nutrients operator *(Nutrients me, float multiplier)
    {
        Nutrients nutrients = new();
        nutrients.calo = me.calo * multiplier;
        nutrients.lipit = me.lipit * multiplier;
        nutrients.glucid = me.glucid * multiplier;
        nutrients.protein = me.protein * multiplier;
        nutrients.celluloza = me.celluloza * multiplier;
        return nutrients;
    }

    public static Nutrients operator /(Nutrients me, float divider)
    {
        Nutrients nutrients = new();
        nutrients.calo = me.calo / divider;
        nutrients.lipit = me.lipit / divider;
        nutrients.glucid = me.glucid / divider;
        nutrients.protein = me.protein / divider;
        nutrients.celluloza = me.celluloza / divider;
        return nutrients;
    }

    public static Nutrients Abs(Nutrients me)
    {
        Nutrients nutrients = new();
        nutrients.calo = FastMath.Abs(me.calo);
        nutrients.lipit = FastMath.Abs(me.lipit);
        nutrients.glucid = FastMath.Abs(me.glucid);
        nutrients.protein = FastMath.Abs(me.protein);
        nutrients.celluloza = FastMath.Abs(me.celluloza);
        return nutrients;
    }

    public override string ToString()
    {
        return $"(Calo: {calo}, Lipit: {lipit}, Glucid: {glucid}, Protein: {protein}, Celluloza: {celluloza})";
    }
}
