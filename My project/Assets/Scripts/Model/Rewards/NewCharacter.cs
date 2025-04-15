using UnityEngine;

namespace Model.Rewards
{
    public class NewCharacter: Reward
    {
        protected Model.Tetri.Tetri tetriInstance;
        protected Model.Tetri.Character characterCellInstance;

        public NewCharacter(Model.Tetri.Character character)
        {
            characterCellInstance = character;
            tetriInstance = new Model.Tetri.Tetri(Tetri.Tetri.TetriType.Character, true);
            tetriInstance.SetCell(1, 1, character); // Place the character at (0, 0)
        }

        public Model.Tetri.Tetri GetTetri()
        {
            return tetriInstance;
        }

        public Model.Tetri.Character GetCharacter()
        {
            return characterCellInstance;
        }

        public override string Description()
        {
            return characterCellInstance.Description();
        }

        public override string Name()
        {
            return characterCellInstance.Name();
        }
    }
}