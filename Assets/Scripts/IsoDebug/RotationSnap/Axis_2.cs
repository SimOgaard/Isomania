using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Axis_2 : RotationSnapAlgorithm
    {
        public Axis_2(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            float snapAngle = 45.0f;

            // Get the angle between the direction and the up vector.
            float angle = Vector3.Angle(input, Vector3.up);

            // Find the nearest multiple of the snap value.
            float snappedAngle = Mathf.Round(angle / snapAngle) * snapAngle;

            // Rotate the direction vector by the difference between the two angles.
            Vector3 snappedDirection = Quaternion.AngleAxis(snappedAngle - angle, Vector3.up) * input;

            return snappedDirection;
        }
    }
}
