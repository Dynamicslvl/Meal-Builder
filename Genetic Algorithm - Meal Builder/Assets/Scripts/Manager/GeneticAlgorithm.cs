using Cysharp.Threading.Tasks;
using Dasis.DesignPattern;
using Dasis.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticAlgorithm : Singleton<GeneticAlgorithm>
{
    [GUIColor(0, 1, 0, 1)]
    public Nutrients targetNutrients;

    [GUIColor(1, 1, 0, 1)]
    public Nutrients allowableRange;

    public int numberOfDays;
    public int numberOfPopulations;
    public int numberOfGenerations;
    public int numberOfObjectives;
    public int numberOfTournamentParticipants;
    public int numberOfMonteCarloSamples;
    public int maxFoodCalo;
    public int maxFoodAmount;

    [Range(0, 1)]
    public float mutationProbability;

    public List<Individual> individuals;
    public List<Individual> filteredIndividuals;

    private List<Individual> rank1Individuals;
    private Vector2[] bounds;
    private int generations;

    [Button, PropertyOrder(Order = -1)]
    public void RandomizeTargetNutrients()
    {
        targetNutrients = Nutrients.RandomNutrientsPerDay() * numberOfDays;
    }

    [Button, PropertyOrder(Order = -1)]
    public async void Run()
    {
        SetupBounds();
        InitializePopulations();
        UpdateIndividuals();
        Debug.Log($"Generation {0}: {GetHyperVolume()}");
        for (generations = 1; generations <= numberOfGenerations; generations++)
        {
            Generation();
            Debug.Log($"Generation {generations}: {GetHyperVolume()}");
            await UniTask.Yield();
        }
        FittingIndividuals(true);
    }

    #region Initial
    public void InitializePopulations()
    {
        individuals = new();
        for (int i = 0; i < numberOfPopulations; i++)
        {
            individuals.Add(GetRandomIndividual());
        }
    }

    public Individual GetRandomIndividual()
    {
        Individual individual = new();
        individual.chromosome.AddRange(GetRandomGens(DataHolder.GlucidFoods, numberOfDays));
        individual.chromosome.AddRange(GetRandomGens(DataHolder.ProteinFoods, numberOfDays));
        individual.chromosome.AddRange(GetRandomGens(DataHolder.CellulozaFoods, numberOfDays));
        return individual;
    }

    public List<Vector2Int> GetRandomGens(FoodDataList foodDataList, int length)
    {
        List<Vector2Int> gens = new();
        foreach (var food in foodDataList.dataList)
        {
            int y = Random.Range(1, 1 + FastMath.Min(Mathf.CeilToInt(maxFoodCalo / food.nutrients.calo), maxFoodAmount));
            gens.Add(new Vector2Int(food.index, y));
        }
        gens.Shuffle();
        gens.RemoveRange(length, gens.Count - length);
        return gens;
    }
    #endregion

    #region Generation
    public void Generation()
    {
        List<Individual> offsprings = new();
        while (offsprings.Count < numberOfPopulations)
        {
            Individual father = Selection();
            Individual mother = Selection();
            while (father == mother)
                father = Selection();
            var (offspring1, offspring2) = Crossover(father, mother);
            Mutation(offspring1);
            Mutation(offspring2);
            offsprings.Add(offspring1);
            offsprings.Add(offspring2);
        }
        individuals.AddRange(offsprings);
        UpdateIndividuals();
        individuals = individuals.Take(numberOfPopulations).ToList();
    }

    public void UpdateIndividuals()
    {
        FittingIndividuals();
        RankingIndividuals();
        SortingIndividuals();
    }

    #region Calculate Fitness
    public void FittingIndividuals(bool keepSign = false)
    {
        foreach (var individual in individuals)
        {
            individual.fitness = CalculateFitness(individual, targetNutrients, keepSign);
        }
    }

    public Nutrients CalculateFitness(Individual individual, Nutrients targetNutrients, bool keepSign)
    {
        Nutrients totalNutrients = new();
        foreach (var gen in individual.chromosome)
        {
            Nutrients nutrients = DataHolder.GetNutrients(gen.x);
            totalNutrients += nutrients * gen.y;
        }
        if (keepSign)
        {
            return totalNutrients - targetNutrients;
        }
        return Nutrients.Abs(targetNutrients - totalNutrients);
    }
    #endregion

    #region Ranking
    public void RankingIndividuals()
    {
        List<Individual> individuals = new();
        individuals.AddRange(this.individuals);

        List<Individual> pareto;
        List<Individual> nonePareto;
        int rank = 1;
        while (individuals.Count > 0)
        {
            (pareto, nonePareto) = GetParetoFrontline(individuals);
            if (rank == 1)
                rank1Individuals = pareto;
            CalculateCrowdingDistance(pareto);
            foreach (var p in pareto)
            {
                p.rank = rank;
            }
            individuals.Clear();
            individuals.AddRange(nonePareto);
            rank++;
            if (rank > 100) break;
        }
    }

    public (List<Individual>, List<Individual>) GetParetoFrontline(List<Individual> individuals)
    {
        bool[] isNonePareto = new bool[individuals.Count];
        for (int i = 0; i < individuals.Count; i++)
        {
            if (isNonePareto[i]) continue;
            for (int j = 0; j < individuals.Count; j++)
            {
                if (i == j || isNonePareto[j]) continue;
                if (individuals[i].IsDominate(individuals[j]))
                {
                    isNonePareto[j] = true;
                }
            }
        }
        List<Individual> pareto = new();
        List<Individual> nonePareto = new();
        for (int i = 0; i < individuals.Count; i++)
        {
            if (isNonePareto[i])
            {
                nonePareto.Add(individuals[i]);
                continue;
            }
            pareto.Add(individuals[i]);
        }
        return (pareto, nonePareto);
    }

    public void CalculateCrowdingDistance(List<Individual> individuals)
    {
        foreach (var individual in individuals)
        {
            individual.distance = 0;
        }
        for (int fitnessIndex = 1; fitnessIndex <= numberOfObjectives; fitnessIndex++)
        {
            CalculateDistance(individuals, fitnessIndex);
        }
    }

    private void CalculateDistance(List<Individual> individuals, int fitnessIndex)
    {
        individuals.Sort((i1, i2) =>
        {
            return i1.GetFitness(fitnessIndex).CompareTo(i2.GetFitness(fitnessIndex));
        });
        individuals[0].distance = float.PositiveInfinity;
        individuals[^1].distance = float.PositiveInfinity;
        float maxDistance = individuals[^1].GetFitness(fitnessIndex) - individuals[0].GetFitness(fitnessIndex);
        for (int i = 1; i < individuals.Count - 1; i++)
        {
            if (individuals[i].distance == float.PositiveInfinity)
                continue;
            individuals[i].distance 
                += (individuals[i + 1].GetFitness(fitnessIndex) - individuals[i - 1].GetFitness(fitnessIndex)) / maxDistance;
        }
    }
    #endregion

    #region Sorting
    public void SortingIndividuals()
    {
        individuals.Sort((i1, i2) =>
        {
            if (i1.rank != i2.rank)
                return i1.rank.CompareTo(i2.rank);
            return i2.distance.CompareTo(i1.distance);
        });
    }
    #endregion

    #region Selection
    public Individual Selection()
    {
        List<int> orderIndexs = FastMath.GetRandomizedOrderList(0, numberOfPopulations);
        orderIndexs.RemoveRange(numberOfTournamentParticipants, numberOfPopulations - numberOfTournamentParticipants);
        return individuals[orderIndexs.Min()];
    }
    #endregion

    #region Crossover
    public (Individual, Individual) Crossover(Individual father, Individual mother)
    {
        Individual offspring1 = CrossoverOneSide(father, mother);
        Individual offspring2 = CrossoverOneSide(mother, father);
        return (offspring1, offspring2);
    }

    public Individual CrossoverOneSide(Individual individualA, Individual individualB)
    {
        Individual offspring = new();
        offspring.chromosome.AddRange(individualA.chromosome);

        int crossoverPoint = Random.Range(1, individualA.chromosome.Count);
        HashSet<int> firstPart = FastMath.ToXIntList(offspring.chromosome.Take(crossoverPoint).ToList()).ToHashSet();

        int replaceIndex = individualB.chromosome.Count - 1;
        for (int copyIndex = replaceIndex; copyIndex >= 0; copyIndex--)
        {
            if (replaceIndex == crossoverPoint - 1) break;
            if (firstPart.Contains(individualB.chromosome[copyIndex].x))
                continue;
            offspring.chromosome[replaceIndex--] = individualB.chromosome[copyIndex];
        }
        return offspring;
    }
    #endregion

    #region Mutation
    public void Mutation(Individual individual)
    {
        if (!FastMath.GachaSucceeded(mutationProbability * (1 - generations * 1f / numberOfGenerations))) 
            return;
        int mutatedPoint = Random.Range(0, individual.chromosome.Count);
        individual.chromosome[mutatedPoint] = GetMutatedGen(mutatedPoint, individual.chromosome);
    }

    public Vector2Int GetMutatedGen(int position, List<Vector2Int> chromosome)
    {
        if (FastMath.GachaSucceeded(0.5f))
        {
            return MutatedAmount(chromosome[position]);
        }
        return MutatedIndex(position, chromosome);
    }

    public Vector2Int MutatedAmount(Vector2Int gen)
    {
        float calo = DataHolder.GetNutrients(gen.x).calo;
        List<int> multipliers = new();
        int multiplier = 1;
        while (calo * multiplier < maxFoodCalo && multiplier <= maxFoodAmount)
        {
            if (multiplier == gen.y)
            {
                multiplier++;
                continue;
            }
            multipliers.Add(multiplier);
            multiplier++;
        }
        if (multipliers.Count == 0)
        {
            return gen;
        }
        return new Vector2Int(gen.x, multipliers[Random.Range(0, multipliers.Count)]);
    }

    public Vector2Int MutatedIndex(int position, List<Vector2Int> chromosome)
    {
        int range = position / numberOfDays;
        HashSet<int> existed = FastMath.ToXIntList(chromosome.GetRange(range * numberOfDays, numberOfDays)).ToHashSet();
        List<FoodData> foods = DataHolder.GetFoods((FoodType)range).dataList;

        List<Vector2Int> noneExists = new();
        foreach (var food in foods)
        {
            Vector2Int gen = new(food.index, chromosome[position].y);
            if (existed.Contains(food.index)) continue;
            noneExists.Add(gen);
        }
        return noneExists[Random.Range(0, noneExists.Count)];
    }
    #endregion
    #endregion

    #region Convergence
    public void SetupBounds()
    {
        bounds = new Vector2[numberOfObjectives + 1];
        bounds[1].y = 6000;
        for (int i = 2; i <= numberOfObjectives; i++)
        {
            bounds[i].y = 800;
        }
    }

    public float GetHyperVolume()
    {
        if (rank1Individuals == null || rank1Individuals.Count == 0) 
            return float.NaN;

        Individual sample = new();
        sample.fitness = new Nutrients();
        float noneDominated = 0;
        for (int i = 0; i < numberOfMonteCarloSamples; i++)
        {
            for (int j = 1; j <= numberOfObjectives; j++)
            {
                sample.fitness.SetNutrient(j, Random.Range(bounds[j].x, bounds[j].y));
            }
            bool isDominated = false;
            foreach (var individual in rank1Individuals)
            {
                if (individual.IsDominate(sample))
                {
                    isDominated = true;
                    break;
                }
            }
            if (isDominated) continue;
            noneDominated++;
        }
        return noneDominated;
    }
    #endregion

    #region Filter
    [Button, PropertyOrder(Order = -1)]
    public void FilterIndividuals()
    {
        filteredIndividuals = new List<Individual>();
        foreach (var individual in individuals)
        {
            bool allowed = true;
            for (int i = 1; i <= numberOfObjectives; i++)
            {
                if (FastMath.Abs(individual.GetFitness(i)) > allowableRange.GetNutrient(i))
                {
                    allowed = false;
                    break;
                }
            }
            if (!allowed) continue;
            filteredIndividuals.Add(individual);
        }
        filteredIndividuals.Sort((i1, i2) =>
        {
            return FastMath.Abs(i1.GetFitness(1)).CompareTo(FastMath.Abs(i2.GetFitness(1)));
        });
    }
    #endregion
}
