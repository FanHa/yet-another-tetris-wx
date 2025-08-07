using Model.Tetri;

namespace Units.Skills
{
    public class LifeEcho : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeEcho;
        public LifeEchoConfig Config { get; }

        public LifeEcho(LifeEchoConfig config)
        {
            Config = config;
        }

        private struct LifeEchoStats
        {
            public StatValue ReflectPercent;
        }

        private LifeEchoStats CalcStats()
        {
            return new LifeEchoStats
            {
                ReflectPercent = new StatValue("反弹伤害比例(%)", Config.DamagePercentToReflect)
            };
        }

        public override string Name() => NameStatic();
        public static string NameStatic() => "生命回响";

        public override string Description()
        {
            var stats = CalcStats();
            return DescriptionStatic() + ":\n" +
                $"{stats.ReflectPercent}";
        }
        public static string DescriptionStatic() => "反弹受到的伤害";

        public void ApplyPassive()
        {
            var stats = CalcStats();
            Owner.AddBuff(new Units.Buffs.LifeEchoBuff(
                Owner,
                this,
                stats.ReflectPercent.Final
            ));
        }
    }
}