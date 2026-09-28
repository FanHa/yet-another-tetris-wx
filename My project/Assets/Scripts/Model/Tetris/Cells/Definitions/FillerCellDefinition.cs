using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Cells/Definitions/Utility/Filler Cell Definition")]
    public sealed class FillerCellDefinition : CellDefinition
    {
        [SerializeField] private Sprite icon;

        public override Sprite Icon => icon;
    }
}
