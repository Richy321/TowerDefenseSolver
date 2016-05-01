using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class ResultStats
    {
        List<ResultsStatsGeneration> generationStats = new List<ResultsStatsGeneration>();
        Dictionary<TowerType, float> towerUsage = new Dictionary<TowerType, float>();
        Dictionary<UInt16, float> tileUsage = new Dictionary<ushort, float>();
        public int firstSolutionGeneration = -1;

        public string filePath = "Results.txt";

        public void AddGenerationStats(int generationNo, List<BuildDecisionsChromosome> chromosomes)
        {
            if(generationNo == 0 && File.Exists(filePath))
                File.WriteAllText(filePath, String.Empty);

            ResultsStatsGeneration genStat = new ResultsStatsGeneration();
            genStat.GenerationNo = generationNo;

            foreach (BuildDecisionsChromosome buildDecisionsChromosome in chromosomes)
                genStat.mapFitnessValues.Add(buildDecisionsChromosome.Fitness);

            genStat.mapFitnessValues.Sort((i, i1) => i1.CompareTo(i));
            generationStats.Add(genStat);
            AppendStatsToLog(genStat);
        }

        public void AppendStatsToLog(ResultsStatsGeneration genStat)
        {
            using (StreamWriter w = File.AppendText(filePath))
            {
                w.WriteLine("Generation #" + genStat.GenerationNo);
                w.WriteLine("Population Fitness:");
                foreach (int mapFitnessValue in genStat.mapFitnessValues)
                    w.WriteLine(mapFitnessValue.ToString());
                w.WriteLine();
            }
        }

        public void CalculateUsageStats(List<BuildDecisionsChromosome> chromosomes)
        {
            float totalFitness = 0;
            foreach (BuildDecisionsChromosome buildDecisionsChromosome in chromosomes)
            {
                foreach (BuildDecision buildDecisionGene in buildDecisionsChromosome.buildDecisionGenes)
                {
                    if(!towerUsage.ContainsKey(buildDecisionGene.towerType))
                        towerUsage.Add(buildDecisionGene.towerType, 0.0f);

                    towerUsage[buildDecisionGene.towerType] += buildDecisionsChromosome.Fitness;

                    //pack coords into a ushort to use as key
                    ushort packed = (ushort)((buildDecisionGene.gridXCoord & 0xFF) << 8);
                    packed |= (ushort)(buildDecisionGene.gridZCoord & 0xFF);

                    if(!tileUsage.ContainsKey(packed))
                        tileUsage.Add(packed, 0.0f);

                    tileUsage[packed] += buildDecisionsChromosome.Fitness;

                    totalFitness += buildDecisionsChromosome.Fitness;
                }
            }

            //calc weighted percentages
            IEnumerable<TowerType> keys = new List<TowerType>(towerUsage.Keys);
            foreach (TowerType key in keys)
                towerUsage[key] /= totalFitness;

            IEnumerable<ushort> tileKeys = new List<ushort>(tileUsage.Keys);
            foreach (ushort key in tileKeys)
                tileUsage[key] /= totalFitness;
        }

        public void AppendUsageStatsToLog(List<BuildDecisionsChromosome> chromosomes)
        {
            CalculateUsageStats(chromosomes);

            using (StreamWriter w = File.AppendText(filePath))
            {
                w.WriteLine("Overall Usage Stats");
                w.WriteLine("First Solution Generation: "+ firstSolutionGeneration);
                w.WriteLine("TowerUsage");
                foreach (KeyValuePair<TowerType, float> keyValuePair in towerUsage)
                    w.WriteLine(keyValuePair.Key + " " + keyValuePair.Value);
                w.WriteLine();
                w.WriteLine("TileUsage");
                foreach (KeyValuePair<UInt16, float> keyValuePair in tileUsage)
                {
                    //unpack coords as bytes
                    byte upper = (byte) (keyValuePair.Key >> 8);
                    byte lower = (byte) (keyValuePair.Key & 0xff);
                    w.WriteLine(upper + "," + lower + " " + keyValuePair.Value);
                }
                w.WriteLine();
            }
        }
    }
}
