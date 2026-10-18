using System;
using System.Linq;
using Controller;
using UnityEngine;

namespace Model.Rewards
{
    public class AddTetri: Reward
    {
        protected Model.Tetri.CellBasedTetri tetriInstance;
        private string name;
        private string description;

        public AddTetri(Model.Tetri.CellBasedTetri tetri)
        {
            tetriInstance = tetri;
            Model.Tetri.Cell cell = tetriInstance.MainCell;
            name = $"获得新能力：{cell.Name()}";
            description = $"{cell.Description()}";

        }



        public Model.Tetri.CellBasedTetri GetTetri()
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