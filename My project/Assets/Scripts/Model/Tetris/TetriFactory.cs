using System;
using System.Collections.Generic;

namespace Model.Tetri
{
    public class TetrisFactory
    {
        private TetriCellFactory cellFactory = new TetriCellFactory();

        private readonly Dictionary<string, List<(int, int)>> shapeDefinitions = new()
        {
            { "T", new List<(int, int)> { (1, 0), (1, 1), (1, 2), (0, 1) } },
            { "I", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (3, 1) } },
            { "O", new List<(int, int)> { (0, 0), (0, 1), (1, 0), (1, 1) } },
            { "L", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 2) } },
            { "J", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 0) } },
            { "S", new List<(int, int)> { (1, 0), (1, 1), (2, 1), (2, 2) } },
            { "Z", new List<(int, int)> { (1, 1), (1, 2), (2, 0), (2, 1) } }
        };

        public Tetri CreateShape(string shapeKey)
        {
            if (!shapeDefinitions.ContainsKey(shapeKey))
                throw new ArgumentException($"Invalid shape key: {shapeKey}");

            Tetri tetri = new Tetri(Tetri.TetriType.Normal);
            foreach (var (row, col) in shapeDefinitions[shapeKey])
            {
                tetri.SetCell(row, col, cellFactory.CreatePadding());
            }
            return tetri;
        }

        public Tetri CreateTShape() => CreateShape("T");
        public Tetri CreateIShape() => CreateShape("I");
        public Tetri CreateOShape() => CreateShape("O");
        public Tetri CreateLShape() => CreateShape("L");
        public Tetri CreateJShape() => CreateShape("J");
        public Tetri CreateSShape() => CreateShape("S");
        public Tetri CreateZShape() => CreateShape("Z");

        public Tetri CreateRandomBaseShape()
        {
            var shapeKeys = new List<string> { "T", "I", "O", "L", "J", "S", "Z" };
            var randomKey = shapeKeys[new Random().Next(shapeKeys.Count)];
            return CreateShape(randomKey);
        }
        
        
        /// <summary>
        /// 创建一个仅在 (1,1) 位置有一个 Padding 类型单元格的 Tetri。
        /// 这个 Tetri 可以稍后被修改以填充特定的单元格类型。
        /// </summary>
        /// <returns>新创建的 Tetri 对象。</returns>
        public Tetri CreateSinglePaddingCellTetri()
        {
            // 创建一个 Tetri，其类型可以设为 Normal，因为它最初只包含一个 Padding cell。
            // 或者，如果有一个更通用的“可自定义”或“单格”类型，也可以使用。
            Tetri tetri = new Tetri(Tetri.TetriType.Normal); // 或者一个更合适的默认类型

            // 从 cellFactory 创建一个 Padding 类型的 Cell
            Cell paddingCell = cellFactory.CreatePadding();

            if (paddingCell == null)
            {
                UnityEngine.Debug.LogError("Failed to create Padding cell in TetrisFactory.");
                // 根据错误处理策略返回，例如返回一个空的 Tetri 或 null
                return tetri; // 或者 return null;
            }

            // 在 (1,1) 位置设置 Padding 单元格
            // 同样需要确保 Tetri 的 Shape 数组在构造时已初始化并足够大以容纳 (1,1)
            tetri.SetCell(1, 1, paddingCell);

            return tetri;
        }

        

    }
}