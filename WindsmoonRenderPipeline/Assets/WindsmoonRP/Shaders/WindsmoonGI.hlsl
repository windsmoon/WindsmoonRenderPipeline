#ifndef WINDSMOON_GI_INCLUDED
#define WINDSMOON_GI_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

TEXTURE2D(unity_lightmap);
SAMPLER(samplerunity_Lightmap);

#if defined(LIGHTMAP_ON)
    #define LIGHT_MAP_ATTRIBUTE_INFO float2 lightMapUV : TEXCOORD1; 
    #define LIGHT_MAP_VARYINGS_INFO float2 lightMapUV : VAR_LIGHT_MAP_UV;
    #define TRANSFER_LIGHT_MAP_INFO(input, output) \
        output.lightMapUV = input.lightMapUV * \
        unity_LightmapST.xy + unity_LightmapST.zw;    
    #define GET_LIGHT_MAP_UV(input) input.lightMapUV
#else
    #define LIGHT_MAP_ATTRIBUTE_INFO
    #define LIGHT_MAP_VARYINGS_INFO
    #define TRANSFER_LIGHT_MAP_INFO(input, output)
    #define GET_LIGHT_MAP_UV(input) 0.0
#endif

struct GI
{
    float3 diffuse;
};

float3 SampleLightMap(float2 lightMapUV)
{
    #if defined(LIGHTMAP_ON)
        #if defined(UNITY_LIGHTMAP_FULL_HDR) // todo : the third argument
			return SampleSingleLightmap(TEXTURE2D_ARGS(unity_lightmap, samplerunity_Lightmap), lightMapUV, float4(1.0, 1.0, 0.0, 0.0), false, float4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0, 0.0));
		#else
			return SampleSingleLightmap(TEXTURE2D_ARGS(unity_lightmap, samplerunity_Lightmap), lightMapUV, float4(1.0, 1.0, 0.0, 0.0), true, float4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0, 0.0));
		#endif
    #else
        return 0.0;
    #endif
}

GI GetGI(float2 lightMapUV)
{
    GI gi;
    //gi.diffuse = float3(lightMapUV, 0); // debug
    gi.diffuse = SampleLightMap(lightMapUV);
    return gi;
}

#endif