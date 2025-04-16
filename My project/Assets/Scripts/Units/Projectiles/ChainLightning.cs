using UnityEngine;

namespace Units.Projectiles
{
    public class ChainLightning : MonoBehaviour
    {
        private LineRenderer lineRenderer; // 线条渲染器
        [SerializeField] private Texture[] textures;
        private int animationStep;
        [SerializeField] private float fps;
        [SerializeField] private float lifetime;
        private float fpsCounter;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();

        }

        private void Start()
        {
            // 在指定时间后销毁对象
            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1f / fps)
            {
                fpsCounter = 0f;
                animationStep = (animationStep + 1) % textures.Length;
                lineRenderer.material.mainTexture = textures[animationStep];
            }
        }
        public void SetLinePoints(Vector3 startPoint, Vector3 endPoint)
        {
            if (lineRenderer == null)
            {
                Debug.LogWarning("LineRenderer is not initialized.");
                return;
            }

            lineRenderer.positionCount = 2; // 设置线段的点数量为 2
            lineRenderer.SetPosition(0, startPoint); // 设置起点
            lineRenderer.SetPosition(1, endPoint);   // 设置终点
        }

    }
}