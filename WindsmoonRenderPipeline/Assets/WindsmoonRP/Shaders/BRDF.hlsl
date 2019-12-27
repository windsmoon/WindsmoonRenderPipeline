#ifndef BRDF_INCLUDED
#define BRDF_INCLUDED

#define MIN_REFLECTIVITY 0.04

struct BRDFLight
{
	float3 diffuse;
	float3 specular;
	float roughness;
};

float OneMinusReflectivity(float metallic) 
{
	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range;
}

BRDFLight GetBRDFLight(Surface surface) 
{
	BRDFLight brdfLight;
	float oneMinusReflectivity = OneMinusReflectivity(surface.metallic);
	brdfLight.diffuse = surface.color * oneMinusReflectivity;
	//brdfLight.specular = 0.0;
	brdfLight.specular = lerp(MIN_REFLECTIVITY, surface.color, surface.metallic);
	float perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surface.smoothness); // disne
	brdfLight.roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
	//brdfLight.roughness = 1.0;
	return brdfLight;
}

float GetSpecularStrength(Surface surface, BRDFLight brdfLight, Light light) 
{
	float3 h = SafeNormalize(light.direction + surface.viewDirection);
	float nh2 = Square(saturate(dot(surface.normal, h)));
	float lh2 = Square(saturate(dot(light.direction, h)));
	float r2 = Square(brdfLight.roughness);
	float d2 = Square(nh2 * (r2 - 1.0) + 1.00001);
	float normalization = brdfLight.roughness * 4.0 + 2.0;
	return r2 / (d2 * max(0.1, lh2) * normalization);
}

float3 GetDirectBRDFLight(Surface surface, BRDFLight brdfLight, Light light) 
{
	return GetSpecularStrength(surface, brdfLight, light) * brdfLight.specular + brdfLight.diffuse;
}

#endif