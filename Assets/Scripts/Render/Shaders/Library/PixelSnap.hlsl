#ifndef PIXEL_SNAP_INCLUDED
#define PIXEL_SNAP_INCLUDED

#define PixelsPerUnit 10.0
#define UnitsPerPixel (1.0 / PixelsPerUnit)
#define HalfUnitsPerPixelOffset (UnitsPerPixel / 2.0)

#define RotationSnap 360.0 / 24.0
#define InverseRotationSnap 1.0 / (RotationSnap)

#include "Math.hlsl"

void PixelSnapMatrix(inout float4x4 matrix4x4)
{
    // Get the transformation matrix in "camera space"
    matrix4x4 = mul(_InverseCameraRotationMatrix, matrix4x4);

    // Snap the world position now in "camera space"
    matrix4x4[0][3] = round(matrix4x4[0][3] * PixelsPerUnit) * UnitsPerPixel;
    matrix4x4[1][3] = round(matrix4x4[1][3] * PixelsPerUnit) * UnitsPerPixel;
    matrix4x4[2][3] = round(matrix4x4[2][3] * PixelsPerUnit) * UnitsPerPixel;

    // Transform back to world
    matrix4x4 = mul(_CameraRotationMatrix, matrix4x4);
}

void SnapRotation(inout float4 rotation)
{
    // Check for possible NaN outputs from atan2
    if (rotation.y == 0 && rotation.x == 0)
    {
        return;
    }

    float magnitude = length(rotation.xyz);

    float azimuth = atan2(rotation.y / magnitude, rotation.x / magnitude);
    float elevation = acos(rotation.z / magnitude);

    float snapAzimuth = round(azimuth * InverseRotationSnap * Rad2Deg) * RotationSnap * Deg2Rad;
    float snapElevation = round(elevation * InverseRotationSnap * Rad2Deg) * RotationSnap * Deg2Rad;

    rotation.x = cos(snapAzimuth) * sin(snapElevation) * magnitude;
    rotation.y = sin(snapAzimuth) * sin(snapElevation) * magnitude;
    rotation.z = cos(snapElevation) * magnitude;
}

void RotationSnapMatrix(inout float4x4 matrix4x4)
{
    // Snap the rotation
    SnapRotation(matrix4x4[0]);
    SnapRotation(matrix4x4[1]);
    SnapRotation(matrix4x4[2]);
}

#endif