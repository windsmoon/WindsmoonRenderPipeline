#ifndef WINDSMOON_LIGHT_INCLUDED
#define WINDSMOON_LIGHT_INCLUDED

#define MAX_DIRECTIONAL_LIGHT_COUNT 4

CBUFFER_START(LightInfo)
	int _DirectionalLightCount;
	float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
	float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

struct Light
{
    float3 direction;
    float3 color;
};

int GetDirectionalLightCount() 
{
	return _DirectionalLightCount;
}

Light GetDirectionalLight(int index)
{
    Light light;
    light.direction = _DirectionalLightDirection[index].xyz;
    light.color = _DirectionalLightColor[index].rgb;
    return light;
}

#endif