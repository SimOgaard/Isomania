using UnityEngine;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace Render.Pipeline.CameraRenderer
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraInitializer : MonoBehaviour
    {
        private void Awake()
        {
            Camera camera = GetComponent<Camera>();

            camera.orthographic = true;
            camera.orthographicSize = PixelsPerUnit * 0.5f;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.clear;
        }
    }
}