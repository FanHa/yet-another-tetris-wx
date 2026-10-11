using Units.Damages;
using Units.Skills;
using UnityEngine;

namespace Units.UI
{
    [RequireComponent(typeof(Unit))]
    public sealed class UnitFloatingTextPresenter : MonoBehaviour
    {
        [SerializeField] private FloatingTextService floatingTextService;

        private Unit unit;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (floatingTextService == null)
                Debug.LogError($"{nameof(floatingTextService)} is not assigned.", this);
        }
#endif

        private void Awake()
        {
            unit = GetComponent<Unit>();
        }

        private void OnEnable()
        {
            unit.OnDamageTaken += HandleDamageTaken;
            unit.OnSkillCast += HandleSkillCast;
        }

        private void OnDisable()
        {
            if (unit == null)
                return;

            unit.OnDamageTaken -= HandleDamageTaken;
            unit.OnSkillCast -= HandleSkillCast;
        }

        private void HandleDamageTaken(Damage damage)
        {
            if (unit.IsActive)
                floatingTextService.ShowDamage(damage, unit.transform.position);
        }

        private void HandleSkillCast(Unit caster, Skill skill)
        {
            if (unit.IsActive)
                floatingTextService.ShowSkillCast(skill, unit.transform.position);
        }
    }
}