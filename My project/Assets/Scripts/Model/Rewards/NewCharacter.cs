using UnityEngine;

namespace Model.Rewards
{
    public class NewCharacter: Reward
    {
        protected Model.Tetri.Character characterCellInstance;

        public NewCharacter(Model.Tetri.Character character)
        {
            characterCellInstance = character;
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