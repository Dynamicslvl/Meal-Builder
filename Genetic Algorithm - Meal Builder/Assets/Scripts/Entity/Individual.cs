using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Individual
{
    public List<Vector2Int> chromosome = new();

    [HorizontalGroup("1")]
    public int rank;
    [HorizontalGroup("1")]
    public float distance;

    public Nutrients fitness = new();

    public bool IsDominate(Individual other)
    {
        for (int i = 1; i <= GeneticAlgorithm.Instance.numberOfObjectives; i++)
        {
            if (GetFitness(i) > other.GetFitness(i))
                return false;
        }
        return true;
    }

    public float GetFitness(int fitnessIndex)
    {
        return fitness.GetNutrient(fitnessIndex);
    }

    [Button]
    public void PrintMeals()
    {
        MealArranger.ReorderChromosome(chromosome);
    }
}