using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Cells/Definitions/Utility/Utility Cell Definition")]
    public sealed class UtilityCellDefinition : CellDefinition
    {
        [SerializeField] private Sprite icon;

        public override Sprite Icon => icon;
    }
}
