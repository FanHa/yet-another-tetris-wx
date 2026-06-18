using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Actions
{
    public sealed class UnitActionRunner
    {
        private readonly Unit owner;
        private readonly List<UnitAction> actions;
        private UnitAction currentAction;

        private bool HasCurrentAction => currentAction != null;

        public UnitActionRunner(Unit owner)
        {
            this.owner = owner;
            actions = CreateDefaultActions(owner)
                .OrderByDescending(action => action.Priority)
                .ToList();
        }

        public void Tick()
        {
            SynchronizeOwnerState();
            TryStartHighestPriority();

            if (!HasCurrentAction)
            {
                return;
            }

            currentAction.Tick();
            if (currentAction.IsCompleted)
            {
                ExitCurrent();
            }
        }

        private void SynchronizeOwnerState()
        {
            if (!HasCurrentAction)
            {
                return;
            }

            if (ShouldYieldToOwnerState(currentAction))
            {
                CancelCurrentInternal();
            }
        }

        private void TryStartHighestPriority()
        {
            foreach (var action in actions)
            {
                if (TryStart(action))
                {
                    return;
                }
            }
        }

        private bool TryStart(UnitAction action)
        {
            if (!action.CanStart())
            {
                return false;
            }

            if (HasCurrentAction)
            {
                if (!action.CanPreempt(currentAction))
                {
                    return false;
                }

                CancelCurrentInternal();
            }

            currentAction = action;
            currentAction.Enter();

            if (currentAction.IsCompleted)
            {
                ExitCurrent();
            }

            return true;
        }

        

        public void NotifyAnimationEvent(AnimationEventType eventType)
        {
            if (!HasCurrentAction)
            {
                Debug.LogWarning($"[UnitActionRunner] Animation event dropped (no current action). unit={owner.name}, event={eventType}, time={Time.time:F3}");
                return;
            }

            if (currentAction is IAnimationEventHandler handler)
            {
                handler.HandleAnimationEvent(eventType);
                return;
            }

            Debug.LogWarning($"[UnitActionRunner] Animation event dropped (action not handler). unit={owner.name}, action={currentAction.GetType().Name}, event={eventType}, time={Time.time:F3}");
        }

        public void OnOwnerDeactivated()
        {
            if (!HasCurrentAction)
            {
                return;
            }

            CancelCurrentInternal();
        }

        private void ExitCurrent()
        {
            var action = currentAction;
            currentAction = null;
            action.Exit();
        }

        private void CancelCurrentInternal()
        {
            var action = currentAction;
            currentAction = null;
            action.Cancel();
        }

        private bool ShouldYieldToOwnerState(UnitAction action)
        {
            if (owner.IsStunned && action.Type != UnitActionType.Stun)
            {
                return true;
            }

            if (owner.IsSkillMotionActive && action.Type != UnitActionType.SkillMotion)
            {
                return true;
            }

            return false;
        }

        

        private static IEnumerable<UnitAction> CreateDefaultActions(Unit owner)
        {
            yield return new StunAction(owner);
            yield return new SkillMotionAction(owner);
            yield return new CastSkillAction(owner);
            yield return new AttackAction(owner);
            yield return new MoveAction(owner);
        }
    }
}
