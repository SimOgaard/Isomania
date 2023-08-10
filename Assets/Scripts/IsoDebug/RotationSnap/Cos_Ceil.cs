using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Cos_Ceil : RotationSnapAlgorithm
    {
        public Cos_Ceil(Color drawColor) : base(drawColor)
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
                Mathf.Cos(Mathf.Ceil(angle.x / snap) * snap),
                Mathf.Cos(Mathf.Ceil(angle.y / snap) * snap),
                Mathf.Cos(Mathf.Ceil(angle.z / snap) * snap)
            );
            Vector3 output = rounded.normalized;

            return output;
        }
    }
}
