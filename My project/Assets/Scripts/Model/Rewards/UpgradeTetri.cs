using Model.Tetri;

namespace Model.Rewards
{
    public abstract class UpgradeTetri : Reward
    {
        public CellBasedTetri TargetTetri { get; }

        protected UpgradeTetri(CellBasedTetri targetTetri)
        {
            TargetTetri = targetTetri;
        }

        public override void Apply(TetriInventoryModel tetriInventoryData)
        {
            TargetTetri.Upgrade();
        }
    }
}