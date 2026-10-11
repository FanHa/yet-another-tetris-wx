using Units.Damages;
using Units.Skills;
using UnityEngine;

namespace Units.UI
{
    [CreateAssetMenu(menuName = "Game Data/Presentation/Floating Text Service")]
    public sealed class FloatingTextService : ScriptableObject
    {
        [SerializeField] private FloatingTextView damageTextPrefab;
        [SerializeField] private FloatingTextView skillNameTextPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (damageTextPrefab == null)
                Debug.LogError($"{nameof(damageTextPrefab)} is not assigned.", this);
            if (skillNameTextPrefab == null)
                Debug.LogError($"{nameof(skillNameTextPrefab)} is not assigned.", this);
        }
#endif

        public void ShowDamage(Damage damage, Vector3 worldPosition)
        {
            string sourceLabel = damage.Type == DamageType.Hit ? string.Empty : damage.SourceLabel;
            string text = sourceLabel + " " + Mathf.RoundToInt(damage.Value);
            Show(damageTextPrefab, text, worldPosition);
        }

        public void ShowSkillCast(Skill skill, Vector3 worldPosition)
        {
            Show(skillNameTextPrefab, skill.Name(), worldPosition);
        }

        private static void Show(FloatingTextView prefab, string text, Vector3 worldPosition)
        {
            FloatingTextView floatingText = Instantiate(prefab, worldPosition, Quaternion.identity);
            floatingText.Initialize(text, worldPosition);
        }
    }
}