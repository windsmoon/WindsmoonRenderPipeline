#ifndef WINDSMOON_LIGHT_INCLUDED
#define WINDSMOON_LIGHT_INCLUDED

#define MAX_DIRECTIONAL_LIGHT_COUNT 4
#define MAX_OTHER_LIGHT_COUNT 64

CBUFFER_START(LightInfo)
	int _DirectionalLightCount;
	float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
	float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
	float4 _DirectionalShadowInfos[MAX_DIRECTIONAL_LIGHT_COUNT];

    int _OtherLightCount;
    float4 _OtherLightColors[MAX_OTHER_LIGHT_COUNT];
    float4 _OtherLightPositions[MAX_OTHER_LIGHT_COUNT];
    float4 _OtherLightDirections[MAX_OTHER_LIGHT_COUNT];
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

int GetOtherLightCount()
{
    return _OtherLightCount;
}

DirectionalShadowData GetDirectionalShadowData(int index, ShadowData shadowData)
{
    DirectionalShadowData data;
    data.shadowStrength = _DirectionalShadowInfos[index].x;
    data.tileIndex = _DirectionalShadowInfos[index].y + shadowData.cascadeIndex;
    data.normalBias = _DirectionalShadowInfos[index].z;
    data.shadowMaskChannel = _DirectionalShadowInfos[index].w;
    return data;
}

Light GetDirectionalLight(int index, Surface sufraceWS, ShadowData shadowData)
{
    Light light;
    light.direction = _DirectionalLightDirections[index].xyz;
    light.color = _DirectionalLightColors[index].rgb;
    DirectionalShadowData directionalShadowInfo = GetDirectionalShadowData(index, shadowData);
    light.attenuation = GetDirectionalShadowAttenuation(directionalShadowInfo, shadowData, sufraceWS);
    // debug : this method can be used to check surface is using which cascade culling sphere
    //light.attenuation = shadowInfo.cascadeIndex * 0.25; 
    return light;
}

Light GetOtherLight(int index, Surface surfaceWS, ShadowData shadowData)
{
    Light light;
    light.color = _OtherLightColors[index].rgb;
    float3 ray = _OtherLightPositions[index].xyz - surfaceWS.position;
    light.direction = normalize(ray);
    float distanceSqrt = max(dot(ray, ray), 0.00001);
    float rangeAttenuation = Square(saturate(1.0 - Square(distanceSqrt * _OtherLightPositions[index].w)));
    float spotAttenuation = saturate(dot(_OtherLightDirections[index].xyz, light.direction));
    // spotAttenuation = 1;
    light.attenuation = spotAttenuation * rangeAttenuation / distanceSqrt; // ?? why divided by distanceSqrt
    // light.attenuation = rangeAttenuation;
    return light;
}

#endif