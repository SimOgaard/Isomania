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
        public static Vector3 RoundToPixel(this Vector3 position)
        {
            Vector3 inversedRotationPosition = InverseCameraRotation * position;

            Vector3 snapped = new Vector3(
                Mathf.Round(inversedRotationPosition.x * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.y * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.z * PixelsPerUnit) * UnitsPerPixel
            );

            return CameraRotation * snapped;
        }

        public static Quaternion RoundToRotation(this Quaternion rotation)
        {
            return Quaternion.Euler(RoundToEulerRotation(rotation.eulerAngles));
        }
        public static Quaternion RoundToRotation(this Vector3 eulerRotation)
        {
            return Quaternion.Euler(RoundToEulerRotation(eulerRotation));
        }
        public static Vector3 RoundToEulerRotation(this Quaternion rotation)
        {
            return RoundToEulerRotation(rotation.eulerAngles);
        }
        public static Vector3 RoundToEulerRotation(this Vector3 eulerRotation)
        {
            return new(
                Mathf.Round(eulerRotation.x * InverseRotationGrid) * RotationGrid,
                Mathf.Round(eulerRotation.y * InverseRotationGrid) * RotationGrid,
                Mathf.Round(eulerRotation.z * InverseRotationGrid) * RotationGrid
            );
        }
    }
}