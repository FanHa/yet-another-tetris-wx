namespace Model.Rewards
{
    public class Rush : Tetri
    {
        private Model.Tetri.Rush cellTemplate = new Model.Tetri.Rush();

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

        public override string GetName() => cellTemplate.Name();
        public override string GetDescription() => cellTemplate.Description();
    }
}