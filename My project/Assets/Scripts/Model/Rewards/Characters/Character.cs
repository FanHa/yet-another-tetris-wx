using UnityEngine;

namespace Model.Rewards
{
    public abstract class Character : Reward
    {
        protected Model.Tetri.Character characterCellInstance;

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