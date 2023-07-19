Shader "Lit"
{
	
	Properties
	{
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	
	SubShader {

		Pass
		{
			Tags
			{
				"LightMode" = "CustomLit"
			}

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment

			#include "LitPass.hlsl"
			ENDHLSL
		}
	}
}