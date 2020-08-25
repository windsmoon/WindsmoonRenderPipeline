#ifndef WINDSMOON_LIGHTING_INCLUDED
#define WINDSMOON_LIGHTING_INCLUDED

float3 GetLighting(Surface surface, Light light, BRDF brdfLight)
{
    // ?? why tutorial do light attenuation in saturate
    //return saturate(dot(surface.normal, light.direction) * light.attenuation;) * light.color * GetDirectBRDFLight(surface, brdfLight, light);

    return saturate(dot(surface.normal, light.direction)) * light.color * GetDirectBRDF(surface, brdfLight, light) * light.attenuation;
}

float3 GetLighting(Surface surfaceWS, BRDF brdf, GI gi) 
{
	//return GetIncomingLight(surface, GetDirectionalLight()) * surface.color;
	ShadowData shadowData = GetShadowData(surfaceWS);
	shadowData.shadowMask = gi.shadowMask;
	//return gi.shadowMask.shadows.rgb; // debug
	
	// todo : gi.diffuse can be replaced to gi, then the diffuse and specular will all be caculated in the brdf.hlsl
	float3 color = GetIndirectBRDF(surfaceWS, brdf, gi.diffuse, gi.specular);
	
	for (int i = 0; i < GetDirectionalLightCount(); i++) 
	{
		color += GetLighting(surfaceWS, GetDirectionalLight(i, surfaceWS, shadowData), brdf);
	}
	
	// debug : this method can be used to check surface is using which cascade culling sphere
	//float cascadeColor = shadowInfo.cascadeIndex * 0.25 + 0.25;
	//return cascadeColor.rrr;
	return color;
}

#endif