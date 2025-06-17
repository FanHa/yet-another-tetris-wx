using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tetri
{
    public class TetriCellFactory
    {
        private readonly Random _random = new Random();

        // 缓存所有 TetriCellCharacter 的子类
        private static readonly List<Type> CachedCharacterTypes;

        // 静态构造函数，用于初始化缓存
        static TetriCellFactory()
        {
            CachedCharacterTypes = typeof(Character).Assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Character)) && !type.IsAbstract)
                .ToList();

            if (CachedCharacterTypes.Count == 0)
            {
                throw new InvalidOperationException("No subclasses of TetriCellCharacter found.");
            }
        }

        public Cell CreateRandomCharacter()
        {
            // 随机选择一个子类并创建实例
            var selectedType = CachedCharacterTypes[_random.Next(CachedCharacterTypes.Count)];
            return (Cell)Activator.CreateInstance(selectedType);
        }

        public Cell CreatePadding()
        {
            return new Padding();
        }
        
        public Cell CreateCell(Type cellType)
        {
            if (cellType == null || !typeof(Cell).IsAssignableFrom(cellType))
            {
                throw new ArgumentException("Invalid cell type provided.");
            }

            return (Cell)Activator.CreateInstance(cellType);
        }

    }

}
