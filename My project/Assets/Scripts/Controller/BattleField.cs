using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Units;
using Model.Tetri;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Model;
using Units.Damages;
using Units.UI;
using Units.Skills;

namespace Controller {
    
    [RequireComponent(typeof(Units.Skills.SkillEffectHandler))]
    public class BattleField : MonoBehaviour
    {
        [Header("Battlefield Bounds")]
        [SerializeField] private Transform battlefieldMinBounds;
        [SerializeField] private Transform battlefieldMaxBounds;

        [Header("UI Elements")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private GameObject skillNameViewerPrefab;
        // [SerializeField] private Canvas damageCanvas;
        [SerializeField] private GameObject floatingViewManager;
        [SerializeField] private BattleStatistics battleStatistics;

        [Header("Controllers")]
        [SerializeField] private Controller.Statistics statisticsController;

        [Header("Spawn Points")]
        public Transform spawnPointA;
        public Transform spawnPointB;
        public Transform factionAParent;
        public Transform factionBParent;

        [Header("Data")]
        [SerializeField] private Model.UnitInventoryModel inventoryData;
        [SerializeField] private Model.UnitInventoryModel enemyData;
        [SerializeField] private Model.TrainGround.Setup trainGroundSetup;
        [SerializeField] private Units.UnitFactory unitFactory;
        [SerializeField] private UnitManager unitManager;
        public event Action OnBattleEnd;

        private Units.Skills.SkillEffectHandler skillEffectHandler;

        private const float RandomOffsetRangeX = 1f;
        private const float RandomOffsetRangeY = 0.5f;

        private List<InventoryItem> factionAConfig;
        private List<InventoryItem> factionBConfig;

        void Awake()
        {
            skillEffectHandler = GetComponent<Units.Skills.SkillEffectHandler>();
            if (skillEffectHandler == null)
            {
                Debug.LogError("SkillEffectHandler component is missing on the BattleField.");
            }
        }
        void Start()
        {
            battleStatistics.OnEndStatistics += EndStatistics;
        }

        public void SetEnemyData(List<Model.InventoryItem> enemyData)
        {
            this.enemyData.Items = enemyData; // 替换敌人数据
        }


        public void StartNewLevelBattle(int level)
        {
            statisticsController.SetLevel(level); // 设置当前关卡
            factionAConfig = inventoryData.Items;

            factionBConfig = enemyData.Items;
            SpawnUnits();
        }

        internal void StartTrainGroundBattle()
        {
            // 将 FactionAUnits 转换为 InventoryItem 列表
            factionAConfig = trainGroundSetup.FactionAUnits
                .Select(unitConfig => unitConfig.ToInventoryItem())
                .ToList();

            // 将 FactionBUnits 转换为 InventoryItem 列表
            factionBConfig = trainGroundSetup.FactionBUnits
                .Select(unitConfig => unitConfig.ToInventoryItem())
                .ToList();
            SpawnUnits();
        }


        private void SpawnUnits()
        {
            SpawnFactionUnits(factionAConfig, spawnPointA, Unit.Faction.FactionA, factionAParent);
            SpawnFactionUnits(factionBConfig, spawnPointB, Unit.Faction.FactionB, factionBParent);
        }

        private void SpawnFactionUnits(List<Model.InventoryItem> items, Transform spawnPoint, Unit.Faction faction, Transform parent)
        {
            foreach (Model.InventoryItem item in items)
            {
                SpawnUnit(spawnPoint, item, faction, parent);

            }
        }

        private void SpawnUnit(Transform spawnPoint, InventoryItem item, Unit.Faction faction, Transform parent)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint.position);
            // 用工厂创建基础Unit
            Unit unit = unitFactory.CreateUnit(item);
            if (unit != null)
            {
                unit.transform.SetParent(parent, false);
                unit.transform.position = spawnPosition;
                InitializeUnitBattle(unit, faction);
            }
        }

        private Vector3 GetRandomSpawnPosition(Vector3 basePosition)
        {
            return basePosition + new Vector3(
                UnityEngine.Random.Range(-RandomOffsetRangeX, RandomOffsetRangeX),
                UnityEngine.Random.Range(-RandomOffsetRangeY, RandomOffsetRangeY),
                0f
            );
        }

        private void InitializeUnitBattle(Unit unit, Unit.Faction faction)
        {
            unit.unitManager = unitManager;
            unit.SetFaction(faction);
            unit.SetBattlefieldBounds(battlefieldMinBounds, battlefieldMaxBounds);

            unit.OnDeath += OnUnitDeath;
            unit.OnDamageTaken += HandleDamageTaken;
        
            unit.OnSkillCast += HandleSkillCast;
            unit.OnSkillEffectTriggered += HandleSkillEffectTriggered;
            unitManager.Register(unit);
            unit.Initialize();
        }

        private void HandleSkillEffectTriggered(SkillEffectContext context)
        {
            skillEffectHandler.HandleSkillEffect(context);
        }

        private void HandleSkillCast(Units.Unit unit, Units.Skills.Skill skill)
        {
            GameObject skillNameInstance = GameObject.Instantiate(skillNameViewerPrefab, floatingViewManager.transform);
            FloatingTextView floatingText = skillNameInstance.GetComponent<FloatingTextView>();
            floatingText.Initialize(skill.Name(), unit.transform.position);
        }

        private void HandleDamageTaken(Units.Damages.Damage damage)
        {
            battleStatistics.AddRecord(damage);
            GameObject damageTextInstance = Instantiate(damageTextPrefab, floatingViewManager.transform);
            FloatingTextView damageview = damageTextInstance.GetComponent<FloatingTextView>();
            string sourceLabel = "";
            if (damage.Type != DamageType.Hit)
            {
                sourceLabel = damage.SourceLabel;
            }
            int roundedDamage = Mathf.RoundToInt(damage.Value); // 将伤害值取整
            string text = sourceLabel + " " + roundedDamage.ToString();
            damageview.Initialize(text, damage.TargetUnit.transform.position);
        }

        private void OnUnitDeath(Unit deadUnit)
        {
            statisticsController.AddScore(1);// todo 以后根据不同单位设置不同分数
            unitManager.Unregister(deadUnit);
            List<Unit> survivals = deadUnit.faction == Unit.Faction.FactionA
                ? unitManager.GetFactionAUnits() : unitManager.GetFactionBUnits();
            if (survivals.Count == 0)
            {
                if (deadUnit.faction == Unit.Faction.FactionA)
                {
                    statisticsController.DecreaseLife(1);
                    Debug.Log("FactionA 全部死亡，生命值减少 1");
                }
                foreach (Unit unit in unitManager.GetAllUnits())
                {
                    unit.StopAction();
                }
                StartCoroutine(ShowBattleStatisticsWithDelay(2f));
            }
        }

        private IEnumerator ShowBattleStatisticsWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            battleStatistics.gameObject.SetActive(true);
            battleStatistics.ShowChoosenFaction(Units.Unit.Faction.FactionA);
        }

        private void DestroyAllUnits()
        {
            // 遍历 factionAParent 的所有子对象并销毁
            foreach (Transform child in factionAParent)
            {
                Destroy(child.gameObject);
            }

            // 遍历 factionBParent 的所有子对象并销毁
            foreach (Transform child in factionBParent)
            {
                Destroy(child.gameObject);
            }

            // 重置 UnitManager
            unitManager.Reset();
        }


        public void EndStatistics()
        {
            battleStatistics.gameObject.SetActive(false);
            DestroyAllUnits(); // 销毁所有单位
            OnBattleEnd?.Invoke();
        }


    }
}
