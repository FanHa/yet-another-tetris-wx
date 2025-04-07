namespace Model.Rewards
{
    public class Repel : Tetri
    {
        private Model.Tetri.Repel cellTemplate = new();

        public override void FillCells()
        {
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new Model.Tetri.Repel());
            }
        }

        public override string GetName() => cellTemplate.Name();
        public override string GetDescription() => cellTemplate.Description();
    }
}