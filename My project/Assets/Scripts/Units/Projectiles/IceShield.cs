using UnityEngine;

namespace Units.Projectiles
{
    public class IceShield : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 90f; // 每秒旋转角度
        [SerializeField] private float radius = 1f;     // 冰盾半径

        private Units.Unit owner;
        private bool initialized = false;
        private float angle = 0f;

        public void Initialize(Units.Unit owner)
        {
            this.owner = owner;

        }

        public void Activate()
        {
            initialized = true;
        }
            

        void Update()
        {
            if (!initialized)
                return;

            // todo unit 死亡需要销毁

            // 计算旋转位置
            angle += rotateSpeed * Time.deltaTime;
            if (angle > 360f) angle -= 360f;

            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
            transform.position = owner.transform.position + offset;
        }
    }
}