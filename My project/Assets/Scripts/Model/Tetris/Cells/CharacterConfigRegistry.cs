using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "ConfigRegistry/CharacterConfigRegistry")]
    public class CharacterConfigRegistry : ScriptableObject
    {
        [Header("角色基础属性")]
        public CharacterBaseStatConfig SquareCharacterBaseStatConfig;
        public CharacterBaseStatConfig TriangleCharacterBaseStatConfig;
        public CharacterBaseStatConfig CircleCharacterBaseStatConfig;
        public CharacterBaseStatConfig AimCharacterBaseStatConfig;
        public CharacterBaseStatConfig HourglassCharacterBaseStatConfig;
    }
}