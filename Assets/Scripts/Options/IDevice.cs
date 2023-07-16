using UnityEngine;

namespace Options
{
    public interface IDevice
    {
        public static IDevice ConnectedDevice = new Keyboard();

        public bool RotateCameraLeft { get; }
        public bool RotateCameraRight { get; }

        public Vector3 LookDirection { get; }
        public Vector3 NormalizedLookDirection => LookDirection.normalized;

        public Vector3 HeadingDirection { get; }
        public Vector3 NormalizedHeadingDirection => HeadingDirection.normalized;
    }
}