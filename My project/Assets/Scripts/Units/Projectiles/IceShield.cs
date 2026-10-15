using UnityEngine;
using Units;

namespace Units.Projectiles
{
    public class IceShield : global::Units.BuffVisual
    {
        [SerializeField] private float rotateSpeed = 90f; // 每秒旋转角度
        [SerializeField] private float radius = 1f;     // 冰盾半径

        private Units.Unit owner;
        private bool isActive = false;
        private float angle = 0f;

        public override void Initialize(Units.Unit owner, Model.Buffs.BuffDefinition definition)
        {
            this.owner = owner;

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

            // 计算旋转位置
            angle += rotateSpeed * Time.deltaTime;
            if (angle > 360f) angle -= 360f;

            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
            transform.position = owner.transform.position + offset;
        }
    }
}