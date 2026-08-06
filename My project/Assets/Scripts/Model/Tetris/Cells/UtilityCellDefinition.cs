using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Cells/Utility Cell Definition")]
    public sealed class UtilityCellDefinition : CellDefinition
    {
        [SerializeField] private Sprite icon;

        public override Sprite Icon => icon;
    }
}
