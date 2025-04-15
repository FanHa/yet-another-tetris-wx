using System;

namespace Units.Buffs
{
    public class DeepFreeze : Buff
    {
        private float freezeDurationSeconds;

        public override Type TetriCellType => typeof(Model.Tetri.Skills.IceRing); // Return the Type of the corresponding TetriCell

        public DeepFreeze(float duration)
        {
            freezeDurationSeconds = duration;
        }

        public override string Name()
        {
            return "冰冻";
        }

        public override float Duration()
        {
            return freezeDurationSeconds;
        }

        public override string Description()
        {
            return $"目标被冻结，无法攻击、移动或释放技能，持续 {freezeDurationSeconds} 秒。";
        }

        public override void Apply(Unit unit)
        {
            unit.isFrozen = true; // 禁止移动、攻击、释放技能
        }

        public override void Remove(Unit unit)
        {
            unit.isFrozen = false; // 恢复正常状态
        }

        public override void Affect(Unit unit)
        {
            
        }
    }
}