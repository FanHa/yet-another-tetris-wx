using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Units;
using Units.Damages;
using System;

namespace Controller
{
    public class BattleStatistics : MonoBehaviour
    {
        
        public event Action OnEndStatistics;
        // [SerializeField] private GameObject statisticsPanel;
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject unitStatisticPrefab;
        [SerializeField] private GameObject damageTypeStatisticPrefab;
        [SerializeField] private Button ChooseFactionA;
        [SerializeField] private Button ChooseFactionB;
        [SerializeField] private Button endStatistics;

        private DamageDatabase damageDatabase = new();
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

        private void ShowBattleStatistics()
        {
            // 清空统计面板
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            var damageByUnit = damageDatabase.GetDamageByUnit();
            var filteredDamageByUnit = damageByUnit
                .Where(unitDamage => unitDamage.SourceUnit.faction == choosenFaction)
                .ToList();

            // 遍历每个单位的统计数据
            foreach (var unitDamage in filteredDamageByUnit)
            {
                // 创建 UnitStatisticPrefab
                GameObject unitStatInstance = Instantiate(unitStatisticPrefab, contentParent);
                var leftSide = unitStatInstance.transform.Find("LeftSide");
                if (leftSide == null)
                {
                    Debug.LogError("LeftSide object not found in UnitStatisticPrefab!");
                    continue;
                }

                // 获取 NameText 和 TotalDamageText
                var unitNameText = leftSide.Find("NameText").GetComponent<TextMeshProUGUI>();
                var totalDamageText = leftSide.Find("TotalDamageText").GetComponent<TextMeshProUGUI>();
                var damageTypePanel = unitStatInstance.transform.Find("DamageTypePanel");

                // 设置单位名称和总伤害
                unitNameText.text = unitDamage.SourceUnit.name;
                totalDamageText.text = "总伤害: " + Mathf.RoundToInt(unitDamage.TotalDamage).ToString();

                foreach (var damageDetail in unitDamage.DamageDetails)
                {
                    // 创建 DamageTypeStatisticPrefab
                    GameObject damageTypeStatInstance = Instantiate(damageTypeStatisticPrefab, damageTypePanel);
                    var damageTypeText = damageTypeStatInstance.transform.Find("TypeText").GetComponent<TextMeshProUGUI>();
                    var damageValueText = damageTypeStatInstance.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();

                    // 设置伤害类型和伤害值
                    damageTypeText.text = damageDetail.DamageSource;
                    damageValueText.text = Mathf.RoundToInt(damageDetail.DamageValue).ToString();
                }
            }
        }

        public void EndStatistics()
        {
            damageDatabase.Reset();
            OnEndStatistics?.Invoke();
        }

        public void AddRecord(Damage damage)
        {
            damageDatabase.AddDamage(damage); // 将伤害记录到数据库

        }
    }
}