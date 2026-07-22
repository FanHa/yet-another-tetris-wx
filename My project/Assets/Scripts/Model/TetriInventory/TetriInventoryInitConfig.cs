using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "TetriInventoryInitConfig", menuName = "Tetris/TetriInventoryInitConfig")]
    public class TetriInventoryInitConfig : ScriptableObject
    {
        public List<string> CellIds;
        public List<CharacterTypeId> CharacterTypeIds;

    }
}