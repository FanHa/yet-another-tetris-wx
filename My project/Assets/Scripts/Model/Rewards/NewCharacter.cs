using UnityEngine;

namespace Model.Rewards
{
    public class NewCharacter: Reward
    {
        protected Model.Tetri.Tetri tetriInstance;
        protected Model.Tetri.Character characterCellInstance;

        public NewCharacter(Model.Tetri.Tetri characterTetri)
        {
            tetriInstance = characterTetri;
            foreach (var pos in tetriInstance.GetOccupiedPositions())
            {
                var cell = tetriInstance.Shape[pos.x, pos.y];
                if (cell is Model.Tetri.Character character)
                {
                    characterCellInstance = character;
                    break;
                }
            }
        }

        public Model.Tetri.Tetri GetTetri()
        {
            return tetriInstance;
        }

        // public Model.Tetri.Character GetCharacter()
        // {
        //     return characterCellInstance;
        // }

        public override string Description()
        {
            return characterCellInstance.Description();
        }

        public override string Name()
        {
            return characterCellInstance.Name();
        }

        public override string Apply(TetriInventoryModel tetriInventoryData)
        {
            throw new System.NotImplementedException();
        }
    }
}