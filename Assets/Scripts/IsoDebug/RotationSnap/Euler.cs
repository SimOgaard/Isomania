using UnityEngine;

namespace IsoDebug.RotationSnap
{
    public class Euler : RotationSnapAlgorithm
    {
        public Euler(Color drawColor) : base(drawColor)
        {
        }

        public override Vector3 Algorithm(Vector3 input)
        {
            const float snap = 45.0f;

            // Step 1: Convert the normalized directional vector to Euler angles
            Quaternion rotation = Quaternion.LookRotation(input);

            // Step 2: Convert the Euler angles to rounded values using the snap value
            Vector3 euler = rotation.eulerAngles;
            euler.x = Mathf.Round(euler.x / snap) * snap;
            euler.y = Mathf.Round(euler.y / snap) * snap;
            euler.z = Mathf.Round(euler.z / snap) * snap;

            // Step 3: Convert the rounded Euler angles back to a directional vector
            Quaternion snappedRotation = Quaternion.Euler(euler);
            Vector3 output = snappedRotation * Vector3.forward;

            return output;
        }
    }
}
