#ifndef WINDSMOON_POST_PROCESSING_INCLUDED
#define WINDSMOON_POST_PROCESSING_INCLUDED

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
    return output;
}

float4 CopyPassFragment(Varyings input) : SV_TARGET
{
    return float4(input.uv, 0.0, 1.0);
}

#endif