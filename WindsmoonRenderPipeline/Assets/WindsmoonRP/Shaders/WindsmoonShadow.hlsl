#ifndef WINDSMOON_SHADOW_INCLUDED
#define WINDSMOON_SHADOW_INCLUDED

#define MAX_DIRECTIONAL_SHADOW_COUNT 4
#define MAX_CASCADE_COUNT 4

TEXTURE2D_SHADOW(_DirectionalShadowMap); // ?? what does TEXTURE2D_SHADOW mean ?
#define SHADOW_SAMPLER sampler_linear_clamp_compare // ??
SAMPLER_CMP(SHADOW_SAMPLER); // ?? note : use a special SAMPLER_CMP macro to define the sampler state, as this does define a different way to sample shadow maps, because regular bilinear filtering doesn't make sense for depth data.

CBUFFER_START(ShadowInfo)
    int _CascadeCount;
    float4 _CascadeCullingSpheres[MAX_CASCADE_COUNT];
    float4x4 _DirectionalShadowMatrices[MAX_DIRECTIONAL_SHADOW_COUNT * MAX_CASCADE_COUNT];
CBUFFER_END 

struct DirectionalShadowInfo
{
    float shadowStrength;
    int tileIndex;
};

struct ShadowInfo
{
    int cascadeIndex;
};

ShadowInfo GetShadowInfo(Surface surfaceWS)
{
    ShadowInfo shadowInfo;
    shadowInfo.cascadeIndex = 0;
}

float SampleDirectionalShadow(float3 positionShadowMap)
{
    return SAMPLE_TEXTURE2D_SHADOW(_DirectionalShadowMap, SHADOW_SAMPLER, positionShadowMap);
}

float GetDirectionalShadowAttenuation(DirectionalShadowInfo info, Surface surfaceWS)
{
    if (info.shadowStrength <= 0.0f) // todo : when strength is zero, this light should be discard in c# part 
    {
		return 1.0f;
	}
	
    float3 positionShadowMap = mul(_DirectionalShadowMatrices[info.tileIndex], float4(surfaceWS.position, 1.0f));
    float shadow = SampleDirectionalShadow(positionShadowMap);
    return lerp(1.0f, shadow, info.shadowStrength); // ?? why directly use shadow map value than cmpare their depth
}

#endif