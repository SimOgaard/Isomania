using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Lookup : RotationSnapAlgorithm
    {
        public Lookup(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            Vector3 lookup = Vector3.zero;

            float lookupAngle = float.PositiveInfinity;

            foreach (Vector3 value in LookupValues)
            {
                Vector3 norm = value.normalized;

                float angle = AngleTo(input, norm);

                if (angle < lookupAngle)
                {
                    lookup = norm;
                    lookupAngle = angle;
                }
            }

            return lookup;
        }

        public static readonly Vector3[] LookupValues = {
            new Vector3(-1f, -1f, -1f), // 0
            new Vector3(-1f, -1f, 0f),  // 1
            new Vector3(-1f, -1f, 1f),  // 2
            new Vector3(-1f, 0f, -1f),  // 3
            new Vector3(-1f, 0f, 0f),   // 4
            new Vector3(-1f, 0f, 1f),   // 5
            new Vector3(-1f, 1f, -1f),  // 6
            new Vector3(-1f, 1f, 0f),   // 7
            new Vector3(-1f, 1f, 1f),   // 8
            new Vector3(0f, -1f, -1f),  // 9
            new Vector3(0f, -1f, 0f),   // 10 
            new Vector3(0f, -1f, 1f),   // 11
            new Vector3(0f, 0f, -1f),   // 12 
            new Vector3(0f, 0f, 1f),    // 13
            new Vector3(0f, 1f, -1f),   // 14
            new Vector3(0f, 1f, 0f),    // 15
            new Vector3(0f, 1f, 1f),    // 16
            new Vector3(1f, -1f, -1f),  // 17
            new Vector3(1f, -1f, 0f),   // 18
            new Vector3(1f, -1f, 1f),   // 19
            new Vector3(1f, 0f, -1f),   // 20
            new Vector3(1f, 0f, 0f),    // 21
            new Vector3(1f, 0f, 1f),    // 22
            new Vector3(1f, 1f, -1f),   // 23
            new Vector3(1f, 1f, 0f),    // 24
            new Vector3(1f, 1f, 1f)     // 25
        };
    }
}
