using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Cells/Padding Cell Definition")]
    public sealed class PaddingCellDefinition : CellDefinition
    {
        [SerializeField] private Sprite icon;

        public override Sprite Icon => icon;
    }
}
