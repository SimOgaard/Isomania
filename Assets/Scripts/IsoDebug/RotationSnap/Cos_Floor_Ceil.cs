using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Cos_Floor_Ceil : RotationSnapAlgorithm
    {
        public Cos_Floor_Ceil(Color drawColor) : base(drawColor)
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

            Mathf.Cos(Mathf.Floor(angle.x / snap) * snap);
            Mathf.Cos(Mathf.Ceil(angle.x / snap) * snap);

            Mathf.Cos(Mathf.Floor(angle.y / snap) * snap);
            Mathf.Cos(Mathf.Ceil(angle.y / snap) * snap);

            Mathf.Cos(Mathf.Floor(angle.z / snap) * snap);
            Mathf.Cos(Mathf.Ceil(angle.z / snap) * snap);

            return angle;
            //if (AngleTo(roundedFloor.normalized, input.normalized) < AngleTo(roundedCeil.normalized, input.normalized))
            //    return roundedFloor.normalized;
            //return roundedCeil.normalized;
        }
    }
}
