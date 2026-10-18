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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (battleUnitPrefab == null)
            {
                Debug.LogError($"[{nameof(UnitFactory)}] Battle unit prefab is missing.", this);
            }

            if (previewUnitPrefab == null)
            {
                Debug.LogError($"[{nameof(UnitFactory)}] Preview unit prefab is missing.", this);
            }
        }
#endif

        public Units.Unit CreateBattleUnit(CharacterBuild build)
        {
            return CreateUnitInternal(battleUnitPrefab, build);
        }

        public Units.Unit CreatePreviewUnit(CharacterBuild build)
        {
            return CreateUnitInternal(previewUnitPrefab, build);
        }

        public Units.Unit CreateBattleUnit(Model.Tetri.Character character)
        {
            CharacterBuild build = new CharacterBuild(character, new List<Model.Tetri.Cell>(), null);
            return CreateBattleUnit(build);
        }

        public Units.Unit CreatePreviewUnit(Model.Tetri.Character character)
        {
            CharacterBuild build = new CharacterBuild(character, new List<Model.Tetri.Cell>(), null);
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
                var affinity = cell.Affinity;
                cellCounts.TryGetValue(affinity, out int count);
                cellCounts[affinity] = count + 1;
            }
            unit.SetCellAffinity(cellCounts);
            unit.CompleteAssembly();

            return unit;
        }
    }
}