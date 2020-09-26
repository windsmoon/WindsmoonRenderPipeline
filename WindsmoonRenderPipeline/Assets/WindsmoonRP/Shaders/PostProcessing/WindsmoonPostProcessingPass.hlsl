#ifndef WINDSMOON_POST_PROCESSING_INCLUDED
#define WINDSMOON_POST_PROCESSING_INCLUDED

TEXTURE2D(_PostProcessingSource);
SAMPLER(sampler_linear_clamp);

float4 _ProjectionParams; // x : if x is less than 0, the v is from top to bottom

struct Varyings
{
    float4 positionCS: SV_POSITION;
    float2 uv : VAR_UV;
};

Varyings PostProcessingVertex(uint vertexID : SV_VertexID) // the order is (-1, -1) (-1. 3) (3, -1)
{
    Varyings output;
    output.positionCS = float4(vertexID <= 1 ? -1.0 : 3.0, vertexID == 1 ? 3.0: -1.0, 0.0, 1.0);
    output.uv = float2(vertexID <= 1 ? 0.0 : 2.0, vertexID == 1 ? 2.0 : 0.0);

    if (_ProjectionParams.x < 0.0)
    {
        output.uv.y = 1.0 - output.uv.y;
    }
    
    return output;
}

float4 GetSource(float2 uv)
{
    return SAMPLE_TEXTURE2D(_PostProcessingSource, sampler_linear_clamp, uv);
}

float4 CopyPassFragment(Varyings input) : SV_TARGET
{
    // return float4(input.uv, 0.0, 1.0);
    return GetSource(input.uv);
}

#endif