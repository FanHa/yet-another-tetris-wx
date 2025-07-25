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


        public override string Description()
        {
            return characterCellInstance.Description();
        }

        public override string Name()
        {
            return "新角色：" + characterCellInstance.Name();
        }

        public override void Apply(TetriInventoryModel tetriInventoryData)
        {
            tetriInventoryData.AddTetri(tetriInstance);
        }
    }
}