using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Model;

namespace Model
{
    [CustomEditor(typeof(TetrisListTemplate))]
    public class TetrisListTemplateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TetrisListTemplate tetrisListTemplate = (TetrisListTemplate)target;

            if (GUILayout.Button("生成数据"))
            {
                GenerateAllShapes(tetrisListTemplate);
                Debug.Log("生成数据成功");
            }

            // 添加“打印数据到控制台”按钮
            if (GUILayout.Button("打印数据到控制台"))
            {
                Debug.Log(tetrisListTemplate.template);
            }
        }
        
        private void GenerateAllShapes(TetrisListTemplate tetrisListTemplate)
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
                createShape(tetri);
                tetrisListTemplate.template.Add(tetri);
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