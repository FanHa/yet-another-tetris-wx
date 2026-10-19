using System.Collections.Generic;
using UnityEngine;

namespace Model.Resonance
{
    [CreateAssetMenu(menuName = "Game Data/Shared/Resonance Database")]
    public sealed class ResonanceDatabase : ScriptableObject
    {
        [SerializeField] private ResonanceDefinition[] definitions = System.Array.Empty<ResonanceDefinition>();

        private Dictionary<ResonanceType, ResonanceDefinition> definitionsByType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (definitions == null)
            {
                Debug.LogError($"{nameof(definitions)} is not assigned.", this);
                return;
            }

            var seenTypes = new HashSet<ResonanceType>();
            for (int index = 0; index < definitions.Length; index++)
            {
                var definition = definitions[index];
                if (definition == null)
                {
                    Debug.LogError($"{nameof(definitions)}[{index}] is not assigned.", this);
                    continue;
                }

                if (!seenTypes.Add(definition.Type))
                {
                    Debug.LogError($"Duplicate resonance definition for '{definition.Type}'.", this);
                }
            }

            foreach (ResonanceType type in System.Enum.GetValues(typeof(ResonanceType)))
            {
                if (type != ResonanceType.None && type != ResonanceType.End && !seenTypes.Contains(type))
                {
                    Debug.LogError($"Missing resonance definition for '{type}'.", this);
                }
            }
        }
#endif

        private void OnEnable()
        {
            definitionsByType = new Dictionary<ResonanceType, ResonanceDefinition>();
            foreach (var definition in definitions)
            {
                definitionsByType.Add(definition.Type, definition);
            }
        }

        public ResonanceDefinition GetDefinition(ResonanceType type)
        {
            return definitionsByType[type];
        }
    }
}