#ifndef CUSTOM_LIT_PASS_INCLUDED
#define CUSTOM_LIT_PASS_INCLUDED

#include "Library/Common.hlsl"
#include "Library/Surface.hlsl"

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attributes
{
	float4 vertex : POSITION;
    float3 normalOS : NORMAL;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
	float4 vertex : SV_POSITION;
	float3 normalWS : VAR_NORMAL;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings LitPassVertex (Attributes input)
{
    Varyings output;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    
    PixelSnapMatrix(unity_ObjectToWorld);
    PixelSnapMatrix(unity_WorldToObject);

    RotationSnapMatrix(unity_ObjectToWorld);
    RotationSnapMatrix(unity_WorldToObject);

    float4 pos = input.vertex;
    pos = mul(UNITY_MATRIX_M, pos);
    pos = mul(UNITY_MATRIX_VP, pos);

    output.vertex = pos;

    output.normalWS = TransformObjectToWorldNormal(input.normalOS);

    return output;
}

float4 LitPassFragment (Varyings input) : SV_TARGET
{
    return float4 (input.normalWS, 1);


	UNITY_SETUP_INSTANCE_ID(input);
	return UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
}

#endif