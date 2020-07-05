#ifndef WINDSMOON_GI_INCLUDED
#define WINDSMOON_GI_INCLUDED

#if defined(LIGHTMAP_ON)
    #define LIGHT_MAP_ATTRIBUTE_INFO float2 lightMapUV : TEXCOORD1; 
    #define LIGHT_MAP_VARYINGS_INFO float2 lightMapUV : VAR_LIGHT_MAP_UV;
    #define TRANSFER_LIGHT_MAP_INFO(input, output) output.lightMapUV = input.lightMapUV
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

GI GetGI(float2 lightMapUV)
{
    GI gi;
    gi.diffuse = float3(lightMapUV, 0);
    return gi;
}

#endif