using UnityEngine;

namespace Units.Projectiles
{
    public class IcyCage : MonoBehaviour
    {
        private Unit targetUnit;
        private bool initialized = false;

        public void Initialize(Unit target)
        {
            targetUnit = target;
            // 可选：设置图片初始位置
            if (targetUnit != null)
                transform.position = targetUnit.transform.position;
        }

        public void Activate()
        {
            initialized = true;
        }

        void Update()
        {
            if (!initialized)
                return;

            // todo targetUnit 死亡需要销毁

            // 始终跟随目标
            transform.position = targetUnit.transform.position;
        }
    }
}