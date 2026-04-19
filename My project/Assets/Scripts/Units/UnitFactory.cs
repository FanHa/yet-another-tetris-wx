using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Units/UnitFactory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Units.Unit battleUnitPrefab;
        [SerializeField] private Units.Unit previewUnitPrefab;
        [SerializeField] private TetriCellTypeResourceMapping resourceMapping;

        public Units.Unit CreateBattleUnit(CharacterInfluence influence)
        {
            return CreateUnitInternal(battleUnitPrefab, influence);
        }

        public Units.Unit CreatePreviewUnit(CharacterInfluence influence)
        {
            return CreateUnitInternal(previewUnitPrefab, influence);
        }

        public Units.Unit CreateBattleUnit(Model.Tetri.Character characterCell)
        {
            CharacterInfluence influence = new CharacterInfluence(characterCell, new List<Model.Tetri.Cell>(), null);
            return CreateBattleUnit(influence);
        }

        public Units.Unit CreatePreviewUnit(Model.Tetri.Character characterCell)
        {
            CharacterInfluence influence = new CharacterInfluence(characterCell, new List<Model.Tetri.Cell>(), null);
            return CreatePreviewUnit(influence);
        }

        private Units.Unit CreateUnitInternal(Units.Unit prefab, CharacterInfluence influence)
        {
            Units.Unit unit = Object.Instantiate(prefab);

            // 基础外观和数据初始化
            Sprite characterSprite = resourceMapping.GetSprite(influence.Character);
            unit.BodySpriteRenderer.sprite = characterSprite;
            unit.Fist1SpriteRenderer.sprite = characterSprite;
            unit.Fist2SpriteRenderer.sprite = characterSprite;

            influence.Character.Apply(unit);
            foreach (var cell in influence.InfluencedCells)
            {
                cell.Apply(unit);
            }

            var cellCounts = new Dictionary<AffinityType, int>();
            foreach (var cell in influence.InfluencedCells)
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
    }
}