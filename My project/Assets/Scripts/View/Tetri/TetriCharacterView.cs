using Model.Tetri;
using UnityEngine;

namespace View.Tetri
{
    public class TetriCharacterView : TetriView<CharacterTetri>
    {
        [SerializeField] private Transform characterRoot;
        [SerializeField] private Units.UnitFactory unitFactory;

        public Units.Unit PreviewUnit { get; private set; } // 公共只读

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (characterRoot == null)
            {
                Debug.LogError($"[{nameof(TetriCharacterView)}] Character root is missing.", this);
            }

            if (unitFactory == null)
            {
                Debug.LogError($"[{nameof(TetriCharacterView)}] Unit factory is missing.", this);
            }
        }
#endif


        protected override void RebuildFromModel()
        {
            if (Model == null) return;

            // 清理旧
            if (PreviewUnit != null)
            {
                PreviewUnit.OnClicked -= HandlePreviewUnitClicked;
                Destroy(PreviewUnit.gameObject);
                PreviewUnit = null;
            }

            PreviewUnit = unitFactory.CreatePreviewUnit(Model.Character);
            PreviewUnit.transform.SetParent(characterRoot, false);
            PreviewUnit.OnClicked += HandlePreviewUnitClicked;
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