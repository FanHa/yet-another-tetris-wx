using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Game Data/Bootstrap/Factories/Unit Factory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Units.Unit battleUnitPrefab;
        [SerializeField] private Units.Unit previewUnitPrefab;

        public Units.Unit CreateBattleUnit(CharacterBuild build)
        {
            return CreateUnitInternal(battleUnitPrefab, build);
        }

        public Units.Unit CreatePreviewUnit(CharacterBuild build)
        {
            return CreateUnitInternal(previewUnitPrefab, build);
        }

        public Units.Unit CreateBattleUnit(Model.Tetri.Character characterCell)
        {
            CharacterBuild build = new CharacterBuild(characterCell, new List<Model.Tetri.Cell>(), null);
            return CreateBattleUnit(build);
        }

        public Units.Unit CreatePreviewUnit(Model.Tetri.Character characterCell)
        {
            CharacterBuild build = new CharacterBuild(characterCell, new List<Model.Tetri.Cell>(), null);
            return CreatePreviewUnit(build);
        }

        private Units.Unit CreateUnitInternal(Units.Unit prefab, CharacterBuild build)
        {
            Units.Unit unit = Object.Instantiate(prefab);

            // 基础外观和数据初始化
            Sprite characterSprite = build.Character.Icon;
            unit.BodySpriteRenderer.sprite = characterSprite;
            unit.Fist1SpriteRenderer.sprite = characterSprite;
            unit.Fist2SpriteRenderer.sprite = characterSprite;

            build.Character.Apply(unit);
            foreach (var cell in build.Cells)
            {
                cell.Apply(unit);
            }

            var cellCounts = new Dictionary<AffinityType, int>();
            foreach (var cell in build.Cells)
            {
                if (!cellCounts.ContainsKey(cell.Affinity))
                    cellCounts[cell.Affinity] = 0;
                cellCounts[cell.Affinity]++;
            }
            unit.SetCellAffinity(cellCounts);
            unit.CompleteAssembly();

            return unit;
        }
    }
}