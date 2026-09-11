using UnityEngine;
using Units.Skills;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Cells/Skill Backed Cell Definition")]
    public sealed class SkillBackedCellDefinition : CellDefinition
    {
        [SerializeField] private SkillDefinition skillDefinition;

        public SkillDefinition SkillDefinition => skillDefinition;
        public override Sprite Icon => skillDefinition != null ? skillDefinition.Icon : null;
        public override string DisplayName => skillDefinition != null ? skillDefinition.DisplayName : null;
        public override string Description => skillDefinition != null ? skillDefinition.Description : null;
    }
}
