using Model.Tetri;

namespace Model.Rewards
{
    public class Health : Tetri
    {
        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.Health());
            }
        }

        public override string GetName() => "Health Boost";
        public override string GetDescription() => "Restores health";
    }
}
