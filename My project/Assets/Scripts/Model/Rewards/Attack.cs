using Model.Tetri;

namespace Model.Reward
{
    public class Attack : Reward
    {
        protected override void FillCells(Tetri.Tetri tetri)
        {
            foreach (var position in tetri.GetOccupiedPositions())
            {
                tetri.SetCell(position.x, position.y, new Tetri.Attack());
            }
        }

        protected override string GetName() => "Attack Boost";
        protected override string GetDescription() => "Increases attack power";
    }
}
