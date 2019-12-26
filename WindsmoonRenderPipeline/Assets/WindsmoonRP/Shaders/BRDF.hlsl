#ifndef BRDF_INCLUDED
#define BRDF_INCLUDED

struct BRDFLight
{
	float3 diffuse;
	float3 specular;
	float roughness;
};

BRDFLight GetBRDFLight(Surface surface) 
{
	BRDFLight brdfLight;
	float oneMinusReflectivity = 1.0 - surface.metallic;
	brdfLight.diffuse = surface.color * oneMinusReflectivity;
	brdfLight.specular = 0.0;
	brdfLight.roughness = 1.0;
	return brdfLight;
}

#endif