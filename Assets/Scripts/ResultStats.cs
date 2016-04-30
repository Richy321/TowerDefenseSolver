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
        Dictionary<TowerType, float> towerUsage;
        Dictionary<UInt16, float> tileUsage;
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
            
            generationStats.Add(genStat);
            AppendStatsToLog(genStat);
        }

        public void AppendStatsToLog(ResultsStatsGeneration genStat)
        {
            using (StreamWriter w = File.AppendText(filePath))
            {
                w.WriteLine("Generation #" + genStat.GenerationNo);
                w.WriteLine("Population Fitness:" + genStat.GenerationNo);
                foreach (int mapFitnessValue in genStat.mapFitnessValues)
                    w.WriteLine("mapFitnessValue");
                w.WriteLine();
            }
        }

        public void AppendUsageStatsToLog(List<BuildDecisionsChromosome> chromosomes)
        {
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
                    byte upper = (byte) (keyValuePair.Key >> 8);
                    byte lower = (byte) (keyValuePair.Key & 0xff);
                    w.WriteLine(upper + "," + lower + " " + keyValuePair.Value);
                }
                w.WriteLine();
            }
        }
    }
}
