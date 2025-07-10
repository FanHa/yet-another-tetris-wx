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
    [RequireComponent(typeof(UnitManager))]
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

        [Header("Data")]
        [SerializeField] private Model.UnitInventoryModel inventoryData;
        [SerializeField] private Model.UnitInventoryModel enemyData;
        [SerializeField] private Model.TrainGround.Setup trainGroundSetup;
        [SerializeField] private TetriCellFactory tetriCellFactory;
        private UnitManager unitManager;
        public event Action OnBattleEnd;

        private Units.Skills.SkillEffectHandler skillEffectHandler;

        private List<UnitInventoryItem> factionAConfig;
        private List<UnitInventoryItem> factionBConfig;


        void Awake()
        {
            skillEffectHandler = GetComponent<Units.Skills.SkillEffectHandler>();
            if (skillEffectHandler == null)
            {
                Debug.LogError("SkillEffectHandler component is missing on the BattleField.");
            }
            unitManager = GetComponent<UnitManager>();
        }
        void Start()
        {
            battleStatistics.OnEndStatistics += HandleEndStatistics;
            unitManager.OnUnitDeath += HandleUnitDeath;
            unitManager.OnFactionAllDead += HandleFactionAllDead;
            unitManager.OnUnitDamageTaken += HandleDamageTaken;
            unitManager.OnSkillCast += HandleSkillCast;
            unitManager.OnSkillEffectTriggered += HandleSkillEffectTriggered;
        }

        public void SetEnemyData(List<Model.UnitInventoryItem> enemyData)
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

        public void StartTrainGroundBattle()
        {
            // 将 FactionAUnits 转换为 InventoryItem 列表
            factionAConfig = trainGroundSetup.FactionAUnits
                .Select(unitConfig => unitConfig.ToInventoryItem(tetriCellFactory))
                .ToList();

            // 将 FactionBUnits 转换为 InventoryItem 列表
            factionBConfig = trainGroundSetup.FactionBUnits
                .Select(unitConfig => unitConfig.ToInventoryItem(tetriCellFactory))
                .ToList();
            SpawnUnits();
        }


        private void SpawnUnits()
        {
            unitManager.SpawnUnits(factionAConfig, spawnPointA, Unit.Faction.FactionA, battlefieldMinBounds, battlefieldMaxBounds);
            unitManager.SpawnUnits(factionBConfig, spawnPointB, Unit.Faction.FactionB, battlefieldMinBounds, battlefieldMaxBounds);
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

        private void HandleUnitDeath(Unit deadUnit)
        {
            statisticsController.AddScore(1);// todo 以后根据不同单位设置不同分数
        }

        private void HandleFactionAllDead(Unit.Faction faction)
        {
            if (faction == Unit.Faction.FactionA)
            {
                statisticsController.DecreaseLife(1);
                Debug.Log("FactionA 全部死亡，生命值减少 1");
            }

            StartCoroutine(ShowBattleStatisticsWithDelay(2f));
        }

        private IEnumerator ShowBattleStatisticsWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            battleStatistics.gameObject.SetActive(true);
            battleStatistics.ShowChoosenFaction(Units.Unit.Faction.FactionA);
        }

        public void HandleEndStatistics()
        {
            battleStatistics.gameObject.SetActive(false);
            unitManager.Reset();
            OnBattleEnd?.Invoke();
        }


    }
}
