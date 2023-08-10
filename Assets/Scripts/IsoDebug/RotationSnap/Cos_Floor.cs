using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Cos_Floor : RotationSnapAlgorithm
    {
        public Cos_Floor(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            float snap = Mathf.Deg2Rad * 45.0f;

            Vector3 angle = new(
                Mathf.Acos(input.x),
                Mathf.Acos(input.y),
                Mathf.Acos(input.z)
            );
            Vector3 rounded = new(
                Mathf.Cos(Mathf.Floor(angle.x / snap) * snap),
                Mathf.Cos(Mathf.Floor(angle.y / snap) * snap),
                Mathf.Cos(Mathf.Floor(angle.z / snap) * snap)
            );
            Vector3 output = rounded.normalized;

            return output;
        }
    }
}
