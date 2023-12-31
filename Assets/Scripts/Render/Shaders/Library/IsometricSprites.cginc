#ifndef ISOMETRIC_SPRITES_INCLUDED
#define ISOMETRIC_SPRITES_INCLUDED

float4x4 _CameraRotationMatrix;
float4x4 _InverseCameraRotationMatrix;
#include "UnityCG.cginc"

#ifdef UNITY_INSTANCING_ENABLED

    UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
        // SpriteRenderer.Color while Non-Batched/Instanced.
        UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
        // this could be smaller but that's how bit each entry is regardless of type
        UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
    UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

    #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)

#endif // instancing

CBUFFER_START(UnityPerDrawSprite)
#ifndef UNITY_INSTANCING_ENABLED
    fixed4 _RendererColor;
#endif
    float _EnableExternalAlpha;
CBUFFER_END

// Material Color.
fixed4 _Color;

struct appdata_t
{
    float4 vertex   : POSITION;
    float4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex   : SV_POSITION;
    fixed4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
};

sampler2D _MainTex;
sampler2D _AlphaTex;
uniform float4 _MainTex_TexelSize;
uniform float4 _AlphaTex_TexelSize;

inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
{
    return float4(pos.xy * flip, pos.z, 1.0);
}

#define yScale (1.0 / cos(radians(-30.0)))
#define PixelsPerUnit 10.0
#define UnitsPerPixel 1.0 / PixelsPerUnit

v2f SpriteVert(appdata_t IN)
{
    v2f OUT;

    UNITY_SETUP_INSTANCE_ID(IN);
    
    // Get the scale of the transform
    float3 scale = float3(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21),
        length(unity_ObjectToWorld._m02_m12_m22)
    );

    // Scale the objects scale in the y-direction
    scale.y *= yScale;
 
    // Get the transformation matrix
    float4x4 modelMatrix = mul(_InverseCameraRotationMatrix, unity_ObjectToWorld);

    // if 
    float offsetX = (1.0f - fmod(_MainTex_TexelSize.z, 2.0)) * UnitsPerPixel * 0.5f;
    float offsetY = (1.0f - fmod(_MainTex_TexelSize.w, 2.0)) * UnitsPerPixel * 0.5f;

    // Set the world position to zero
    modelMatrix[0][3] = round(modelMatrix[0][3] * PixelsPerUnit) * UnitsPerPixel + offsetX;
    modelMatrix[1][3] = round(modelMatrix[1][3] * PixelsPerUnit) * UnitsPerPixel + offsetY;
    modelMatrix[2][3] = round(modelMatrix[2][3] * PixelsPerUnit) * UnitsPerPixel;

    modelMatrix = mul(_CameraRotationMatrix, modelMatrix);

    modelMatrix[0][0] = scale.x;
    modelMatrix[1][1] = scale.y;
    modelMatrix[2][2] = scale.z;

    // Copy them so we can change them (demonstration purposes only)
    float4x4 m = UNITY_MATRIX_M;
    float4x4 v = UNITY_MATRIX_V;
    float4x4 p = UNITY_MATRIX_P;

    // Break out the axis
    float3 right = normalize(v._m00_m01_m02);
    float3 up = float3(0, 1, 0);
    float3 forward = normalize(v._m20_m21_m22);
    // Get the rotation parts of the matrix
    float4x4 rotationMatrix = float4x4(
        right, 0,
        up, 0,
        forward, 0,
        0, 0, 0, 1
    );

    // The inverse of a rotation matrix happens to always be the transpose
    float4x4 rotationMatrixInverse = transpose(rotationMatrix);

    // Apply the rotationMatrixInverse, model, view, and projection matrix
    float4 pos = IN.vertex;
    pos = mul(rotationMatrixInverse, pos);
    pos = mul(modelMatrix, pos);
    pos = mul(v, pos);
    pos = mul(p, pos);

    OUT.vertex = pos;

    OUT.texcoord = IN.texcoord;
    OUT.color = IN.color * _Color * _RendererColor;

    return OUT;
}

fixed4 SampleSpriteTexture (float2 uv)
{
    fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
    fixed4 alpha = tex2D (_AlphaTex, uv);
    color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif

    return color;
}

fixed4 SpriteFrag(v2f IN) : SV_Target
{
    fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
    c.rgb *= c.a;
    return c;
}

#endif // ISOMETRIC_SPRITES_INCLUDED