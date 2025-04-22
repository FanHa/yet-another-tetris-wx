using System.Collections.Generic;
using Model;
using Units;
using UnityEngine;

namespace Model.TrainGround
{
    [CreateAssetMenu(fileName = "TrainGroundSetup", menuName = "ScriptableObjects/TrainGroundSetup", order = 1)]
    public class Setup : ScriptableObject
    {
        [Header("Faction A Units")]
        public List<UnitConfig> FactionAUnits; // FactionA 的单位列表

        [Header("Faction B Units")]
        public List<UnitConfig> FactionBUnits;

    }
}
