using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// Freeze：完全冻结目标，无法移动、攻击、释放技能，能量回复为0
    /// </summary>
    public class Freeze : Buff
    {
        private GameObject vfxInstance;
        public Freeze(
            float duration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
        {
        }

        public override string Name() => "Freeze";
        public override string Description() => "完全冻结，无法行动，能量回复为0";

        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            // 攻速、移速、能量回复全部-100%
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -100);
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, -100);
            unit.Attributes.EnergyPerSecond.AddPercentageModifier(this, -100);
            var vfxPrefab = unit.ProjectileConfig.IcyCagePrefab;
            vfxInstance = Object.Instantiate(vfxPrefab, unit.transform.position, Quaternion.identity);
            var icyCageComp = vfxInstance.GetComponent<Units.Projectiles.IcyCage>();
            icyCageComp.Initialize(unit);
            icyCageComp.Activate();

        }

        public override void OnRemove(Unit unit)
        {
            unit.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            unit.Attributes.MoveSpeed.RemovePercentageModifier(this);
            unit.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            if (vfxInstance != null)
            {
                Object.Destroy(vfxInstance);
                vfxInstance = null;
            }

        }
    }
}