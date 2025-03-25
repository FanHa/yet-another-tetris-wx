using System;

namespace Model.Tetri
{
    public class TetrisFactory {
        private TetriCellFactory cellFactory = new TetriCellFactory();

        public Tetri CreateTShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(1, 0, cellFactory.CreatePadding());
            tetri.SetCell(1, 1, cellFactory.CreatePadding());
            tetri.SetCell(1, 2, cellFactory.CreatePadding());
            tetri.SetCell(0, 1, cellFactory.CreatePadding());
            return tetri;
        }

        public Tetri CreateIShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 1, cellFactory.CreatePadding());
            tetri.SetCell(1, 1, cellFactory.CreatePadding());
            tetri.SetCell(2, 1, cellFactory.CreatePadding());
            tetri.SetCell(3, 1, cellFactory.CreatePadding());
            return tetri;
        }

        public Tetri CreateOShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 0, cellFactory.CreatePadding());
            tetri.SetCell(0, 1, cellFactory.CreatePadding());
            tetri.SetCell(1, 0, cellFactory.CreatePadding());
            tetri.SetCell(1, 1, cellFactory.CreatePadding());
            return tetri;
        }

        public Tetri CreateLShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 1, cellFactory.CreatePadding());
            tetri.SetCell(1, 1, cellFactory.CreatePadding());
            tetri.SetCell(2, 1, cellFactory.CreatePadding());
            tetri.SetCell(2, 2, cellFactory.CreatePadding());
            return tetri;
        }

        public Tetri CreateJShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 1, cellFactory.CreatePadding());
            tetri.SetCell(1, 1, cellFactory.CreatePadding());
            tetri.SetCell(2, 1, cellFactory.CreatePadding());
            tetri.SetCell(2, 0, cellFactory.CreatePadding());
            return tetri;
        }

        public Tetri CreateSShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(1, 0, cellFactory.CreatePadding());
            tetri.SetCell(1, 1, cellFactory.CreatePadding());
            tetri.SetCell(2, 1, cellFactory.CreatePadding());
            tetri.SetCell(2, 2, cellFactory.CreatePadding());
            return tetri;
        }

        public Tetri CreateZShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(1, 1, cellFactory.CreatePadding());
            tetri.SetCell(1, 2, cellFactory.CreatePadding());
            tetri.SetCell(2, 0, cellFactory.CreatePadding());
            tetri.SetCell(2, 1, cellFactory.CreatePadding());
            return tetri;
        }

        public Tetri CreateRandomShape()
        {
            return new Random().Next(7) switch
            {
                0 => CreateTShape(),
                1 => CreateIShape(),
                2 => CreateOShape(),
                3 => CreateLShape(),
                4 => CreateJShape(),
                5 => CreateSShape(),
                6 => CreateZShape(),
                _ => CreateTShape() // Default case
            };
        }
    }
}