Shader "Windsmoon RP/Windsmoon Unlit"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma multi_compile_instancing
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #include "WindsmoonUnlit.hlsl"
            ENDHLSL
        }
    }
}