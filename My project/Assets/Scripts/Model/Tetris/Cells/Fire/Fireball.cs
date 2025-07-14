using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class Fireball : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.Fireball;

        public Fireball()
        {
            Affinity = AffinityType.Fire;
        }

        public override string Description()
        {
            return "发射一个火球，造成火焰伤害。";
        }

        public override string Name()
        {
            return "火球";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.FireballConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.Fireball(config);
            unit.AddSkill(skillInstance);
        }
    }
}