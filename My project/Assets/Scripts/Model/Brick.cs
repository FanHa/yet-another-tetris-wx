using Model.Tetri;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Model
{
    [System.Serializable]
    public class Brick
    {
        // [SerializeField] private TileBase tile; // 砖块的Tile
        [SerializeField] private TetriCell cell; // 砖块包含的Cell

        public TetriCell Cell => cell; // 只读属性
        // public TileBase Tile => tile; // 只读属性

        public Brick(TetriCell cell)
        {
            this.cell = cell;
        }

    }
}
