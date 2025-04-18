
using UnityEngine;

namespace Units
{
    public class TrailController : MonoBehaviour {
        private SpriteRenderer spriteRenderer; 
        private TrailRenderer trailRenderer;
        
        void Awake()
        {
            // 初始化组件引用
            spriteRenderer = GetComponent<SpriteRenderer>();
            trailRenderer = GetComponent<TrailRenderer>();
        }
        void Start()
        {
            float spriteHeight = spriteRenderer.bounds.size.y;
            trailRenderer.startWidth = spriteHeight;
            trailRenderer.endWidth = 0f;

            // 设置 TrailRenderer 的颜色与 SpriteRenderer 一致
            Color spriteColor = spriteRenderer.color;
            trailRenderer.startColor = spriteColor;
            trailRenderer.endColor = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0f); // 末端颜色透明
        }
    }
}