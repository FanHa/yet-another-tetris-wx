using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Cells/Definitions/Skills/Skill Cell Definition")]
    public sealed class SkillBackedCellDefinition : CellDefinition
    {
        [SerializeField] private Model.Skills.SkillDefinition skillDefinition;

        public Model.Skills.SkillDefinition SkillDefinition => skillDefinition;
        public override Sprite Icon => skillDefinition != null ? skillDefinition.Icon : null;
        public override string DisplayName => skillDefinition != null ? skillDefinition.DisplayName : null;
        public override string Description => skillDefinition != null ? skillDefinition.Description : null;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (skillDefinition == null)
            {
                Debug.LogError($"[{nameof(SkillBackedCellDefinition)}] Skill definition is missing.", this);
            }
        }
#endif
    }
}
