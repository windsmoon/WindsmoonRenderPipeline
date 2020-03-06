#ifndef WINDSMOON_LIGHTING_INCLUDED
#define WINDSMOON_LIGHTING_INCLUDED

float3 GetLight(Surface surface, Light light, BRDFLight brdfLight)
{
    // ?? why tutorial do light attenuation in saturate
    //return saturate(dot(surface.normal, light.direction) * light.attenuation;) * light.color * GetDirectBRDFLight(surface, brdfLight, light);

    return saturate(dot(surface.normal, light.direction)) * light.color * GetDirectBRDFLight(surface, brdfLight, light) * light.attenuation;
}

float3 GetLighting(Surface surfaceWS, BRDFLight brdfLight) 
{
	//return GetIncomingLight(surface, GetDirectionalLight()) * surface.color;
	
	float3 color = 0.0;
	
	for (int i = 0; i < GetDirectionalLightCount(); i++) 
	{
		color += GetLight(surfaceWS, GetDirectionalLight(i, surfaceWS), brdfLight);
	}
	
	return color;
}

#endif