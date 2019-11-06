#ifndef WINDSMOON_UNLIT_INCLUDED
#define WINDSMOON_UNLIT_INCLUDED

#include "WindsmoonCommon.hlsl"

float4 UnlitVertex(float3 positionOS : POSITION) : SV_Position
{
    float3 worldPos = TransformObjectToWorld(positionOS);
    return TransformWorldToHClip(worldPos);
}

float4 UnlitFragment() : SV_Target
{
    return 1.0;
}

#endif