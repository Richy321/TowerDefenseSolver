﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Assets.Scripts
{
    public interface IMap
    {
        void AddTower(int rowIndex, int colIndex, TowerType type, bool checkResources);
        void RemoveTower(int rowIndex, int colIndex);
        void ClearTowers();
        void SetTowers(MapChromosome towerLayout);
    }
}
