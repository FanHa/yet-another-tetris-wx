using Model.Tetri;
using UnityEngine;

namespace Model
{
    public class UnitSpawnPlacement
    {
        public CharacterBuild CharacterBuild { get; }
        public Vector3 SpawnOffset { get; }

        public UnitSpawnPlacement(CharacterBuild characterBuild, Vector3 spawnOffset)
        {
            CharacterBuild = characterBuild;
            SpawnOffset = spawnOffset;
        }
    }
}