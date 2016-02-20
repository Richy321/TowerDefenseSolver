using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Assets.Scripts
{
    public class GridNode : PathNode
    {
        public PathNode PathNode
        {
            get { return this; }
        }

        public TowerType towerType = TowerType.None;
    }
}
