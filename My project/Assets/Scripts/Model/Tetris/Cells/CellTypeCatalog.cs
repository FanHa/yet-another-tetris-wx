using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    public enum CellTypeId
    {
        // 维护规则：
        // 1) 禁止调整现有枚举项的数值。
        // 2) 新增类型只能追加在末尾。
        // 3) 废弃类型保留原数值，不要复用旧ID。
        FrostZone = 0,
        IceShield = 1,
        IcyCage = 2,
        Snowball = 3,
        Fireball = 4,
        FlameInject = 5,
        BlazingField = 6,
        FlameRing = 7,
        Padding = 8,
        None = 9,
        WindShift = 10,
        WildWind = 11,
        AttackBoost = 12,
        [Obsolete("Deprecated cell type. Keep this ID for compatibility only; do not use for new content.")]
        HitAndRun = 13,
        LifeBomb = 14,
        LifeShield = 15,
        LifePower = 16,
        LifeEcho = 17,
        ShadowAttack = 18,
        ShadowStep = 19,
        ShadowArrow = 20,
        Charge = 21,
        IceBreaker = 22,
        EnergyAbsorb = 23,
        ThunderStrike = 24

    }

    public enum CharacterTypeId
    {
        Square,
        Triangle,
        Circle,
        Aim,
        Hourglass
    }
}

