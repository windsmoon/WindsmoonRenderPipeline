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
    //float _MaxShadowDistance;
    float4 _ShadowDistanceFade; // x means 1/maxShadowDistance, y means 1/distanceFade
CBUFFER_END 

struct DirectionalShadowInfo
{
    float shadowStrength; // if surface is not in any culling sphere, global shadowStrength set to 0 to avoid any shadow 
    int tileIndex;
};

struct ShadowInfo
{
    int cascadeIndex;
    float strength;
};

float GetFadedShadowStrength(float depth, float scale, float fadeScale) // fadeScale is from 0 to 1 but not equal 0
{
    // (1 - depth / maxDistance) / fadeScale
    // (1 - depth / maxDistance) means from 0 to 1, the shadow strength from 1 to 0 linearly
    // divided by fadeScale and saturate the resulit means the fade begin at the point (1 - fadeScale) in the line form 0 to maxDisatance
    return saturate((1.0 - depth * scale) * fadeScale);
}

// todo : add cascade keyword
ShadowInfo GetShadowInfo(Surface surfaceWS)
{
    ShadowInfo shadowInfo;
    
    // ?? : is this fade meaningful ?
    
    // the outermost culling sphere doesn't end exactly at the max shadow distance but extends a bit beyond it
    
    //if (surfaceWS.depth >= _MaxShadowDistance)
    //{
    //    shadowInfo.cascadeIndex = 0;
    //    shadowInfo.strength = 0.0f;
    //    return shadowInfo;
    //}
    
    shadowInfo.strength = GetFadedShadowStrength(surfaceWS.depth, _ShadowDistanceFade.x, _ShadowDistanceFade.y);
    
    for (int i = 0; i < _CascadeCount; ++i)
    {
        float4 cullingSphere = _CascadeCullingSpheres[i];
        
        if (GetDistanceSquared(cullingSphere.xyz, surfaceWS.position) < cullingSphere.w)
        {
            shadowInfo.cascadeIndex = i;
            //shadowInfo.strength = 1.0f;
            return shadowInfo;
        }
    }
    
    shadowInfo.cascadeIndex = 0;
    shadowInfo.strength = 0.0f;
    return shadowInfo;
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