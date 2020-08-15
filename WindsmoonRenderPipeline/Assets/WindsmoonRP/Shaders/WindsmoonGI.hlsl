#ifndef WINDSMOON_GI_INCLUDED
#define WINDSMOON_GI_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

TEXTURE2D(unity_Lightmap);
SAMPLER(samplerunity_Lightmap);

TEXTURE2D(unity_ShadowMask);
SAMPLER(samplerunity_ShadowMask);

TEXTURE3D_FLOAT(unity_ProbeVolumeSH);
SAMPLER(samplerunity_ProbeVolumeSH);

#if defined(LIGHTMAP_ON)
	#define GI_ATTRIBUTE_DATA float2 lightMapUV : TEXCOORD1;
	
	#define GI_VARYINGS_DATA float2 lightMapUV : VAR_LIGHT_MAP_UV;
		#define TRANSFER_GI_DATA(input, output) \
		output.lightMapUV = input.lightMapUV * \
		unity_LightmapST.xy + unity_LightmapST.zw;

	#define GI_FRAGMENT_DATA(input) input.lightMapUV
#else
	#define GI_ATTRIBUTE_DATA
	#define GI_VARYINGS_DATA
	#define TRANSFER_GI_DATA(input, output)
	#define GI_FRAGMENT_DATA(input) 0.0
#endif

struct GI
{
    float3 diffuse;
    ShadowMask shadowMask;
};

float3 SampleLightMap(float2 lightMapUV) 
{
	#if defined(LIGHTMAP_ON)
		return SampleSingleLightmap(TEXTURE2D_ARGS(unity_Lightmap, samplerunity_Lightmap), lightMapUV, float4(1.0, 1.0, 0.0, 0.0),
		    #if defined(UNITY_LIGHTMAP_FULL_HDR)
				false,
			#else
				true,
			#endif
			float4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0, 0.0)); // gi uv has been transferred in vertex shader
	#else
		return 0.0;
	#endif
}

float4 SampleBakedShadows (float2 lightMapUV) 
{
	#if defined(LIGHTMAP_ON)
		return SAMPLE_TEXTURE2D(unity_ShadowMask, samplerunity_ShadowMask, lightMapUV);
	#else
		return unity_ProbesOcclusion;
	#endif
}

float3 SampleLightProbes(Surface surfaceWS) // ?? just know how to use
{
    #if defined(LIGHTMAP_ON)
        return 0.0;
    #else
        if (unity_ProbeVolumeParams.x) // 1 means use LPPV, L1 spherical harmonics, less precise
        {
			return SampleProbeVolumeSH4(
				TEXTURE3D_ARGS(unity_ProbeVolumeSH, samplerunity_ProbeVolumeSH),
				surfaceWS.position, surfaceWS.normal,
				unity_ProbeVolumeWorldToObject,
				unity_ProbeVolumeParams.y, unity_ProbeVolumeParams.z,
				unity_ProbeVolumeMin.xyz, unity_ProbeVolumeSizeInv.xyz
			);
		}
		
		else // may be L2 spherical harmonics
		{
		    float4 coefficients[7];
		    coefficients[0] = unity_SHAr;
		    coefficients[1] = unity_SHAg;
		    coefficients[2] = unity_SHAb;
		    coefficients[3] = unity_SHBr;
		    coefficients[4] = unity_SHBg;
		    coefficients[5] = unity_SHBb;
		    coefficients[6] = unity_SHC;
		    return max(0.0, SampleSH9(coefficients, surfaceWS.normal));
		}
    #endif
}

GI GetGI(float2 lightMapUV, Surface surfaceWS)
{
    GI gi;
    gi.diffuse = SampleLightMap(lightMapUV) + SampleLightProbes(surfaceWS);
	
	#if defined(SHADOW_MASK_DISTANCE)
		gi.shadowMask.useDistanceShadowMask = true;
		gi.shadowMask.shadows = SampleBakedShadows(lightMapUV);
	#else
	    gi.shadowMask.useDistanceShadowMask = false;
	    gi.shadowMask.shadows = 1.0;
	#endif
	
    return gi;
}

#endif