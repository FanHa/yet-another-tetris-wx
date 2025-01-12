using UnityEngine;
using UnityEngine.Tilemaps;

namespace Model
{
    [System.Serializable]
    public class Brick
    {
        [SerializeField] private TileBase tile; // 砖块的Tile

        public TileBase Tile => tile; // 只读属性

        public Brick(TileBase tile)
        {
            this.tile = tile;
        }

    }
}
