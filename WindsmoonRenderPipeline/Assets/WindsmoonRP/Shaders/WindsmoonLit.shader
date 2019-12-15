Shader "Windsmoon RP/Windsmoon Lit"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
		[Enum(Off, 0, On, 1)] _ZWrite("Z Write", Float) = 1
		[Toggle(ALPHA_CLIPPING)] _alphaClipping ("Alpha Clipping", Float) = 0
    }
    
    SubShader
    {
        Pass
        {
            Tags
            {
                "LightMode" = "WindsmoonLit"
            }
            
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            
            HLSLPROGRAM
            #pragma multi_compile _ ALPHA_CLIPPING
            #pragma multi_compile_instancing
            #pragma vertex LitVertex
            #pragma fragment LitFragment
            #include "WindsmoonLit.hlsl"
            ENDHLSL
        }
    }
}