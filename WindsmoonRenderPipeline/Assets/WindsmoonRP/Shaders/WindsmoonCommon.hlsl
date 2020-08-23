﻿#ifndef WINDSMOON_COMMON_INCLUDED
#define WINDSMOON_COMMON_INCLUDED



/*float3 TransformObjectToWorld(float3 positionOS)
{
    return mul(unity_ObjectToWorld, float4(positionOS, 1.0)).xyz;
}

float4 TransformWorldToHClip(float3 positionWS) 
{
	return mul(unity_MatrixVP, float4(positionWS, 1.0));
}*/

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "WindsmoonInput.hlsl"

// the macrosis are expected in SpaceTransforms.hlsl
#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_P glstate_matrix_projection

// the occlusion data of light probe can get instanced automatically, but UnityInstancing only does this when SHADOWS_SHADOWMASK is defined (from catlike)
#if defined(SHADOW_MASK_ALWAYS) || defined(SHADOW_MASK_DISTANCE)
	#define SHADOWS_SHADOWMASK
#endif

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

float Square (float v) 
{
	return v * v;
}

float GetDistanceSquared(float3 p1, float3 p2)
{
    float3 diff = p2 - p1;
    return dot(diff, diff);
}

#endif