using UnityEngine;

namespace Utils
{
    public class CameraFollower : MonoBehaviour
    { 
        
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private float smoothSpeed;

        [Header("运行时设置")]
        [SerializeField] private Transform target;

        private void LateUpdate()
        {
            if (target == null)
                return;
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}