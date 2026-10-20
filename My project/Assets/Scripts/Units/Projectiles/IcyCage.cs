using UnityEngine;
using Model.Resonance;

namespace Units.Projectiles
{
    public class IcyCage : global::Units.BuffVisual
    {
        private Unit targetUnit;
        private bool isActive = false;

        public override void Initialize(Unit target, ResonanceType resonance)
        {
            targetUnit = target;
            // 可选：设置图片初始位置
            if (targetUnit != null)
                transform.position = targetUnit.transform.position;
        }

        public override void Activate()
        {
            isActive = true;
        }

        public override void Release()
        {
            isActive = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        void Update()
        {
            if (!isActive)
                return;

            // 始终跟随目标
            transform.position = targetUnit.transform.position;
        }
    }
}