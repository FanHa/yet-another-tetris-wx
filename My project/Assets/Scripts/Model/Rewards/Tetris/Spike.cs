namespace Model.Rewards
{
    public class Spike : Tetri
    {
        private Model.Tetri.Spike cellTemplate;
        public override void FillCells()
        {
            
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new Model.Tetri.Spike());
            }
        }
        public override string GetName() => "尖刺";
        public override string GetDescription() => cellTemplate.Description();

    }
}
