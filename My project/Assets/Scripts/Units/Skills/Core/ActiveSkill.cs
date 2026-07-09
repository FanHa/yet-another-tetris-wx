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

        // 施放前置条件检查（例如缓存目标是否仍有效）。
        // 与缓存目标无关的技能可保持默认 true。
        public virtual bool CanExecuteNow() => true;

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
            // 前置条件不满足时直接失败，不消耗能量。
            if (!CanExecuteNow())
            {
                return false;
            }

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
