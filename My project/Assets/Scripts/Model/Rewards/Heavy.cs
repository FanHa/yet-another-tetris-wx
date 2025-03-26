using Model.Tetri;

namespace Model.Reward
{
    public class Heavy : Reward
    {
        protected override void FillCells(Tetri.Tetri tetri)
        {
            foreach (var position in tetri.GetOccupiedPositions())
            {
                tetri.SetCell(position.x, position.y, new Tetri.Heavy());
            }
        }

        protected override string GetName() => "Heavy Boost";
        protected override string GetDescription() => "Increases weight";
    }
}
