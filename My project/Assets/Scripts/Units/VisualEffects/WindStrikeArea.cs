using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.VisualEffects
{
    public class WindStrikeArea : MonoBehaviour
    {
        [Header("Sprite Settings")]
        public SpriteRenderer spriteRenderer; // 关联的 SpriteRenderer
        public float rotationSpeed = 90f; // 旋转速度（度/秒）

        public float effectDuration; // 区域效果持续时间
        public float damageInterval; // 每次伤害间隔
        public float damageMultiplier; // 伤害倍数
        public float radius; // 区域半径
        private Unit caster; // 技能施放者
        private float elapsedTime = 0f; // 效果已持续时间
        private float damageTimer = 0f; // 伤害计时器
        private bool initialized = false;

        private void Update()
        {
            if (!initialized)
            {
                return;
            }
            // 不断旋转 Sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            // 更新效果持续时间
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= effectDuration)
            {
                Destroy(gameObject); // 销毁区域效果
                return;
            }

            // 更新伤害计时器
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                ApplyDamage(); // 触发伤害逻辑
                damageTimer -= damageInterval; // 减去一个伤害间隔
            }
        }

        public void Initialize(Unit caster, float effectDuration, float damageInterval, float damageMultiplier, float radius)
        {
            this.caster = caster;
            this.effectDuration = effectDuration;
            this.damageInterval = damageInterval;
            this.damageMultiplier = damageMultiplier;
            this.radius = radius;

            Vector2 spriteSize = spriteRenderer.sprite.bounds.size; // 获取图片的实际大小
                Vector3 scale = new Vector3(
                    radius * 2 / spriteSize.x, // 计算 X 轴缩放比例
                    radius * 2 / spriteSize.y, // 计算 Y 轴缩放比例
                    1f
                );
            spriteRenderer.transform.localScale = scale;

            this.initialized = true;
        }

        private void ApplyDamage()
        {
            float damagePerTick = caster.Attributes.MoveSpeed.finalValue * damageMultiplier;

            // 检测范围内的敌人
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var collider in colliders)
            {
                Unit enemy = collider.GetComponent<Unit>();
                if (enemy != null && enemy.faction != caster.faction)
                {
                    // 对敌人造成伤害
                    Units.Damages.Damage damage = new Units.Damages.Damage(
                        damagePerTick,
                        "风刃伤害",
                        Units.Damages.DamageType.Skill,
                        caster,
                        enemy,
                        new List<Units.Buffs.Buff>()
                    );
                    enemy.TakeDamage(damage);
                }
            }
        }

        
    }
}