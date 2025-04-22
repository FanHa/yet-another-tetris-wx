using System;
using System.Collections.Generic;

namespace Model.Tetri
{
    public class TetrisFactory
    {
        private TetriCellFactory cellFactory = new TetriCellFactory();

        private readonly Dictionary<string, List<(int, int)>> shapeDefinitions = new()
        {
            { "T", new List<(int, int)> { (1, 0), (1, 1), (1, 2), (0, 1) } },
            { "I", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (3, 1) } },
            { "O", new List<(int, int)> { (0, 0), (0, 1), (1, 0), (1, 1) } },
            { "L", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 2) } },
            { "J", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 0) } },
            { "S", new List<(int, int)> { (1, 0), (1, 1), (2, 1), (2, 2) } },
            { "Z", new List<(int, int)> { (1, 1), (1, 2), (2, 0), (2, 1) } }
        };

        public Tetri CreateShape(string shapeKey)
        {
            if (!shapeDefinitions.ContainsKey(shapeKey))
                throw new ArgumentException($"Invalid shape key: {shapeKey}");

            Tetri tetri = new Tetri(Tetri.TetriType.Normal);
            foreach (var (row, col) in shapeDefinitions[shapeKey])
            {
                tetri.SetCell(row, col, cellFactory.CreatePadding());
            }
            return tetri;
        }

        public Tetri CreateTShape() => CreateShape("T");
        public Tetri CreateIShape() => CreateShape("I");
        public Tetri CreateOShape() => CreateShape("O");
        public Tetri CreateLShape() => CreateShape("L");
        public Tetri CreateJShape() => CreateShape("J");
        public Tetri CreateSShape() => CreateShape("S");
        public Tetri CreateZShape() => CreateShape("Z");

        public Tetri CreateRandomBaseShape()
        {
            var shapeKeys = new List<string> { "T", "I", "O", "L", "J", "S", "Z" };
            var randomKey = shapeKeys[new Random().Next(shapeKeys.Count)];
            return CreateShape(randomKey);
        }

        public Tetri CreateCircleShape()
        {

            Tetri tetri = new Tetri(Tetri.TetriType.Normal);


            var circlePattern = new List<(int, int)>
            {
                (0, 0), (0, 1), (0, 2),
                (1, 0),         (1, 2),
                (2, 0), (2, 1), (2, 2)
            };            
            foreach (var (row, col) in circlePattern)
            {
                tetri.SetCell(row, col, cellFactory.CreatePadding());
            }
            return tetri;
        }
    }
}