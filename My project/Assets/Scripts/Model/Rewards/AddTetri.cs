using System;
using System.Linq;
using Controller;
using UnityEngine;

namespace Model.Rewards
{
    public class AddTetri: Reward
    {
        protected Model.Tetri.Tetri tetriInstance;
        private string name;
        private string description;

        public AddTetri(Model.Tetri.Tetri tetri)
        {
            tetriInstance = tetri;
            // 找到 tetriInstance 中非 Padding 的 Cell
            var cell = tetriInstance.GetOccupiedPositions()
                .Select(pos => tetriInstance.Shape[pos.x, pos.y])
                .FirstOrDefault(c => !(c is Model.Tetri.Padding));

            if (cell != null)
            {
                name = $"获得新方块：{cell.GetType().Name}";
                description = $"获得一个包含 {cell.GetType().Name} 的新方块";
            }
            else
            {
                name = "获得新方块";
                description = "获得一个新方块";
            }
        }



        public Model.Tetri.Tetri GetTetri()
        {
            return tetriInstance;
        }


        public override string Name() => name;
        public override string Description() => description;

        public override string Apply(TetriInventoryModel tetriInventoryData)
        {
            throw new NotImplementedException();
        }
    }
}