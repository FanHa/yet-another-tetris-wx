using System;
using UnityEngine;

namespace Units.Buffs
{
    public class BuffViewer : MonoBehaviour
    {
        private float radius = 0.25f; // 半径
        private float elapsedTime = 0f; // 累计时间

        private Vector3 centerPosition; // 原点位置

        private void Start()
        {
            // 初始化中心点为当前对象的初始位置
            centerPosition = transform.position;
        }

        private void Update()
        {
            // 累计时间
            elapsedTime += Time.deltaTime;

            // 计算角度（1秒转一圈，角速度为 2π 弧度/秒）
            float angle = elapsedTime * Mathf.PI * 2f; // 弧度

            // 计算新位置
            float x = centerPosition.x + Mathf.Cos(angle) * radius;
            float y = centerPosition.y + Mathf.Sin(angle) * radius;

            // 更新对象位置
            transform.position = new Vector3(x, y, transform.position.z);
            FaceCamera();
        }

        private void FaceCamera()
        {
            // 获取主摄像机
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // 使对象的朝向与摄像机一致
                transform.rotation = Quaternion.LookRotation(Vector3.forward, mainCamera.transform.up);
            }
        }

        public void SetBuffSprite(Sprite sprite)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;

                // 统一设置大小
                if (sprite != null)
                {
                    float targetSize = 0.25f; // 目标大小（单位：世界坐标）
                    float spriteWidth = sprite.bounds.size.x;
                    float spriteHeight = sprite.bounds.size.y;

                    // 计算缩放比例
                    float scaleX = targetSize / spriteWidth;
                    float scaleY = targetSize / spriteHeight;
                    float uniformScale = Mathf.Min(scaleX, scaleY); // 保持宽高比例一致

                    spriteRenderer.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
                }
                else
                {
                    spriteRenderer.transform.localScale = Vector3.one; // 重置缩放
                }
            }
        }

    }
}