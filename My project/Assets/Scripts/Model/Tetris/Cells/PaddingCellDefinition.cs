using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Cells/Definitions/Utility/Padding Cell Definition")]
    public sealed class PaddingCellDefinition : CellDefinition
    {
        [SerializeField] private Sprite icon;

        public override Sprite Icon => icon;
    }
}
