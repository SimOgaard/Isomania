using UnityEngine;

namespace Options
{
    public class Keyboard : ScriptableObject, IDevice
    {
        public bool RotateCameraLeft => Input.GetKeyDown(KeyCode.Q);

        public bool RotateCameraRight => Input.GetKeyDown(KeyCode.E);
    }
}
