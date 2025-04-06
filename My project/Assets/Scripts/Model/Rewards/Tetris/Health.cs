using Model.Tetri;

namespace Model.Rewards
{
    public class Health : Tetri
    {
        private Model.Tetri.Health cellTemplate = new Model.Tetri.Health();

        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.Health());
            }
        }

        public override string GetName() => cellTemplate.Name();
        public override string GetDescription() => cellTemplate.Description();
    }
}
