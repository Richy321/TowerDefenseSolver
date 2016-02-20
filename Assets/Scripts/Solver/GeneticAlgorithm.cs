using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public interface IChromosome<T>
    {
        void Randomise();
        int Length { get; }
        int Fitness { get; }
        void Crossover(int pos, T other);
        void Mutate(int geneIndex);
        T Clone();
    }

    class GeneticAlgorithm
    {
        


    }


    class GeneticAlgorithm<T> where T : IChromosome<T>, new()
    {
        public static int populationSize = 25;

        public float crossoverRate = 0.9f;
        public float mutationRate = 0.1f;
        public bool elitism = false;
        public int totalFitness;
        public int generationNo;
        public List<T> population = new List<T>(populationSize);
        public SelectionAlgorithm selectionAlgorithm = SelectionAlgorithm.RouletteWheel;
        public Action<List<T>>  onRequestGenerateFitness;


        /// <summary>
        ///Roulette-wheel selection - normalised decending, random value (0-1), first to accum to that value
        ///Stocastic Universal sampling - multiple equally spaced out pointers
        ///Tournament selection - best individual of a randomly chosen subset
        ///Truncation selection - best % of of the population is kept.
        /// </summary>
        public enum SelectionAlgorithm
        {
            RouletteWheel,
            StocasticUniversalSampling,
            TournamentSelection,
            TruncationSelection
        }

        public void InitialisePopulation()
        {
            for (int i = 0; i < populationSize; i++)
            {
                population.Add((new T()));
                population.Last().Randomise();
            }
        }

        void GeneratePopulationFitness()
        {
            if (onRequestGenerateFitness == null)
                throw new UnityException("onRequestGenerateFitness not assigned");

            onRequestGenerateFitness(population); //fill in fitness values (by ref)

            //sort fitness high->low
            population.Sort((a,b) => b.Fitness.CompareTo(a.Fitness));
        }

        public void EvolvePopulation()
        {
            GeneratePopulationFitness();

            totalFitness = 0;
            population.ForEach(v => totalFitness += v.Fitness); //get total fitness

            List<T> newPopulation = new List<T>();
            //Selection Process
            while (newPopulation.Count < populationSize)
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

            for (int i = 0; i < populationSize; i++)
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
        
        public virtual void Crossover(T a, T b)
        {
            if (Random.value <= crossoverRate)
            {
                int splitPoint = Random.Range(0, a.Length -1);
                a.Crossover(splitPoint, b);
            }
        }

        public virtual void Mutate(T chromosome)
        {
            for (int i = 0; i < chromosome.Length; i++)
            {
                if (Random.value <= mutationRate)
                    chromosome.Mutate(i);
            }
        }
    }
}
