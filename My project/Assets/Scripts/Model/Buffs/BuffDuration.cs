using System;
using UnityEngine;

namespace Model.Buffs
{
    public enum BuffDurationMode
    {
        Timed,
        Permanent
    }

    [Serializable]
    public struct BuffDuration
    {
        [SerializeField] private BuffDurationMode mode;
        [SerializeField] private float seconds;

        public BuffDurationMode Mode => mode;
        public float Seconds => seconds;
        public bool IsPermanent => mode == BuffDurationMode.Permanent;
        public bool IsTimed => mode == BuffDurationMode.Timed;

        public static BuffDuration Permanent => new(BuffDurationMode.Permanent, 0f);

        public static BuffDuration ForSeconds(float seconds) =>
            new(BuffDurationMode.Timed, seconds);

        public string ToDisplayString(string label)
        {
            return IsPermanent
                ? $"{label}: 永久"
                : $"{label}: <final>{seconds}</final>";
        }

        public bool IsValid()
        {
            if (mode == BuffDurationMode.Permanent)
            {
                return true;
            }

            return mode == BuffDurationMode.Timed
                && seconds > 0f
                && !float.IsNaN(seconds)
                && !float.IsInfinity(seconds);
        }

        private BuffDuration(BuffDurationMode mode, float seconds)
        {
            this.mode = mode;
            this.seconds = seconds;
        }
    }
}