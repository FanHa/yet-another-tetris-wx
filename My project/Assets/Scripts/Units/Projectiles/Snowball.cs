using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class Snowball : ProjectileToUnit
    {
        [SerializeField] private ChilledBuffDefinition chilledDefinition;
        private float chilledDuration;
        private int moveSlowPercent;
        private int atkSlowPercent;
        private int actionSlowPercent;
        private BuffSource chilledSource;

        public void Init(
            Units.Unit caster,
            Units.Unit target,
            Units.Skills.Skill sourceSkill,
            int resonantCellCount
        )
        {
            base.Init(caster, target);
            var stats = chilledDefinition.CalculateStats(resonantCellCount);
            chilledDuration = stats.Duration.Final;
            moveSlowPercent = Mathf.RoundToInt(stats.MoveSlowPercent.Final);
            atkSlowPercent = Mathf.RoundToInt(stats.AttackSlowPercent.Final);
            actionSlowPercent = Mathf.RoundToInt(stats.ActionSlowPercent.Final);
            chilledSource = new BuffSource(caster, sourceSkill);
        }

        public string DescribeEffect(int resonantCellCount)
        {
            return chilledDefinition.DescribeEffect(resonantCellCount);
        }

        protected override void HandleHitTarget()
        {
            // 施加冰霜减速Buff
            var chilledBuff = new Chilled(
                chilledDefinition,
                chilledDuration,
                moveSlowPercent,
                atkSlowPercent,
                actionSlowPercent,
                chilledSource);
            target.AddBuff(chilledBuff);

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (chilledDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(chilledDefinition)} is not assigned.", this);
            }
        }
#endif

    }
}