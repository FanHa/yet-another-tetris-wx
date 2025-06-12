using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// IcyCage：对攻击范围内一个敌人施加Freeze（冻结）Buff，持续时间随冰系Cell数量提升
    /// </summary>
    public class IcyCage : Skill
    {
        public float BaseFreezeDuration = 2f;
        public float FreezeDurationPerIceCell = 0.5f;

        private int iceCellCount = 0;

        public IcyCage()
        {
            RequiredEnergy = 65f;
        }

        public void SetIceCellCount(int iceCellCount)
        {
            this.iceCellCount = iceCellCount;
        }

        public float GetDuration()
        {
            return BaseFreezeDuration + iceCellCount * FreezeDurationPerIceCell;
        }

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange)
                .OrderBy(enemy => Vector3.Distance(caster.transform.position, enemy.transform.position))
                .ToList();

            if (enemiesInRange.Count == 0)
                return;

            Unit targetEnemy = enemiesInRange.First();

            float freezeDuration = BaseFreezeDuration + iceCellCount * FreezeDurationPerIceCell;

            var freezeBuff = new Buffs.Freeze(
                freezeDuration,
                caster,
                this
            );
            targetEnemy.AddBuff(freezeBuff);

            TriggerEffect(new SkillEffectContext
            {
                Skill = this,
                Target = targetEnemy,
            });
        }

        public override string Description()
        {
            return $"对攻击范围内一个敌人施加冻结（Freeze）效果，持续时间随冰系Cell数量提升。";
        }

        public override string Name()
        {
            return "冰牢";
        }
    }
}