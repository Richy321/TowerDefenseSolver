using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MapChromosome : MonoBehaviour, IChromosome<MapChromosome>
    {
        public List<List<TowerType>> chromosome;

        public Map map;

        public void Initialise()
        {
            Length = map.nodeCountX * map.nodeCountZ;
            chromosome = new List<List<TowerType>>(map.nodeCountX);
           
            for (int z = 0; z < map.nodeCountZ; z++)
            {
                chromosome.Add(new List<TowerType>());
                for (int x = 0; x < map.nodeCountX; x++)
                    chromosome[z].Add(TowerType.None);
            }
        }

        public void Randomise()
        {
            for (int z = 0; z < map.nodeCountZ; z++)
                for (int x = 0; x < map.nodeCountX; x++)
                    chromosome[z][x] = GetRandomTowerType();
        }

        TowerType GetRandomTowerType()
        {
            return (TowerType)Random.Range(0, Enum.GetValues(typeof(TowerType)).Length);
        }

        public int Length { get; set; }

        public int Fitness { get; set; }

        public void Crossover(int pos, MapChromosome other)
        {
            int xIndex;
            int zIndex;
            GetXZCoords(pos, out xIndex, out zIndex);

            for (int i = 0; i < zIndex; i++)
            {
                List<TowerType> tmp = chromosome[i];
                chromosome[i] = other.chromosome[i];
                other.chromosome[i] = tmp;
            }

            for (int i = xIndex; i < map.nodeCountX; i++)
            {
                TowerType leftTypeTmp = chromosome[pos + 1][i];
                chromosome[pos + 1][i] = other.chromosome[pos + 1][i];
                other.chromosome[pos + 1][i] = leftTypeTmp;
            }
        }

        public void Mutate(int geneIndex)
        {
            int x;
            int z;
            GetXZCoords(geneIndex, out x, out z);
            chromosome[z][x] = GetRandomTowerType();
        }

        public MapChromosome Clone()
        {
            MapChromosome clone = new MapChromosome
            {
                chromosome = new List<List<TowerType>>(chromosome),
                map = map
            };

            return clone;
        }

        private void GetXZCoords(int index, out int x, out int z)
        {
            x = index % map.nodeCountZ; 
            z = index / map.nodeCountZ;
        }
    }
}
