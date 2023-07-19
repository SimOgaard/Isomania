#ifndef PIXEL_SNAP_INCLUDED
#define PIXEL_SNAP_INCLUDED

#define PixelsPerUnit 10.0
#define UnitsPerPixel (1.0 / PixelsPerUnit)
#define HalfUnitsPerPixelOffset (UnitsPerPixel / 2.0)

#define RotationSnap 360.0 / 32.0
#define InverseRotationSnap 1.0 / (RotationSnap)

#define Epsilon 1e-5
#define PI 3.14159265359
#define Rad2Deg (180.0 / PI)
#define Deg2Rad (PI / 180.0)

void SnapRotation(inout float4 rotation)
{
    float azimuth = atan2(rotation.y + Epsilon, rotation.x);
    float elevation = acos(rotation.z);

    float snapAzimuth = round(azimuth * InverseRotationSnap * Rad2Deg) * RotationSnap * Deg2Rad;
    float snapElevation = round(elevation * InverseRotationSnap * Rad2Deg) * RotationSnap * Deg2Rad;

    float magnitude = length(rotation.xyz);

    rotation.x = cos(snapAzimuth) * sin(snapElevation) * magnitude;
    rotation.y = sin(snapAzimuth) * sin(snapElevation) * magnitude;
    rotation.z = cos(snapElevation) * magnitude;
}

void PixelSnapObjectToWorldMatrix()
{
    // Snap the rotation
    SnapRotation(unity_ObjectToWorld[0]);
    SnapRotation(unity_ObjectToWorld[1]);
    SnapRotation(unity_ObjectToWorld[2]);

    // Get the transformation matrix in "camera space"
    float4x4 objectToWorld = mul(_InverseCameraRotationMatrix, unity_ObjectToWorld);

    // Snap the world position now in "camera space"
    objectToWorld[0][3] = round(objectToWorld[0][3] * PixelsPerUnit) * UnitsPerPixel;
    objectToWorld[1][3] = round(objectToWorld[1][3] * PixelsPerUnit) * UnitsPerPixel;
    objectToWorld[2][3] = round(objectToWorld[2][3] * PixelsPerUnit) * UnitsPerPixel;

    // Transform back to world
    unity_ObjectToWorld = mul(_CameraRotationMatrix, objectToWorld);
}

#endif