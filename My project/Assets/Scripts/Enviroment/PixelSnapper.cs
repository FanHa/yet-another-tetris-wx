using UnityEngine;
using UnityEngine.U2D;

namespace Enviroment
{
    /// <summary>
    /// PixelSnapper is a MonoBehaviour that enables pixel snapping for the PixelPerfectCamera component.
    /// </summary>
    [RequireComponent(typeof(PixelPerfectCamera))]
    [DisallowMultipleComponent]
    public class PixelSnapper : MonoBehaviour
    {
        void Awake()
        {
            var pixelCam = GetComponent<PixelPerfectCamera>();
            pixelCam.pixelSnapping = true;
        }
    }
}
