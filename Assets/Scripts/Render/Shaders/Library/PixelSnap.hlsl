#ifndef PIXEL_SNAP_INCLUDED
#define PIXEL_SNAP_INCLUDED

#define PixelsPerUnit 10.0
#define UnitsPerPixel 1.0 / PixelsPerUnit
#define HalfUnitsPerPixelOffset UnitsPerPixel / 2.0

void PixelSnapObjectToWorldMatrix()
{
    // Get the transformation matrix
    unity_ObjectToWorld = mul(_InverseCameraRotationMatrix, unity_ObjectToWorld);

    // Set the world position to zero
    unity_ObjectToWorld[0][3] = round(unity_ObjectToWorld[0][3] * PixelsPerUnit) * UnitsPerPixel;
    unity_ObjectToWorld[1][3] = round(unity_ObjectToWorld[1][3] * PixelsPerUnit) * UnitsPerPixel;
    unity_ObjectToWorld[2][3] = round(unity_ObjectToWorld[2][3] * PixelsPerUnit) * UnitsPerPixel;

    unity_ObjectToWorld = mul(_CameraRotationMatrix, unity_ObjectToWorld);
}

#endif