using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Spherical : RotationSnapAlgorithm
    {
        public Spherical(Color drawColor) : base(drawColor)
        {
        }

        // Convert XYZ components to spherical coordinates
        private static void CartesianToSpherical(Vector3 cartesian, out float r, out float theta, out float phi)
        {
            r = cartesian.magnitude;
            theta = Mathf.Atan2(cartesian.y, cartesian.x);
            phi = Mathf.Acos(cartesian.z / r);
        }

        // Snap an angle (in radians) to the nearest degree value
        private static float SnapToDegree(float angleRad, float degree)
        {
            float degreeRad = Mathf.PI / 180 * degree;
            return Mathf.Round(angleRad / degreeRad) * degreeRad;
        }

        // Convert spherical coordinates back to XYZ components
        private static Vector3 SphericalToCartesian(float r, float theta, float phi)
        {
            float x = r * Mathf.Cos(theta) * Mathf.Sin(phi);
            float y = r * Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = r * Mathf.Cos(phi);
            return new Vector3(x, y, z);
        }

        // Snap the directional Vector3 to a degree value
        private static Vector3 SnapToDegree(Vector3 vector, float degree)
        {
            CartesianToSpherical(vector, out float r, out float theta, out float phi);
            float snappedTheta = SnapToDegree(theta, degree);
            float snappedPhi = SnapToDegree(phi, degree);
            return SphericalToCartesian(r, snappedTheta, snappedPhi);
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            return SnapToDegree(input, 45.0f);
        }
    }
}
