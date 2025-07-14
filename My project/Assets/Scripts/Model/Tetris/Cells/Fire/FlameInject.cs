using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class FlameInject : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameInject;
        public FlameInject()
        {
            Affinity = AffinityType.Fire;
        }

        public override string Description()
        {
            return "攻击时对目标附加火焰伤害并施加灼烧。";
        }

        public override string Name()
        {
            return "炎附";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.FlameInjectConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.FlameInject(config);
            unit.AddSkill(skillInstance);
        }
    }
}