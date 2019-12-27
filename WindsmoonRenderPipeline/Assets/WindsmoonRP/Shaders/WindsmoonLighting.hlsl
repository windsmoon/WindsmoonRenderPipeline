#ifndef WINDSMOON_LIGHTING_INCLUDED
#define WINDSMOON_LIGHTING_INCLUDED

float3 GetLight(Surface surface, Light light, BRDFLight brdfLight)
{
    return saturate(dot(surface.normal, light.direction)) * light.color * GetDirectBRDFLight(surface, brdfLight, light);
}

float3 GetLighting(Surface surface, BRDFLight brdfLight) 
{
	//return GetIncomingLight(surface, GetDirectionalLight()) * surface.color;
	
	float3 color = 0.0;
	
	for (int i = 0; i < GetDirectionalLightCount(); i++) 
	{
		color += GetLight(surface, GetDirectionalLight(i), brdfLight);
	}
	
	return color;
}

#endif