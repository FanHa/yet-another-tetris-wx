namespace Units.Damages
{
    public class Damage
    {
        public float Value { get; set; } // 基础伤害
        public string DamageType { get; set; } // 伤害类型
        public bool CanBeReflected { get; set; } // 是否可以被反弹

        public Damage(float value, string damageType, bool canBeReflected)
        {
            Value = value;
            DamageType = damageType;
            CanBeReflected = canBeReflected;
        }
    }
        

}