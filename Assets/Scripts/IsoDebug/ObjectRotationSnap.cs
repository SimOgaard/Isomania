using System.Text;
using UnityEngine;

namespace IsoDebug
{
    [ExecuteAlways]
    public class ObjectRotationSnap : MonoBehaviour
    {
        // Define the snap values for each axis (in degrees)
        [SerializeField]
        private float RotationSnap = 45.0f;

        [SerializeField]
        private Vector3 EulerAngles = Vector3.zero;

        [SerializeField]
        private Transform CPUTransform;
        [SerializeField]
        private Transform GPUTransform;

        private void Update()
        {
            if (CPUTransform is null || GPUTransform is null)
                return;

            // Snap the Euler angles to the nearest snap values
            float snappedAngleX = Mathf.Round(EulerAngles.x / RotationSnap) * RotationSnap;
            float snappedAngleY = Mathf.Round(EulerAngles.y / RotationSnap) * RotationSnap;
            float snappedAngleZ = Mathf.Round(EulerAngles.z / RotationSnap) * RotationSnap;

            // Create a new Quaternion with the snapped Euler angles
            CPUTransform.rotation = Quaternion.Euler(snappedAngleX, snappedAngleY, snappedAngleZ);

            Matrix4x4 CPUTransformMatrix = CPUTransform.localToWorldMatrix;

            GPUTransform.rotation = Quaternion.Euler(EulerAngles);
            Matrix4x4 GPUTransformMatrix = GPUTransform.localToWorldMatrix;

            Vector4 SnapRotation(Vector4 rotation)
            {
                float InverseRotationSnap = 1.0f / RotationSnap;

                float azimuth = Mathf.Atan2(rotation.y, rotation.x);
                float elevation = Mathf.Acos(rotation.z);

                //azimuth = Mathf.Atan2(rotation.z, rotation.x);
                //elevation = Mathf.Acos(rotation.y);

                float snapAzimuth = Mathf.Round(azimuth * InverseRotationSnap * Mathf.Rad2Deg) * RotationSnap * Mathf.Deg2Rad;
                float snapElevation = Mathf.Round(elevation * InverseRotationSnap * Mathf.Rad2Deg) * RotationSnap * Mathf.Deg2Rad;

                rotation.x = Mathf.Cos(snapAzimuth) * Mathf.Sin(snapElevation);
                rotation.y = Mathf.Sin(snapAzimuth) * Mathf.Sin(snapElevation);
                rotation.z = Mathf.Cos(snapElevation);

                return rotation;
            }

            Vector4 rotationRow_1 = SnapRotation(GPUTransformMatrix.GetRow(0));
            Vector4 rotationRow_2 = SnapRotation(GPUTransformMatrix.GetRow(1));
            Vector4 rotationRow_3 = SnapRotation(GPUTransformMatrix.GetRow(2));

            GPUTransformMatrix.SetRow(0, rotationRow_1);
            GPUTransformMatrix.SetRow(1, rotationRow_2);
            GPUTransformMatrix.SetRow(2, rotationRow_3);

            // log
            LogMatrixRotation(CPUTransformMatrix, GPUTransformMatrix);
        }

        private void LogMatrixRotation(Matrix4x4 correctMatrix4x4, Matrix4x4 computedMatrix4x4)
        {
            StringBuilder log = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                Vector3 rotationRow = correctMatrix4x4.GetRow(i);
                log.AppendLine(rotationRow.ToString());
            }
            for (int i = 0; i < 3; i++)
            {
                Vector3 rotationRow = computedMatrix4x4.GetRow(i);
                log.AppendLine(rotationRow.ToString());
            }

            Debug.Log(log);
        }

        Vector3 SnapVector(Vector3 vector)
        {
            float RotationSnap = 45.0f;
            float InverseRotationSnap = 1.0f / RotationSnap;

            //float phi = Mathf.Atan2(vector.z, vector.x);
            //float theta = Mathf.Acos(vector.y);

            float theta = Mathf.Acos(vector.z);
            float phi = Mathf.Sign(vector.y) * Mathf.Acos(vector.x / Mathf.Sqrt((vector.x * vector.x) + (vector.y * vector.y)));

            float snapPhi = Mathf.Round(phi * RotationSnap * Mathf.Rad2Deg) * InverseRotationSnap * Mathf.Deg2Rad;
            float snapTheta = Mathf.Round(theta * RotationSnap * Mathf.Rad2Deg) * InverseRotationSnap * Mathf.Deg2Rad;

            vector.x = Mathf.Sin(snapTheta) * Mathf.Cos(snapPhi);
            vector.y = Mathf.Cos(snapTheta);
            vector.z = Mathf.Sin(snapTheta) * Mathf.Sin(snapPhi);

            return vector;
        }
    }
}