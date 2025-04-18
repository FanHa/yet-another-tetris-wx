using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    // todo 更直观的inspector 查看
    [Serializable]
    public class Attribute
    {
        [SerializeField] private float baseValue; // 基础值
        [SerializeField] private Dictionary<object, float> flatModifiers; // 绝对值修改来源
        [SerializeField] private Dictionary<object, float> percentageModifiers; // 百分比修改来源

        [SerializeField] public float finalValue;

        public Attribute(float baseValue)
        {
            this.baseValue = baseValue;
            this.flatModifiers = new Dictionary<object, float>();
            this.percentageModifiers = new Dictionary<object, float>();
            RecalculateFinalValue();
        }

        /// <summary>
        /// 设置基础值并重新计算最终值。
        /// </summary>
        public void SetBaseValue(float value)
        {
            baseValue = value;
            RecalculateFinalValue();
        }

        /// <summary>
        /// 添加绝对值修改。
        /// </summary>
        public void AddFlatModifier(object source, float value)
        {
            if (flatModifiers.ContainsKey(source))
            {
                
            }
            else
            {
                flatModifiers[source] = value;
            }
            RecalculateFinalValue();
        }

        /// <summary>
        /// 移除绝对值修改。
        /// </summary>
        public void RemoveFlatModifier(object source)
        {
            if (flatModifiers.ContainsKey(source))
            {
                flatModifiers.Remove(source);
                RecalculateFinalValue();
            }
        }

        /// <summary>
        /// 添加百分比修改。
        /// </summary>
        public void AddPercentageModifier(object source, float percentage)
        {
            if (percentageModifiers.ContainsKey(source))
            {
                
            }
            else
            {
                percentageModifiers[source] = percentage;
            }
            RecalculateFinalValue();
        }

        /// <summary>
        /// 移除百分比修改。
        /// </summary>
        public void RemovePercentageModifier(object source)
        {
            if (percentageModifiers.ContainsKey(source))
            {
                percentageModifiers.Remove(source);
                RecalculateFinalValue();
            }
        }

        /// <summary>
        /// 重新计算最终值。
        /// </summary>
        private void RecalculateFinalValue()
        {
            float flatSum = 0f;
            foreach (var modifier in flatModifiers.Values)
            {
                flatSum += modifier;
            }

            float percentageSum = 0f;
            foreach (var modifier in percentageModifiers.Values)
            {
                percentageSum += modifier;
            }

            finalValue = (baseValue + flatSum) * (1 + percentageSum / 100f);
        }
    }
}