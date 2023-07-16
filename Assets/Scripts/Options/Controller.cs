using UnityEngine;

namespace Options
{
    public class Controller : ScriptableObject, IDevice
    {
        public bool RotateCameraLeft => throw new System.NotImplementedException();

        public bool RotateCameraRight => throw new System.NotImplementedException();

        public Vector3 LookDirection => throw new System.NotImplementedException();

        public Vector3 HeadingDirection => throw new System.NotImplementedException();
    }
}
