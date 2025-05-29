using Model;
using Model.Tetri;
using UnityEngine;

namespace Units
{
    public class UnitFactory
    {
        private readonly TetriCellTypeResourceMapping resourceMapping;
        private readonly GameObject unitPrefab;
        public UnitFactory(GameObject unitPrefab, TetriCellTypeResourceMapping resourceMapping)
        {
            this.unitPrefab = unitPrefab;
            this.resourceMapping = resourceMapping;
        }

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

            Model.Tetri.GarbageReuse garbageReuseCell = null;
            if (item.TetriCells != null)
            {
                foreach (var cell in item.TetriCells)
                {
                    cell.Apply(unit);
                    if (cell is Model.Tetri.GarbageReuse garbageReuse)
                        garbageReuseCell = garbageReuse;
                }
            }
            garbageReuseCell?.Reuse(item.TetriCells, unit);

            // unit.Initialize();

            return unit;
        }
    }
}