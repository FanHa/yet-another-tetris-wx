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
        FrostZone,
        IceShield,
        IcyCage,
        Snowball,
        Fireball,
        FlameInject,
        BlazingField,
        FlameRing,
        Padding,
        None,
        WindShift,
        WildWind,
        AttackBoost,
        HitAndRun
    }

    public enum CharacterTypeId
    {
        Square,
        Triangle,
        Circle,
        Aim
    }
}

