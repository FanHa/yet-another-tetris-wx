using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class BlazingField : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.BlazingField;
        public BlazingFieldConfig Config { get; }
        public BlazingField(BlazingFieldConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            var targetEnemy = caster.UnitManager.FindRandomEnemyInRange(caster, caster.Attributes.AttackRange);
            if (targetEnemy == null)
                return false;

            Vector3 center = targetEnemy.transform.position;
            int fireCellCount = caster.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            float radius = Config.BaseRadius + fireCellCount * Config.RadiusPerFireCell;
            float duration = Config.BaseDuration + fireCellCount * Config.DurationPerFireCell;
            float dotDps = Config.BaseDotDps + fireCellCount * Config.DotDpsPerFireCell;
            float dotDuration = Config.BaseDotDuration + fireCellCount * Config.DotDurationPerFireCell;

            var prefab = caster.ProjectileConfig.BlazingFieldPrefab; // 你需要在配置里加上这个Prefab
            var blazingFieldObj = Object.Instantiate(prefab, center, Quaternion.identity);
            var effect = blazingFieldObj.GetComponent<Units.Projectiles.BlazingField>();
            effect.Init(
                caster: caster,
                radius: radius,
                duration: duration,
                dotDps: dotDps,
                dotDuration: dotDuration,
                sourceSkill: this
            );
            effect.Activate();
            return true;
        }


        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "在目标区域制造一片燃烧区域, 对敌人造成灼烧.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "焰域";
    }
}