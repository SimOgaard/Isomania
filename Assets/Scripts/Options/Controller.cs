using UnityEngine;

namespace Options
{
    public class Controller : ScriptableObject, IDevice
    {
        public bool RotateCameraLeft => throw new System.NotImplementedException();

        public bool RotateCameraRight => throw new System.NotImplementedException();
    }
}
