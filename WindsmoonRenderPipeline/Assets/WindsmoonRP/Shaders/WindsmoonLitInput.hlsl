#ifndef WINDSMOON_LIT_INPUT_INCLUDED
#define WINDSMOON_LIT_INPUT_INCLUDED

TEXTURE2D(_BaseMap);
TEXTURE2D(_EmissionMap);
TEXTURE2D(_MaskMap);
SAMPLER(sampler_BaseMap);

TEXTURE2D(_DetailMap);
SAMPLER(sampler_DetailMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
    UNITY_DEFINE_INSTANCED_PROP(float4, _DetailMap_ST)
	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
	UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
	UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
    UNITY_DEFINE_INSTANCED_PROP(float, _Occlusion)
	UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
    UNITY_DEFINE_INSTANCED_PROP(float, _Fresnel)
    UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

#define INPUT_PROP(name) UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, name)

float2 TransformBaseUV(float2 baseUV)
{
    float4 baseST = INPUT_PROP(_BaseMap_ST);
    return baseUV * baseST.xy + baseST.zw;
}

float2 TransformDetailUV (float2 detailUV)
{
    float4 detailST = INPUT_PROP(_DetailMap_ST);
    return detailUV * detailST.xy + detailST.zw;
}

float4 GetDetail (float2 uv)
{
    float4 detailMap = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, uv);
    // do this for the caculation latter, the detail value can be used as lerp t.
    // when the detailMap is less than 0, it means the value will be darker, otherwise the value will be brighter
    detailMap = detailMap * 2 - 1;
    return detailMap;
}

float4 GetBaseColor(float2 uv, float2 detailUV = 0.0)
{
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    float4 color = INPUT_PROP(_BaseColor);

    float detail = GetDetail(detailUV).r;
    //map += detail;
    // baseMap.rgb = lerp(baseMap.rgb, detail < 0.0 ? 0.0 : 1.0, abs(detail)); // the base map will be darker or brighter


    baseMap.rgb = lerp(sqrt(baseMap.rgb), detail < 0.0 ? 0.0 : 1.0, abs(detail)); // the base map will be darker or brighter
    baseMap.rgb *= baseMap.rgb;
    return baseMap * color;
}

float4 GetMask(float2 uv)
{
    return SAMPLE_TEXTURE2D(_MaskMap, sampler_BaseMap, uv);
}

float GetCutoff(float2 uv)
{
    return INPUT_PROP(_Cutoff);
}

float GetMetallic(float2 uv)
{
    float metallic = INPUT_PROP(_Metallic);
    metallic *= GetMask(uv).r;
    return metallic;
}

float GetOcclusion(float2 uv)
{
    // debug :
    // return 1.0;
    // return GetMask(uv).g;

    float strength = INPUT_PROP(_Occlusion);
    float occlusion = GetMask(uv).g;
    occlusion = lerp(occlusion, 1.0, strength);
    return occlusion;
}

float GetSmoothness(float2 uv)
{
    float smoothness = INPUT_PROP(_Smoothness);
    smoothness *= GetMask(uv).a;
    return smoothness;
}

float GetFresnel(float2 uv)
{
    return INPUT_PROP(_Fresnel);
}

float3 GetEmission(float2 uv)
{
    float4 emissionMap = SAMPLE_TEXTURE2D(_EmissionMap, sampler_BaseMap, uv);
    float4 emissionColor = INPUT_PROP(_EmissionColor);
    return emissionMap.rgb * emissionColor.rgb;
}

#endif