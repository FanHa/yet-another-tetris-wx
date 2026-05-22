using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class UnitInfo : MonoBehaviour
    {
        public Action OnClosed;
        [SerializeField] private Button closeButton;

        [Header("属性")]
        [SerializeField] private UI.UnitInfo.Health healthAttribute;
        [SerializeField] private UI.UnitInfo.Attribute moveSpeedAttribute;
        [SerializeField] private UI.UnitInfo.Attribute attackPowerAttribute;
        [SerializeField] private UI.UnitInfo.Attribute attacksPerTenSecondsAttribute;
        [SerializeField] private UI.UnitInfo.Attribute energyPerSecondAttribute;
        [SerializeField] private UI.UnitInfo.Attribute attackRangeAttribute;

        [Header("Affinity")]
        [SerializeField] private UI.UnitInfo.Affinity affinityInfoPrefab;
        [SerializeField] private Transform affinityRoot;
        [SerializeField] private AffinityResourceMapping affinityResourceMapping;

        [Header("技能")]
        [SerializeField] private UI.UnitInfo.Skill unitSkillPrefab;
        [SerializeField] private Transform skillRoot;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;

        [Header("Buff")]
        [SerializeField] private UI.UnitInfo.Buff buffInfoPrefab;
        [SerializeField] private Transform buffRoot;
        [SerializeField] private TMPro.TextMeshProUGUI buffDescriptionText;

        [Header("实例设置")]
        [SerializeField] private Utils.CameraFollower buffSourceUnitCamera;
        [SerializeField] private Utils.CameraFollower currentUnitCamera;


        private Units.Unit currentUnit;

        public void Awake()
        {
            closeButton.onClick.AddListener(HideUnitInfo);
        }

        public void HideUnitInfo()
        {
            currentUnit = null;
            gameObject.SetActive(false);
            OnClosed?.Invoke();
        }

        public void ShowUnitInfo(Units.Unit unit)
        {
            currentUnit = unit;
            gameObject.SetActive(true);
            RefreshInfo();
            unit.BuffAdded += HandleUnitBuffChanged;
            unit.BuffRemoved += HandleUnitBuffChanged;
            
        }
        
        private void HandleUnitBuffChanged(Units.Buffs.Buff buff)
        {
            RefreshBuffInfo();
        }

        private void RefreshBuffInfo()
        {
            SetDetailText(""); // 清空描述区域
            if (currentUnit == null) return;

            foreach (Transform child in buffRoot)
                Destroy(child.gameObject);

            List<Units.Buffs.Buff> buffs = currentUnit.GetActiveBuffs();

            foreach (Units.Buffs.Buff buff in buffs)
            {
                UI.UnitInfo.Buff buffInfo = Instantiate(buffInfoPrefab, buffRoot);
                buffInfo.SetBuff(buff);
                buffInfo.OnBuffClicked += ShowBuffDescription;
            }
            
        }

        private void RefreshSkillInfo()
        {
            SetDetailText(""); // 清空描述区域
            foreach (Transform child in skillRoot)
                Destroy(child.gameObject);
            IReadOnlyList<Units.Skills.Skill> skills = currentUnit.GetSkills();
            foreach (Units.Skills.Skill skill in skills)
            {
                UI.UnitInfo.Skill skillInfo = Instantiate(unitSkillPrefab, skillRoot);
                skillInfo.SetSkill(skill);
                skillInfo.OnClicked += ShowSkillDescription;
            }
        }

        private void RefreshAttributes()
        {
            if (currentUnit == null) return;

            Units.Attributes attributes = currentUnit.Attributes;

            healthAttribute.BindAttributes(attributes);
            moveSpeedAttribute.SetAttribute(attributes.MoveSpeed);
            attackPowerAttribute.SetAttribute(attributes.AttackPower);
            attacksPerTenSecondsAttribute.SetAttribute(attributes.AttacksPerTenSeconds);
            energyPerSecondAttribute.SetAttribute(attributes.EnergyPerSecond);
            attackRangeAttribute.SetAttribute(attributes.AttackRange);
        }
        private void RefreshAffinity()
        {
            if (currentUnit == null) return;

            foreach (Transform child in affinityRoot)
                Destroy(child.gameObject);

            Dictionary<Model.Tetri.AffinityType, int> affinities = currentUnit.CellCounts;
            foreach (var affinity in affinities
                         .Where(pair => pair.Key != Model.Tetri.AffinityType.None && pair.Value > 0)
                         .OrderByDescending(pair => pair.Value)
                         .ThenBy(pair => pair.Key))
            {
                UI.UnitInfo.Affinity affinityInfo = Instantiate(affinityInfoPrefab, affinityRoot);
                affinityInfo.SetAffinity(affinity.Key, affinity.Value);
                affinityInfo.OnClicked += ShowAffinityDescription;
            }
        }

        private void RefreshInfo()
        {
            currentUnitCamera.SetTarget(currentUnit.transform);

            RefreshAttributes();
            RefreshBuffInfo();
            RefreshSkillInfo();
            RefreshAffinity();
        }

        public void ShowBuffDescription(Units.Buffs.Buff buff)
        {
            SetDetailText("<b>" + buff.Name() + "</b>\n" + buff.Description());

            // 部分 Buff 没有来源单位或未配置来源相机，描述展示不应因此失败。
            if (buffSourceUnitCamera != null && buff.SourceUnit != null)
            {
                buffSourceUnitCamera.SetTarget(buff.SourceUnit.transform);
            }
        }
        public void ShowSkillDescription(Units.Skills.Skill skill)
        {
            string desc = skill.Description();

            // 替换自定义标签为富文本颜色
            desc = desc.Replace("<base>", "<color=#FFFFFF>")
                    .Replace("</base>", "</color>")
                    .Replace("<bonus>", "<color=#00FF66>")
                    .Replace("</bonus>", "</color>")
                    .Replace("<debuff>", "<color=#FF5555>")
                    .Replace("</debuff>", "</color>")
                    .Replace("<final>", "<color=#FFD700>")
                    .Replace("</final>", "</color>");

            SetDetailText("<b>" + skill.Name() + "</b>\n" + desc);
        }

        private void ShowAffinityDescription(AffinityType type, int count)
        {
            string affinityName = GetAffinityDisplayName(type);
            string title = $"<b>{affinityName}加成 (X{count})</b>";
            string description = GetAffinityDescription(type);
            SetDetailText(title + "\n" + description);
        }

        private string GetAffinityDisplayName(AffinityType type)
        {
            return affinityResourceMapping.GetName(type);
        }

        private string GetAffinityDescription(AffinityType type)
        {
            return affinityResourceMapping.GetDescription(type);
        }

        private void SetDetailText(string text)
        {
            if (skillDescriptionText != null)
            {
                skillDescriptionText.text = text;
            }

            if (buffDescriptionText != null && buffDescriptionText != skillDescriptionText)
            {
                buffDescriptionText.text = text;
            }
        }
    }
}