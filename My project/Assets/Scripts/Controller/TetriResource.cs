using System;
using Model.Tetri;
using UI.TetrisResource;
using UnityEngine;

namespace Controller
{
    public class TetriResource : MonoBehaviour
    {
        [SerializeField] private Model.TetrisResources tetrisResourcesData;
        [SerializeField] private UI.TetrisResource.TetrisResourcePanel tetrisResourcePanelUI;
        [SerializeField] private Model.TetrisListTemplate tetrisListTemplate;
        public event Action<TetrisResourceItem> OnTetriBegainDrag;
        public void Initialize()
        {
            tetrisResourcePanelUI.OnTetriResourceItemBeginDrag += HandleTetriBeginDrag;
            tetrisResourcesData.OnDataChanged += UpdateResourcesPanelUI;
            tetrisResourcesData.Reset();
            tetrisResourcesData.InitialUnusedTetris(tetrisListTemplate.template);
            tetrisResourcesData.DrawRandomTetriFromUnusedList(3);
        }

        public void PrepareNewRound()
        {
            tetrisResourcesData.DrawRandomTetriFromUnusedList(1);
        }

        public void UseTetri(TetrisResourceItem item)
        {
            tetrisResourcesData.UseTetri(item.GetTetri());
        }

        public void AddUsableTetri(Tetri tetri)
        {
            tetrisResourcesData.AddUsableTetri(tetri);
        }

        public UI.TetrisResource.TetrisResourceItem GetTetriResourceItem(Transform parent, Tetri tetri)
        {
            return tetrisResourcePanelUI.CreateTetrisResourceItem(parent, tetri);
        }

        private void HandleTetriBeginDrag(TetrisResourceItem item)
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