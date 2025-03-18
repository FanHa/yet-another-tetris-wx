using System;
using System.Collections.Generic;
using Model.Tetri;

namespace Model.Tetri
{
    public class TetriCellFactory
    {
        private readonly Random _random = new Random();

        public TetriCell CreateBasicCell()
        {
            var possibleCells = new List<TetriCell>
            {
                new TetriCellCharacterCircle(),
                new TetriCellCharacterTriangle(),
                new TetriCellCharacterSquare()
            };
            return possibleCells[_random.Next(possibleCells.Count)];
        }

    }
}
