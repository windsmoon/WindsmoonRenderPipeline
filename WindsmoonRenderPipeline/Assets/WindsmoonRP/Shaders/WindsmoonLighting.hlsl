#ifndef WINDSMOON_LIGHTING_INCLUDED
#define WINDSMOON_LIGHTING_INCLUDED

float3 GetIncomingLight(Surface surface, Light light)
{
    return saturate(dot(surface.normal, light.direction)) * light.color;
}

float3 GetLighting(Surface surface) 
{
	//return GetIncomingLight(surface, GetDirectionalLight()) * surface.color;
	
	float3 color = 0.0;
	
	for (int i = 0; i < GetDirectionalLightCount(); i++) 
	{
		color += GetIncomingLight(surface, GetDirectionalLight(i));
	}
	return color;
}

#endif