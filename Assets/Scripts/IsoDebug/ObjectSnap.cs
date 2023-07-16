using UnityEngine;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace IsoDebug
{
    [ExecuteAlways]
    public class ObjectSnap : MonoBehaviour
    {
        private Vector3 GridRotation = new Vector3(30f, 0f, 0f);
        private Vector3 InverseGridRotation = new Vector3(-30f, 0f, 0f);

        private void Update()
        {
            Vector3 position = transform.position;
            Vector3 inversedRotationPosition = Quaternion.Euler(InverseGridRotation) * position;

            Vector3 snapped = new Vector3(
                Mathf.Round(inversedRotationPosition.x * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.y * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.z * PixelsPerUnit) * UnitsPerPixel
            );

            transform.position = Quaternion.Euler(GridRotation) * snapped;
        }
    }
}