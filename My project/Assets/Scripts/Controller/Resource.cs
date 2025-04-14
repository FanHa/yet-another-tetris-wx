using System;
using Model.Tetri;
using UI.Resource;
using UnityEngine;

namespace Controller
{
    public class Resource : MonoBehaviour
    {
        [SerializeField] private Model.TetrisResources tetrisResourcesData;
        [SerializeField] private UI.Resource.Panel tetrisResourcePanelUI;
        public event Action<ItemSlot> OnTetriBegainDrag;
        public void Initialize()
        {
            tetrisResourcePanelUI.OnTetriResourceItemBeginDrag += HandleTetriBeginDrag;
            tetrisResourcesData.OnDataChanged += UpdateResourcesPanelUI;
            tetrisResourcesData.Reset();
            tetrisResourcesData.DrawRandomTetriFromUnusedList(3);
        }

        public void PrepareNewRound()
        {
            tetrisResourcesData.DrawRandomTetriFromUnusedList(1);
            tetrisResourcePanelUI.ResetTab();
        }

        public void UseTetri(ItemSlot item)
        {
            tetrisResourcesData.UseTetri(item.GetTetri());
        }

        private void HandleTetriBeginDrag(ItemSlot item)
        {

            // 调用TetrisResourcesSO的方法设置某个Tetri被拖动
            tetrisResourcesData.SetTetriDragged(item.GetTetri());
            OnTetriBegainDrag?.Invoke(item);

        }

        private void UpdateResourcesPanelUI()
        {
            // 更新资源面板UI
            tetrisResourcePanelUI.UpdatePanels(
                tetrisResourcesData.GetUsableTetris(),
                tetrisResourcesData.GetUsedTetris(),
                tetrisResourcesData.GetUnusedTetris());
        }

    }
}