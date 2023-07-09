using UnityEngine;

namespace Options
{
    public class Keyboard : ScriptableObject, IDevice
    {
        public bool RotateCameraLeft => Input.GetKey(KeyCode.E);

        public bool RotateCameraRight => Input.GetKey(KeyCode.Q);
    }
}
