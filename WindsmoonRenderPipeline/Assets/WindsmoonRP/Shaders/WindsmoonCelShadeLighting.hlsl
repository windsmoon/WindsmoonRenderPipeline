#ifndef WINDSMOON_CEL_SHADE_LIGHTING_INCLUDED
#define WINDSMOON_CEL_SHADE_LIGHTING_INCLUDED

float3 GetLighting(Surface surface, Light light, BRDF brdfLight)
{
    // return saturate(dot(surface.normal, light.direction)) * light.color * GetDirectBRDF(surface, brdfLight, light) * light.attenuation;
	float nDotL = dot(surface.normal, light.direction);

	// diffuse
	float halfLambert = nDotL * 0.5 + 0.5;
	half ramp = GetRamp(halfLambert - GetShadowRange()); // todo : edge light ?
	// half ramp = smoothstep(0, _ShadowSmooth, halfLambert -  GetShadowRange());
	// float3 diffuse = halfLambert >  GetShadowRange() ? GetCelShadeColor() : GetShadowColor();
	float3 diffuse = lerp(GetShadowColor(), GetCelShadeColor(), ramp);

	float nDotV = saturate(dot(surface.normal, surface.viewDirection));

	// rim light
	float f = 1 - nDotV;
	float4 rimColor = GetRimColor();
	float2 rimRange = GetRimRange();
	f = smoothstep(rimRange.x, rimRange.y, f);
	float3 rim = f * rimColor.rgb * rimColor.a;
	
	float3 color = (diffuse * GetDirectBRDF(surface, brdfLight, light) + rim) * light.color * light.attenuation;
	return color;
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

#if defined(LIGHTS_PER_OBJECT)
	// unity_LightIndices is a array of lenght 2 and type real4, so up to 8 light per object are supported
	// but the unity_LightData.y is not limitted, so we have to do this
	for (int j = 0; j < min(unity_LightData.y, 8); ++j)
	{
		int lightIndex = unity_LightIndices[j / 4][j % 4];
		Light light = GetOtherLight(lightIndex, surfaceWS, shadowData);
		color += GetLighting(surfaceWS, light, brdf);
	}
#else
	// if the variable is the same as the loop above, there could have error in some cases
	for (int j = 0; j < GetOtherLightCount(); j++)
	{
		Light light = GetOtherLight(j, surfaceWS, shadowData);
		color += GetLighting(surfaceWS, light, brdf);
	}
#endif

	
	return color;
}

#endif