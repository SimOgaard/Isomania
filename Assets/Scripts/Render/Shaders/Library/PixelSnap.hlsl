#ifndef PIXEL_SNAP_INCLUDED
#define PIXEL_SNAP_INCLUDED

#include "Math.hlsl"

#define PixelsPerUnit 10.0
#define HalfUnitsPerPixelOffset (0.5 / PixelsPerUnit)

void PixelSnapMatrix(inout float4x4 matrix4x4)
{
    // Get the transformation matrix in "camera space"
    matrix4x4 = mul(_InverseCameraRotationMatrix, matrix4x4);

    // Snap the world position now in "camera space"
    matrix4x4[0][3] = round(matrix4x4[0][3] * PixelsPerUnit) / PixelsPerUnit;
    matrix4x4[1][3] = round(matrix4x4[1][3] * PixelsPerUnit) / PixelsPerUnit;
    matrix4x4[2][3] = round(matrix4x4[2][3] * PixelsPerUnit) / PixelsPerUnit;

    // Transform back to world
    matrix4x4 = mul(_CameraRotationMatrix, matrix4x4);
}

#define RotationSnap ((360.0 / 24.0) * Deg2Rad)

void SnapRotation(inout float4 rotation)
{
    float magnitude = length(rotation.xyz);

    float x = acos(rotation.x / magnitude);
    float y = acos(rotation.y / magnitude);
    float z = acos(rotation.z / magnitude);

    rotation.x = cos(round(x / RotationSnap) * RotationSnap) * magnitude;
    rotation.y = cos(round(y / RotationSnap) * RotationSnap) * magnitude;
    rotation.z = cos(round(z / RotationSnap) * RotationSnap) * magnitude;
}

void RotationSnapMatrix(inout float4x4 matrix4x4)
{
    // Snap the rotation
    SnapRotation(matrix4x4[0]);
    SnapRotation(matrix4x4[1]);
    SnapRotation(matrix4x4[2]);
}

#endif