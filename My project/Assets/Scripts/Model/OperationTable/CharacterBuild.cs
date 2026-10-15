using System.Collections.Generic;
using Model.Tetri;

namespace Model
{
    public class CharacterInfluence
    {
        public Character Character { get; }
        public List<Cell> InfluencedCells { get; }
        public Model.Tetri.Tetri OwnerTetri { get; }
        public CharacterInfluence(Character character, List<Cell> influencedCells, Model.Tetri.Tetri ownerTetri)
        {
            Character = character;
            InfluencedCells = influencedCells ?? new List<Cell>();
            OwnerTetri = ownerTetri;
        }
    }
}