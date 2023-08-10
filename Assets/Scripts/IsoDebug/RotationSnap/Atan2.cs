using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Atan2 : RotationSnapAlgorithm
    {
        public Atan2(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            // Check for possible NaN outputs from atan2
            if (input.y == 0 && input.x == 0)
            {
                return input;
            }

            const float RotationSnap = 45.0f;
            const float InverseRotationSnap = 1.0f / RotationSnap;

            float magnitude = input.magnitude;

            float azimuth = Mathf.Atan2(input.y / magnitude, input.x / magnitude);
            float elevation = Mathf.Acos(input.z / magnitude);

            float snapAzimuth = Mathf.Round(azimuth * InverseRotationSnap * Mathf.Rad2Deg) * RotationSnap * Mathf.Deg2Rad;
            float snapElevation = Mathf.Round(elevation * InverseRotationSnap * Mathf.Rad2Deg) * RotationSnap * Mathf.Deg2Rad;

            input.x = Mathf.Cos(snapAzimuth) * Mathf.Sin(snapElevation) * magnitude;
            input.y = Mathf.Sin(snapAzimuth) * Mathf.Sin(snapElevation) * magnitude;
            input.z = Mathf.Cos(snapElevation) * magnitude;

            return input;
        }
    }
}
