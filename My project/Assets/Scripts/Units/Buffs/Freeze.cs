namespace Units
{
    public class Freeze : Buff
    {
        private float moveSpeedReductionPercentage = 20f;
        private float attackSpeedReductionPercentage = 20f;
        public float freezeDurationSeconds = 3f; // 冻结持续时间

        public override string Name()
        {
            return "冻结";
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
            throw new System.NotImplementedException();
        }

        public override void Remove(Unit unit)
        {
            throw new System.NotImplementedException();
        }

        public override void Affect(Unit unit)
        {
            throw new System.NotImplementedException();
        }
    }
}