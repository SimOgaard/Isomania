using System;
using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class TestXY : RotationSnapAlgorithm
    {
        public TestXY(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            float roundAngle = 45.0f * Mathf.Deg2Rad;
            float angle = (float)Math.Atan2(input.y, input.x);
            Vector2 newNormal;

            if (angle % roundAngle != 0)
            {
                float newAngle = (float)Math.Round(angle / roundAngle) * roundAngle;
                newNormal = new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));
            }
            else
            {
                newNormal = input.normalized;
            }
            return newNormal;
        }
    }
}
