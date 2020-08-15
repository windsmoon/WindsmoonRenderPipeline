#ifndef WINDSMOON_LIGHT_INCLUDED
#define WINDSMOON_LIGHT_INCLUDED

#define MAX_DIRECTIONAL_LIGHT_COUNT 4

CBUFFER_START(LightInfo)
	int _DirectionalLightCount;
	float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
	float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
	float3 _DirectionalShadowInfos[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

struct Light
{
    float3 direction;
    float3 color;
    float attenuation;
};

int GetDirectionalLightCount() 
{
	return _DirectionalLightCount;
}

DirectionalShadowInfo GetDirectionalShadowInfo(int index, ShadowData shadowData)
{
    DirectionalShadowInfo info;
    info.shadowStrength = _DirectionalShadowInfos[index].x * shadowData.strength;
    info.tileIndex = _DirectionalShadowInfos[index].y + shadowData.cascadeIndex;
    info.normalBias = _DirectionalShadowInfos[index].z;
    return info;
}

Light GetDirectionalLight(int index, Surface sufraceWS, ShadowData shadowData)
{
    Light light;
    light.direction = _DirectionalLightDirections[index].xyz;
    light.color = _DirectionalLightColors[index].rgb;
    DirectionalShadowInfo directionalShadowInfo = GetDirectionalShadowInfo(index, shadowData);
    light.attenuation = GetDirectionalShadowAttenuation(directionalShadowInfo, shadowData, sufraceWS);
    // debug : this method can be used to check surface is using which cascade culling sphere
    //light.attenuation = shadowInfo.cascadeIndex * 0.25; 
    return light;
}

#endif