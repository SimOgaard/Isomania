Shader "Debug/Ground"
{
	Properties
	{
		_LightColor("Light Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DarkColor("Dark Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_GridSize("Grid Size", Float) = 10
	}

	SubShader {

		Pass
		{
			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma vertex UnlitPassVertex
			#pragma fragment UnlitPassFragment
			#include "Library/Common.hlsl"

			float4 _LightColor;
			float4 _DarkColor;
			float _GridSize;

			struct Attributes
			{
				float4 vertex : POSITION;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
			};

			Varyings UnlitPassVertex(Attributes input)
			{
				Varyings output;

				//calculate the position in clip space to render the object
                output.vertex = mul(UNITY_MATRIX_MVP, input.vertex);
                //calculate the position of the vertex in the world
                output.worldPos = mul(unity_ObjectToWorld, input.vertex);

				return output;
			}

			float4 UnlitPassFragment(Varyings input) : SV_TARGET
			{
                //scale the position to adjust for shader input
                float3 adjustedWorldPos = input.worldPos / _GridSize + HalfUnitsPerPixelOffset;
				//floor the values so we have whole numbers
                float3 flooredAdjustedWorldPos = floor(adjustedWorldPos);
                //add different dimensions
                float chessboard = flooredAdjustedWorldPos.x + flooredAdjustedWorldPos.y + flooredAdjustedWorldPos.z;
                //divide it by 2 and get the fractional part, resulting in a value of 0 for even and 0.5 for off numbers.
                chessboard = frac(chessboard * 0.5);
                //multiply it by 2 to make odd values 1 instead of 0.5
                chessboard *= 2;

                //interpolate between color for even fields (0) and color for odd fields (1)
                float4 color = lerp(_LightColor, _DarkColor, chessboard);
                return color;
			}

			ENDHLSL
		}
	}
}
