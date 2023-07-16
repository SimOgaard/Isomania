using UnityEngine;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace Options
{
    public class Keyboard : ScriptableObject, IDevice
    {
        public bool RotateCameraLeft => Input.GetKeyDown(KeyCode.Q);

        public bool RotateCameraRight => Input.GetKeyDown(KeyCode.E);

        public Vector3 Direction
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
