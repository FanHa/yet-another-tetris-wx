using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Units/UnitFactory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private TetriCellTypeResourceMapping resourceMapping;

        public Units.Unit CreateUnit(UnitInventoryItem item)
        {
            var go = Object.Instantiate(unitPrefab);
            var unit = go.GetComponent<Units.Unit>();

            // 基础外观和数据初始化
            var characterSprite = resourceMapping.GetSprite(item.CharacterCell);
            unit.BodySpriteRenderer.sprite = characterSprite;
            unit.Fist1SpriteRenderer.sprite = characterSprite;
            unit.Fist2SpriteRenderer.sprite = characterSprite;

            var cellCounts = new Dictionary<AffinityType, int>();
            foreach (var cell in item.TetriCells)
            {
                if (!cellCounts.ContainsKey(cell.Affinity))
                    cellCounts[cell.Affinity] = 0;
                cellCounts[cell.Affinity]++;
            }
            unit.CellCounts = cellCounts;

            item.CharacterCell.Apply(unit);
            foreach (var cell in item.TetriCells)
            {
                cell.Apply(unit);
            }
            
            foreach (Units.Skills.Skill skill in unit.GetSkills())
            {
                if (skill is Units.Skills.IPassiveSkill passive)
                {
                    passive.ApplyPassive(unit);
                }
            }

            return unit;
        }
    }
}