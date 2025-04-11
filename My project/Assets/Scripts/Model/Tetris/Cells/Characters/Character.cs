using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Character : Cell
    {
        [SerializeField] private string characterName; // 永久角色名
        public string CharacterName => characterName; // 只读属性，获取角色名

        public Character()
        {
            // 生成唯一的角色名
            characterName = GenerateUniqueName();
        }
        private string GenerateUniqueName()
        {
            return $"{Name()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}"; // 生成基于角色类型和GUID的唯一名称
        }

    }
}