#ifndef WINDSMOON_LIGHTING_INCLUDED
#define WINDSMOON_LIGHTING_INCLUDED

float3 GetLighting(Surface surface) 
{
	return surface.normal.y * surface.color;
}

float3 GetIncomingLight(Surface surface, Light light)
{
    return dot(surface.normal, light.direction) * light.color;
}

#endif