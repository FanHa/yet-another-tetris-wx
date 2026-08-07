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
    }
}
