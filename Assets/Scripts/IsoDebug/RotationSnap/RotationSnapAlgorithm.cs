using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public abstract class RotationSnapAlgorithm
    {
        public Color DrawColor { get; }
        public Vector3 Result { get; private set; }
        public float ResultAngle { get; private set; }

        public abstract Vector3 Algorithm(Vector3 input);

        protected float AngleTo(Vector3 input, Vector3 to)
        {
            return Mathf.Acos(Vector3.Dot(input, to));
        }

        public void Run(Vector3 input)
        {
            Result = Algorithm(input);
            ResultAngle = AngleTo(input, Result);
        }

        public RotationSnapAlgorithm(Color drawColor)
        {
            DrawColor = drawColor;
        }
    }
}