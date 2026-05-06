using UnityEngine;

namespace Units.Skills
{
    public abstract class ActiveSkill : Skill, IActiveSkill
    {
        public float RequiredEnergy;
        public float CurrentEnergy;

        public virtual bool IsReady()
        {
            return CurrentEnergy >= RequiredEnergy;
        }

        // 检查入队时锁定的目标是否仍然可选中（存活且激活）
        // 位置型技能默认 true；缓存 Unit 目标的技能需覆写
        public virtual bool IsCachedTargetValid() => true;

        public virtual void AddEnergy(float amount)
        {
            CurrentEnergy += amount;
            if (CurrentEnergy > RequiredEnergy)
            {
                CurrentEnergy = RequiredEnergy; // 确保不会超过最大能量
            }
        }

        public virtual bool Execute()
        {
            if (ExecuteCore())
            {
                CurrentEnergy -= RequiredEnergy; // 执行技能后消耗能量
                return true;
            }
            else
            {
                CurrentEnergy -= RequiredEnergy * 0.5f; // 执行失败时消耗一半能量
                Debug.Log($"{Name()} 技能施放失败，消耗一半能量。单位：{Owner.name}");

                return false;
            }
        }


        protected abstract bool ExecuteCore();
    }
}
