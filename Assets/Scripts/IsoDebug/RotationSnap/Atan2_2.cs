using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Atan2_2 : RotationSnapAlgorithm
    {
        public Atan2_2(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            float snap = Mathf.Deg2Rad * 45.0f;

            Vector3 angle = new(
                Mathf.Atan2(-input.y, input.z),
                Mathf.Atan2(-input.z, input.x),
                Mathf.Atan2(input.y, input.x)
            );
            Vector3 rounded = new(
                Mathf.Round(angle.x / snap) * snap,
                Mathf.Round(angle.y / snap) * snap,
                Mathf.Round(angle.z / snap) * snap
            );

            // replace this part with reconstruction of input given rounded vector3
            return  new(
    Mathf.Atan2(input.y, input.z),
    Mathf.Atan2(input.z, input.x),
    Mathf.Atan2(-input.y, -input.x)
);
            //return output;
        }
    }
}
