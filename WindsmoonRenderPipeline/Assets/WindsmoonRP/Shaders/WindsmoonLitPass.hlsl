#ifndef WINDSMOON_LIT_PASS_INCLUDED
#define WINDSMOON_LIT_PASS_INCLUDED

//#include "WindsmoonCommon.hlsl"
#include "WindsmoonSurface.hlsl"
#include "WindsmoonShadow.hlsl"
#include "WindsmoonLight.hlsl"
#include "WindsmoonBRDF.hlsl"
#include "WindsmoonGI.hlsl"
#include "WindsmoonLighting.hlsl"

//CBUFFER_START(UnityPerMaterial)
  //  float4 _BaseColor;
//CBUFFER_END

//TEXTURE2D(_BaseMap);
//SAMPLER(sampler_BaseMap);

//UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
//    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
//	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
//	UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
//	UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
//	UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
//UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attribute
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 baseUV : TEXCOORD0;
    GI_ATTRIBUTE_DATA
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_Position;
    float3 positionWS : VAR_POSITION;
    float3 normalWS : VAR_NORMAL;
    float2 baseUV : VAR_BASE_UV;
    GI_VARYINGS_DATA
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings LitVertex(Attribute input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    TRANSFER_GI_DATA(input, output);
    //float3 worldPos = TransformObjectToWorld(input.positionOS);
    output.positionWS = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(output.positionWS);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
    //float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
	output.baseUV = TransformBaseUV(input.baseUV);
    return output;
}

float4 LitFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);

	ClipForLOD(input.positionCS.xy, unity_LODFade.x);
    //float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.baseUV);
	//float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
	//baseColor *= baseMap;
	float4 baseColor = GetBaseColor(input.baseUV);

	#if defined(ALPHA_CLIPPING)
	    clip(baseColor.a - GetCutoff(input.baseUV));
	#endif
	
	Surface surface;
	surface.position = input.positionWS;
	surface.depth = -TransformWorldToView(input.positionWS).z;
	surface.normal = normalize(input.normalWS);
	surface.viewDirection = normalize(_WorldSpaceCameraPos - input.positionWS);
	surface.color = baseColor.rgb;
	surface.alpha = baseColor.a;
	surface.metallic = GetMetallic(input.baseUV);
	surface.smoothness = GetSmoothness(input.baseUV);
	surface.dither = InterleavedGradientNoise(input.positionCS.xy, 0);
	BRDF brdf = GetBRDF(surface);
	GI gi = GetGI(GI_FRAGMENT_DATA(input), surface);
	return float4(GetLighting(surface, brdf, gi) + GetEmission(input.baseUV), surface.alpha);
}
#endif