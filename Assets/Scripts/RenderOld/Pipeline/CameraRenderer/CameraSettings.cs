using UnityEngine;

namespace Render.Pipeline.CameraRenderer
{
    public static class CameraSettings
    {
        public const float PixelsPerUnit = 10f;
        public const float UnitsPerPixel = 1f / PixelsPerUnit;

        public const float RotationGrid = 30f / 16f;

        public const float CameraDistance = 50f;
        public const float CameraDistanceX = 0;
        public const float CameraDistanceY = CameraDistance * 0.5f; // Mathf.Sin(30f * Mathf.Deg2Rad) * CameraDistance;
        public const float CameraDistanceZ = -CameraDistance * 0.8660254037f; // -Mathf.Cos(30f * Mathf.Deg2Rad) * CameraDistance;
        public static Vector3 CameraDistanceVector = new Vector3(CameraDistanceX, CameraDistanceY, CameraDistanceZ);

        public static Vector2Int RenderResolution = new Vector2Int(512, 288);
        public static Vector2Int RenderResolutionExtended;

        public static float OrthographicSize;

        static CameraSettings()
        {
            RecalculateCameraSettings();
        }

        public static void RecalculateCameraSettings()
        {
            OrthographicSize = RenderResolution.y / (PixelsPerUnit * 2f);
            RenderResolutionExtended = RenderResolution + Vector2Int.one * 2;
        }
    }
}