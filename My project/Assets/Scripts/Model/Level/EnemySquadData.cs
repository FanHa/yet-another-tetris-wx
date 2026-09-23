using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    /// <summary>
    /// Plain generated data used to describe one enemy squad for the current battle.
    /// This is not a ScriptableObject or asset configuration; it is just a runtime-transfer object.
    /// </summary>
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
