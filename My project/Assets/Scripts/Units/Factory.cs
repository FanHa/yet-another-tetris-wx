using System;
using System.Collections.Generic;
using UnityEngine;
using Model.Tetri;
using Model;

namespace Units
{
    public class Factory : MonoBehaviour
    {
        // 定义一个方法，根据传入的 List<Brick> 生成不同的 Unit
        public Unit CreateUnit(List<TetriCell> cells)
        {
            // 统计各个 Brick 的类型
            Dictionary<Type, int> cellTypeCounts = new Dictionary<Type, int>();
            foreach (var cell in cells)
            {
                var cellType = cell.GetType();
                if (cellTypeCounts.ContainsKey(cellType))
                {
                    cellTypeCounts[cellType]++;
                }
                else
                {
                    cellTypeCounts[cellType] = 1;
                }
            }

            // // 根据统计结果生成不同的 Unit
            Unit unit = new Unit();
            // foreach (var kvp in brickTypeCounts)
            // {
            //     Type cellType = kvp.Key;
            //     int count = kvp.Value;

            //     // 根据 cellType 和 count 设置 unit 的属性
            //     // 这里可以根据具体需求进行设置
            //     unit.AddCellTypeCount(cellType, count);
            // }

            return unit;
        }
    }
}