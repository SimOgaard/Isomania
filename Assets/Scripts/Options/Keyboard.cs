using UnityEngine;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace Options
{
    public class Keyboard : ScriptableObject, IDevice
    {
        public bool RotateCameraLeft => Input.GetKeyDown(KeyCode.Q);

        public bool RotateCameraRight => Input.GetKeyDown(KeyCode.E);

        public Vector3 LookDirection
        {
            get
            {
                if (CameraManagerTransform is null)
                    return Vector3.zero;

                // get cameramanager position from its transform
                Vector3 cameraManagerPosition = CameraManagerTransform.position;

                // create a plane on cameramanager transform
                Plane plane = new Plane(Vector3.up, cameraManagerPosition);

                // cast a ray from the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // perform the raycast
                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 worldPosition = ray.GetPoint(enter);
                    return Vector3.ClampMagnitude((worldPosition - cameraManagerPosition) / 3.5f, 5f);
                }

                return Vector3.zero;
            }
        }

        public Vector3 HeadingDirection
        {
            get
            {
                float vertical = 0;
                if (Input.GetKey(KeyCode.W))
                    vertical++;
                if (Input.GetKey(KeyCode.S))
                    vertical--;

                float horizontal = 0;
                if (Input.GetKey(KeyCode.D))
                    horizontal++;
                if (Input.GetKey(KeyCode.A))
                    horizontal--;

                Vector3 direction = new Vector3(horizontal, 0, vertical);
                Vector3 cameraFixedDirection = Quaternion.Euler(0f, CameraYRotation, 0.0f) * direction;

                return cameraFixedDirection;
            }
        }
    }
}
