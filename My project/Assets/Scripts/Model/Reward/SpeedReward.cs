using Model.Tetri;

namespace Model.Reward
{
    public class SpeedReward : BaseReward
    {
        protected override void FillCells(Tetri.Tetri tetri)
        {
            foreach (var position in tetri.GetOccupiedPositions())
            {
                tetri.SetCell(position.x, position.y, new Speed());
            }
        }

        protected override string GetName() => "Speed Boost";
        protected override string GetDescription() => "Increases speed";
    }
}
