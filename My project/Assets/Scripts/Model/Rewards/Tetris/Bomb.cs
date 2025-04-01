namespace Model.Rewards
{
    public class Bomb : Tetri
    {
        public override void FillCells()
        {
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new Model.Tetri.Bomb());
            }
        }

        public override string GetName() => "爆破";
        public override string GetDescription() => "attack add burn effect";
    }
}