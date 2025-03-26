using Model.Tetri;

namespace Model.Reward
{
    public class Speed : Reward
    {
        protected override void FillCells(Tetri.Tetri tetri)
        {
            foreach (var position in tetri.GetOccupiedPositions())
            {
                tetri.SetCell(position.x, position.y, new Tetri.Speed());
            }
        }

        protected override string GetName() => "Speed Boost";
        protected override string GetDescription() => "Increases speed";
    }
}
