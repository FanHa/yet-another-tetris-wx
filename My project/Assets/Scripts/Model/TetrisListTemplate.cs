using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model{
    [CreateAssetMenu(fileName = "TetrisListTemplate", menuName = "ScriptableObjects/TetrisListTemplate", order = 1)]
    public class TetrisListTemplate : ScriptableObject
    {
        public List<Tetri.Tetri> template = new(); // 未使用的Tetri列表模板

        private void OnEnable()
        {   
            template = new();
            GenerateAllShapes();
        }
        private void GenerateAllShapes()
        {
            var shapes = new List<System.Action<Tetri.Tetri>>
            {
                CreateTShape,
                CreateIShape,
                CreateOShape,
                CreateLShape,
                CreateJShape,
                CreateSShape,
                CreateZShape
            };

            foreach (var createShape in shapes)
            {
                var tetri = new Tetri.Tetri();
                tetri.InitializeShape();
                createShape(tetri);
                template.Add(tetri);
            }
        }

        private void CreateTShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 0, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 2, new Tetri.TetriCellBasic());
            tetri.SetCell(0, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 1, new Tetri.TetriCellBasic());
        }

        private void CreateIShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(3, 1, new Tetri.TetriCellBasic());
        }

        private void CreateOShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 0, new Tetri.TetriCellBasic());
            tetri.SetCell(0, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 0, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 1, new Tetri.TetriCellBasic());
        }

        private void CreateLShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 2, new Tetri.TetriCellBasic());
        }

        private void CreateJShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 0, new Tetri.TetriCellBasic());
        }

        private void CreateSShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 0, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 2, new Tetri.TetriCellBasic());
        }

        private void CreateZShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 1, new Tetri.TetriCellBasic());
            tetri.SetCell(1, 2, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 0, new Tetri.TetriCellBasic());
            tetri.SetCell(2, 1, new Tetri.TetriCellBasic());
        }
    }
}