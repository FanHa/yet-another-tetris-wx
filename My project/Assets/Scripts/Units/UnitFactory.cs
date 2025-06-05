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

        public Units.Unit CreateUnit(InventoryItem item)
        {
            var go = Object.Instantiate(unitPrefab);
            var unit = go.GetComponent<Units.Unit>();
            if (unit == null)
                return null;

            // 基础外观和数据初始化
            var characterSprite = resourceMapping.GetSprite(item.CharacterCell);
            unit.BodySpriteRenderer.sprite = characterSprite;
            unit.Fist1SpriteRenderer.sprite = characterSprite;
            unit.Fist2SpriteRenderer.sprite = characterSprite;

            item.CharacterCell.Apply(unit);

            if (item.TetriCells != null)
            {
                foreach (var cell in item.TetriCells)
                {
                    cell.Apply(unit);
                }

                foreach (var cell in item.TetriCells)
                {
                    cell.PostApply(unit, item.TetriCells);
                }
            }

            return unit;
        }
    }
}