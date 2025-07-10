using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.UnitInfo
{
    public class UnitInfo : MonoBehaviour
    {
        [SerializeField] private Utils.CameraFollower unitInfoCamera;
        [SerializeField] private GameObject panel;

        [SerializeField] private Transform attributeRoot;
        [SerializeField] private UI.UnitInfo.Attribute unitAttributePrefab; 

        public void ShowUnitInfo(Units.Unit unit)
        {
            unitInfoCamera.SetTarget(unit.transform);
            panel.SetActive(true);
            Units.Attributes attributes = unit.Attributes;
            var AttributeToShow = new List<Units.Attribute>(
                new[]
                {
                    attributes.MoveSpeed,
                    attributes.AttackPower,
                    attributes.MaxHealth,
                    attributes.AttacksPerTenSeconds,
                    attributes.EnergyPerTick
                }
            );
            foreach (var attribute in AttributeToShow)
            {
                var instantiatedAttribute = Instantiate(unitAttributePrefab, attributeRoot);
                instantiatedAttribute.SetAttribute(attribute.Name, attribute.finalValue.ToString());
            }
        }
    }
}