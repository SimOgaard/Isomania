using UnityEngine;

namespace Options
{
    public interface IDevice
    {
        public static IDevice ConnectedDevice = new Keyboard();

        public bool RotateCameraLeft { get; }
        public bool RotateCameraRight { get; }

        public Vector3 Direction { get; }
        public Vector3 NormalizedDirection => Direction.normalized;
    }
}