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

        public TetriCell CreateRandomCharacter()
        {
            // 随机选择一个子类并创建实例
            var selectedType = CachedCharacterTypes[_random.Next(CachedCharacterTypes.Count)];
            return (TetriCell)Activator.CreateInstance(selectedType);
        }

        public TetriCell CreatePadding()
        {
            return new Padding();
        }

    }

}
