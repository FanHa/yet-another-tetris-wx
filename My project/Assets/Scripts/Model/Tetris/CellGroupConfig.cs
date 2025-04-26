using System;
using System.Collections.Generic;
using System.Linq;
using UI.Inventories.Description;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CellGroupConfig", menuName = "ScriptableObjects/CellGroupConfig", order = 1)]
    public class CellGroupConfig : ScriptableObject
    {
        public enum Group
        {
            None,
            Attack,
            Speed,
            RangeAttack,
            Health,
            AttackFrequency,
            Ice,
            Wind,
        }
        [Serializable]
        public class GroupConfig
        {
            public string Description;
            public Group group;
            public List<CellTypeReference> cellTypes; // Group 包含的 CellType
        }

        public List<GroupConfig> Groups = new List<GroupConfig>();

        /// <summary>
        /// 获取指定 Cell 的所有 Group 名称
        /// </summary>
        public List<Group> GetGroupsForCell(Type cellType)
        {
            return Groups
                .Where(group => group.cellTypes.Any(cellRef => cellRef.Type == cellType))
                .Select(group => group.group)
                .ToList();
        }

        /// <summary>
        /// 获取指定 Group 下的所有 Cell 类型
        /// </summary>
        public List<Type> GetCellsForGroup(Group group)
        {
            var groupConfig = Groups.FirstOrDefault(g => g.group == group);
            return groupConfig?.cellTypes.Select(cellRef => cellRef.Type).ToList() ?? new List<Type>();
        }
    }
}