using Model.Tetri;
using UnityEngine;

namespace UI.TetrisResource{
    public class TetrisResourceItemFactory
    {
        public static TetrisResourceItem CreateInstance(
            GameObject prefab, Transform parent, Tetri tetri,
            TetriCellTypeResourceMapping tetriCellTypeSpriteMapping)
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            TetrisResourceItem resourceItem = instance.GetComponent<TetrisResourceItem>();
            if (resourceItem != null)
            {
                resourceItem.Initialize(tetri, tetriCellTypeSpriteMapping);
            }
            return resourceItem;
        }
    }
}
