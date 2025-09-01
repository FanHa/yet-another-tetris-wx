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
            GameObject go = Object.Instantiate(unitPrefab);
            Units.Unit unit = go.GetComponent<Units.Unit>();

            // 基础外观和数据初始化
            Sprite characterSprite = resourceMapping.GetSprite(item.CharacterCell);
            unit.BodySpriteRenderer.sprite = characterSprite;
            unit.Fist1SpriteRenderer.sprite = characterSprite;
            unit.Fist2SpriteRenderer.sprite = characterSprite;

            item.CharacterCell.Apply(unit);
            foreach (var cell in item.TetriCells)
            {
                cell.Apply(unit);
            }

            var cellCounts = new Dictionary<AffinityType, int>();
            foreach (var cell in item.TetriCells)
            {
                if (!cellCounts.ContainsKey(cell.Affinity))
                    cellCounts[cell.Affinity] = 0;
                cellCounts[cell.Affinity]++;
            }
            unit.SetCellAffinity(cellCounts);

            foreach (Units.Skills.Skill skill in unit.GetSkills())
            {
                if (skill is Units.Skills.IPassiveSkill passive)
                {
                    passive.ApplyPassive();
                }
            }

            unit.Attributes.RefillHealthToMax();

            return unit;
        }
        
        public Units.Unit CreateUnit(Model.Tetri.Character characterCell)
        {
            var item = new Model.UnitInventoryItem(characterCell, new List<Model.Tetri.Cell>());
            return CreateUnit(item);
        }
    }
}