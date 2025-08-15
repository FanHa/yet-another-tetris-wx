using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowAttack : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.ShadowAttack;
        public ShadowAttackConfig Config { get; }

        public ShadowAttack(ShadowAttackConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }


        protected override bool ExecuteCore()
        {

            Unit targetEnemy = Owner.UnitManager.FindWeakestEnemy(Owner);
            if (targetEnemy == null)
                return false;

            Vector3 dir = (targetEnemy.transform.position - Owner.transform.position).normalized;
            Vector3 targetPos = targetEnemy.transform.position + dir * 1.2f; // 1.2f为身后距离，可调整

            Owner.transform.position = targetPos;

            // todo 施放易伤buff
            // 造成伤害
            var damage = new Damages.Damage(Config.Damage, Damages.DamageType.Hit);
            damage.SetSourceUnit(Owner);
            damage.SetTargetUnit(targetEnemy);
            damage.SetSourceLabel("影袭");
            targetEnemy.TakeDamage(damage);
            // todo 动画

            return true;
        }

        public override string Description()
        {
            var weakLevel = new StatValue("易伤层数", Config.WeakLevel);

            return
                DescriptionStatic() + ":\n" +
                $"{weakLevel}\n";
        }

        public static string DescriptionStatic() => "闪现到最脆弱敌人身后,施加易伤并造成伤害";

        public override string Name() => NameStatic();
        public static string NameStatic() => "影袭";
    }
}