using System;
using System.Collections.Generic;

namespace Units.Damages
{
    public enum DamageType
    {
        Hit,   // 普通攻击（无论是近战还是远程）
        Skill  // 技能伤害
    }

    public class Damage
    {
        public float Value { get; set; } // 伤害数值
        public string SourceName { get; set; } // 伤害来源名称（技能名称或攻击方式）
        public DamageType Type { get; set; } // 伤害类型（Hit 或 Skill）
        public Unit SourceUnit { get; set; } // 伤害来源单位
        public Unit TargetUnit { get; set; } // 伤害目标单位
        public List<Buffs.Buff> Buffs { get; set; } // 附带的 Buff 效果


        // 构造函数
        public Damage(float value, string sourceName, DamageType type, Unit sourceUnit, Unit targetUnit, List<Buffs.Buff> buffs)
        {
            Value = value;
            SourceName = sourceName;
            Type = type;
            SourceUnit = sourceUnit;
            TargetUnit = targetUnit;
            Buffs = buffs;
        }

        // 重写 ToString 方法，便于调试
        public override string ToString()
        {
            return $"Damage(Value: {Value}, SourceName: {SourceName}, Type: {Type}, SourceUnit: {SourceUnit?.name}, TargetUnit: {TargetUnit?.name})";
        }
    }
        

}