using Dasis.DesignPattern;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dasis.Utility;

public class FuzzySystem : Singleton<FuzzySystem>
{
    public Gender gender;
    public float height;
    public float weight;
    public bool enableLog;

    public Nutrients nutrients;

    private readonly float[] Weight = new float[3];
    private readonly float[] Height = new float[3];

    private readonly float[] Lipit = new float[5];
    private readonly float[] Glucid = new float[5];
    private readonly float[] Protein = new float[5];
    private readonly float[] Celluloza = new float[5];

    private readonly NutrientLevel[,] LipitRulesets = new NutrientLevel[,]
    {
                             /* Low */              /* Average */                /* High */ 
        /* Low */       {NutrientLevel.Low,         NutrientLevel.PrettyLow,     NutrientLevel.PrettyHigh},
        /* Average */   {NutrientLevel.PrettyLow,   NutrientLevel.Medium,        NutrientLevel.High},
        /* High */      {NutrientLevel.Medium,      NutrientLevel.High,          NutrientLevel.High},
    };

    private readonly NutrientLevel[,] GlucidRulesets = new NutrientLevel[,]
    {
                             /* Low */              /* Average */                /* High */ 
        /* Low */       {NutrientLevel.Low,         NutrientLevel.PrettyLow,     NutrientLevel.PrettyHigh},
        /* Average */   {NutrientLevel.PrettyLow,   NutrientLevel.Medium,        NutrientLevel.PrettyHigh},
        /* High */      {NutrientLevel.PrettyHigh,  NutrientLevel.High,          NutrientLevel.High},
    };

    private readonly NutrientLevel[,] ProteinRulesets = new NutrientLevel[,]
    {
                             /* Low */              /* Average */                /* High */ 
        /* Low */       {NutrientLevel.Low,         NutrientLevel.PrettyLow,     NutrientLevel.PrettyHigh},
        /* Average */   {NutrientLevel.PrettyLow,   NutrientLevel.Medium,        NutrientLevel.PrettyHigh},
        /* High */      {NutrientLevel.Medium,      NutrientLevel.PrettyHigh,    NutrientLevel.High},
    };

    private readonly NutrientLevel[,] CellulozaRulesets = new NutrientLevel[,]
    {
                             /* Low */              /* Average */                /* High */ 
        /* Low */       {NutrientLevel.Low,         NutrientLevel.PrettyLow,     NutrientLevel.PrettyHigh},
        /* Average */   {NutrientLevel.PrettyLow,   NutrientLevel.Medium,        NutrientLevel.PrettyHigh},
        /* High */      {NutrientLevel.PrettyHigh,  NutrientLevel.High,          NutrientLevel.High},
    };

    private float[] W, H, L, G, P, C;

    [Button]
    public void Run()
    {
        Initial();
        Fuzzification();
        Inference();
        DeFuzzification();
        GeneticAlgorithm.Instance.targetNutrients = nutrients * GeneticAlgorithm.Instance.numberOfDays;
    }

    public void Initial()
    {
        W = new float[3];
        H = new float[3];
        L = new float[5];
        G = new float[5];
        P = new float[5];
        C = new float[5];

        float deltaHeight = (gender == Gender.Male) ? 0 : -5;
        Height[(int)MeasureLevel.Low] = 105 + deltaHeight;
        Height[(int)MeasureLevel.Average] = 115 + deltaHeight;
        Height[(int)MeasureLevel.High] = 125 + deltaHeight;

        float deltaWeight = (gender == Gender.Male) ? 0 : -1;
        Weight[(int)MeasureLevel.Low] = 16 + deltaWeight;
        Weight[(int)MeasureLevel.Average] = 20 + deltaWeight;
        Weight[(int)MeasureLevel.High] = 24 + deltaWeight;

        Lipit[(int)NutrientLevel.Low] = 25;
        Lipit[(int)NutrientLevel.PrettyLow] = 30;
        Lipit[(int)NutrientLevel.Medium] = 35;
        Lipit[(int)NutrientLevel.PrettyHigh] = 40;
        Lipit[(int)NutrientLevel.High] = 45;

        Glucid[(int)NutrientLevel.Low] = 180;
        Glucid[(int)NutrientLevel.PrettyLow] = 190;
        Glucid[(int)NutrientLevel.Medium] = 200;
        Glucid[(int)NutrientLevel.PrettyHigh] = 210;
        Glucid[(int)NutrientLevel.High] = 220;

        Protein[(int)NutrientLevel.Low] = 15;
        Protein[(int)NutrientLevel.PrettyLow] = 20;
        Protein[(int)NutrientLevel.Medium] = 25;
        Protein[(int)NutrientLevel.PrettyHigh] = 30;
        Protein[(int)NutrientLevel.High] = 35;

        Celluloza[(int)NutrientLevel.Low] = 5;
        Celluloza[(int)NutrientLevel.PrettyLow] = 8;
        Celluloza[(int)NutrientLevel.Medium] = 11;
        Celluloza[(int)NutrientLevel.PrettyHigh] = 14;
        Celluloza[(int)NutrientLevel.High] = 17;
    }

