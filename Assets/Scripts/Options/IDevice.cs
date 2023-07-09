namespace Options
{
    public interface IDevice
    {
        public static IDevice ConnectedDevice = new Keyboard();

        public bool RotateCameraLeft { get; }
        public bool RotateCameraRight{ get; }
    }
}