using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using TMPro;
using UI;
using Units.Buffs;
using UnityEngine;

namespace Units
{
    public class Unit : MonoBehaviour
    {
        public Attributes Attributes;
        private Units.Buffs.Manager buffManager;// Buff管理器
        private Movement movementController;
        private Units.Skills.Manager skillManager = new(); // 技能管理器

        public enum Faction
        {
            FactionA,
            FactionB
        }
        [SerializeField] private Color factionAColor;
        [SerializeField] private Color factionBColor;

        public event Action<Unit> OnDeath;

        public event Action<Damages.EventArgs> OnDamageTaken;


        public event Action<Unit> OnAttacked; // 被攻击事件

        public Faction faction; // 单位的阵营
        protected float lastAttackTime = 0;

        private List<ITakeDamageBehavior> damageBehaviors = new List<ITakeDamageBehavior>(); // 伤害行为链
        
        private Animator animator;
        private HealthBar healthBar;

        // [SerializeField] private GameObject damageTextPrefab; // 伤害显示的Prefab
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        [SerializeField] private SpriteRenderer Fist1SpriteRenderer;
        [SerializeField] private SpriteRenderer Fist2SpriteRenderer;
        private HitEffect hitEffect;
        public GameObject projectilePrefab; // 投射物预制体
        public GameObject bombPrefab; // TODO 暂时所有projectile的prefab都放到这里,以后再改
        public GameObject PrecisionArrowPrefab;
        public Transform projectileSpawnPoint; // 投射物生成位置
        public List<Transform> targetEnemies;
        private Transform factionAParent;
        private Transform factionBParent;

        public List<Buffs.Buff> attackEffects = new List<Buffs.Buff>(); // 攻击效果列表
        private bool isActive = false; // 是否处于活动状态
        public bool moveable = true;
        private void Awake()
        {
            // 获取当前对象的 Animator 组件
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator component not found on the same GameObject.");
            }
            healthBar = GetComponentInChildren<HealthBar>();
            hitEffect = GetComponent<HitEffect>();
            Attributes.OnHealthChanged += UpdateHealthBar;
            buffManager = GetComponent<Units.Buffs.Manager>();
            movementController = GetComponent<Movement>();
            movementController.Initialize(Attributes); // 将 Attributes 传递给 Movement

        }

        // Update is called once per frame
        void Update()
        {
            if (isActive) {
                AttackEnemies();
            }
        }

        void FixedUpdate()
        {
            if (isActive)
            {
                if (targetEnemies != null && targetEnemies.Count > 0)
                {
                    Transform closestEnemy = targetEnemies[0];
                    movementController.MoveTowardsEnemy(closestEnemy, targetEnemies, faction);
                }
                movementController.ClampPositionToBattlefield();
            }
        }

        public void Initialize()
        {
            Attributes.CurrentHealth = Attributes.MaxHealth.finalValue;
            InvokeRepeating(nameof(BuffEffect), 1f, 1f);
            InvokeRepeating(nameof(FindClosestEnemies), 0f, 0.1f);

            skillManager.Initialize(this); // 初始化技能管理器
            InvokeRepeating(nameof(CastSkills) , 0f, 0.2f); // 每秒调用一次技能
            isActive = true;
        }
        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        public void SetBattlefieldBounds(Transform minBounds, Transform maxBounds)
        {
           movementController.SetBattlefieldBounds(minBounds, maxBounds);
        }

        
        public void AddSkill(Skills.Skill newSkill)
        {
            skillManager.AddSkill(newSkill); // 添加技能
        }
        private void CastSkills()
        {
            skillManager.CastSkills(); // 释放技能
        }

        public void StopAction()
        {
            CancelInvoke(nameof(BuffEffect));
            CancelInvoke(nameof(FindClosestEnemies));
            CancelInvoke(nameof(CastSkills));
            isActive = false;
        }

        public void AddDamageBehavior(ITakeDamageBehavior behavior)
        {
            damageBehaviors.Add(behavior);
        }

        public void RemoveDamageBehavior(ITakeDamageBehavior behavior)
        {
            damageBehaviors.Remove(behavior);
        }


        public void AddBuff(Units.Buffs.Buff buff)
        {
            buffManager.AddBuff(buff, this);
        }

        private void BuffEffect()
        {
            buffManager.UpdateBuffs(this);

        }




        public void SetFactionParent(Transform factionA, Transform factionB)
        {
            factionAParent = factionA;
            factionBParent = factionB;
        }

