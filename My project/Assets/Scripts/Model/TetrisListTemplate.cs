using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model{
    [CreateAssetMenu(fileName = "TetrisListTemplate", menuName = "ScriptableObjects/TetrisListTemplate", order = 1)]
    public class TetrisListTemplate : ScriptableObject
    {
        public List<Tetri.Tetri> template = new List<Tetri.Tetri>(); // 未使用的Tetri列表模板

        // public Tetri.Tetri tetri = new();

    }
}