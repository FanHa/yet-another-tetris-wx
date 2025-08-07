using System;
using System.Collections.Generic;

namespace Units.Damages
{
    public enum DamageType
    {
        Hit,   // 普通攻击（无论是近战还是远程
        Skill,
        Reflect
    }

    public class Damage
    {
        public float Value { get; private set; }
        public string SourceLabel { get; private set; }
        public DamageType Type { get; private set; }
        public Unit SourceUnit { get; private set; }
        public Unit TargetUnit { get; private set; }

        public Damage(float value, DamageType type)
        {
            Value = value;
            Type = type;
        }
        

        // 重写 ToString 方法，便于调试
        public override string ToString()
        {
            return $"Damage(Value: {Value}, SourceLabel: {SourceLabel}, Type: {Type}, SourceUnit: {SourceUnit?.name}, TargetUnit: {TargetUnit?.name})";
        }

        public Damage SetSourceUnit(Unit unit)
        {
            SourceUnit = unit;
            return this;
        }

        public Damage SetTargetUnit(Unit unit)
        {
            TargetUnit = unit;
            return this;
        }

        public Damage SetSourceLabel(string label)
        {
            SourceLabel = label;
            return this;
        }

        public Damage SetValue(float value)
        {
            Value = value;
            return this;
        }

        
    }
        

}