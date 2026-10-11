using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Units;
using System;
using Model.Battle.Statistics;

namespace UI
{
    public class BattleStatistics : MonoBehaviour
    {
        public event Action OnEndStatistics;
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject unitStatisticPrefab;
        [SerializeField] private GameObject damageTypeStatisticPrefab;
        [SerializeField] private Button ChooseFactionA;
        [SerializeField] private Button ChooseFactionB;
        [SerializeField] private Button endStatistics;

        private IReadOnlyList<UnitDamageSummary> damageSummaries = Array.Empty<UnitDamageSummary>();
        private Unit.Faction choosenFaction = Unit.Faction.FactionA;

        void Start()
        {
            endStatistics.onClick.AddListener(EndStatistics);
            ChooseFactionA.onClick.AddListener(() =>
            {
                ShowChoosenFaction(Unit.Faction.FactionA);
            });
            ChooseFactionB.onClick.AddListener(() =>
            {
                ShowChoosenFaction(Unit.Faction.FactionB);
            });
        }

        public void ShowChoosenFaction(Unit.Faction faction)
        {
            choosenFaction = faction;
            ShowBattleStatistics();
        }

        public void ShowStatistics(IReadOnlyList<UnitDamageSummary> summaries, Unit.Faction faction)
        {
            damageSummaries = summaries;
            ShowChoosenFaction(faction);
        }

        private void ShowBattleStatistics()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            var filteredDamageByUnit = damageSummaries
                .Where(unitDamage => unitDamage.Faction == choosenFaction)
                .ToList();

            foreach (var unitDamage in filteredDamageByUnit)
            {
                GameObject unitStatInstance = Instantiate(unitStatisticPrefab, contentParent);
                var leftSide = unitStatInstance.transform.Find("LeftSide");
                if (leftSide == null)
                {
                    Debug.LogError("LeftSide object not found in UnitStatisticPrefab!");
                    continue;
                }

                var unitNameText = leftSide.Find("NameText").GetComponent<TextMeshProUGUI>();
                var totalDamageText = leftSide.Find("TotalDamageText").GetComponent<TextMeshProUGUI>();
                var damageTypePanel = unitStatInstance.transform.Find("DamageTypePanel");

                unitNameText.text = unitDamage.UnitName;
                totalDamageText.text = "总伤害: " + Mathf.RoundToInt(unitDamage.TotalDamage).ToString();

                foreach (var damageDetail in unitDamage.DamageDetails)
                {
                    GameObject damageTypeStatInstance = Instantiate(damageTypeStatisticPrefab, damageTypePanel);
                    var damageTypeText = damageTypeStatInstance.transform.Find("TypeText").GetComponent<TextMeshProUGUI>();
                    var damageValueText = damageTypeStatInstance.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();

                    damageTypeText.text = damageDetail.DamageSource;
                    damageValueText.text = Mathf.RoundToInt(damageDetail.DamageValue).ToString();
                }
            }
        }

        public void EndStatistics()
        {
            OnEndStatistics?.Invoke();
        }
    }
}