using System;
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
        public static readonly Vector3 CameraDistanceVector = new(CameraDistanceX, CameraDistanceY, CameraDistanceZ);

        public static Vector2 TestRenderResolution { get; private set; }
        public static Vector2Int RenderResolution { get; private set; }
        public static Vector2Int RenderResolutionExtended { get; private set; }

        public static Vector2 RenderOffset { get; private set; }
        public static Vector2 RenderScale { get; private set; }

        public static float OrthographicSize { get; private set; }

        static CameraSettings()
        {
            RecalculateCameraSettings(Screen.width, Screen.height);
        }
        private static (float width, float height) CalculateResolution(int screenWidth, int screenHeight)
        {
            const float idealPixelDensity = 512f * 288f;

            float closestIdealPixelDensity = float.MaxValue;
            float idealWidth = 0f;
            float idealHeight = 0f;

            for (int factor = 1; factor <= 8; factor++)
            {
                float width = screenWidth / (float)factor;
                float height = screenHeight / (float)factor;
                float pixelDensity = width * height;
                float difference = Math.Abs(idealPixelDensity - pixelDensity);

                if (difference < Math.Abs(idealPixelDensity - closestIdealPixelDensity))
                {
                    idealWidth = width;
                    idealHeight = height;
                    closestIdealPixelDensity = pixelDensity;
                }
            }

            return (idealWidth, idealHeight);
        }

        public static void RecalculateCameraSettings(int screenWidth, int screenHeight)
        {
            (float renderWidth, float renderHeight) = CalculateResolution(screenWidth, screenHeight);

            TestRenderResolution = new Vector2(renderWidth, renderHeight);

            RenderResolution = new Vector2Int(Mathf.CeilToInt(renderWidth), Mathf.CeilToInt(renderHeight));
            RenderResolutionExtended = RenderResolution;// + Vector2Int.one * 2;

            float renderScaleX = (float)renderWidth / RenderResolutionExtended.x;
            float renderScaleY = (float)renderHeight / RenderResolutionExtended.y;
            float renderOffsetX = (1f - ((float)screenWidth / (RenderResolutionExtended.x * renderScaleX))) / 2f; // this does not work ! ! !
            float renderOffsetY = (1f - ((float)screenHeight / (RenderResolutionExtended.y * renderScaleY))) / 2f;

            RenderScale = new Vector2(renderScaleX, renderScaleY);
            RenderOffset = new Vector2(renderOffsetX, renderOffsetY);
            RenderOffset = new Vector2(0f,0f);

            OrthographicSize = RenderResolutionExtended.y / (PixelsPerUnit * 2f);

            Debug.Log($"{screenWidth} {screenHeight}");
        }
    }
}