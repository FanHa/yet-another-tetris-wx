using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class WildWind : Skill
    {
        public WildWindConfig Config { get; }

        public WildWind(WildWindConfig config)
        {
            this.Config = config;
            this.RequiredEnergy = config.RequiredEnergy;
        }

        public override CellTypeId CellTypeId => CellTypeId.WildWind;

        public override string Name() => "狂风";
        public override string Description() => "在自身位置制造一阵狂风，击退敌人并造成伤害";

        protected override bool ExecuteCore(Unit caster)
        {
            int windCellCount = caster.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            float finalDamage = Config.Damage + windCellCount * Config.DamageAdditionPerWindCell;

            var prefab = caster.ProjectileConfig.WildWindPrefab; // 需要在配置中添加WildWindPrefab
            var wildWindObj = Object.Instantiate(prefab, caster.transform.position, Quaternion.identity);
            var wildWind = wildWindObj.GetComponent<Units.Projectiles.WildWind>();
            wildWind.Initialize(
                caster: caster,
                radius: Config.Radius,
                duration: Config.Duration,
                damage: finalDamage,
                debuffDuration: Config.DebuffDuration,
                moveSlowPercent: Config.MoveSlowPercent,
                atkReducePercent: Config.AtkReducePercent
            );
            wildWind.Activate();
            return true;
        }
    }
}