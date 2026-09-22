using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    [Serializable]
    public class EnemySquadData
    {
        public CharacterDefinition characterDefinition;
        public List<EnemyCellData> cellConfigs = new();
    }

    [Serializable]
    public class EnemyCellData
    {
        public CellDefinition definition;
        public int level = 1;
    }
}
