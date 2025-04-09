using System;

namespace Units.Buffs
{
    public class Freeze : Buff
    {
        private float moveSpeedReductionPercentage = 20f;
        private float attackSpeedReductionPercentage = 20f;
        public float freezeDurationSeconds = 8f; // 冻结持续时间

        public override string Name()
        {
            return "冰霜";
        }

        public override float Duration()
        {
            return freezeDurationSeconds;
        }

        public override string Description()
        {
            return $"降低目标移动速度{moveSpeedReductionPercentage}%, 降低目标攻击速度{attackSpeedReductionPercentage}%, 持续{freezeDurationSeconds}秒";
        }

        public override void Apply(Unit unit)
        {
            unit.moveSpeed.AddPercentageModifier(this, -moveSpeedReductionPercentage); // 减少移动速度
            unit.attacksPerTenSeconds.AddPercentageModifier(this, -attackSpeedReductionPercentage);
        }

        public override void Remove(Unit unit)
        {
            unit.moveSpeed.RemovePercentageModifier(this);
            unit.attacksPerTenSeconds.RemovePercentageModifier(this);
        }

        public override void Affect(Unit unit)
        {
        }

        public override Type TetriCellType => typeof(Model.Tetri.Freeze); // Return the Type of the corresponding TetriCell
    }
}