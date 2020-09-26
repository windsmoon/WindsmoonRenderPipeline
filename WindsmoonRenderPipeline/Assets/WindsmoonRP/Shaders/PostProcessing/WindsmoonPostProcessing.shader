Shader "Windsmoon RP/Windsmoon Lit" 
{
	SubShader 
	{
		Cull Off
		ZTest Always
		ZWrite Off
		
		HLSLINCLUDE
		#include "../WindsmoonCommon.hlsl"
		#include "WindsmoonPostProcessingPass.hlsl"
		ENDHLSL

		Pass 
		{
			Name "Copy"
			
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex PostProcessingVertex
			#pragma fragment CopyPassFragment
			ENDHLSL
		}
	}
}