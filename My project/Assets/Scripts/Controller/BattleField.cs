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
        // public Transform spawnPointA;
        public Transform spawnPointB;
        [SerializeField] private float timeDelayBeforeBattle; // 战斗开始前的延迟时间
        private Coroutine spawnRoutine; // 处理战斗开始生成单位后需要延迟一小段时间再开打

        [Header("Data")]
        [SerializeField] private Model.UnitInventoryModel playerUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel enemyUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel trainGroundDataFactionA;
        [SerializeField] private Model.UnitInventoryModel trainGroundDataFactionB;
        private UnitManager unitManager;
        public event Action OnBattleEnd;
        public event Action<Unit> OnUnitClicked;

        private List<CharacterPlacement> factionAConfig;
        private List<CharacterPlacement> factionBConfig;

        void Awake()
        {
            unitManager = GetComponent<UnitManager>();
        }
        void Start()
        {
            battleStatistics.OnEndStatistics += HandleEndStatistics;
            unitManager.OnUnitDeath += HandleUnitDeath;
            unitManager.OnFactionAllDead += HandleFactionAllDead;
            unitManager.OnUnitDamageTaken += HandleDamageTaken;
            unitManager.OnGlobalSkillCast += HandleSkillCast;
            unitManager.OnUnitClicked += HandleUnitClicked;
        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        public void StartNewLevelBattle(int level)
        {
            statisticsController.SetLevel(level); // 设置当前关卡
            factionAConfig = playerUnitInventoryData.Items;

            factionBConfig = enemyUnitInventoryData.Items;
            SpawnUnits();
        }

        public void StartTrainGround()
        {
            factionAConfig = trainGroundDataFactionA.Items;
            factionBConfig = trainGroundDataFactionB.Items;
            SpawnUnits();
        }


        private void SpawnUnits()
        {
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
            }
            spawnRoutine = StartCoroutine(SpawnUnitsRoutine());

        }

        private IEnumerator SpawnUnitsRoutine()
        {
            unitManager.SpawnUnits(
                factionAConfig,
                transform,
                Unit.Faction.FactionA,
                battlefieldMinBounds,
                battlefieldMaxBounds
            );

            unitManager.SpawnUnits(
                factionBConfig,
                spawnPointB,
                Unit.Faction.FactionB,
                battlefieldMinBounds,
                battlefieldMaxBounds
            );

            yield return new WaitForSeconds(timeDelayBeforeBattle);
            // 4) 同时激活所有单位
            unitManager.ActivateAllUnits();

            spawnRoutine = null;
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
