#ifndef WINDSMOON_CEL_SHADING_INPUT_INCLUDED
#define WINDSMOON_CEL_SHADING_INPUT_INCLUDED

TEXTURE2D(_BaseMap);
TEXTURE2D(_EmissionMap);
TEXTURE2D(_MaskMap);
TEXTURE2D(_NormalMap);
SAMPLER(sampler_BaseMap);

TEXTURE2D(_DetailMap);
TEXTURE2D(_DetailNormalMap);
SAMPLER(sampler_DetailMap);

TEXTURE2D(_RampMap);
SAMPLER(sampler_RampMap);

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
    UNITY_DEFINE_INSTANCED_PROP(float, _DetailAlbedo)
    UNITY_DEFINE_INSTANCED_PROP(float, _DetailSmoothness)
    UNITY_DEFINE_INSTANCED_PROP(float, _NormalScale)
    UNITY_DEFINE_INSTANCED_PROP(float, _DetailNormalScale)
    UNITY_DEFINE_INSTANCED_PROP(float, _OutlineWidth)
    UNITY_DEFINE_INSTANCED_PROP(float4, _OutlineColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _CelShadeColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowColor)
    UNITY_DEFINE_INSTANCED_PROP(float, _ShadowRange)
    UNITY_DEFINE_INSTANCED_PROP(float, _ShadowSmooth)
    UNITY_DEFINE_INSTANCED_PROP(float4, _RimColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _RimRange)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

#define INPUT_PROP(name) UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, name)

struct InputConfig
{
    float2 uv;
    float2 detailUV;
    // bool useMask;
};

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

InputConfig GetInputConfig(float2 uv, float2 detailUV = 0.0)
{
    InputConfig config;
    config.uv = uv;
    config.detailUV = detailUV;
    // config.useMask = false;
    return config;
}

// todo : when USE_DETAIL is disabled, there should be no sample for detail map, even return 0 is incorrect
float4 GetDetail(InputConfig config)
{
    #if defined(DETAIL_MAP)
        float4 detailMap = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, config.detailUV);
        // do this for the caculation latter, the detail value can be used as lerp t.
        // when the detailMap is less than 0, it means the value will be darker, otherwise the value will be brighter
        detailMap = detailMap * 2 - 1;
        return detailMap;
    #else
        return 0.0;
    #endif
}

float4 GetMask(InputConfig config)
{
    #if defined(MASK_MAP)
        return SAMPLE_TEXTURE2D(_MaskMap, sampler_BaseMap, config.uv);
    #else
        return 1.0;
    #endif
}

float4 GetBaseColor(InputConfig config)
{
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, config.uv);
    float4 color = INPUT_PROP(_BaseColor);

    #if defined(DETAIL_MAP)
        float detail = GetDetail(config).r * INPUT_PROP(_DetailAlbedo);
        float detailMask = GetMask(config).b;
        // ?? why do the sqrt in https://catlikecoding.com/unity/tutorials/custom-srp/complex-maps
        baseMap.rgb = lerp(sqrt(baseMap.rgb), detail < 0.0 ? 0.0 : 1.0, abs(detail) * detailMask); // the base map will be darker or brighter
        baseMap.rgb *= baseMap.rgb;
    #endif
    
    return baseMap * color;
}

float GetCutoff(InputConfig config)
{
    return INPUT_PROP(_Cutoff);
}

float GetMetallic(InputConfig config)
{
    float metallic = INPUT_PROP(_Metallic);
    metallic *= GetMask(config).r;
    return metallic;
}

float GetOcclusion(InputConfig config)
{
    // debug :
    // return 1.0;
    // return GetMask(uv).g;

    float strength = INPUT_PROP(_Occlusion);
    float occlusion = GetMask(config).g;
    occlusion = lerp(occlusion, 1.0, strength);
    return occlusion;
}

float GetSmoothness(InputConfig config)
{
    float smoothness = INPUT_PROP(_Smoothness);
    smoothness *= GetMask(config).a;

    #if defined(DETAIL_MAP)
        float detail = GetDetail(config).b * INPUT_PROP(_DetailSmoothness);
        float detailMask = GetMask(config).b;
        smoothness = lerp(smoothness, detail < 0.0 ? 0.0 : 1.0, abs(detail) * detailMask);
    #endif

    return smoothness;
}

float GetFresnel(InputConfig config)
{
    return INPUT_PROP(_Fresnel);
}

float3 GetNormalTS(InputConfig config)
{
    float4 normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_BaseMap, config.uv);
    float scale = INPUT_PROP(_NormalScale);
    float3 normal = DecodeNormal(normalMap, scale);

    #if defined(DETAIL_MAP)
        normalMap = SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailMap, config.detailUV);
        scale = INPUT_PROP(_DetailNormalScale) * GetMask(config).b;
        float3 detail = DecodeNormal(normalMap, scale);
        normal = BlendNormalRNM(normal, detail);
    #endif
    
    return normal;
}

float3 GetEmission(InputConfig config)
{
    float4 emissionMap = SAMPLE_TEXTURE2D(_EmissionMap, sampler_BaseMap, config.uv);
    float4 emissionColor = INPUT_PROP(_EmissionColor);
    return emissionMap.rgb * emissionColor.rgb;
}

float GetRamp(float rampUV)
{
    return SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(rampUV, 0.5)).r;
}

float GetOutlineWidth()
{
    return INPUT_PROP(_OutlineWidth);
}

float4 GetOutlineColor()
{
    return INPUT_PROP(_OutlineColor);
}

float3 GetCelShadeColor()
{
    return INPUT_PROP(_CelShadeColor);
}

float3 GetShadowColor()
{
    return INPUT_PROP(_ShadowColor);
}

float GetShadowRange()
{
    return INPUT_PROP(_ShadowRange);
}

float GetShadowSmooth()
{
    return INPUT_PROP(_ShadowSmooth);
}

float4 GetRimColor()
{
    return INPUT_PROP(_RimColor);
}

float2 GetRimRange()
{
    return INPUT_PROP(_RimRange); 
}

#endif