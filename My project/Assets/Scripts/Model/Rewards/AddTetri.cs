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
            Model.Tetri.Cell cell = tetriInstance.GetMainCell();
            name = $"获得新能力：{cell.Name()}";
            description = $"{cell.Description()}";

        }



        public Model.Tetri.Tetri GetTetri()
        {
            return tetriInstance;
        }


        public override string Name() => name;
        public override string Description() => description;

        public override void Apply(TetriInventoryModel tetriInventoryData)
        {
            tetriInventoryData.AddTetri(tetriInstance);
        }
    }
}