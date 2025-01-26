using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model{
    [CreateAssetMenu(fileName = "TetrisListTemplate", menuName = "ScriptableObjects/TetrisListTemplate", order = 1)]
    public class TetrisListTemplate : ScriptableObject
    {
        [SerializeField]
        public List<Tetri.Tetri> template = new List<Tetri.Tetri>(); // 未使用的Tetri列表模板

    }
}