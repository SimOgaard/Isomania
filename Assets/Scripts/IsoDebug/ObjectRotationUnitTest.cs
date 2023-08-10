using Render;
using System;
using UnityEngine;
using static Render.Pipeline.CameraRenderer.CameraSettings;

public class ObjectRotationUnitTest : MonoBehaviour
{
    [SerializeField]
    private float rotationIncrease = 1e-3f;
    private const float fullRotation = 360.0f;

    private Vector4 SnapRotation(Vector4 rotation)
    {
        float magnitude = new Vector3(rotation.x, rotation.y, rotation.z).magnitude;

        float azimuth = Mathf.Atan2(rotation.y / magnitude, rotation.x / magnitude);
        float elevation = Mathf.Acos(rotation.z / magnitude);

        float snapAzimuth = Mathf.Round(azimuth * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;
        float snapElevation = Mathf.Round(elevation * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;

        return new(
            Mathf.Cos(snapAzimuth) * Mathf.Sin(snapElevation) * magnitude,
            Mathf.Sin(snapAzimuth) * Mathf.Sin(snapElevation) * magnitude,
            Mathf.Cos(snapElevation) * magnitude,
            rotation.w
        );
    }

    private Vector4 SnapRotation2(Vector4 rotation)
    {
        float magnitude = new Vector3(rotation.x, rotation.y, rotation.z).magnitude;

        float x = Mathf.Acos(rotation.x / magnitude);
        float y = Mathf.Acos(rotation.y / magnitude);
        float z = Mathf.Acos(rotation.z / magnitude);

        float snapX = Mathf.Round(x * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;
        float snapY = Mathf.Round(y * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;
        float snapZ = Mathf.Round(z * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;

        return new(
            Mathf.Cos(snapX) * magnitude,
            Mathf.Cos(snapY) * magnitude,
            Mathf.Cos(snapZ) * magnitude,
            rotation.w
        );
    }

    private Vector4 SnapRotation3(Vector4 rotation)
    {
        float magnitude = new Vector3(rotation.x, rotation.y, rotation.z).magnitude;

        float x = Mathf.Atan2(-rotation.y / magnitude, rotation.z / magnitude);
        float y = Mathf.Atan2(-rotation.z / magnitude, rotation.x / magnitude);
        float z = Mathf.Atan2(rotation.y / magnitude, rotation.x / magnitude);

        float snapX = Mathf.Round(x * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;
        float snapY = Mathf.Round(y * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;
        float snapZ = Mathf.Round(z * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;

        return new(
            Mathf.Cos(snapX) * magnitude,
            Mathf.Cos(snapY) * magnitude,
            Mathf.Cos(snapZ) * magnitude,
            rotation.w
        );
    }

    private Vector4 SnapRotation4(Vector4 rotation)
    {
        float magnitude = new Vector3(rotation.x, rotation.y, rotation.z).magnitude;

        // Calculate the heading.
        Vector3 headingVector = new Vector3(rotation.x, rotation.y, 0);
        float heading = Mathf.Atan2(headingVector.y, headingVector.x);

        // Calculate the pitch.
        float pitch = Mathf.Asin(rotation.z);

        // Calculate the bank angle.
        Vector3 wingVector = new Vector3(-rotation.y, rotation.x, 0);
        Vector3 expectedUpVector = Vector3.Cross(wingVector, rotation);
        float bank = Mathf.Atan2(Vector3.Dot(wingVector, Vector3.up), Vector3.Dot(expectedUpVector, Vector3.up));

        float snapHeading = Mathf.Round(heading * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;
        float snapPitch = Mathf.Round(pitch * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;
        float snapBank = Mathf.Round(bank * InverseRotationGrid * Mathf.Rad2Deg) * RotationGrid * Mathf.Deg2Rad;

        // Calculate the x-component of the direction vector.
        float x = Mathf.Cos(snapHeading) * Mathf.Cos(snapPitch) * Mathf.Cos(snapBank) - Mathf.Sin(snapHeading) * Mathf.Sin(snapPitch) * Mathf.Sin(snapBank);

        // Calculate the y-component of the direction vector.
        float y = Mathf.Sin(snapHeading) * Mathf.Cos(snapPitch) * Mathf.Cos(snapBank) + Mathf.Cos(snapHeading) * Mathf.Sin(snapPitch) * Mathf.Sin(snapBank);

        // Calculate the z-component of the direction vector.
        float z = Mathf.Sin(snapPitch);

        return new(
            x * magnitude,
            y * magnitude,
            z * magnitude,
            rotation.w
        );
    }

    private Vector4 SnapRotation5(Vector4 rotation)
    {
        // Step 1: Calculate Heading (angle_H)
        Vector3 heading = new Vector3(rotation.x, rotation.y, 0f);
        float angle_H = Mathf.Atan2(heading.y, heading.x);

        // Step 2: Calculate Pitch (angle_P)
        float angle_P = Mathf.Asin(rotation.z);

        // Step 3: Calculate Bank Angle (angle_B)
        Vector3 W0 = new Vector3(-rotation.y, rotation.x, 0f);
        Vector3 U0 = Vector3.Cross(W0, rotation).normalized;
        Vector3 U = Vector3.Cross(W0, U0);
        float dotW0U = Vector3.Dot(W0, U);
        float dotU0U = Vector3.Dot(U0, U);
        float angle_B = Mathf.Atan2(dotW0U / W0.magnitude, dotU0U / U0.magnitude);

        // Convert angles from radians to degrees
        angle_H *= Mathf.Rad2Deg;
        angle_P *= Mathf.Rad2Deg;
        angle_B *= Mathf.Rad2Deg;

        float snapHeading = Mathf.Round(angle_H * InverseRotationGrid) * RotationGrid;
        float snapPitch = Mathf.Round(angle_P * InverseRotationGrid) * RotationGrid;
        float snapBank = Mathf.Round(angle_B * InverseRotationGrid) * RotationGrid;

        // Calculate the final Euler angles
        Vector4 eulerAngles = new Vector4(-snapPitch, snapHeading, snapBank, rotation.w);

        // Adjust the euler angles to match Unity's coordinate system
        eulerAngles.x = -eulerAngles.x;






        // Adjust the euler angles to match Unity's coordinate system
        eulerAngles.x = -eulerAngles.x;

        // Convert angles from degrees to radians
        float _angle_H = eulerAngles.y * Mathf.Deg2Rad;
        float _angle_P = -eulerAngles.x * Mathf.Deg2Rad;
        float _angle_B = eulerAngles.z * Mathf.Deg2Rad;

        // Step 1: Calculate the direction vector (D) without bank angle
        Vector3 _heading = new Vector3(Mathf.Cos(_angle_H), Mathf.Sin(_angle_H), 0f);
        Vector3 _pitch = new Vector3(0f, 0f, Mathf.Sin(_angle_P));

        Vector3 _directionVectorWithoutBank = Quaternion.Euler(eulerAngles.y, -eulerAngles.x, 0f) * Vector3.forward;

        // Step 2: Calculate the direction vector (D) with bank angle
        Vector3 _W0 = new Vector3(-_heading.y, _heading.x, 0f);
        Vector3 _U0 = Vector3.Cross(_W0, _directionVectorWithoutBank).normalized;
        Vector3 _U = Vector3.Cross(_W0, _U0);

        float _cosAngleB = Mathf.Cos(_angle_B);
        float _sinAngleB = Mathf.Sin(_angle_B);

        Vector3 _directionVector = _directionVectorWithoutBank + _cosAngleB * _U0 + _sinAngleB * _W0;

        // Step 3: Project the direction vector onto the plane defined by the up vector
        Vector3 _projectionOntoUpPlane = _directionVector - Vector3.Dot(_directionVector, Vector3.up) * Vector3.up;
        _directionVector = _projectionOntoUpPlane.normalized;


        return new Vector4(_directionVector.x, _directionVector.y, _directionVector.z, rotation.w);
    }

    private void RotationSnapMatrix(ref Matrix4x4 matrix4x4)
    {
        // Snap the rotation
        matrix4x4.SetColumn(0, SnapRotation(matrix4x4.GetColumn(0)));
        matrix4x4.SetColumn(1, SnapRotation(matrix4x4.GetColumn(1)));
        matrix4x4.SetColumn(2, SnapRotation(matrix4x4.GetColumn(2)));
    }

    private void RotationSnapMatrix2(ref Matrix4x4 matrix4x4)
    {
        // Snap the rotation
        matrix4x4.SetColumn(0, SnapRotation2(matrix4x4.GetColumn(0)));
        matrix4x4.SetColumn(1, SnapRotation2(matrix4x4.GetColumn(1)));
        matrix4x4.SetColumn(2, SnapRotation2(matrix4x4.GetColumn(2)));
    }

    private void RotationSnapMatrix3(ref Matrix4x4 matrix4x4)
    {
        // Snap the rotation
        matrix4x4.SetColumn(0, SnapRotation3(matrix4x4.GetColumn(0)));
        matrix4x4.SetColumn(1, SnapRotation3(matrix4x4.GetColumn(1)));
        matrix4x4.SetColumn(2, SnapRotation3(matrix4x4.GetColumn(2)));
    }

    private void RotationSnapMatrix4(ref Matrix4x4 matrix4x4)
    {
        // Snap the rotation
        matrix4x4.SetColumn(0, SnapRotation4(matrix4x4.GetColumn(0)));
        matrix4x4.SetColumn(1, SnapRotation4(matrix4x4.GetColumn(1)));
        matrix4x4.SetColumn(2, SnapRotation4(matrix4x4.GetColumn(2)));
    }

    private void RotationSnapMatrix5(ref Matrix4x4 matrix4x4)
    {
        // Snap the rotation
        matrix4x4.SetColumn(0, SnapRotation5(matrix4x4.GetColumn(0)));
        matrix4x4.SetColumn(1, SnapRotation5(matrix4x4.GetColumn(1)));
        matrix4x4.SetColumn(2, SnapRotation5(matrix4x4.GetColumn(2)));
    }

    private void Start()
    {
        Transform trueTransform = transform;

        for (float x = 0; x < fullRotation; x += rotationIncrease)
        {
            for (float y = 0; y < fullRotation; y += rotationIncrease)
            {
                for (float z = 0; z < fullRotation; z += rotationIncrease)
                {
                    Vector3 eulerRotation = new Vector3(x, y, z);
                    Quaternion rotation = Quaternion.Euler(eulerRotation);
                    Vector3 snappedEulerRotation = eulerRotation.RoundToEulerRotation();
                    Quaternion snappedRotation = Quaternion.Euler(snappedEulerRotation);

                    trueTransform.rotation = rotation;
                    Matrix4x4 calculatedMatrix = trueTransform.localToWorldMatrix;

                    trueTransform.rotation = snappedRotation;
                    Matrix4x4 trueMatrix = trueTransform.localToWorldMatrix;
                    RotationSnapMatrix5(ref calculatedMatrix);

                    if (!Compare(trueMatrix, calculatedMatrix))
                    {
                        return;
                    }
                }
            }
        }
    }

    public static bool ApproximatelyEqual(float a, float b, float epsilon)
    {
        // Check if the difference between the two floats is within the specified epsilon.
        return Math.Abs(a - b) < epsilon;
    }

    private bool Compare(Matrix4x4 trueMatrix, Matrix4x4 calculatedMatrix)
    {
        for (int i = 0; i < 16; i++)
        {
            float trueValue = trueMatrix[i];
            float calculatedValue = calculatedMatrix[i];

            if (!ApproximatelyEqual(trueValue, calculatedValue, 0.001f))
            {
                Debug.Log(trueMatrix);
                Debug.Log(calculatedMatrix);
                return false;
            }
        }
        return true;
    }
}
