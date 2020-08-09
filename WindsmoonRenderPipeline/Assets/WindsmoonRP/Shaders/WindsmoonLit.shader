Shader "Windsmoon RP/Windsmoon Lit"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [Toggle(ALPHA_CLIPPING)] _AlphaClipping ("Alpha Clipping", Float) = 0
        [Toggle(PREMULTIPLY_ALPHA)] _PremultiplyAlpha ("Premultiply Alpha", Float) = 0
        _Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
		[Enum(Off, 0, On, 1)] _ZWrite("Z Write", Float) = 1
		[KeywordEnum(On, Clip, Dither, Off)] _Shadow_Mode("Shadow Mode", Float) = 0
		[Toggle(RECEIVE_SHADOWS)] _ReceiveShadows ("Receive Shadows", Float) = 1
    }
    
    SubShader
    {
        HLSLINCLUDE
            #include "WindsmoonCommon.hlsl"
            #include "WindsmoonLitInput.hlsl"
        ENDHLSL
        
        Pass
        {
            Tags
            {
                "LightMode" = "WindsmoonLit"
            }
            
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            
            HLSLPROGRAM
            #pragma target 3.5 // for loops which are use a variable length
            #pragma multi_compile _ ALPHA_CLIPPING
            #pragma multi_compile _ PREMULTIPLY_ALPHA
            #pragma multi_compile _ DIRECTIONAL_PCF3X3 DIRECTIONAL_PCF5X5 DIRECTIONAL_PCF7X7
            #pragma multi_compile _ CASCADE_BLEND_SOFT CASCADE_BLEND_DITHER
            #pragma multi_compile _ RECEIVE_SHADOWS
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_instancing
            #pragma vertex LitVertex
            #pragma fragment LitFragment
            #include "WindsmoonLitPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            
            ColorMask 0
            
            HLSLPROGRAM
            #pragma target 3.5 // for loops which are use a variable length
//            #pragma multi_compile _ ALPHA_CLIPPING
            #pragma multi_compile _ _SHADOW_MODE_CLIP _SHADOW_MODE_DITHER
			#pragma multi_compile_instancing
			#pragma vertex ShadowCasterVertex
			#pragma fragment ShadowCasterFragment
			#include "WindsmoonShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    
    CustomEditor "WindsmoonRP.Editor.WindsmoonShaderGUI"
}