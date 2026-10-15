using System.Collections.Generic;
using Model.Tetri;

namespace Model
{
    public class CharacterBuild
    {
        public Character Character { get; }
        public List<Cell> Cells { get; }
        public Model.Tetri.Tetri OwnerTetri { get; }
        public CharacterBuild(Character character, List<Cell> cells, Model.Tetri.Tetri ownerTetri)
        {
            Character = character;
            Cells = cells ?? new List<Cell>();
            OwnerTetri = ownerTetri;
        }
    }
}