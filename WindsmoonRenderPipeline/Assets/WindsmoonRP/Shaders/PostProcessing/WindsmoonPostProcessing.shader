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
			Name "Bloom Pre Filter"
			
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex PostProcessingVertex
			#pragma fragment BloomPrefilterPassFragment
			ENDHLSL
		}
		

		Pass 
		{
			Name "Bloom Horizontal Blur"
			
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex PostProcessingVertex
			#pragma fragment BloomHorizontalBlurFragment
			ENDHLSL
		}
		
		Pass 
		{
			Name "Bloom Vertical Blur"
			
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex PostProcessingVertex
			#pragma fragment BloomVerticalBlurFragment
			ENDHLSL
		}
		
		Pass 
		{
			Name "Bloom Combine Blur"
			
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex PostProcessingVertex
			#pragma fragment BloomCombineFragment
			ENDHLSL
		}
		
		Pass 
		{
			Name "Copy"
			
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex PostProcessingVertex
			#pragma fragment CopyFragment
			ENDHLSL
		}
	}
}