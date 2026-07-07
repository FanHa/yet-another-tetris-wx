using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Buffs
{
    public class BuffHandler
    {
        private float buffTickTimer = 0f;
        private const float BUFF_TICK_INTERVAL = 1f;
        private global::Units.Buffs.IBuffContext owner;

        // Active Buff storage.
        private readonly List<Buff> activeBuffs = new List<Buff>();
        private readonly Dictionary<string, Buff> activeBuffByKey = new Dictionary<string, Buff>();

        // Lifecycle state storage owned by BuffHandler.
        private readonly Dictionary<Buff, BuffLifecycleState> buffStates = new Dictionary<Buff, BuffLifecycleState>();

        // Request queues (to be consumed by future commit pipeline).
        private readonly List<Buff> pendingAddQueue = new List<Buff>();
        private readonly List<Buff> pendingRemoveQueue = new List<Buff>();
        private IReadOnlyList<Buff> activeBuffsReadOnly;
        private bool isClearingAllBuffs = false;

        internal event Action<Buff> BuffAdded;
        internal event Action<Buff> BuffRemoved;

        public BuffHandler(global::Units.Buffs.IBuffContext owner)
        {
            this.owner = owner;
            activeBuffsReadOnly = activeBuffs.AsReadOnly();
        }

        public void Tick(float deltaTime)
        {
            if (activeBuffs.Count == 0 && pendingAddQueue.Count == 0 && pendingRemoveQueue.Count == 0)
                return;

            FlushPendingRequests();

            if (activeBuffs.Count == 0)
                return;

            buffTickTimer += deltaTime;
            if (buffTickTimer >= BUFF_TICK_INTERVAL)
            {
                buffTickTimer -= BUFF_TICK_INTERVAL;
                TickBuffInterval(BUFF_TICK_INTERVAL);
            }

            FlushPendingRequests();
        }

        private void TickBuffInterval(float interval)
        {
            var snapshot = RentActiveBuffSnapshot(out int count);
            try
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    Buff buff = snapshot[i];
                    buff.UpdateTime(interval);
                    if (buff is ITick tickBuff)
                    {
                        tickBuff.OnTick(owner);
                    }
                    if (buff.IsExpired())
                    {
                        EnqueueRemove(buff);
                    }
                }
            }
            finally
            {
                ReturnActiveBuffSnapshot(snapshot, count);
            }
        }


        public void ApplyBuff(Buff buff)
        {
            EnqueueAdd(buff);
        }

        public void RemoveBuff(Buff buff)
        {
            EnqueueRemove(buff);
        }

        public IReadOnlyList<Buff> GetActiveBuffsReadOnly()
        {
            return activeBuffsReadOnly;
        }

        public void DispatchAttackHit(Unit target, ref Damages.Damage damage)
        {
            var snapshot = RentActiveBuffSnapshot(out int count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Buff buff = snapshot[i];
                    if (buff is IAttackHitTrigger attackHitTrigger)
                    {
                        attackHitTrigger.OnAttackHit(owner, owner.SelfUnit, target, ref damage);
                    }
                }
            }
            finally
            {
                ReturnActiveBuffSnapshot(snapshot, count);
            }
        }

        public void DispatchTakeHit(Unit attacker, ref Damages.Damage damage)
        {
            var snapshot = RentActiveBuffSnapshot(out int count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Buff buff = snapshot[i];
                    if (buff is ITakeHitTrigger takeHitTrigger)
                    {
                        takeHitTrigger.OnTakeHit(owner, owner.SelfUnit, attacker, ref damage);
                    }
                }
            }
            finally
            {
                ReturnActiveBuffSnapshot(snapshot, count);
            }
        }

        public void DispatchAfterTakeDamage(ref Damages.Damage damage)
        {
            var snapshot = RentActiveBuffSnapshot(out int count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Buff buff = snapshot[i];
                    if (buff is IAfterTakeDamageTrigger afterTakeDamageTrigger)
                    {
                        afterTakeDamageTrigger.OnAfterTakeDamage(owner, ref damage);
                    }
                }
            }
            finally
            {
                ReturnActiveBuffSnapshot(snapshot, count);
            }
        }

        public void DispatchGlobalSkillCast(Skills.Skill skill)
        {
            var snapshot = RentActiveBuffSnapshot(out int count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Buff buff = snapshot[i];
                    if (buff is IGlobalSkillCastTrigger globalSkillCastTrigger)
                    {
                        globalSkillCastTrigger.OnGlobalSkillCast(owner, owner.SelfUnit, skill);
                    }
                }
            }
            finally
            {
                ReturnActiveBuffSnapshot(snapshot, count);
            }
        }

        public void RequestRemoveAllActiveBuffs()
        {
            var snapshot = RentActiveBuffSnapshot(out int count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    EnqueueRemove(snapshot[i]);
                }
            }
            finally
            {
                ReturnActiveBuffSnapshot(snapshot, count);
            }
        }

        private Buff[] RentActiveBuffSnapshot(out int count)
        {
            count = activeBuffs.Count;
            int rentSize = count > 0 ? count : 1;
            Buff[] snapshot = ArrayPool<Buff>.Shared.Rent(rentSize);
            for (int i = 0; i < count; i++)
            {
                snapshot[i] = activeBuffs[i];
            }

            return snapshot;
        }

        private static void ReturnActiveBuffSnapshot(Buff[] snapshot, int count)
        {
            Array.Clear(snapshot, 0, count);
            ArrayPool<Buff>.Shared.Return(snapshot);
        }

        public void RemoveAllActiveBuffsImmediately()
        {
            if (isClearingAllBuffs)
                return;

            isClearingAllBuffs = true;
            try
            {
                DiscardPendingAdds();
            RequestRemoveAllActiveBuffs();
            FlushPendingRequests();

                pendingAddQueue.Clear();
                pendingRemoveQueue.Clear();
            }
            finally
            {
                isClearingAllBuffs = false;
            }
        }

        private void DiscardPendingAdds()
        {
            if (pendingAddQueue.Count == 0)
                return;

            for (int i = 0; i < pendingAddQueue.Count; i++)
            {
                Buff buff = pendingAddQueue[i];
                if (buff == null)
                    continue;

                if (!IsTrackedAsActive(buff))
                {
                    SetState(buff, BuffLifecycleState.Removed);
                }
            }

            pendingAddQueue.Clear();
        }

        private void FlushPendingRequests()
        {
            int safetyCounter = 0;
            const int maxFlushIterations = 32;

            while ((pendingRemoveQueue.Count > 0 || pendingAddQueue.Count > 0) && safetyCounter < maxFlushIterations)
            {
                CommitRemovals();
                CommitAdds();
                safetyCounter++;
            }

            if (safetyCounter >= maxFlushIterations)
            {
                Debug.LogWarning("BuffHandler.FlushPendingRequests reached max iterations. Pending requests remain in queue.");
            }
        }

        private void CommitRemovals()
        {
            if (pendingRemoveQueue.Count == 0)
                return;

            var removals = new List<Buff>(pendingRemoveQueue);
            pendingRemoveQueue.Clear();

            for (int i = 0; i < removals.Count; i++)
            {
                Buff buff = removals[i];
                if (buff == null)
                    continue;

                int index = activeBuffs.IndexOf(buff);
                if (index < 0)
                    continue;

                SetState(buff, BuffLifecycleState.PendingRemove);
                buff.OnRemove();
                RemoveActiveBuffAt(index);
                BuffRemoved?.Invoke(buff);
                SetState(buff, BuffLifecycleState.Removed);
            }
        }

        private void CommitAdds()
        {
            if (pendingAddQueue.Count == 0)
                return;

            var adds = new List<Buff>(pendingAddQueue);
            pendingAddQueue.Clear();

            for (int i = 0; i < adds.Count; i++)
            {
                Buff buff = adds[i];
                if (buff == null)
                    continue;

                if (IsTrackedAsActive(buff))
                {
                    buff.RefreshDuration();
                    SetState(buff, BuffLifecycleState.Active);
                    continue;
                }

                SetState(buff, BuffLifecycleState.PendingAdd);
                buff.OnApply(owner);
                AddActiveBuff(buff);
                SetState(buff, BuffLifecycleState.Active);
                BuffAdded?.Invoke(buff);
            }
        }

        private void AddActiveBuff(Buff buff)
        {
            activeBuffs.Add(buff);
            activeBuffByKey[buff.GetKey()] = buff;
        }

        private void RemoveActiveBuffAt(int index)
        {
            Buff buff = activeBuffs[index];
            activeBuffs.RemoveAt(index);

            string key = buff.GetKey();
            if (activeBuffByKey.TryGetValue(key, out var mapped) && ReferenceEquals(mapped, buff))
            {
                activeBuffByKey.Remove(key);
            }
        }

        private BuffLifecycleState ResolveCurrentState(Buff buff)
        {
            if (pendingRemoveQueue.Contains(buff))
                return BuffLifecycleState.PendingRemove;

            if (pendingAddQueue.Contains(buff))
                return BuffLifecycleState.PendingAdd;

            if (IsTrackedAsActive(buff))
                return BuffLifecycleState.Active;

            if (buffStates.TryGetValue(buff, out var state))
                return state;

            return BuffLifecycleState.None;
        }

        private void SetState(Buff buff, BuffLifecycleState next)
        {
            var current = ResolveCurrentState(buff);
            if (!CanTransition(current, next))
            {
                throw new InvalidOperationException($"Invalid buff lifecycle transition in BuffHandler: {current} -> {next}");
            }

            buffStates[buff] = next;
        }

        private static bool CanTransition(BuffLifecycleState current, BuffLifecycleState next)
        {
            if (current == next)
                return true;

            return current switch
            {
                BuffLifecycleState.None => next == BuffLifecycleState.PendingAdd || next == BuffLifecycleState.Removed,
                BuffLifecycleState.PendingAdd => next == BuffLifecycleState.Active || next == BuffLifecycleState.Removed,
                BuffLifecycleState.Active => next == BuffLifecycleState.PendingRemove,
                BuffLifecycleState.PendingRemove => next == BuffLifecycleState.Active || next == BuffLifecycleState.Removed,
                BuffLifecycleState.Removed => false,
                _ => false
            };
        }

        private void EnqueueAdd(Buff buff)
        {
            if (buff == null)
                return;

            if (isClearingAllBuffs)
                return;

            string key = buff.GetKey();

            // Rule 1: if active buff with same key exists, enqueue refresh on the active instance.
            if (activeBuffByKey.TryGetValue(key, out var activeWithSameKey))
            {
                if (pendingRemoveQueue.Remove(activeWithSameKey))
                {
                    SetState(activeWithSameKey, BuffLifecycleState.Active);
                }

                if (!pendingAddQueue.Contains(activeWithSameKey))
                {
                    pendingAddQueue.Add(activeWithSameKey);
                }

                return;
            }

            // Rule 2: for non-active keys, keep only one pending add per key (last write wins).
            int existingIndex = FindPendingAddIndexByKey(key);
            if (existingIndex >= 0)
            {
                pendingAddQueue[existingIndex] = buff;
                return;
            }

            // Rule 3: if this buff was pending remove before activation, cancel that remove.
            pendingRemoveQueue.Remove(buff);

            SetState(buff, BuffLifecycleState.PendingAdd);
            pendingAddQueue.Add(buff);
        }

        private void EnqueueRemove(Buff buff)
        {
            if (buff == null)
                return;

            // Rule 1: removing a not-yet-committed add cancels that add request directly.
            if (pendingAddQueue.Remove(buff))
            {
                SetState(buff, BuffLifecycleState.Removed);
                return;
            }

            Buff target = buff;
            if (!IsTrackedAsActive(target))
            {
                if (!activeBuffByKey.TryGetValue(buff.GetKey(), out target))
                    return;
            }

            // Rule 2: dedupe remove requests.
            if (pendingRemoveQueue.Contains(target))
                return;

            SetState(target, BuffLifecycleState.PendingRemove);
            pendingRemoveQueue.Add(target);
        }

        private int FindPendingAddIndexByKey(string key)
        {
            for (int i = 0; i < pendingAddQueue.Count; i++)
            {
                if (pendingAddQueue[i].GetKey() == key)
                    return i;
            }

            return -1;
        }

        private bool IsTrackedAsActive(Buff buff)
        {
            if (buff == null)
                return false;

            string key = buff.GetKey();
            if (!activeBuffByKey.TryGetValue(key, out var mapped))
                return activeBuffs.Contains(buff);

            return ReferenceEquals(mapped, buff);
        }

    }
}