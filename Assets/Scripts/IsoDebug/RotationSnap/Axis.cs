using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Axis : RotationSnapAlgorithm
    {
        public Axis(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            float snapAngle = 45.0f;

            float angle = Vector3.Angle(input, Vector3.up);
            if (angle < snapAngle / 2.0f)          // Cannot do cross product 
                return Vector3.up * input.magnitude;  //   with angles 0 & 180
            if (angle > 180.0f - snapAngle / 2.0f)
                return Vector3.down * input.magnitude;

            float t = Mathf.Round(angle / snapAngle);

            float deltaAngle = (t * snapAngle) - angle;

            Vector3 axis = Vector3.Cross(Vector3.up, input);
            Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);
            return q * input;
        }
    }
}
