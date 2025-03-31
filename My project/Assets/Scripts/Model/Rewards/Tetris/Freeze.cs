namespace Model.Rewards
{
    public class Freeze : Tetri
    {
        public override void FillCells()
        {
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new Model.Tetri.Freeze());
            }
        }

        public override string GetName() => "冰霜";
        public override string GetDescription() => "attack add Freeze effect";
    }
}