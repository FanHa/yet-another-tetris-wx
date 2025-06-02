using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "TetriInventoryInitConfig", menuName = "Tetris/TetriInventoryInitConfig")]
    public class TetriInventoryInitConfig : ScriptableObject
    {
        public List<CellTypeReference> initialSpecialCellTypes;
        public List<CharacterTypeReference> initialSingleCharacterCells;
    }
}