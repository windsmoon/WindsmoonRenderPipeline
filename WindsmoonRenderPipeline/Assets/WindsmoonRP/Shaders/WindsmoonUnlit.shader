Shader "Windsmoon RP/Windsmoon Unlit"
{
    Properties
    {
    }
    
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #include "WindsmoonUnlit.hlsl"
            ENDHLSL
        }
    }
}