using UnityEngine;

namespace Operation
{
    public class TetriCharacter : Tetri
    {
        [SerializeField] private Transform characterRoot;
        [SerializeField] private Units.UnitFactory unitFactory;

        public Units.Unit PreviewUnit { get; private set; } // 公共只读


        protected override void RebuildFromModel()
        {
            if (ModelTetri == null) return;

            // 清理旧
            if (PreviewUnit != null)
            {
                PreviewUnit.OnClicked -= HandlePreviewUnitClicked;
                Destroy(PreviewUnit.gameObject);
                PreviewUnit = null;
            }

            // 找到主 Character cell
            Model.Tetri.Character mainCell = ModelTetri.GetMainCell() as Model.Tetri.Character;

            if (mainCell != null)
            {
                PreviewUnit = unitFactory.CreateUnit(mainCell);
                PreviewUnit.transform.SetParent(characterRoot, false);
                PreviewUnit.OnClicked += HandlePreviewUnitClicked; // 订阅

            }
        }

        private void HandlePreviewUnitClicked(Units.Unit _)
        {
            // 触发基类统一点击流程
            TriggerClick();
        }

        protected void OnDestroy()
        {
            if (PreviewUnit != null)
            {
                PreviewUnit.OnClicked -= HandlePreviewUnitClicked;
                PreviewUnit = null;
            }
        }

    }
}