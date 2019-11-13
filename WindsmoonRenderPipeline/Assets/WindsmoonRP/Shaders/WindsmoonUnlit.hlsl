#ifndef WINDSMOON_UNLIT_INCLUDED
#define WINDSMOON_UNLIT_INCLUDED

#include "WindsmoonCommon.hlsl"

//CBUFFER_START(UnityPerMaterial)
  //  float4 _BaseColor;
//CBUFFER_END

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attribute
{
    float3 positionOS : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_Position;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings UnlitVertex(Attribute input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    float3 worldPos = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(worldPos);
    return output;
}

float4 UnlitFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input)
    return UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
 }

#endif