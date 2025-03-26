using Model.Tetri;

namespace Model.Reward
{
    public class Health : Reward
    {
        protected override void FillCells(Tetri.Tetri tetri)
        {
            foreach (var position in tetri.GetOccupiedPositions())
            {
                tetri.SetCell(position.x, position.y, new Tetri.Health());
            }
        }

        protected override string GetName() => "Health Boost";
        protected override string GetDescription() => "Restores health";
    }
}
