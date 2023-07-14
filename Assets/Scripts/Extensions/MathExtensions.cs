public static class MathExtensions
{
    public static float ClampRotation(this float rotation)
    {
        return (rotation + 360) % 360;
    }
}
