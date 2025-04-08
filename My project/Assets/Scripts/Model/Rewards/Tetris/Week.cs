namespace Model.Rewards
{
    public class Week : Tetri
    {
        private Model.Tetri.Week cellTemplate = new Model.Tetri.Week(); // 使用 WeekCell 模板

        public override void FillCells()
        {
            // 获取当前 Tetris 实例中被占用的位置
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                // 随机选择一个被占用的位置
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];

                // 在随机位置放置一个 WeekCell
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new Model.Tetri.Week());
            }
        }

        public override string GetName() => cellTemplate.Name(); // 返回 WeekCell 的名称

        public override string GetDescription() => cellTemplate.Description(); // 返回 WeekCell 的描述
    }
}