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
        public event Action<ItemSlot> OnTetriEndDrag;
        public void Initialize()
        {
            tetrisResourcePanelUI.OnTetriResourceItemBeginDrag += HandleTetriBeginDrag;
            tetrisResourcePanelUI.OnTetriResourceItemEndDrag += HandleTetriEndDrag;
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
            OnTetriBegainDrag?.Invoke(item);

        }

        private void HandleTetriEndDrag(ItemSlot item)
        {
            OnTetriEndDrag?.Invoke(item);
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