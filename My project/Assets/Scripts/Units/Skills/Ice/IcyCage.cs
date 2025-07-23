using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// IcyCage：对攻击范围内一个敌人施加Freeze（冻结）Buff，持续时间随冰系Cell数量提升
    /// </summary>
    public class IcyCage : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.IcyCage;
        public IcyCageConfig Config { get; }

        public IcyCage(IcyCageConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public float GetDuration(int iceCellCount)
        {
            return Config.BaseFreezeDuration + iceCellCount * Config.FreezeDurationPerIceCell;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            var targetEnemy = caster.UnitManager.FindRandomEnemyInRange(caster, caster.Attributes.AttackRange);
            if (targetEnemy == null)
                return false;

            int iceCellCount = caster.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            float freezeDuration = Config.BaseFreezeDuration + iceCellCount * Config.FreezeDurationPerIceCell;

            var freezeBuff = new Buffs.Freeze(
                freezeDuration,
                caster,
                this
            );
            targetEnemy.AddBuff(freezeBuff);
            return true;
        }

        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "对攻击范围内一个敌人施加冻结效果";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "冰牢";
    }
}