using UnityEngine;

namespace Units.Actions
{
    /// <summary>
    /// 通用动作时间线进度模型：通过逐帧采样的速度推进 0~1 进度。
    /// </summary>
    public sealed class ActionTimelineProgress
    {
        private float progress;

        public float Value => progress;

        public bool IsCompleted => progress >= 1f;

        public void Reset()
        {
            progress = 0f;
        }

        public void CompleteNow()
        {
            progress = 1f;
        }

        public float Advance(float deltaTime, float timelineSpeed, float baseDurationSeconds)
        {
            float duration = Mathf.Max(0.001f, baseDurationSeconds);
            float speed = Mathf.Max(0f, timelineSpeed);

            progress += deltaTime * speed / duration;
            progress = Mathf.Clamp01(progress);
            return progress;
        }
    }
}
