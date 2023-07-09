#ifndef ISOMETRIC_SPRITES_INCLUDED
#define ISOMETRIC_SPRITES_INCLUDED

#include "UnityCG.cginc"

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
    UNITY_VERTEX_OUTPUT_STEREO
};

inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
{
    return float4(pos.xy * flip, pos.z, 1.0);
}

#define yScale (1.0 / cos(radians(-30.0)))

v2f SpriteVert(appdata_t IN)
{
    v2f OUT;

    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
    
    float3 scale = float3(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21),
        length(unity_ObjectToWorld._m02_m12_m22)
    );

    // Scale the object in the y-direction
    scale.y *= yScale;
 
    // Apply the scaled values to the transformation matrix
    float4x4 modelMatrix = unity_ObjectToWorld;
    modelMatrix[0][0] = scale.x;
    modelMatrix[1][1] = scale.y;
    modelMatrix[2][2] = scale.z;

    // Copy them so we can change them (demonstration purposes only)
    float4x4 m = UNITY_MATRIX_M;
    float4x4 v = UNITY_MATRIX_V;
    float4x4 p = UNITY_MATRIX_P;

    // Break out the axis
    float3 right = normalize(v._m00_m01_m02);
    float3 up = float3(0,1,0);
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
    OUT.color = IN.color;

    OUT.vertex = UnityPixelSnap(OUT.vertex);

    return OUT;
}

sampler2D _MainTex;

float4 SampleSpriteTexture(float2 uv)
{    
    return tex2D (_MainTex, uv);
}

float4 SpriteFrag(v2f IN) : SV_Target
{
    float4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
    c.rgb *= c.a;
    return c;
}

#endif // ISOMETRIC_SPRITES_INCLUDED