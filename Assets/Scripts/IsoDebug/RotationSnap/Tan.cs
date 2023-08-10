using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Tan : RotationSnapAlgorithm
    {
        public Tan(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            // scale vector to unit cube
            float scaleDivisor = Mathf.Abs(input.x);

            if (Mathf.Abs(input.y) > scaleDivisor)
                scaleDivisor = Mathf.Abs(input.y);

            if (Mathf.Abs(input.z) > scaleDivisor)
                scaleDivisor = Mathf.Abs(input.z);

            input /= scaleDivisor;

            Vector3 rounded = new Vector3(round(input.x), round(input.y), round(input.z));

            return rounded.normalized;

            float round(float f)
            {
                if (Mathf.Abs(f) < Mathf.Tan(Mathf.PI / 8))
                    return 0;
                return Mathf.Sign(f);
            }
        }
    }
}
