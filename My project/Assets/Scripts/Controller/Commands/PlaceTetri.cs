using Model;
using UI.Resource;
using UnityEngine;

namespace Controller.Commands
{
    public class PlaceTetri : ICommand
    {
        private readonly OperationTable operationTableData;
        private readonly ItemSlot itemSlot;
        private readonly Vector2Int position;
        private readonly Resource tetriResource;

        private bool isPlaced;


        public PlaceTetri(OperationTable operationTableData, ItemSlot itemSlot, Vector2Int position, Resource tetriResource)
        {
            this.operationTableData = operationTableData;
            this.itemSlot = itemSlot;
            this.position = position;
            this.tetriResource = tetriResource;

        }

        public bool Execute()
        {
            // 执行放置逻辑
            isPlaced = operationTableData.PlaceTetriWithHistory(position, itemSlot.GetTetri());
            if (isPlaced)
            {
                // 如果放置成功，执行延迟逻辑
                tetriResource.UseTetri(itemSlot);
            }
            return isPlaced;
        }

        public void Undo()
        {
            if (isPlaced)
            {
                // 撤销放置逻辑
                // operationTableData.UndoLastPlacement();

                // 恢复资源
                tetriResource.UndoLastUseTetri();
            }
        }
    }
}