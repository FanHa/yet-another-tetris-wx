using UnityEngine;

namespace Operation
{
    public class TetriCharacter : Tetri
    {
        [SerializeField] private Transform characterRoot;
        [SerializeField] private Units.UnitFactory unitFactory;

        private Units.Unit previewUnit;

        protected override void RebuildFromModel()
        {
            if (ModelTetri == null) return;

            // 清理旧
            if (previewUnit != null)
            {
                previewUnit.OnClicked -= HandlePreviewUnitClicked;
                Destroy(previewUnit.gameObject);
                previewUnit = null;
            }

            // 找到主 Character cell
            Model.Tetri.Character mainCell = ModelTetri.GetMainCell() as Model.Tetri.Character;

            if (mainCell != null)
            {
                previewUnit = unitFactory.CreateUnit(mainCell);
                previewUnit.transform.SetParent(characterRoot, false);
                previewUnit.OnClicked += HandlePreviewUnitClicked; // 订阅

            }
        }

        private void HandlePreviewUnitClicked(Units.Unit _)
        {
            // 触发基类统一点击流程
            TriggerClick();
        }

        protected void OnDestroy()
        {
            if (previewUnit != null)
            {
                previewUnit.OnClicked -= HandlePreviewUnitClicked;
                previewUnit = null;
            }
        }

    }
}