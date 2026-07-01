using Units.Actions;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(AnimationController))]
    public class UnitActionFeedbackListener : MonoBehaviour
    {
        [Header("Optional Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip attackStartSfx;
        [SerializeField] private AudioClip castSkillStartSfx;
        [SerializeField] private AudioClip attackCommittedSfx;
        [SerializeField] private AudioClip skillCommittedSfx;

        [Header("Optional VFX")]
        [SerializeField] private GameObject attackCommittedVfx;
        [SerializeField] private GameObject skillCommittedVfx;

        private Unit unit;
        private AnimationController animationController;

        private void Awake()
        {
            unit = GetComponent<Unit>();
            animationController = GetComponent<AnimationController>();
        }

        private void OnEnable()
        {
            unit.OnActionStarted += HandleActionStarted;
            unit.OnActionCanceled += HandleActionStopped;
            unit.OnActionInterrupted += HandleActionStopped;
            unit.OnActionCommitted += HandleActionCommitted;
        }

        private void OnDisable()
        {
            if (unit == null)
            {
                return;
            }

            unit.OnActionStarted -= HandleActionStarted;
            unit.OnActionCanceled -= HandleActionStopped;
            unit.OnActionInterrupted -= HandleActionStopped;
            unit.OnActionCommitted -= HandleActionCommitted;
        }

        private void HandleActionStarted(Unit sender, UnitActionType actionType)
        {
            switch (actionType)
            {
                case UnitActionType.Attack:
                    animationController.PlayAttack();
                    PlaySfx(attackStartSfx);
                    break;
                case UnitActionType.CastSkill:
                    animationController.PlayCastSkill();
                    PlaySfx(castSkillStartSfx);
                    break;
            }
        }

        private void HandleActionStopped(Unit sender, UnitActionType actionType)
        {
            switch (actionType)
            {
                case UnitActionType.Attack:
                case UnitActionType.CastSkill:
                    animationController.StopAction();
                    break;
            }
        }

        private void HandleActionCommitted(Unit sender, UnitActionType actionType, UnitActionCommitKind commitKind)
        {
            switch (commitKind)
            {
                case UnitActionCommitKind.AttackProjectileLaunched:
                    SpawnVfx(attackCommittedVfx, unit.projectileSpawnPoint != null ? unit.projectileSpawnPoint.position : unit.transform.position);
                    PlaySfx(attackCommittedSfx);
                    break;
                case UnitActionCommitKind.SkillExecuted:
                    SpawnVfx(skillCommittedVfx, unit.transform.position);
                    PlaySfx(skillCommittedSfx);
                    break;
            }
        }

        private void PlaySfx(AudioClip clip)
        {
            if (audioSource == null || clip == null)
            {
                return;
            }

            audioSource.PlayOneShot(clip);
        }

        private static void SpawnVfx(GameObject prefab, Vector3 position)
        {
            if (prefab == null)
            {
                return;
            }

            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
