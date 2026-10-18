using UnityEngine;

namespace Model.Rewards
{
    public class NewCharacter: Reward
    {
        protected Model.Tetri.CharacterTetri tetriInstance;

        public NewCharacter(Model.Tetri.CharacterTetri characterTetri)
        {
            tetriInstance = characterTetri;
        }

        public Model.Tetri.CharacterTetri GetTetri()
        {
            return tetriInstance;
        }


        public override string Description()
        {
            return tetriInstance.Character.Description();
        }

        public override string Name()
        {
            return "新角色：" + tetriInstance.Character.Name();
        }

        public override void Apply(TetriInventoryModel tetriInventoryData)
        {
            tetriInventoryData.AddTetri(tetriInstance);
        }
    }
}