using UnityEngine;

namespace Model
{
    public class PlacedTetri
    {
        public Tetri.Tetri Tetri { get; }
        public Vector2Int Position { get; }
        public PlacedTetri(Tetri.Tetri tetri, Vector2Int position)
        {
            Tetri = tetri;
            Position = position;
        }
    }
}