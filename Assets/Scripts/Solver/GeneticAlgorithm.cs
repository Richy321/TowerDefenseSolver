using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public interface IChromosome<T>
    {
        int Length { get; }
        int Fitness { get; }
        void Crossover(int pos, T other);
        void Mutate(int geneIndex);
        T Clone();
    }

    public enum SelectionAlgorithm
    {
        RouletteWheel,
        StocasticUniversalSampling,
        TournamentSelection,
        TruncationSelection
    }

    public class GeneticAlgorithm : GeneticAlgorithm<BuildDecisionsChromosome>
    {
    }


    public class GeneticAlgorithm<T> : MonoBehaviour where T : IChromosome<T>, new()
    {
        /// <summary>
        ///Roulette-wheel selection - normalised decending, random value (0-1), first to accum to that value
        ///Stocastic Universal sampling - multiple equally spaced out pointers
        ///Tournament selection - best individual of a randomly chosen subset
        ///Truncation selection - best % of of the population is kept.
        /// </summary>
        public float crossoverRate = 0.9f;
        public float mutationRate = 0.1f;
        public bool elitism = true;
        public float elitismPercentage = 0.1f;
        public int totalFitness;
        public int generationNo;
        public int highestFitness;
        public List<T> population;
        public SelectionAlgorithm selectionAlgorithm = SelectionAlgorithm.RouletteWheel;
        public Action<List<T>>  onRequestGenerateFitness;

        public void SetInitialPopulation(List<T> initialPopulation )
        {
            population = initialPopulation;
        }

        public void EvolvePopulation()
        {
            List<T> newPopulation = new List<T>();

            //sort fitness high->low
            population.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));

            //total fitness
            totalFitness = 0;
            population.ForEach(v => totalFitness += v.Fitness); //get total fitness
            highestFitness = population.Count > 0 ? population[0].Fitness : 0;

            //elistism
            if (elitism)
            {
                int elistimKeepCount = (int)Math.Ceiling(population.Count * elitismPercentage);
                for (int i = 0; i < elistimKeepCount; i++)
                    newPopulation.Add(population[i].Clone());
            }

            //Selection Process
            while (newPopulation.Count < population.Count)
            {
                T offspringA;
                T offspringB;

                switch (selectionAlgorithm)
                {
                    case SelectionAlgorithm.RouletteWheel:
                        offspringA = RouletteSelection();
                        offspringB = RouletteSelection();
                        break;
                    case SelectionAlgorithm.StocasticUniversalSampling:
                        offspringA = StocasticUniversalSampling();
                        offspringB = StocasticUniversalSampling();
                        break;
                    case SelectionAlgorithm.TournamentSelection:
                        offspringA = TournamentSelection();
                        offspringB = TournamentSelection();
                        break;
                    case SelectionAlgorithm.TruncationSelection:
                        offspringA = TruncationSelection();
                        offspringB = TruncationSelection();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //Apply Crossover Genetic Operator
                Crossover(offspringA, offspringB);

                //Apply Mutation Genetic Operator
                Mutate(offspringA);
                Mutate(offspringB);

                newPopulation.Add(offspringA);
                newPopulation.Add(offspringB);

            }
            generationNo++;
        }

        T RouletteSelection()
        {
            //Generate random number between 0-1
            float randNumber = Random.value;

            float runningNormalisedFitness = 0;

            for (int i = 0; i < population.Count; i++)
            {
                runningNormalisedFitness += (float) population[i].Fitness / totalFitness;

                if (runningNormalisedFitness > randNumber)
                {
                    //return a clone
                    return population[Mathf.Max(0, i)].Clone();
                }
            }

            //we have a problem if we get here.
            throw new UnityException("RouletteSelection didn't find a value... normalisation must be wrong");
        }

        T StocasticUniversalSampling()
        {
            throw new NotImplementedException();
        }

        T TournamentSelection()
        {
            throw new NotImplementedException();
        }

        T TruncationSelection()
        {
            throw new NotImplementedException();
        }
        
        public void Crossover(T a, T b)
        {
            if (Random.value <= crossoverRate)
            {
                int splitPoint = Random.Range(0, a.Length -1);
                a.Crossover(splitPoint, b);
            }
        }

        public void Mutate(T chromosome)
        {
            for (int i = 0; i < chromosome.Length; i++)
            {
                if (Random.value <= mutationRate)
                    chromosome.Mutate(i);
            }
        }
    }
}
