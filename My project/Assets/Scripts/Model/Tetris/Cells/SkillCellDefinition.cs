using UnityEngine;
using Units.Skills;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Cells/Skill Cell Definition")]
    public sealed class SkillCellDefinition : CellDefinition
    {
        [SerializeField] private SkillDefinition skillDefinition;

        public SkillDefinition SkillDefinition => skillDefinition;
        public override Sprite Icon => skillDefinition != null ? skillDefinition.Icon : null;
    }
}
