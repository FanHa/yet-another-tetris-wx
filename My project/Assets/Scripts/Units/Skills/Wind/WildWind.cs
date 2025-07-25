using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class WildWind : ActiveSkill
    {
        public WildWindConfig Config { get; }

        public WildWind(WildWindConfig config)
        {
            this.Config = config;
            this.RequiredEnergy = config.RequiredEnergy;
        }

        public override CellTypeId CellTypeId => CellTypeId.WildWind;

        public override string Description()
        {
            return
                $"在自身周围制造一阵狂风，" +
                $"对范围内敌人造成伤害：a[b + c]，" +
                $"其中 a=风元素数，b={Config.Damage}[基础]，c={Config.DamageAdditionPerWindCell}[每个风元素加成]；" +
                $"击退敌人，并使其移动速度降低{Config.MoveSlowPercent}%、攻击力降低{Config.AtkReducePercent}%，" +
                $"持续{Config.DebuffDuration}秒。";
        }
        public static string DescriptionStatic() => "在自身周围制造一阵狂风, 击退敌人并造成伤害.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "狂风";

        protected override bool ExecuteCore()
        {
            int windCellCount = Owner.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            float finalDamage = Config.Damage + windCellCount * Config.DamageAdditionPerWindCell;

            var prefab = Owner.ProjectileConfig.WildWindPrefab; // 需要在配置中添加WildWindPrefab
            var wildWindObj = Object.Instantiate(prefab, Owner.transform.position, Quaternion.identity);
            var wildWind = wildWindObj.GetComponent<Units.Projectiles.WildWind>();
            wildWind.Initialize(
                caster: Owner,
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