using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
        [Header("UI Elements")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private GameObject skillNameViewerPrefab;
        // [SerializeField] private Canvas damageCanvas;
        [SerializeField] private GameObject floatingViewManager;
        [SerializeField] private BattleStatistics battleStatistics;

        [Header("Controllers")]

        [SerializeField] private Controller.GameStatusHUD gameStatusHud;

        [Header("Spawn Points")]
        // public Transform spawnPointA;
        public Transform spawnPointB;
        [SerializeField] private float timeDelayBeforeBattle; // 战斗开始前的延迟时间
        private Coroutine spawnRoutine; // 处理战斗开始生成单位后需要延迟一小段时间再开打
        private Coroutine statisticsRoutine;

        [Header("Data")]
        private UnitManager unitManager;
        public event Action OnBattleEnd;
        public event Action<Unit> OnUnitClicked;

        private IReadOnlyList<CharacterPlacement> factionAConfig;
        private IReadOnlyList<CharacterPlacement> factionBConfig;

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

        public void StartNewLevelBattle(
            int level,
            IReadOnlyList<CharacterPlacement> factionAItems,
            IReadOnlyList<CharacterPlacement> factionBItems)
        {
            gameStatusHud.SetLevel(level); // 设置当前关卡
            factionAConfig = factionAItems;
            factionBConfig = factionBItems;
            SpawnUnits();
            StartDelayedActivation();
        }

        public void StartTrainGround(
            IReadOnlyList<CharacterPlacement> factionAItems,
            IReadOnlyList<CharacterPlacement> factionBItems)
        {
            factionAConfig = factionAItems;
            factionBConfig = factionBItems;
            SpawnUnits();
            StartDelayedActivation();
        }

        public void PreviewBattle(
            IReadOnlyList<CharacterPlacement> factionAItems,
            IReadOnlyList<CharacterPlacement> factionBItems)
        {
            CancelPendingActivation();
            factionAConfig = factionAItems;
            factionBConfig = factionBItems;
            SpawnUnits();
        }

        public void ClearPreview()
        {
            CancelPendingActivation();
            CancelPendingStatistics();
            unitManager.Reset();
        }

        private void StartDelayedActivation()
        {
            CancelPendingActivation();
            spawnRoutine = StartCoroutine(DelayActivateUnitsCoroutine());
        }

        private void CancelPendingActivation()
        {
            if (spawnRoutine == null)
            {
                return;
            }

            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        private void CancelPendingStatistics()
        {
            if (statisticsRoutine == null)
            {
                return;
            }

            StopCoroutine(statisticsRoutine);
            statisticsRoutine = null;
        }

        private void SpawnUnits()
        {
            CancelPendingStatistics();
            unitManager.Reset();

            unitManager.SpawnUnits(
                factionAConfig,
                transform,
                Unit.Faction.FactionA
            );

            unitManager.SpawnUnits(
                factionBConfig,
                spawnPointB,
                Unit.Faction.FactionB
            );
        }



        private IEnumerator DelayActivateUnitsCoroutine()
        {
            yield return new WaitForSeconds(timeDelayBeforeBattle);
            spawnRoutine = null;
            unitManager.ActivateAllUnits();
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
            gameStatusHud.AddScore(1);// todo 以后根据不同单位设置不同分数
        }

        private void HandleFactionAllDead(Unit.Faction faction)
        {
            if (faction == Unit.Faction.FactionA)
            {
                gameStatusHud.DecreaseLife(1);
                Debug.Log("FactionA 全部死亡，生命值减少 1");
            }

            CancelPendingStatistics();
            statisticsRoutine = StartCoroutine(ShowBattleStatisticsWithDelay(2f));
        }

        private IEnumerator ShowBattleStatisticsWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            statisticsRoutine = null;
            battleStatistics.gameObject.SetActive(true);
            battleStatistics.ShowChoosenFaction(Units.Unit.Faction.FactionA);
        }

        public void HandleEndStatistics()
        {
            battleStatistics.gameObject.SetActive(false);
            CancelPendingActivation();
            CancelPendingStatistics();
            unitManager.Reset();
            OnBattleEnd?.Invoke();
        }

        
    }
}
