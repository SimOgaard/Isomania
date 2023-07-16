using UnityEngine;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace Render
{
    public static class PixelPerfectExtensions
    {
        /// <summary>
        /// Rounds given Vector3 position to pixel grid
        /// </summary>
        /// <param name="position">Vector position to round</param>
        /// <returns>Rounded <see cref="Vector3"/> to </returns>
        public static Vector3 RoundToPixel(Vector3 position)
        {
            Vector3 inversedRotationPosition = InverseCameraRotation * position;

            Vector3 snapped = new Vector3(
                Mathf.Round(inversedRotationPosition.x * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.y * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.z * PixelsPerUnit) * UnitsPerPixel
            );

            return CameraRotation * snapped;
        }
    }
}