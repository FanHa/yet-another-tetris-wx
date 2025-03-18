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

        public void CreateTShape(Tetri tetri)
        {
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(1, 2, CreateBasicCell());
            tetri.SetCell(0, 1, CreateBasicCell());
        }

        public void CreateIShape(Tetri tetri)
        {
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(3, 1, CreateBasicCell());
        }

        public void CreateOShape(Tetri tetri)
        {
            tetri.SetCell(0, 0, CreateBasicCell());
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
        }

        public void CreateLShape(Tetri tetri)
        {
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 2, CreateBasicCell());
        }

        public void CreateJShape(Tetri tetri)
        {
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 0, CreateBasicCell());
        }

        public void CreateSShape(Tetri tetri)
        {
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 2, CreateBasicCell());
        }

        public void CreateZShape(Tetri tetri)
        {
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(1, 2, CreateBasicCell());
            tetri.SetCell(2, 0, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
        }
    }
}
