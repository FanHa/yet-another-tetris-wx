namespace Model.Rewards
{
    public class Rush : Tetri
    {
        public override void FillCells()
        {
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new Model.Tetri.Rush());
            }
        }

        public override string GetName() => "冲锋";
        public override string GetDescription() => "冲锋并对敌人造成伤害";
    }
}