using UnityEngine;
using UnityEngine.Tilemaps;

namespace Model
{
    [CreateAssetMenu(fileName = "Brick", menuName = "ScriptableObjects/Brick", order = 1)]
    public class Brick : ScriptableObject
    {
        [SerializeField] private TileBase tile; // 砖块的Tile

        public TileBase Tile => tile; // 只读属性

    }
}
