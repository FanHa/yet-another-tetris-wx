using Model.Tetri;
using UnityEngine;

namespace UI{
    public class TetrisResourceItemFactory
    {
        public static TetrisResourceItem CreateInstance(GameObject prefab, Transform parent, Tetri tetri)
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            TetrisResourceItem resourceItem = instance.GetComponent<TetrisResourceItem>();
            if (resourceItem != null)
            {
                resourceItem.Initialize(tetri);
            }
            return resourceItem;
        }
    }
}