        protected void FindClosestEnemies()
        {
            Transform enemyParent = faction == Faction.FactionA ? factionBParent : factionAParent;
            if (enemyParent == null)
            {
                Debug.LogWarning("Enemy parent is not set for this unit.");
                return;
            }
            List<Transform> enemiesInRange = new List<Transform>();
            
            foreach (Transform enemy in enemyParent)
            {
                Unit unit = enemy.GetComponent<Unit>();
                if (unit != null && unit.faction != faction) // 判断是否为敌对阵营
                {
                    enemiesInRange.Add(enemy);
                }
            }

            // 按距离排序并选择最近的 attackTargetNumber 个敌人
            targetEnemies = enemiesInRange
                .OrderBy(enemy => Vector2.Distance(transform.position, enemy.position))
                .Take((int)Attributes.AttackTargetNumber)
                .ToList();
        }

        protected void AttackEnemies()
        {
            float attackCooldown = 10f / Attributes.AttacksPerTenSeconds.finalValue;
            if (Time.time < lastAttackTime + attackCooldown) 
                return; // 检查攻击冷却时间
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                Transform closestEnemy = targetEnemies[0]; // 获取最近的敌人
                if (closestEnemy != null)
                {
                    float distance = Vector2.Distance(transform.position, closestEnemy.position);
                    if (distance <= Attributes.AttackRange) // 检查最近的敌人是否在攻击范围内
                    {
                        TriggerAttack();
                        lastAttackTime = Time.time; // 更新攻击时间
                    }
                }
            }
        }

        private void TriggerAttack()
        {
            animator.SetTrigger("Attack");
            
        }

        // 这个方法会被animator的event触发
        public void HandleAttackActionHit()
        {
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                foreach (var target in targetEnemies)
                {
                    if (target == null) continue; // 检查目标是否已被销毁

                    float distance = Vector2.Distance(transform.position, target.position);
                    if (distance <= Attributes.AttackRange)
                    {
                        Unit enemyUnit = target.GetComponent<Unit>();
                        if (enemyUnit != null)
                        {
                            Attack(enemyUnit, Attributes.AttackPower.finalValue/targetEnemies.Count); // 执行攻击逻辑
                        }
                    }
                }
            }
        }

        private void Attack(Unit target, float damage)
        {
            if (Attributes.IsRanged)
            {
                // 发射投射物
                FireProjectile(target, damage);
            }
            else
            {
                // 近战攻击逻辑
                target.TakeHit(this, new Damages.Damage(damage, "近战攻击", true), attackEffects);
            }
        }

        private void FireProjectile(Unit target, float damage)
        {
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
                Projectiles.Projectile projectile = projectileObject.GetComponent<Projectiles.Projectile>();
                
                if (projectile != null)
                {
                    projectile.target = target.transform;
                    projectile.damage = new Damages.Damage(damage, "远程攻击", true); // 设置投射物的伤害
                    projectile.debuffs = new List<Buffs.Buff>(attackEffects); // 传递攻击效果作为Debuff
                    projectile.caster = this; // 设置投射物的施法者

                    // 设置投射物的 Sprite 为 Fist1SpriteRenderer 的 Sprite
                    SpriteRenderer projectileSpriteRenderer = projectileObject.GetComponent<SpriteRenderer>();
                    if (projectileSpriteRenderer != null && Fist1SpriteRenderer != null)
                    {
                        projectileSpriteRenderer.sprite = Fist1SpriteRenderer.sprite;
                    }
                    projectileObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                }
            }
        }



        public virtual void TakeHit(Unit source, Units.Damages.Damage  damageReceived, List<Buffs.Buff> buffs)
        {
            foreach (var buff in buffs)
            {
                AddBuff(buff); // 添加Debuff到自己身上
            }
            TakeDamage(source, damageReceived); // 扣除生命值
            hitEffect.PlayAll();
            OnAttacked?.Invoke(source); // 触发被攻击事件
        }

        public void TakeDamage(Unit source, Units.Damages.Damage damageReceived)
        {   
            foreach (var behavior in damageBehaviors)
            {
                damageReceived = behavior.ModifyDamage(source, damageReceived);
            }

            float finalDamage = Mathf.Max(1, Mathf.Round(damageReceived.Value));

            Attributes.CurrentHealth -= finalDamage;
            
            OnDamageTaken?.Invoke(new Damages.EventArgs(source, this, damageReceived));

            CheckHealth();
        }


        private void CheckHealth()
        {
            if (Attributes.CurrentHealth <= 0)
            {
                OnDeath?.Invoke(this);
                gameObject.SetActive(false); // 将 GameObject 设置为失活
            }
        }

        void OnDrawGizmosSelected()
        {

            // 绘制攻击范围的Gizmos
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Attributes.AttackRange);
        }

        /// <summary>
        /// Sets the faction of the unit and updates the color of the body sprite renderer accordingly.
        /// <param name="faction">The faction to set for the unit.</param>
        public void SetFaction(Faction faction)
        {
            this.faction = faction;
            Color color = faction == Faction.FactionA ? factionAColor : factionBColor;
            bodySpriteRenderer.color = color;
            Fist1SpriteRenderer.color = color;
            Fist2SpriteRenderer.color = color;
        }

        
    }
}
