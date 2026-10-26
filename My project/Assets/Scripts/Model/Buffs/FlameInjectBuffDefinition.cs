using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Flame Inject")]
    public sealed class FlameInjectBuffDefinition : BuffDefinition
    {
        [SerializeField] private BurnBuffDefinition burnDefinition;

        public BurnBuffDefinition BurnDefinition => burnDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (burnDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(burnDefinition)} is not assigned.", this);
            }

        }
#endif
    }
}