    #region Fuzzification
    public void Fuzzification()
    {
        if (weight < Weight[(int)MeasureLevel.Low])
            W[(int)MeasureLevel.Low] = 1;
        if (weight > Weight[(int)MeasureLevel.High])
            W[(int)MeasureLevel.High] = 1;
        if (height < Height[(int)MeasureLevel.Low])
            H[(int)MeasureLevel.Low] = 1;
        if (height > Height[(int)MeasureLevel.High])
            H[(int)MeasureLevel.High] = 1;

        for (int i = 0; i < 2; i++)
        {
            W[i] += Fuzzing(weight, Weight[i], Weight[i + 1], false);
            W[i + 1] += Fuzzing(weight, Weight[i], Weight[i + 1], true);
            H[i] += Fuzzing(height, Height[i], Height[i + 1], false);
            H[i + 1] += Fuzzing(height, Height[i], Height[i + 1], true);
        }

        for (int i = 0; i < 3; i++)
        {
            W[i] = FastMath.Min(1, W[i]);
            H[i] = FastMath.Min(1, H[i]);
        }

        LogArray(H, "Height: ");
        LogArray(W, "Weight: ");
    }

    public float Fuzzing(float value, float min, float max, bool isUp)
    {
        if (value < min || value > max) return 0;
        if (isUp)
            return (value - min) / (max - min);
        return (max - value) / (max - min);
    }
    #endregion

    public void Inference()
    {
        for (int w = 0; w < 3; w++)
        {
            for (int h = 0; h < 3; h++)
            {
                L[(int)LipitRulesets[w, h]] = FastMath.Max(L[(int)LipitRulesets[w, h]], FastMath.Min(W[w], H[h]));
                G[(int)GlucidRulesets[w, h]] = FastMath.Max(G[(int)GlucidRulesets[w, h]], FastMath.Min(W[w], H[h]));
                P[(int)ProteinRulesets[w, h]] = FastMath.Max(P[(int)ProteinRulesets[w, h]], FastMath.Min(W[w], H[h]));
                C[(int)CellulozaRulesets[w, h]] = FastMath.Max(C[(int)CellulozaRulesets[w, h]], FastMath.Min(W[w], H[h]));
            }
        }

        LogArray(L, "Lipit: ");
        LogArray(G, "Glucid: ");
        LogArray(P, "Protein: ");
        LogArray(C, "Celluloza: ");
    }

    public void DeFuzzification()
    {
        nutrients = new Nutrients();

        // Lipit
        float sum = 0;
        for (int i = 0; i < 5; i++)
        {
            sum += L[i];
            nutrients.lipit += L[i] * Lipit[i];
        }
        nutrients.lipit /= sum;
        
        // Glucid
        sum = 0;
        for (int i = 0; i < 5; i++)
        {
            sum += G[i];
            nutrients.glucid += G[i] * Glucid[i];
        }
        nutrients.glucid /= sum;

        // Protein
        sum = 0;
        for (int i = 0; i < 5; i++)
        {
            sum += P[i];
            nutrients.protein += P[i] * Protein[i];
        }
        nutrients.protein /= sum;

        // Celluloza
        sum = 0;
        for (int i = 0; i < 5; i++)
        {
            sum += C[i];
            nutrients.celluloza += C[i] * Celluloza[i];
        }
        nutrients.celluloza /= sum;

        nutrients.calo = nutrients.protein * 4 + nutrients.glucid * 4 + nutrients.lipit * 9;
    }

    private void LogArray<T>(T[] array, string prefix = "")
    {
        if (!enableLog) return;
        string log = string.Empty;
        for (int i = 0; i < array.Length; i++)
        {
            log += $"{array[i]}";
            if (i == array.Length - 1) continue;
            log += ", ";
        }
        Debug.Log(prefix + log);
    }
}

public enum Gender
{
    Male,
    Female,
}

public enum MeasureLevel
{
    Low,
    Average,
    High,
}

public enum NutrientLevel
{
    Low,
    PrettyLow,
    Medium,
    PrettyHigh,
    High,
}