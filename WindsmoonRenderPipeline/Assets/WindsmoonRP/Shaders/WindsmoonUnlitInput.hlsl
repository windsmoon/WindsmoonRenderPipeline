#ifndef WINDSMOON_UNLIT_INPUT_INCLUDED
#define WINDSMOON_UNLIT_INPUT_INCLUDED

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
	UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

float2 TransformBaseUV(float2 baseUV)
{
    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
    return baseUV * baseST.xy + baseST.zw;
}

float4 GetBaseColor(float2 uv)
{
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    float4 color = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
    return baseMap * color;
}

float GetCutoff(float2 uv)
{
    return UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff);
}

float GetMetallic(float2 uv)
{
    return 0.0;
}

float GetSmoothness(float2 uv)
{
    return 0.0;
}

#endif