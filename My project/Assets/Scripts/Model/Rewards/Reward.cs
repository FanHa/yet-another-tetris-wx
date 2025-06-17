using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public abstract class Reward
    {
        public abstract string Name();
        public abstract string Description();

        public abstract string Apply(TetriInventoryModel tetriInventoryData);
    }
}
